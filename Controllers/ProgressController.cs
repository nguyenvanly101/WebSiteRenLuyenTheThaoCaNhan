using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebsiteRenLuyenTheThaoCaNhan.Infrastructure;
using WebsiteRenLuyenTheThaoCaNhan.Services;

namespace WebsiteRenLuyenTheThaoCaNhan.Controllers;

[Authorize]
public class ProgressController : Controller
{
    private readonly IProgressService _progressService;

    public ProgressController(IProgressService progressService)
    {
        _progressService = progressService;
    }

    public async Task<IActionResult> Index()
    {
        var userId = User.GetUserId();
        if (userId is null)
        {
            return RedirectToAction("Login", "Account");
        }

        var model = await _progressService.GetProgressDataAsync(userId.Value);
        return View(model);
    }
}
