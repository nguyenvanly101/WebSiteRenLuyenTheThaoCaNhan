using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebsiteRenLuyenTheThaoCaNhan.Data;
using WebsiteRenLuyenTheThaoCaNhan.Infrastructure;
using WebsiteRenLuyenTheThaoCaNhan.Models;
using WebsiteRenLuyenTheThaoCaNhan.ViewModels;

namespace WebsiteRenLuyenTheThaoCaNhan.Controllers;

[Authorize(Roles = AppRoles.Admin)]
public class AdminUsersController : Controller
{
    private readonly ApplicationDbContext _context;

    public AdminUsersController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var items = await _context.Users
            .OrderByDescending(item => item.CreatedAt)
            .Select(item => new AdminUserListItemViewModel
            {
                User = item,
                PlanCount = item.WorkoutPlans.Count,
                GoalCount = item.Goals.Count,
                LogCount = item.WorkoutLogs.Count
            })
            .ToListAsync();

        return View(items);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user is null)
        {
            return NotFound();
        }

        PopulateRoles(user.Role);

        return View(new AdminUserEditViewModel
        {
            UserID = user.UserID,
            FullName = user.FullName,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role,
            IsActive = user.IsActive
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, AdminUserEditViewModel model)
    {
        var user = await _context.Users.FindAsync(id);
        if (user is null)
        {
            return NotFound();
        }

        PopulateRoles(model.Role);

        if (await _context.Users.AnyAsync(item => item.UserID != id && item.Username == model.Username.Trim()))
        {
            ModelState.AddModelError(nameof(model.Username), "Ten dang nhap da ton tai.");
        }

        if (await _context.Users.AnyAsync(item => item.UserID != id && item.Email == model.Email.Trim()))
        {
            ModelState.AddModelError(nameof(model.Email), "Email da ton tai.");
        }

        var currentAdminId = User.GetUserId();
        if (currentAdminId == id && !model.IsActive)
        {
            ModelState.AddModelError(nameof(model.IsActive), "Ban khong the khoa chinh tai khoan admin dang su dung.");
        }

        if (currentAdminId == id && model.Role != AppRoles.Admin)
        {
            ModelState.AddModelError(nameof(model.Role), "Ban khong the tu go bo quyen admin cua chinh minh.");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        user.FullName = model.FullName.Trim();
        user.Username = model.Username.Trim();
        user.Email = model.Email.Trim();
        user.Role = model.Role;
        user.IsActive = model.IsActive;

        await _context.SaveChangesAsync();
        SetStatus("Thong tin nguoi dung da duoc cap nhat.", "success");
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await _context.Users.FindAsync(id);
        return user is null ? NotFound() : View(user);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (User.GetUserId() == id)
        {
            SetStatus("Ban khong the xoa chinh tai khoan dang dang nhap.", "danger");
            return RedirectToAction(nameof(Index));
        }

        var user = await _context.Users.FindAsync(id);
        if (user is null)
        {
            return NotFound();
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        SetStatus("Nguoi dung da duoc xoa.", "success");
        return RedirectToAction(nameof(Index));
    }

    private void PopulateRoles(string? role = null)
    {
        ViewBag.Roles = new SelectList(new[] { AppRoles.User, AppRoles.Admin }, role);
    }

    private void SetStatus(string message, string type)
    {
        TempData["StatusMessage"] = message;
        TempData["StatusType"] = type;
    }
}
