using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebsiteRenLuyenTheThaoCaNhan.Data;
using WebsiteRenLuyenTheThaoCaNhan.Models;
using WebsiteRenLuyenTheThaoCaNhan.ViewModels;

namespace WebsiteRenLuyenTheThaoCaNhan.Services
{
    public class GoalService : IGoalService
    {
        private readonly ApplicationDbContext _context;

        public GoalService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Goal>> GetUserGoalsAsync(int userId)
        {
            return await _context.Goals
                .Where(item => item.UserID == userId)
                .OrderBy(item => item.EndDate)
                .ToListAsync();
        }

        public async Task<Goal?> GetGoalByIdAsync(int id, int userId)
        {
            return await _context.Goals
                .FirstOrDefaultAsync(item => item.GoalID == id && item.UserID == userId);
        }

        public async Task<bool> CreateGoalAsync(int userId, GoalFormViewModel model)
        {
            var goal = new Goal
            {
                UserID = userId,
                Title = model.Title.Trim(),
                GoalType = model.GoalType,
                TargetValue = model.TargetValue,
                CurrentValue = model.CurrentValue,
                Unit = model.Unit,
                Status = ResolveGoalStatus(model),
                StartDate = model.StartDate,
                EndDate = model.EndDate
            };

            _context.Goals.Add(goal);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateGoalAsync(int id, int userId, GoalFormViewModel model)
        {
            var goal = await _context.Goals
                .FirstOrDefaultAsync(item => item.GoalID == id && item.UserID == userId);

            if (goal is null)
            {
                return false;
            }

            goal.Title = model.Title.Trim();
            goal.GoalType = model.GoalType;
            goal.TargetValue = model.TargetValue;
            goal.CurrentValue = model.CurrentValue;
            goal.Unit = model.Unit;
            goal.Status = ResolveGoalStatus(model);
            goal.StartDate = model.StartDate;
            goal.EndDate = model.EndDate;

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteGoalAsync(int id, int userId)
        {
            var goal = await _context.Goals
                .FirstOrDefaultAsync(item => item.GoalID == id && item.UserID == userId);

            if (goal is null)
            {
                return false;
            }

            _context.Goals.Remove(goal);
            return await _context.SaveChangesAsync() > 0;
        }

        private static string ResolveGoalStatus(GoalFormViewModel model)
        {
            if (model.TargetValue > 0 && model.CurrentValue >= model.TargetValue)
            {
                return "Completed";
            }

            return model.Status;
        }
    }
}
