using Docker_AspNetCore_MVC_Condominio.Web.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;

namespace Docker_AspNetCore_MVC_Condominio.Web.Areas.Identity.Pages.Account.Manage;

public partial class EmailModel(
    UserManager<ApplicationUser> userManager,
    IEmailSender emailSender) : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IEmailSender _emailSender = emailSender;

    public required string Username { get; set; }

    public required string Email { get; set; }

    public bool IsEmailConfirmed { get; set; }

    [TempData]
    public required string StatusMessage { get; set; }

    [BindProperty]
    public required InputModel Input { get; set; }

    public class InputModel
    {
        [Required(ErrorMessage = "O campo Novo email é obrigatório.")]
        [EmailAddress]
        [Display(Name = "Novo email")]
        public required string NewEmail { get; set; }
    }

    private async Task LoadAsync(ApplicationUser user)
    {
        var email = await _userManager.GetEmailAsync(user);
        Email = email!;

        Input = new InputModel
        {
            NewEmail = email!,
        };

        IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Incapaz de carregar usuário com ID  '{_userManager.GetUserId(User)}'.");
        }

        await LoadAsync(user);
        return Page();
    }

    public async Task<IActionResult> OnPostChangeEmailAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Incapaz de carregar usuário com ID  '{_userManager.GetUserId(User)}'.");
        }

        if (!ModelState.IsValid)
        {
            await LoadAsync(user);
            return Page();
        }

        var email = await _userManager.GetEmailAsync(user);
        if (Input.NewEmail != email)
        {
            var userId = await _userManager.GetUserIdAsync(user);
            var code = await _userManager.GenerateChangeEmailTokenAsync(user, Input.NewEmail);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmailChange",
                pageHandler: null,
                values: new { userId = userId, email = Input.NewEmail, code = code },
                protocol: Request.Scheme);
            await _emailSender.SendEmailAsync(
                Input.NewEmail,
                "Confirme seu email",
                $"Por favor, confirme sua conta até <a href='{HtmlEncoder.Default.Encode(callbackUrl!)}'>clicando aqui</a>.");

            StatusMessage = "Link de confirmação para alteração de e-mail enviado. Por favor verifique seu email.";
            return RedirectToPage();
        }

        StatusMessage = "Seu e-mail não foi alterado.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostSendVerificationEmailAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Incapaz de carregar usuário com ID '{_userManager.GetUserId(User)}'.");
        }

        if (!ModelState.IsValid)
        {
            await LoadAsync(user);
            return Page();
        }

        var userId = await _userManager.GetUserIdAsync(user);
        var email = await _userManager.GetEmailAsync(user);
        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        var callbackUrl = Url.Page(
            "/Account/ConfirmEmail",
            pageHandler: null,
            values: new { area = "Identity", userId = userId, code = code },
            protocol: Request.Scheme);
        await _emailSender.SendEmailAsync(
            email!,
            "Confirme seu email",
            $"Por favor, confirme sua conta até <a href='{HtmlEncoder.Default.Encode(callbackUrl!)}'>clicando aqui</a>.");

        StatusMessage = "E-mail de verificação enviado. Por favor verifique seu email.";
        return RedirectToPage();
    }
}
