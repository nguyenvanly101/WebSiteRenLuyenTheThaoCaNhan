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
public class GoalsController : Controller
{
    private readonly ApplicationDbContext _context;

    public GoalsController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var goals = await _context.Goals
            .Where(item => item.UserID == GetCurrentUserId())
            .OrderBy(item => item.EndDate)
            .ToListAsync();

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

        _context.Goals.Add(new Goal
        {
            UserID = GetCurrentUserId(),
            Title = model.Title.Trim(),
            GoalType = model.GoalType,
            TargetValue = model.TargetValue,
            CurrentValue = model.CurrentValue,
            Unit = model.Unit,
            Status = ResolveGoalStatus(model),
            StartDate = model.StartDate,
            EndDate = model.EndDate
        });

        await _context.SaveChangesAsync();
        SetStatus("Muc tieu da duoc tao.", "success");
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var goal = await GetOwnedGoalsQuery().FirstOrDefaultAsync(item => item.GoalID == id);
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
        var goal = await GetOwnedGoalsQuery().FirstOrDefaultAsync(item => item.GoalID == id);
        if (goal is null)
        {
            return NotFound();
        }

        PopulateGoalSelections(model.GoalType, model.Status, model.Unit);
        ValidateGoalDates(model);

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        goal.Title = model.Title.Trim();
        goal.GoalType = model.GoalType;
        goal.TargetValue = model.TargetValue;
        goal.CurrentValue = model.CurrentValue;
        goal.Unit = model.Unit;
        goal.Status = ResolveGoalStatus(model);
        goal.StartDate = model.StartDate;
        goal.EndDate = model.EndDate;

        await _context.SaveChangesAsync();
        SetStatus("Muc tieu da duoc cap nhat.", "success");
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var goal = await GetOwnedGoalsQuery().FirstOrDefaultAsync(item => item.GoalID == id);
        return goal is null ? NotFound() : View(goal);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var goal = await GetOwnedGoalsQuery().FirstOrDefaultAsync(item => item.GoalID == id);
        if (goal is null)
        {
            return NotFound();
        }

        _context.Goals.Remove(goal);
        await _context.SaveChangesAsync();
        SetStatus("Muc tieu da duoc xoa.", "success");
        return RedirectToAction(nameof(Index));
    }

    private IQueryable<Goal> GetOwnedGoalsQuery()
    {
        var userId = GetCurrentUserId();
        return _context.Goals.Where(item => item.UserID == userId);
    }

    private int GetCurrentUserId()
    {
        return User.GetUserId() ?? throw new InvalidOperationException("Authenticated user id is missing.");
    }

    private static string ResolveGoalStatus(GoalFormViewModel model)
    {
        if (model.TargetValue > 0 && model.CurrentValue >= model.TargetValue)
        {
            return "Completed";
        }

        return model.Status;
    }

    private void ValidateGoalDates(GoalFormViewModel model)
    {
        if (model.EndDate < model.StartDate)
        {
            ModelState.AddModelError(nameof(model.EndDate), "Ngay ket thuc phai sau ngay bat dau.");
        }
    }

    private void PopulateGoalSelections(string? goalType = null, string? status = null, string? unit = null)
    {
        ViewBag.GoalTypes = new SelectList(new[] { "Giam can", "Tang co", "Tang suc ben", "Cai thien thanh tich" }, goalType);
        ViewBag.GoalStatuses = new SelectList(new[] { "Active", "Paused", "Completed" }, status);
        ViewBag.Units = new SelectList(new[] { "kg", "%", "km", "buoi", "cm" }, unit);
    }

    private void SetStatus(string message, string type)
    {
        TempData["StatusMessage"] = message;
        TempData["StatusType"] = type;
    }
}
