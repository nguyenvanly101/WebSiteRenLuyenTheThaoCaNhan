using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebsiteRenLuyenTheThaoCaNhan.Infrastructure;
using WebsiteRenLuyenTheThaoCaNhan.Models;
using WebsiteRenLuyenTheThaoCaNhan.Services;

namespace WebsiteRenLuyenTheThaoCaNhan.Controllers;

[Authorize(Roles = AppRoles.Admin)]
public class AdminExercisesController : Controller
{
    private readonly IExerciseService _exerciseService;

    public AdminExercisesController(IExerciseService exerciseService)
    {
        _exerciseService = exerciseService;
    }

    public async Task<IActionResult> Index()
    {
        var items = await _exerciseService.GetAdminExercisesAsync();
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

        await _exerciseService.CreateAsync(model);
        SetStatus("Bài tập đã được tạo.", "success");
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var exercise = await _exerciseService.GetByIdAsync(id);
        return exercise is null ? NotFound() : View(exercise);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Exercise model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _exerciseService.UpdateAsync(id, model);
        if (!result)
        {
            return NotFound();
        }

        SetStatus("Bài tập đã được cập nhật.", "success");
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var exercise = await _exerciseService.GetByIdAsync(id);
        return exercise is null ? NotFound() : View(exercise);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            var result = await _exerciseService.DeleteAsync(id);
            if (!result)
            {
                return NotFound();
            }
            SetStatus("Bài tập đã được xóa.", "success");
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateException)
        {
            SetStatus("Không thể xóa bài tập đang được sử dụng trong lịch tập hoặc buổi tập.", "danger");
        }

        return RedirectToAction(nameof(Index));
    }

    private void SetStatus(string message, string type)
    {
        TempData["StatusMessage"] = message;
        TempData["StatusType"] = type;
    }
}
