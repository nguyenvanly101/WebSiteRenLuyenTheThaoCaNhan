using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebsiteRenLuyenTheThaoCaNhan.Data;
using WebsiteRenLuyenTheThaoCaNhan.Infrastructure;
using WebsiteRenLuyenTheThaoCaNhan.ViewModels;

namespace WebsiteRenLuyenTheThaoCaNhan.Controllers;

[Authorize(Roles = AppRoles.Admin)]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;

    public AdminController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var model = new AdminDashboardViewModel
        {
            TotalUsers = await _context.Users.CountAsync(),
            ActiveUsers = await _context.Users.CountAsync(item => item.IsActive),
            TotalExercises = await _context.Exercises.CountAsync(),
            TotalPlans = await _context.WorkoutPlans.CountAsync(),
            TotalLogs = await _context.WorkoutLogs.CountAsync(),
            RecentUsers = await _context.Users.OrderByDescending(item => item.CreatedAt).Take(5).ToListAsync()
        };

        return View(model);
    }
}
