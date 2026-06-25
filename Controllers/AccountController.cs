using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebsiteRenLuyenTheThaoCaNhan.Infrastructure;
using WebsiteRenLuyenTheThaoCaNhan.Models;
using WebsiteRenLuyenTheThaoCaNhan.Services;
using WebsiteRenLuyenTheThaoCaNhan.ViewModels;

namespace WebsiteRenLuyenTheThaoCaNhan.Controllers;

[AllowAnonymous]
public class AccountController : Controller
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectAfterLogin();
        }

        ViewData["ReturnUrl"] = returnUrl;
        return View(new LoginViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var login = model.Login.Trim();
        var user = await _accountService.AuthenticateAsync(login, model.Password);

        if (user is null)
        {
            ModelState.AddModelError(string.Empty, "Thông tin đăng nhập không hợp lệ hoặc tài khoản đã bị khóa.");
            return View(model);
        }

        await _accountService.UpdateLastLoginAsync(user.UserID);
        await SignInUserAsync(user, model.RememberMe);

        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectAfterLogin(user.Role);
    }

    [HttpGet]
    public IActionResult Register()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectAfterLogin();
        }

        return View(new RegisterViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var usernameExists = await _accountService.UsernameExistsAsync(model.Username.Trim());
        var emailExists = await _accountService.EmailExistsAsync(model.Email.Trim());

        if (usernameExists)
        {
            ModelState.AddModelError(nameof(model.Username), "Tên đăng nhập đã tồn tại.");
        }

        if (emailExists)
        {
            ModelState.AddModelError(nameof(model.Email), "Email đã được sử dụng.");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = new User
        {
            FullName = model.FullName.Trim(),
            Username = model.Username.Trim(),
            Email = model.Email.Trim(),
            Role = AppRoles.User,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var success = await _accountService.RegisterAsync(user, model.Password);
        if (!success)
        {
            ModelState.AddModelError(string.Empty, "Lỗi tạo tài khoản mới.");
            return View(model);
        }

        await SignInUserAsync(user, false);
        TempData["StatusMessage"] = "Tài khoản đã được tạo thành công.";
        TempData["StatusType"] = "success";

        return RedirectToAction("Index", "Dashboard");
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    private async Task SignInUserAsync(User user, bool isPersistent)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.UserID.ToString()),
            new(ClaimTypes.Name, user.FullName),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role),
            new("username", user.Username)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity),
            new AuthenticationProperties
            {
                IsPersistent = isPersistent,
                ExpiresUtc = isPersistent ? DateTimeOffset.UtcNow.AddDays(7) : null
            });
    }

    private IActionResult RedirectAfterLogin(string? role = null)
    {
        var resolvedRole = role ?? (User.IsInRole(AppRoles.Admin) ? AppRoles.Admin : AppRoles.User);
        return resolvedRole == AppRoles.Admin
            ? RedirectToAction("Index", "Admin")
            : RedirectToAction("Index", "Dashboard");
    }
}
