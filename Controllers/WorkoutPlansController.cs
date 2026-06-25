using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebsiteRenLuyenTheThaoCaNhan.Infrastructure;
using WebsiteRenLuyenTheThaoCaNhan.Models;
using WebsiteRenLuyenTheThaoCaNhan.Services;
using WebsiteRenLuyenTheThaoCaNhan.ViewModels;

namespace WebsiteRenLuyenTheThaoCaNhan.Controllers;

[Authorize]
public class WorkoutPlansController : Controller
{
    private readonly IWorkoutPlanService _workoutPlanService;
    private readonly IExerciseService _exerciseService;

    public WorkoutPlansController(IWorkoutPlanService workoutPlanService, IExerciseService exerciseService)
    {
        _workoutPlanService = workoutPlanService;
        _exerciseService = exerciseService;
    }

    public async Task<IActionResult> Index()
    {
        var plans = await _workoutPlanService.GetUserPlansAsync(GetCurrentUserId());
        return View(plans);
    }

    public async Task<IActionResult> Details(int id)
    {
        var plan = await _workoutPlanService.GetPlanDetailsAsync(id, GetCurrentUserId());
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

        var (success, planId) = await _workoutPlanService.CreatePlanAsync(GetCurrentUserId(), model);
        if (!success)
        {
            ModelState.AddModelError(string.Empty, "Lỗi tạo kế hoạch tập.");
            return View(model);
        }

        SetStatus("Kế hoạch tập đã được tạo.", "success");
        return RedirectToAction(nameof(Details), new { id = planId });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var plan = await _workoutPlanService.GetPlanByIdAsync(id, GetCurrentUserId());
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
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _workoutPlanService.UpdatePlanAsync(id, GetCurrentUserId(), model);
        if (!result)
        {
            return NotFound();
        }

        SetStatus("Kế hoạch tập đã được cập nhật.", "success");
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var plan = await _workoutPlanService.GetPlanDetailsAsync(id, GetCurrentUserId());
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
        var result = await _workoutPlanService.DeletePlanAsync(id, GetCurrentUserId());
        if (!result)
        {
            return NotFound();
        }

        SetStatus("Kế hoạch tập đã được xóa.", "success");
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> AddDay(int planId)
    {
        var plan = await _workoutPlanService.GetPlanDetailsAsync(planId, GetCurrentUserId());
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
        var plan = await _workoutPlanService.GetPlanByIdAsync(planId, GetCurrentUserId());
        if (plan is null)
        {
            return NotFound();
        }

        ViewBag.PlanName = plan.PlanName;

        if (await _workoutPlanService.DayNumberExistsAsync(planId, model.DayNumber))
        {
            ModelState.AddModelError(nameof(model.DayNumber), "Ngày tập này đã tồn tại trong kế hoạch.");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        await _workoutPlanService.AddDayAsync(planId, GetCurrentUserId(), model);
        SetStatus("Đã thêm ngày tập vào kế hoạch.", "success");
        return RedirectToAction(nameof(Details), new { id = planId });
    }

    [HttpGet]
    public async Task<IActionResult> EditDay(int id)
    {
        var day = await _workoutPlanService.GetDayByIdAsync(id, GetCurrentUserId());
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
        var day = await _workoutPlanService.GetDayByIdAsync(id, GetCurrentUserId());
        if (day is null)
        {
            return NotFound();
        }

        ViewBag.PlanName = day.WorkoutPlan.PlanName;

        if (await _workoutPlanService.DayNumberExistsAsync(day.PlanID, model.DayNumber, id))
        {
            ModelState.AddModelError(nameof(model.DayNumber), "Ngày tập này đã tồn tại trong kế hoạch.");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        await _workoutPlanService.EditDayAsync(id, GetCurrentUserId(), model);
        SetStatus("Ngày tập đã được cập nhật.", "success");
        return RedirectToAction(nameof(Details), new { id = day.PlanID });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteDay(int id)
    {
        var day = await _workoutPlanService.GetDayByIdAsync(id, GetCurrentUserId());
        if (day is null)
        {
            return NotFound();
        }

        var planId = day.PlanID;
        await _workoutPlanService.DeleteDayAsync(id, GetCurrentUserId());
        SetStatus("Đã xóa ngày tập.", "success");
        return RedirectToAction(nameof(Details), new { id = planId });
    }

    [HttpGet]
    public async Task<IActionResult> AddExercise(int dayId)
    {
        var day = await _workoutPlanService.GetDayByIdAsync(dayId, GetCurrentUserId());
        if (day is null)
        {
            return NotFound();
        }

        // We need to reload detailed day info to find max order
        var detailedPlan = await _workoutPlanService.GetPlanDetailsAsync(day.PlanID, GetCurrentUserId());
        var detailedDay = detailedPlan?.WorkoutDays.FirstOrDefault(d => d.DayID == dayId);
        int nextOrder = 1;
        if (detailedDay != null && detailedDay.WorkoutExercises.Any())
        {
            nextOrder = detailedDay.WorkoutExercises.Max(item => item.DisplayOrder) + 1;
        }

        await PopulateExerciseOptionsAsync();
        ViewBag.DayLabel = $"Ngày {day.DayNumber} - {day.FocusArea}";
        return View(new WorkoutExerciseFormViewModel
        {
            DayId = dayId,
            DisplayOrder = nextOrder
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddExercise(int dayId, WorkoutExerciseFormViewModel model)
    {
        var day = await _workoutPlanService.GetDayByIdAsync(dayId, GetCurrentUserId());
        if (day is null)
        {
            return NotFound();
        }

        await PopulateExerciseOptionsAsync(model.ExerciseID);
        ViewBag.DayLabel = $"Ngày {day.DayNumber} - {day.FocusArea}";

        var exercise = await _exerciseService.GetByIdAsync(model.ExerciseID);
        if (exercise is null)
        {
            ModelState.AddModelError(nameof(model.ExerciseID), "Bài tập không tồn tại.");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        await _workoutPlanService.AddExerciseAsync(dayId, GetCurrentUserId(), model);
        SetStatus("Đã thêm bài tập vào ngày tập.", "success");
        return RedirectToAction(nameof(Details), new { id = day.PlanID });
    }

    [HttpGet]
    public async Task<IActionResult> EditExercise(int id)
    {
        var item = await _workoutPlanService.GetWorkoutExerciseByIdAsync(id, GetCurrentUserId());
        if (item is null)
        {
            return NotFound();
        }

        await PopulateExerciseOptionsAsync(item.ExerciseID);
        ViewBag.DayLabel = $"Ngày {item.WorkoutDay.DayNumber} - {item.WorkoutDay.FocusArea}";
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
        var item = await _workoutPlanService.GetWorkoutExerciseByIdAsync(id, GetCurrentUserId());
        if (item is null)
        {
            return NotFound();
        }

        await PopulateExerciseOptionsAsync(model.ExerciseID);
        ViewBag.DayLabel = $"Ngày {item.WorkoutDay.DayNumber} - {item.WorkoutDay.FocusArea}";

        var exercise = await _exerciseService.GetByIdAsync(model.ExerciseID);
        if (exercise is null)
        {
            ModelState.AddModelError(nameof(model.ExerciseID), "Bài tập không tồn tại.");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        await _workoutPlanService.EditExerciseAsync(id, GetCurrentUserId(), model);
        SetStatus("Chi tiết bài tập đã được cập nhật.", "success");
        return RedirectToAction(nameof(Details), new { id = item.WorkoutDay.PlanID });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteExercise(int id)
    {
        var item = await _workoutPlanService.GetWorkoutExerciseByIdAsync(id, GetCurrentUserId());
        if (item is null)
        {
            return NotFound();
        }

        var planId = item.WorkoutDay.PlanID;
        await _workoutPlanService.DeleteExerciseAsync(id, GetCurrentUserId());
        SetStatus("Đã xóa bài tập khỏi ngày tập.", "success");
        return RedirectToAction(nameof(Details), new { id = planId });
    }

    private int GetCurrentUserId()
    {
        return User.GetUserId() ?? throw new InvalidOperationException("Authenticated user id is missing.");
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
