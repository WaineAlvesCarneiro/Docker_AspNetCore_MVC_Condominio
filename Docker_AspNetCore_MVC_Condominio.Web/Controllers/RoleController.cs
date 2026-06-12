using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Docker_AspNetCore_MVC_Condominio.Web.Controllers;

[Authorize(Roles = "Suporte")]
public class RoleController(RoleManager<IdentityRole> roleManager) : Controller
{
    readonly RoleManager<IdentityRole> _roleManager = roleManager;

    [Authorize(Roles = "Suporte")]
    public IActionResult ListarRoles()
    {
        var roles = _roleManager.Roles.ToList();
        return View(roles);
    }

    [Authorize(Roles = "Suporte")]
    public IActionResult CriarRole()
    {
        return View(new IdentityRole());
    }

    [Authorize(Roles = "Suporte")]
    [HttpPost]
    public async Task<IActionResult> CriarRole(IdentityRole role)
    {
        await _roleManager.CreateAsync(role);
        return RedirectToAction("ListarRoles");
    }
}
