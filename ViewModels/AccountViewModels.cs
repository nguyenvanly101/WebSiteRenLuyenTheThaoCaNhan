using System.ComponentModel.DataAnnotations;

namespace WebsiteRenLuyenTheThaoCaNhan.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "Vui long nhap ten dang nhap hoac email.")]
    [Display(Name = "Ten dang nhap hoac Email")]
    public string Login { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui long nhap mat khau.")]
    [DataType(DataType.Password)]
    [Display(Name = "Mat khau")]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Ghi nho dang nhap")]
    public bool RememberMe { get; set; }
}

public class RegisterViewModel
{
    [Required(ErrorMessage = "Vui long nhap ho ten.")]
    [StringLength(80)]
    [Display(Name = "Ho ten")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui long nhap ten dang nhap.")]
    [StringLength(40)]
    [Display(Name = "Ten dang nhap")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui long nhap email.")]
    [EmailAddress]
    [StringLength(120)]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui long nhap mat khau.")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Mat khau toi thieu 8 ky tu.")]
    [DataType(DataType.Password)]
    [Display(Name = "Mat khau")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui long xac nhan mat khau.")]
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "Mat khau xac nhan khong khop.")]
    [Display(Name = "Xac nhan mat khau")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
