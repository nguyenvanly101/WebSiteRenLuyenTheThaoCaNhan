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
public class WorkoutPlansController : Controller
{
    private readonly ApplicationDbContext _context;

    public WorkoutPlansController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var userId = GetCurrentUserId();
        var plans = await _context.WorkoutPlans
            .Where(item => item.UserID == userId)
            .Include(item => item.WorkoutDays)
            .ThenInclude(day => day.WorkoutExercises)
            .AsSplitQuery()
            .OrderByDescending(item => item.CreatedAt)
            .ToListAsync();

        return View(plans);
    }

    public async Task<IActionResult> Details(int id)
    {
        var plan = await GetOwnedPlanQuery()
            .Include(item => item.WorkoutDays)
            .ThenInclude(day => day.WorkoutExercises)
            .ThenInclude(item => item.Exercise)
            .FirstOrDefaultAsync(item => item.PlanID == id);

        if (plan is null)
        {
            return NotFound();
        }

        return View(new WorkoutPlanDetailsViewModel
        {
            Plan = plan,
            TotalDays = plan.WorkoutDays.Count,
            TotalExercises = plan.WorkoutDays.SelectMany(day => day.WorkoutExercises).Count()
        });
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new WorkoutPlanFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(WorkoutPlanFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var plan = new WorkoutPlan
        {
            UserID = GetCurrentUserId(),
            PlanName = model.PlanName.Trim(),
            Goal = model.Goal.Trim(),
            Level = model.Level.Trim(),
            Summary = model.Summary.Trim(),
            Duration = model.Duration,
            CreatedAt = DateTime.UtcNow
        };

        _context.WorkoutPlans.Add(plan);
        await _context.SaveChangesAsync();

        SetStatus("Ke hoach tap da duoc tao.", "success");
        return RedirectToAction(nameof(Details), new { id = plan.PlanID });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var plan = await GetOwnedPlanQuery().FirstOrDefaultAsync(item => item.PlanID == id);
        if (plan is null)
        {
            return NotFound();
        }

        return View(new WorkoutPlanFormViewModel
        {
            PlanName = plan.PlanName,
            Goal = plan.Goal,
            Level = plan.Level,
            Summary = plan.Summary,
            Duration = plan.Duration
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, WorkoutPlanFormViewModel model)
    {
        var plan = await GetOwnedPlanQuery().FirstOrDefaultAsync(item => item.PlanID == id);
        if (plan is null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        plan.PlanName = model.PlanName.Trim();
        plan.Goal = model.Goal.Trim();
        plan.Level = model.Level.Trim();
        plan.Summary = model.Summary.Trim();
        plan.Duration = model.Duration;

        await _context.SaveChangesAsync();
        SetStatus("Ke hoach tap da duoc cap nhat.", "success");
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var plan = await GetOwnedPlanQuery()
            .Include(item => item.WorkoutDays)
            .ThenInclude(day => day.WorkoutExercises)
            .FirstOrDefaultAsync(item => item.PlanID == id);

        if (plan is null)
        {
            return NotFound();
        }

        return View(plan);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var plan = await GetOwnedPlanQuery().FirstOrDefaultAsync(item => item.PlanID == id);
        if (plan is null)
        {
            return NotFound();
        }

        _context.WorkoutPlans.Remove(plan);
        await _context.SaveChangesAsync();
        SetStatus("Ke hoach tap da duoc xoa.", "success");
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> AddDay(int planId)
    {
        var plan = await GetOwnedPlanQuery()
            .Include(item => item.WorkoutDays)
            .FirstOrDefaultAsync(item => item.PlanID == planId);

        if (plan is null)
        {
            return NotFound();
        }

        ViewBag.PlanName = plan.PlanName;
        return View(new WorkoutDayFormViewModel
        {
            PlanId = planId,
            DayNumber = plan.WorkoutDays.Count == 0 ? 1 : plan.WorkoutDays.Max(item => item.DayNumber) + 1
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddDay(int planId, WorkoutDayFormViewModel model)
    {
        var plan = await GetOwnedPlanQuery().FirstOrDefaultAsync(item => item.PlanID == planId);
        if (plan is null)
        {
            return NotFound();
        }

        ViewBag.PlanName = plan.PlanName;

        if (await _context.WorkoutDays.AnyAsync(item => item.PlanID == planId && item.DayNumber == model.DayNumber))
        {
            ModelState.AddModelError(nameof(model.DayNumber), "Ngay tap nay da ton tai trong ke hoach.");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        _context.WorkoutDays.Add(new WorkoutDay
        {
            PlanID = planId,
            DayNumber = model.DayNumber,
            FocusArea = model.FocusArea.Trim(),
            Note = model.Note.Trim()
        });

        await _context.SaveChangesAsync();
        SetStatus("Da them ngay tap vao ke hoach.", "success");
        return RedirectToAction(nameof(Details), new { id = planId });
    }

    [HttpGet]
    public async Task<IActionResult> EditDay(int id)
    {
        var day = await _context.WorkoutDays
            .Include(item => item.WorkoutPlan)
            .FirstOrDefaultAsync(item => item.DayID == id && item.WorkoutPlan.UserID == GetCurrentUserId());

        if (day is null)
        {
            return NotFound();
        }

        ViewBag.PlanName = day.WorkoutPlan.PlanName;
        return View(new WorkoutDayFormViewModel
        {
            PlanId = day.PlanID,
            DayNumber = day.DayNumber,
            FocusArea = day.FocusArea,
            Note = day.Note
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditDay(int id, WorkoutDayFormViewModel model)
    {
        var day = await _context.WorkoutDays
            .Include(item => item.WorkoutPlan)
            .FirstOrDefaultAsync(item => item.DayID == id && item.WorkoutPlan.UserID == GetCurrentUserId());

        if (day is null)
        {
            return NotFound();
        }

        ViewBag.PlanName = day.WorkoutPlan.PlanName;

        var duplicate = await _context.WorkoutDays.AnyAsync(item =>
            item.PlanID == day.PlanID &&
            item.DayNumber == model.DayNumber &&
            item.DayID != id);

        if (duplicate)
        {
            ModelState.AddModelError(nameof(model.DayNumber), "Ngay tap nay da ton tai trong ke hoach.");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        day.DayNumber = model.DayNumber;
        day.FocusArea = model.FocusArea.Trim();
        day.Note = model.Note.Trim();

        await _context.SaveChangesAsync();
        SetStatus("Ngay tap da duoc cap nhat.", "success");
        return RedirectToAction(nameof(Details), new { id = day.PlanID });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteDay(int id)
    {
        var day = await _context.WorkoutDays
            .Include(item => item.WorkoutPlan)
            .FirstOrDefaultAsync(item => item.DayID == id && item.WorkoutPlan.UserID == GetCurrentUserId());

        if (day is null)
        {
            return NotFound();
        }

        var planId = day.PlanID;
        _context.WorkoutDays.Remove(day);
        await _context.SaveChangesAsync();

        SetStatus("Da xoa ngay tap.", "success");
        return RedirectToAction(nameof(Details), new { id = planId });
    }

    [HttpGet]
    public async Task<IActionResult> AddExercise(int dayId)
    {
        var day = await _context.WorkoutDays
            .Include(item => item.WorkoutPlan)
            .ThenInclude(plan => plan.WorkoutDays)
            .ThenInclude(item => item.WorkoutExercises)
            .FirstOrDefaultAsync(item => item.DayID == dayId && item.WorkoutPlan.UserID == GetCurrentUserId());

        if (day is null)
        {
            return NotFound();
        }

        await PopulateExerciseOptionsAsync();
        ViewBag.DayLabel = $"Ngay {day.DayNumber} - {day.FocusArea}";
        return View(new WorkoutExerciseFormViewModel
        {
            DayId = dayId,
            DisplayOrder = day.WorkoutExercises.Count == 0 ? 1 : day.WorkoutExercises.Max(item => item.DisplayOrder) + 1
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddExercise(int dayId, WorkoutExerciseFormViewModel model)
    {
        var day = await _context.WorkoutDays
            .Include(item => item.WorkoutPlan)
            .FirstOrDefaultAsync(item => item.DayID == dayId && item.WorkoutPlan.UserID == GetCurrentUserId());

        if (day is null)
        {
            return NotFound();
        }

        await PopulateExerciseOptionsAsync(model.ExerciseID);
        ViewBag.DayLabel = $"Ngay {day.DayNumber} - {day.FocusArea}";

        if (!await _context.Exercises.AnyAsync(item => item.ExerciseID == model.ExerciseID))
        {
            ModelState.AddModelError(nameof(model.ExerciseID), "Bai tap khong ton tai.");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        _context.WorkoutExercises.Add(new WorkoutExercise
        {
            DayID = dayId,
            ExerciseID = model.ExerciseID,
            Sets = model.Sets,
            Reps = model.Reps,
            RestTime = model.RestTime,
            DisplayOrder = model.DisplayOrder
        });

        await _context.SaveChangesAsync();
        SetStatus("Da them bai tap vao ngay tap.", "success");
        return RedirectToAction(nameof(Details), new { id = day.PlanID });
    }

    [HttpGet]
    public async Task<IActionResult> EditExercise(int id)
    {
        var item = await _context.WorkoutExercises
            .Include(entry => entry.WorkoutDay)
            .ThenInclude(day => day.WorkoutPlan)
            .FirstOrDefaultAsync(entry => entry.ID == id && entry.WorkoutDay.WorkoutPlan.UserID == GetCurrentUserId());

        if (item is null)
        {
            return NotFound();
        }

        await PopulateExerciseOptionsAsync(item.ExerciseID);
        ViewBag.DayLabel = $"Ngay {item.WorkoutDay.DayNumber} - {item.WorkoutDay.FocusArea}";
        return View(new WorkoutExerciseFormViewModel
        {
            DayId = item.DayID,
            ExerciseID = item.ExerciseID,
            Sets = item.Sets,
            Reps = item.Reps,
            RestTime = item.RestTime,
            DisplayOrder = item.DisplayOrder
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditExercise(int id, WorkoutExerciseFormViewModel model)
    {
        var item = await _context.WorkoutExercises
            .Include(entry => entry.WorkoutDay)
            .ThenInclude(day => day.WorkoutPlan)
            .FirstOrDefaultAsync(entry => entry.ID == id && entry.WorkoutDay.WorkoutPlan.UserID == GetCurrentUserId());

        if (item is null)
        {
            return NotFound();
        }

        await PopulateExerciseOptionsAsync(model.ExerciseID);
        ViewBag.DayLabel = $"Ngay {item.WorkoutDay.DayNumber} - {item.WorkoutDay.FocusArea}";

        if (!await _context.Exercises.AnyAsync(entry => entry.ExerciseID == model.ExerciseID))
        {
            ModelState.AddModelError(nameof(model.ExerciseID), "Bai tap khong ton tai.");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        item.ExerciseID = model.ExerciseID;
        item.Sets = model.Sets;
        item.Reps = model.Reps;
        item.RestTime = model.RestTime;
        item.DisplayOrder = model.DisplayOrder;

        await _context.SaveChangesAsync();
        SetStatus("Chi tiet bai tap da duoc cap nhat.", "success");
        return RedirectToAction(nameof(Details), new { id = item.WorkoutDay.PlanID });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteExercise(int id)
    {
        var item = await _context.WorkoutExercises
            .Include(entry => entry.WorkoutDay)
            .ThenInclude(day => day.WorkoutPlan)
            .FirstOrDefaultAsync(entry => entry.ID == id && entry.WorkoutDay.WorkoutPlan.UserID == GetCurrentUserId());

        if (item is null)
        {
            return NotFound();
        }

        var planId = item.WorkoutDay.PlanID;
        _context.WorkoutExercises.Remove(item);
        await _context.SaveChangesAsync();

        SetStatus("Da xoa bai tap khoi ngay tap.", "success");
        return RedirectToAction(nameof(Details), new { id = planId });
    }

    private IQueryable<WorkoutPlan> GetOwnedPlanQuery()
    {
        var userId = GetCurrentUserId();
        return _context.WorkoutPlans.Where(item => item.UserID == userId);
    }

    private int GetCurrentUserId()
    {
        return User.GetUserId() ?? throw new InvalidOperationException("Authenticated user id is missing.");
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
