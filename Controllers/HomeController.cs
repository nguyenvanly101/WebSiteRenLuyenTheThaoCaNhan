using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebsiteRenLuyenTheThaoCaNhan.Data;
using WebsiteRenLuyenTheThaoCaNhan.Infrastructure;
using WebsiteRenLuyenTheThaoCaNhan.Models;
using WebsiteRenLuyenTheThaoCaNhan.ViewModels;

namespace WebsiteRenLuyenTheThaoCaNhan.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var model = new LandingPageViewModel();

            try
            {
                model = new LandingPageViewModel
                {
                    TotalUsers = await _context.Users.CountAsync(),
                    TotalExercises = await _context.Exercises.CountAsync(),
                    TotalPlans = await _context.WorkoutPlans.CountAsync(),
                    TotalLogs = await _context.WorkoutLogs.CountAsync()
                };
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
