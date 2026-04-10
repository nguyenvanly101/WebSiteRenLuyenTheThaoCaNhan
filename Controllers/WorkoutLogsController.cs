using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebsiteRenLuyenTheThaoCaNhan.Data;
using WebsiteRenLuyenTheThaoCaNhan.Infrastructure;
using WebsiteRenLuyenTheThaoCaNhan.Models;
using WebsiteRenLuyenTheThaoCaNhan.ViewModels;

namespace WebsiteRenLuyenTheThaoCaNhan.Controllers;

[Authorize]
public class WorkoutLogsController : Controller
{
    private readonly ApplicationDbContext _context;

    public WorkoutLogsController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var userId = GetCurrentUserId();
        var logs = await _context.WorkoutLogs
            .Where(item => item.UserID == userId)
            .Include(item => item.WorkoutLogDetails)
            .OrderByDescending(item => item.WorkoutDate)
            .ToListAsync();

        return View(logs);
    }

    public async Task<IActionResult> Details(int id)
    {
        var log = await _context.WorkoutLogs
            .Where(item => item.UserID == GetCurrentUserId() && item.LogID == id)
            .Include(item => item.WorkoutLogDetails)
            .ThenInclude(item => item.Exercise)
            .FirstOrDefaultAsync();

        if (log is null)
        {
            return NotFound();
        }

        return View(new WorkoutLogDetailsViewModel
        {
            Log = log,
            TotalVolume = log.WorkoutLogDetails.Sum(item => item.Reps * item.Weight)
        });
    }

    [HttpGet]
    public IActionResult Create()
    {
        PopulateEnergyLevels();
        return View(new WorkoutLogFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(WorkoutLogFormViewModel model)
    {
        PopulateEnergyLevels(model.EnergyLevel);

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var log = new WorkoutLog
        {
            UserID = GetCurrentUserId(),
            WorkoutDate = model.WorkoutDate,
            DurationMinutes = model.DurationMinutes,
            EnergyLevel = model.EnergyLevel,
            Note = model.Note.Trim()
        };

        _context.WorkoutLogs.Add(log);
        await _context.SaveChangesAsync();

        SetStatus("Buoi tap da duoc ghi nhan.", "success");
        return RedirectToAction(nameof(Details), new { id = log.LogID });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var log = await GetOwnedLogsQuery().FirstOrDefaultAsync(item => item.LogID == id);
        if (log is null)
        {
            return NotFound();
        }

        PopulateEnergyLevels(log.EnergyLevel);
        return View(new WorkoutLogFormViewModel
        {
            WorkoutDate = log.WorkoutDate,
            DurationMinutes = log.DurationMinutes,
            EnergyLevel = log.EnergyLevel,
            Note = log.Note
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, WorkoutLogFormViewModel model)
    {
        var log = await GetOwnedLogsQuery().FirstOrDefaultAsync(item => item.LogID == id);
        if (log is null)
        {
            return NotFound();
        }

        PopulateEnergyLevels(model.EnergyLevel);

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        log.WorkoutDate = model.WorkoutDate;
        log.DurationMinutes = model.DurationMinutes;
        log.EnergyLevel = model.EnergyLevel;
        log.Note = model.Note.Trim();

        await _context.SaveChangesAsync();
        SetStatus("Buoi tap da duoc cap nhat.", "success");
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var log = await GetOwnedLogsQuery()
            .Include(item => item.WorkoutLogDetails)
            .FirstOrDefaultAsync(item => item.LogID == id);

        if (log is null)
        {
            return NotFound();
        }

        return View(log);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var log = await GetOwnedLogsQuery().FirstOrDefaultAsync(item => item.LogID == id);
        if (log is null)
        {
            return NotFound();
        }

        _context.WorkoutLogs.Remove(log);
        await _context.SaveChangesAsync();
        SetStatus("Buoi tap da duoc xoa.", "success");
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> AddDetail(int logId)
    {
        var log = await GetOwnedLogsQuery()
            .Include(item => item.WorkoutLogDetails)
            .FirstOrDefaultAsync(item => item.LogID == logId);
        if (log is null)
        {
            return NotFound();
        }

        await PopulateExerciseOptionsAsync();
        ViewBag.LogLabel = log.WorkoutDate.ToString("dd/MM/yyyy");
        return View(new WorkoutLogDetailFormViewModel
        {
            LogId = logId,
            SetNumber = log.WorkoutLogDetails.Count == 0
                ? 1
                : log.WorkoutLogDetails.Max(item => item.SetNumber) + 1
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddDetail(int logId, WorkoutLogDetailFormViewModel model)
    {
        var log = await GetOwnedLogsQuery().FirstOrDefaultAsync(item => item.LogID == logId);
        if (log is null)
        {
            return NotFound();
        }

        await PopulateExerciseOptionsAsync(model.ExerciseID);
        ViewBag.LogLabel = log.WorkoutDate.ToString("dd/MM/yyyy");

        if (!await _context.Exercises.AnyAsync(item => item.ExerciseID == model.ExerciseID))
        {
            ModelState.AddModelError(nameof(model.ExerciseID), "Bai tap khong ton tai.");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        _context.WorkoutLogDetails.Add(new WorkoutLogDetail
        {
            LogID = logId,
            ExerciseID = model.ExerciseID,
            SetNumber = model.SetNumber,
            Reps = model.Reps,
            Weight = model.Weight
        });

        await _context.SaveChangesAsync();
        SetStatus("Da them chi tiet buoi tap.", "success");
        return RedirectToAction(nameof(Details), new { id = logId });
    }

    [HttpGet]
    public async Task<IActionResult> EditDetail(int id)
    {
        var detail = await _context.WorkoutLogDetails
            .Include(item => item.WorkoutLog)
            .FirstOrDefaultAsync(item => item.ID == id && item.WorkoutLog.UserID == GetCurrentUserId());

        if (detail is null)
        {
            return NotFound();
        }

        await PopulateExerciseOptionsAsync(detail.ExerciseID);
        ViewBag.LogLabel = detail.WorkoutLog.WorkoutDate.ToString("dd/MM/yyyy");

        return View(new WorkoutLogDetailFormViewModel
        {
            LogId = detail.LogID,
            ExerciseID = detail.ExerciseID,
            SetNumber = detail.SetNumber,
            Reps = detail.Reps,
            Weight = detail.Weight
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditDetail(int id, WorkoutLogDetailFormViewModel model)
    {
        var detail = await _context.WorkoutLogDetails
            .Include(item => item.WorkoutLog)
            .FirstOrDefaultAsync(item => item.ID == id && item.WorkoutLog.UserID == GetCurrentUserId());

        if (detail is null)
        {
            return NotFound();
        }

        await PopulateExerciseOptionsAsync(model.ExerciseID);
        ViewBag.LogLabel = detail.WorkoutLog.WorkoutDate.ToString("dd/MM/yyyy");

        if (!await _context.Exercises.AnyAsync(item => item.ExerciseID == model.ExerciseID))
        {
            ModelState.AddModelError(nameof(model.ExerciseID), "Bai tap khong ton tai.");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        detail.ExerciseID = model.ExerciseID;
        detail.SetNumber = model.SetNumber;
        detail.Reps = model.Reps;
        detail.Weight = model.Weight;

        await _context.SaveChangesAsync();
        SetStatus("Chi tiet buoi tap da duoc cap nhat.", "success");
        return RedirectToAction(nameof(Details), new { id = detail.LogID });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteDetail(int id)
    {
        var detail = await _context.WorkoutLogDetails
            .Include(item => item.WorkoutLog)
            .FirstOrDefaultAsync(item => item.ID == id && item.WorkoutLog.UserID == GetCurrentUserId());

        if (detail is null)
        {
            return NotFound();
        }

        var logId = detail.LogID;
        _context.WorkoutLogDetails.Remove(detail);
        await _context.SaveChangesAsync();

        SetStatus("Da xoa chi tiet buoi tap.", "success");
        return RedirectToAction(nameof(Details), new { id = logId });
    }

    private IQueryable<WorkoutLog> GetOwnedLogsQuery()
    {
        var userId = GetCurrentUserId();
        return _context.WorkoutLogs.Where(item => item.UserID == userId);
    }

    private int GetCurrentUserId()
    {
        return User.GetUserId() ?? throw new InvalidOperationException("Authenticated user id is missing.");
    }

    private void PopulateEnergyLevels(string? selected = null)
    {
        ViewBag.EnergyLevels = new SelectList(new[] { "Low", "Balanced", "High" }, selected);
    }

    private async Task PopulateExerciseOptionsAsync(int? selectedId = null)
    {
        var exercises = await _context.Exercises.OrderBy(item => item.Name).ToListAsync();
        ViewBag.Exercises = new SelectList(exercises, nameof(Exercise.ExerciseID), nameof(Exercise.Name), selectedId);
    }

    private void SetStatus(string message, string type)
    {
        TempData["StatusMessage"] = message;
        TempData["StatusType"] = type;
    }
}
