using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebsiteRenLuyenTheThaoCaNhan.Infrastructure;
using WebsiteRenLuyenTheThaoCaNhan.Models;
using WebsiteRenLuyenTheThaoCaNhan.Services;
using WebsiteRenLuyenTheThaoCaNhan.ViewModels;

namespace WebsiteRenLuyenTheThaoCaNhan.Controllers;

[Authorize]
public class WorkoutLogsController : Controller
{
    private readonly IWorkoutLogService _workoutLogService;
    private readonly IExerciseService _exerciseService;

    public WorkoutLogsController(IWorkoutLogService workoutLogService, IExerciseService exerciseService)
    {
        _workoutLogService = workoutLogService;
        _exerciseService = exerciseService;
    }

    public async Task<IActionResult> Index()
    {
        var logs = await _workoutLogService.GetUserLogsAsync(GetCurrentUserId());
        return View(logs);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LogDay(int dayId)
    {
        var log = await _workoutLogService.LogDayAsync(dayId, GetCurrentUserId());
        if (log is null)
        {
            return NotFound();
        }

        SetStatus("Đã ghi nhận buổi tập từ kế hoạch. Bạn có thể cập nhật chi tiết bên dưới.", "success");
        return RedirectToAction(nameof(Details), new { id = log.LogID });
    }

    public async Task<IActionResult> Details(int id)
    {
        var log = await _workoutLogService.GetLogDetailsAsync(id, GetCurrentUserId());
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

        var (success, logId) = await _workoutLogService.CreateLogAsync(GetCurrentUserId(), model);
        if (!success)
        {
            ModelState.AddModelError(string.Empty, "Lỗi ghi nhận buổi tập.");
            return View(model);
        }

        SetStatus("Buổi tập đã được ghi nhận.", "success");
        return RedirectToAction(nameof(Details), new { id = logId });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var log = await _workoutLogService.GetLogByIdAsync(id, GetCurrentUserId());
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
        PopulateEnergyLevels(model.EnergyLevel);

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _workoutLogService.UpdateLogAsync(id, GetCurrentUserId(), model);
        if (!result)
        {
            return NotFound();
        }

        SetStatus("Buổi tập đã được cập nhật.", "success");
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var log = await _workoutLogService.GetLogDetailsAsync(id, GetCurrentUserId());
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
        var result = await _workoutLogService.DeleteLogAsync(id, GetCurrentUserId());
        if (!result)
        {
            return NotFound();
        }

        SetStatus("Buổi tập đã được xóa.", "success");
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> AddDetail(int logId)
    {
        var log = await _workoutLogService.GetLogDetailsAsync(logId, GetCurrentUserId());
        if (log is null)
        {
            return NotFound();
        }

        await PopulateExerciseOptionsAsync();
        ViewBag.LogLabel = log.WorkoutDate.ToString("dd/MM/yyyy");
        return View(new WorkoutLogDetailFormViewModel
        {
            LogId = logId,
            SetNumber = log.WorkoutLogDetails.Count == 0 ? 1 : log.WorkoutLogDetails.Max(item => item.SetNumber) + 1
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddDetail(int logId, WorkoutLogDetailFormViewModel model)
    {
        var log = await _workoutLogService.GetLogByIdAsync(logId, GetCurrentUserId());
        if (log is null)
        {
            return NotFound();
        }

        await PopulateExerciseOptionsAsync(model.ExerciseID);
        ViewBag.LogLabel = log.WorkoutDate.ToString("dd/MM/yyyy");

        var exercise = await _exerciseService.GetByIdAsync(model.ExerciseID);
        if (exercise is null)
        {
            ModelState.AddModelError(nameof(model.ExerciseID), "Bài tập không tồn tại.");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        await _workoutLogService.AddDetailAsync(logId, GetCurrentUserId(), model);
        SetStatus("Đã thêm chi tiết buổi tập.", "success");
        return RedirectToAction(nameof(Details), new { id = logId });
    }

    [HttpGet]
    public async Task<IActionResult> EditDetail(int id)
    {
        var detail = await _workoutLogService.GetDetailByIdAsync(id, GetCurrentUserId());
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
        var detail = await _workoutLogService.GetDetailByIdAsync(id, GetCurrentUserId());
        if (detail is null)
        {
            return NotFound();
        }

        await PopulateExerciseOptionsAsync(model.ExerciseID);
        ViewBag.LogLabel = detail.WorkoutLog.WorkoutDate.ToString("dd/MM/yyyy");

        var exercise = await _exerciseService.GetByIdAsync(model.ExerciseID);
        if (exercise is null)
        {
            ModelState.AddModelError(nameof(model.ExerciseID), "Bài tập không tồn tại.");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        await _workoutLogService.EditDetailAsync(id, GetCurrentUserId(), model);
        SetStatus("Chi tiết buổi tập đã được cập nhật.", "success");
        return RedirectToAction(nameof(Details), new { id = detail.LogID });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteDetail(int id)
    {
        var detail = await _workoutLogService.GetDetailByIdAsync(id, GetCurrentUserId());
        if (detail is null)
        {
            return NotFound();
        }

        var logId = detail.LogID;
        await _workoutLogService.DeleteDetailAsync(id, GetCurrentUserId());
        SetStatus("Đã xóa chi tiết buổi tập.", "success");
        return RedirectToAction(nameof(Details), new { id = logId });
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
        var exercises = await _exerciseService.GetAllExercisesAsync();
        ViewBag.Exercises = new SelectList(exercises, nameof(Exercise.ExerciseID), nameof(Exercise.Name), selectedId);
    }

    private void SetStatus(string message, string type)
    {
        TempData["StatusMessage"] = message;
        TempData["StatusType"] = type;
    }
}
