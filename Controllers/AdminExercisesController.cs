using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebsiteRenLuyenTheThaoCaNhan.Data;
using WebsiteRenLuyenTheThaoCaNhan.Infrastructure;
using WebsiteRenLuyenTheThaoCaNhan.Models;
using WebsiteRenLuyenTheThaoCaNhan.ViewModels;

namespace WebsiteRenLuyenTheThaoCaNhan.Controllers;

[Authorize(Roles = AppRoles.Admin)]
public class AdminExercisesController : Controller
{
    private readonly ApplicationDbContext _context;

    public AdminExercisesController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var items = await _context.Exercises
            .OrderBy(item => item.Name)
            .Select(item => new AdminExerciseListItemViewModel
            {
                Exercise = item,
                PlanUsageCount = item.WorkoutExercises.Count,
                LogUsageCount = item.WorkoutLogDetails.Count
            })
            .ToListAsync();

        return View(items);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new Exercise());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Exercise model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        model.CreatedAt = DateTime.UtcNow;
        _context.Exercises.Add(model);
        await _context.SaveChangesAsync();

        SetStatus("Bai tap da duoc tao.", "success");
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var exercise = await _context.Exercises.FindAsync(id);
        return exercise is null ? NotFound() : View(exercise);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Exercise model)
    {
        var exercise = await _context.Exercises.FindAsync(id);
        if (exercise is null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        exercise.Name = model.Name.Trim();
        exercise.MuscleGroup = model.MuscleGroup.Trim();
        exercise.Equipment = model.Equipment.Trim();
        exercise.Difficulty = model.Difficulty.Trim();
        exercise.Description = model.Description.Trim();
        exercise.VideoUrl = model.VideoUrl.Trim();

        await _context.SaveChangesAsync();
        SetStatus("Bai tap da duoc cap nhat.", "success");
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var exercise = await _context.Exercises.FindAsync(id);
        return exercise is null ? NotFound() : View(exercise);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var exercise = await _context.Exercises.FindAsync(id);
        if (exercise is null)
        {
            return NotFound();
        }

        try
        {
            _context.Exercises.Remove(exercise);
            await _context.SaveChangesAsync();
            SetStatus("Bai tap da duoc xoa.", "success");
        }
        catch (DbUpdateException)
        {
            SetStatus("Khong the xoa bai tap dang duoc su dung trong lich tap hoac buoi tap.", "danger");
        }

        return RedirectToAction(nameof(Index));
    }

    private void SetStatus(string message, string type)
    {
        TempData["StatusMessage"] = message;
        TempData["StatusType"] = type;
    }
}
