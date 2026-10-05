using Docker_AspNetCore_MVC_Condominio.Web.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace Docker_AspNetCore_MVC_Condominio.Web.Areas.Identity.Pages.Account.Manage;

public class ChangePasswordModel(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    ILogger<ChangePasswordModel> logger) : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private readonly ILogger<ChangePasswordModel> _logger = logger;

    [BindProperty]
    public required InputModel Input { get; set; }

    [TempData]
    public required string StatusMessage { get; set; }

    public class InputModel
    {
        [Required(ErrorMessage = "É obrigatório informar a senha atual.")]
        [DataType(DataType.Password)]
        [Display(Name = "Senha atual")]
        public required string OldPassword { get; set; }

        [Required(ErrorMessage = "A nova senha é obrigatória.")]
        [StringLength(100, ErrorMessage = "A senha deve ter pelo menos {2} e no máximo {1} caracteres.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Nova senha")]
        public required string NewPassword { get; set; }

        [Display(Name = "Confirmar senha")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "A senha e a senha de confirmação não correspondem.")]
        public required string ConfirmPassword { get; set; }
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Incapaz de carregar usuário com ID '{_userManager.GetUserId(User)}'.");
        }

        var hasPassword = await _userManager.HasPasswordAsync(user);
        if (!hasPassword)
        {
            return RedirectToPage("./SetPassword");
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Incapaz de carregar usuário com ID '{_userManager.GetUserId(User)}'.");
        }

        var changePasswordResult = await _userManager.ChangePasswordAsync(user, Input.OldPassword, Input.NewPassword);
        if (!changePasswordResult.Succeeded)
        {
            foreach (var error in changePasswordResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return Page();
        }

        await _signInManager.RefreshSignInAsync(user);
        _logger.LogInformation("O usuário alterou a senha com sucesso.");
        StatusMessage = "Sua senha foi alterada.";

        return RedirectToPage();
    }
}
