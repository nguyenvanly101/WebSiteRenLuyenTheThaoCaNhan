using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebsiteRenLuyenTheThaoCaNhan.Infrastructure;
using WebsiteRenLuyenTheThaoCaNhan.Services;
using WebsiteRenLuyenTheThaoCaNhan.ViewModels;

namespace WebsiteRenLuyenTheThaoCaNhan.Controllers;

[Authorize]
public class GoalsController : Controller
{
    private readonly IGoalService _goalService;

    public GoalsController(IGoalService goalService)
    {
        _goalService = goalService;
    }

    public async Task<IActionResult> Index()
    {
        var goals = await _goalService.GetUserGoalsAsync(GetCurrentUserId());
        return View(goals);
    }

    [HttpGet]
    public IActionResult Create()
    {
        PopulateGoalSelections();
        return View(new GoalFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(GoalFormViewModel model)
    {
        PopulateGoalSelections(model.GoalType, model.Status, model.Unit);
        ValidateGoalDates(model);

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        await _goalService.CreateGoalAsync(GetCurrentUserId(), model);
        SetStatus("Mục tiêu đã được tạo.", "success");
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var goal = await _goalService.GetGoalByIdAsync(id, GetCurrentUserId());
        if (goal is null)
        {
            return NotFound();
        }

        PopulateGoalSelections(goal.GoalType, goal.Status, goal.Unit);
        return View(new GoalFormViewModel
        {
            Title = goal.Title,
            GoalType = goal.GoalType,
            TargetValue = goal.TargetValue,
            CurrentValue = goal.CurrentValue,
            Unit = goal.Unit,
            Status = goal.Status,
            StartDate = goal.StartDate,
            EndDate = goal.EndDate
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, GoalFormViewModel model)
    {
        PopulateGoalSelections(model.GoalType, model.Status, model.Unit);
        ValidateGoalDates(model);

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _goalService.UpdateGoalAsync(id, GetCurrentUserId(), model);
        if (!result)
        {
            return NotFound();
        }

        SetStatus("Mục tiêu đã được cập nhật.", "success");
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var goal = await _goalService.GetGoalByIdAsync(id, GetCurrentUserId());
        return goal is null ? NotFound() : View(goal);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var result = await _goalService.DeleteGoalAsync(id, GetCurrentUserId());
        if (!result)
        {
            return NotFound();
        }

        SetStatus("Mục tiêu đã được xóa.", "success");
        return RedirectToAction(nameof(Index));
    }

    private int GetCurrentUserId()
    {
        return User.GetUserId() ?? throw new InvalidOperationException("Authenticated user id is missing.");
    }

    private void ValidateGoalDates(GoalFormViewModel model)
    {
        if (model.EndDate < model.StartDate)
        {
            ModelState.AddModelError(nameof(model.EndDate), "Ngày kết thúc phải sau ngày bắt đầu.");
        }
    }

    private void PopulateGoalSelections(string? goalType = null, string? status = null, string? unit = null)
    {
        ViewBag.GoalTypes = new SelectList(new[] { "Giảm cân", "Tăng cơ", "Tăng sức bền", "Cải thiện thành tích" }, goalType);
        ViewBag.GoalStatuses = new SelectList(new[] { "Active", "Paused", "Completed" }, status);
        ViewBag.Units = new SelectList(new[] { "kg", "%", "km", "buổi", "cm" }, unit);
    }

    private void SetStatus(string message, string type)
    {
        TempData["StatusMessage"] = message;
        TempData["StatusType"] = type;
    }
}
