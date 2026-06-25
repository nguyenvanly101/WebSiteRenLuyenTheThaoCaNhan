using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebsiteRenLuyenTheThaoCaNhan.Data;
using WebsiteRenLuyenTheThaoCaNhan.Infrastructure;
using WebsiteRenLuyenTheThaoCaNhan.Models;
using WebsiteRenLuyenTheThaoCaNhan.ViewModels;

namespace WebsiteRenLuyenTheThaoCaNhan.Services
{
    public class AdminService : IAdminService
    {
        private readonly ApplicationDbContext _context;

        public AdminService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<LandingPageViewModel> GetLandingPageStatsAsync()
        {
            return new LandingPageViewModel
            {
                TotalUsers = await _context.Users.CountAsync(),
                TotalExercises = await _context.Exercises.CountAsync(),
                TotalPlans = await _context.WorkoutPlans.CountAsync(),
                TotalLogs = await _context.WorkoutLogs.CountAsync()
            };
        }

        public async Task<AdminDashboardViewModel> GetAdminDashboardStatsAsync()
        {
            return new AdminDashboardViewModel
            {
                TotalUsers = await _context.Users.CountAsync(),
                ActiveUsers = await _context.Users.CountAsync(item => item.IsActive),
                TotalExercises = await _context.Exercises.CountAsync(),
                TotalPlans = await _context.WorkoutPlans.CountAsync(),
                TotalLogs = await _context.WorkoutLogs.CountAsync(),
                RecentUsers = await _context.Users.OrderByDescending(item => item.CreatedAt).Take(5).ToListAsync()
            };
        }

        public async Task<List<AdminUserListItemViewModel>> GetUsersListAsync()
        {
            return await _context.Users
                .OrderByDescending(item => item.CreatedAt)
                .Select(item => new AdminUserListItemViewModel
                {
                    User = item,
                    PlanCount = item.WorkoutPlans.Count,
                    GoalCount = item.Goals.Count,
                    LogCount = item.WorkoutLogs.Count
                })
                .ToListAsync();
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateUserAsync(int id, AdminUserEditViewModel model, int currentAdminId)
        {
            var user = await _context.Users.FindAsync(id);
            if (user is null)
            {
                return (false, "Người dùng không tồn tại.");
            }

            if (await _context.Users.AnyAsync(item => item.UserID != id && item.Username == model.Username.Trim()))
            {
                return (false, "Tên đăng nhập đã tồn tại.");
            }

            if (await _context.Users.AnyAsync(item => item.UserID != id && item.Email == model.Email.Trim()))
            {
                return (false, "Email đã tồn tại.");
            }

            if (currentAdminId == id)
            {
                if (!model.IsActive)
                {
                    return (false, "Bạn không thể khóa chính tài khoản admin đang sử dụng.");
                }

                if (model.Role != AppRoles.Admin)
                {
                    return (false, "Bạn không thể tự gỡ bỏ quyền admin của chính mình.");
                }
            }

            user.FullName = model.FullName.Trim();
            user.Username = model.Username.Trim();
            user.Email = model.Email.Trim();
            user.Role = model.Role;
            user.IsActive = model.IsActive;

            var success = await _context.SaveChangesAsync() > 0;
            return (success, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> DeleteUserAsync(int id, int currentAdminId)
        {
            if (currentAdminId == id)
            {
                return (false, "Bạn không thể xóa chính tài khoản đang đăng nhập.");
            }

            var user = await _context.Users.FindAsync(id);
            if (user is null)
            {
                return (false, "Người dùng không tồn tại.");
            }

            _context.Users.Remove(user);
            var success = await _context.SaveChangesAsync() > 0;
            return (success, null);
        }
    }
}
