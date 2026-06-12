using Docker_AspNetCore_MVC_Condominio.Web.Controllers;
using Docker_AspNetCore_MVC_Condominio.Web.Data;
using Docker_AspNetCore_MVC_Condominio.Web.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Docker_AspNetCore_MVC_Condominio.Web.Areas.Identity.Pages.Account;

[Authorize(Roles = "Suporte")]
public class RegisterModel(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    RoleManager<IdentityRole> roleManager,
    ILogger<RegisterModel> logger,
    IEmailSender emailSender,
    ApplicationDbContext context) : PageModel
{
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly RoleManager<IdentityRole> _roleManager = roleManager;
    private readonly ILogger<RegisterModel> _logger = logger;
    private readonly IEmailSender _emailSender = emailSender;
    private readonly ApplicationDbContext _context = context;

    [BindProperty]
    public InputModel? Input { get; set; }

    public string? ReturnUrl { get; set; }

    public IList<AuthenticationScheme>? ExternalLogins { get; set; }

    public class InputModel
    {
        [Required(ErrorMessage = "O Nome de usuário é obrigatório.")]
        [Display(Name = "Nome usuário")]
        public required string UserName { get; set; }

        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress]
        [Display(Name = "E-mail")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "A senha é obrigatória.")]
        [StringLength(100, ErrorMessage = "A senha deve ter pelo menos {2} e no máximo {1} caracteres.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Senha")]
        public required string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "A senha e a senha de confirmação não correspondem.")]
        [Display(Name = "Confirmar senha")]
        public required string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "O perfil é obrigatório.")]
        public required string Name { get; set; }

        public Empresa? Empresa { get; set; }

        [Required(ErrorMessage = "A Código Empresa é obrigatório.")]
        public int CodigoEmpresa { get; set; }
    }

    public async Task OnGetAsync(string returnUrl = null!)
    {
        ViewData["roles"] = _roleManager.Roles.ToList();
        ViewData["EmpresaId"] = new SelectList(_context.Empresas, "Id", "RazaoSocial");
        ReturnUrl = returnUrl;
        ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
    }

    public async Task<IActionResult> OnPostAsync(string returnUrl = null!)
    {
        returnUrl ??= Url.Content("~/");
        var role = _roleManager.FindByIdAsync(Input!.Name).Result;
        ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        if (ModelState.IsValid)
        {
            var user = new ApplicationUser { UserName = Input.UserName, Email = Input.Email, EmpresaId = Input.CodigoEmpresa };
            user.EmailConfirmed = true;
            var result = await _userManager.CreateAsync(user, Input.Password);
            if (result.Succeeded)
            {
                _logger.LogInformation("O usuário criou uma nova conta com senha.");
                await _userManager.AddToRoleAsync(user, role!.Name!);
                return RedirectToAction(nameof(UsuariosController.ListarUsuarios), "Usuarios");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
                return RedirectToAction(nameof(error), new { message = "Já existe usuário cadastrado com esse nome." });
            }
        }
        return Page();
    }
}
