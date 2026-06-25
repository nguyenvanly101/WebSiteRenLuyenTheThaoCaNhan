using System.Threading.Tasks;
using WebsiteRenLuyenTheThaoCaNhan.Models;

namespace WebsiteRenLuyenTheThaoCaNhan.Services
{
    public interface IAccountService
    {
        Task<User?> AuthenticateAsync(string usernameOrEmail, string password);
        Task<bool> RegisterAsync(User user, string password);
        Task<bool> UsernameExistsAsync(string username);
        Task<bool> EmailExistsAsync(string email);
        Task UpdateLastLoginAsync(int userId);
    }
}
