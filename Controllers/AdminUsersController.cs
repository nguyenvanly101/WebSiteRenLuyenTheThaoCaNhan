using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebsiteRenLuyenTheThaoCaNhan.Infrastructure;
using WebsiteRenLuyenTheThaoCaNhan.Services;
using WebsiteRenLuyenTheThaoCaNhan.ViewModels;

namespace WebsiteRenLuyenTheThaoCaNhan.Controllers;

[Authorize(Roles = AppRoles.Admin)]
public class AdminUsersController : Controller
{
    private readonly IAdminService _adminService;

    public AdminUsersController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    public async Task<IActionResult> Index()
    {
        var items = await _adminService.GetUsersListAsync();
        return View(items);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var user = await _adminService.GetUserByIdAsync(id);
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
        PopulateRoles(model.Role);

        var currentAdminId = User.GetUserId() ?? 0;
        var (success, errorMessage) = await _adminService.UpdateUserAsync(id, model, currentAdminId);
        
        if (!success)
        {
            if (errorMessage == "Người dùng không tồn tại.")
            {
                return NotFound();
            }

            if (errorMessage == "Tên đăng nhập đã tồn tại.")
            {
                ModelState.AddModelError(nameof(model.Username), errorMessage);
            }
            else if (errorMessage == "Email đã tồn tại.")
            {
                ModelState.AddModelError(nameof(model.Email), errorMessage);
            }
            else if (errorMessage == "Bạn không thể khóa chính tài khoản admin đang sử dụng.")
            {
                ModelState.AddModelError(nameof(model.IsActive), errorMessage);
            }
            else if (errorMessage == "Bạn không thể tự gỡ bỏ quyền admin của chính mình.")
            {
                ModelState.AddModelError(nameof(model.Role), errorMessage);
            }
            else
            {
                ModelState.AddModelError(string.Empty, errorMessage ?? "Lỗi cập nhật người dùng.");
            }

            return View(model);
        }

        SetStatus("Thông tin người dùng đã được cập nhật.", "success");
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await _adminService.GetUserByIdAsync(id);
        return user is null ? NotFound() : View(user);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var currentAdminId = User.GetUserId() ?? 0;
        var (success, errorMessage) = await _adminService.DeleteUserAsync(id, currentAdminId);

        if (!success)
        {
            if (errorMessage == "Người dùng không tồn tại.")
            {
                return NotFound();
            }

            SetStatus(errorMessage ?? "Không thể xóa người dùng.", "danger");
            return RedirectToAction(nameof(Index));
        }

        SetStatus("Người dùng đã được xóa.", "success");
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
