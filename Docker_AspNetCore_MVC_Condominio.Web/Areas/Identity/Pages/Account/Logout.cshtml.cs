using Docker_AspNetCore_MVC_Condominio.Web.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Docker_AspNetCore_MVC_Condominio.Web.Areas.Identity.Pages.Account;

[AllowAnonymous]
public class LogoutModel(SignInManager<ApplicationUser> signInManager, ILogger<LogoutModel> logger) : PageModel
{
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private readonly ILogger<LogoutModel> _logger = logger;

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPost(string returnUrl = null!)
    {
        await _signInManager.SignOutAsync();
        _logger.LogInformation("Usuário desconectado.");
        if (returnUrl != null)
        {
            return LocalRedirect(returnUrl);
        }
        else
        {
            return RedirectToPage();
        }
    }
}
