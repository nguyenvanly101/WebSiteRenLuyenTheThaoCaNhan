using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebsiteRenLuyenTheThaoCaNhan.Models;
using WebsiteRenLuyenTheThaoCaNhan.Services;
using WebsiteRenLuyenTheThaoCaNhan.ViewModels;

namespace WebsiteRenLuyenTheThaoCaNhan.Controllers
{
    public class HomeController : Controller
    {
        private readonly IAdminService _adminService;

        public HomeController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        public async Task<IActionResult> Index()
        {
            var model = new LandingPageViewModel();

            try
            {
                model = await _adminService.GetLandingPageStatsAsync();
            }
            catch (Exception)
            {
                // Trang chu van duoc hien thi voi so lieu mac dinh neu co so du lieu tam thoi khong truy cap duoc.
            }

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
