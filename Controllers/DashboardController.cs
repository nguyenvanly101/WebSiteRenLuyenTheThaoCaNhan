using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebsiteRenLuyenTheThaoCaNhan.Infrastructure;
using WebsiteRenLuyenTheThaoCaNhan.Services;

namespace WebsiteRenLuyenTheThaoCaNhan.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly IProgressService _progressService;

    public DashboardController(IProgressService progressService)
    {
        _progressService = progressService;
    }

    public async Task<IActionResult> Index()
    {
        if (User.IsInRole(AppRoles.Admin))
        {
            return RedirectToAction("Index", "Admin");
        }

        var userId = User.GetUserId();
        if (userId is null)
        {
            return RedirectToAction("Login", "Account");
        }

        var model = await _progressService.GetDashboardDataAsync(userId.Value);
        return View(model);
    }
}
