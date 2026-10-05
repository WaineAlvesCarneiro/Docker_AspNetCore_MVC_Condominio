using Docker_AspNetCore_MVC_Condominio.Web.Data;
using Docker_AspNetCore_MVC_Condominio.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Docker_AspNetCore_MVC_Condominio.Web.Controllers;

[Authorize(Roles = "Suporte")]
public class UsuariosController(UserManager<ApplicationUser> userManager, ApplicationDbContext context) : Controller
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly ApplicationDbContext _context = context;

    public async Task<IActionResult> ListarUsuarios(string username, string empresanome, string perfilacesso)
    {
        var usuariosQuery = await _userManager.Users
            .Include(u => u.Empresa)
            .ToListAsync();

        var listaUsuariosVM = new List<UsuarioViewModel>();

        foreach (var user in usuariosQuery)
        {
            var roles = await _userManager.GetRolesAsync(user);

            listaUsuariosVM.Add(new UsuarioViewModel
            {
                Id = user.Id,
                UserName = user.UserName ?? "-",
                Email = user.Email ?? "-",
                Perfil = roles.FirstOrDefault() ?? "Sem Perfil",
                EmpresaNome = user.Empresa?.RazaoSocial ?? "Suporte Sistema"
            });
        }

        if (username == null && empresanome == null && perfilacesso == null)
        {
            return View(listaUsuariosVM);
        }
        else
        {
            ViewData["username"] = username;
            ViewData["empresanome"] = empresanome;
            ViewData["perfilacesso"] = perfilacesso;

            if (!string.IsNullOrWhiteSpace(username))
                listaUsuariosVM = listaUsuariosVM
                    .Where(x => x.UserName != null && x.UserName.Contains(username, StringComparison.OrdinalIgnoreCase))
                    .ToList();

            if (!string.IsNullOrWhiteSpace(empresanome))
                listaUsuariosVM = listaUsuariosVM
                    .Where(x => x.EmpresaNome != null && x.EmpresaNome.Contains(empresanome, StringComparison.OrdinalIgnoreCase))
                    .ToList();

            if (!string.IsNullOrWhiteSpace(perfilacesso))
                listaUsuariosVM = listaUsuariosVM
                    .Where(x => x.Perfil != null && x.Perfil.Contains(perfilacesso, StringComparison.OrdinalIgnoreCase))
                    .ToList();
        }

        ViewData["username"] = null;
        ViewData["empresanome"] = null;
        ViewData["perfilacesso"] = null;
        
        return View(listaUsuariosVM);
    }

    public async Task<IActionResult> DetalharUsuario(string id)
    {
        if (string.IsNullOrEmpty(id)) return NotFound();

        var user = await _userManager.Users
            .Include(u => u.Empresa)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null) return NotFound();

        var roles = await _userManager.GetRolesAsync(user);

        var viewModel = new UsuarioViewModel
        {
            Id = user.Id,
            UserName = user.UserName ?? "-",
            Email = user.Email ?? "-",
            Perfil = roles.FirstOrDefault() ?? "Sem Perfil",
            EmpresaNome = user.Empresa?.RazaoSocial ?? "Suporte Sistema",
            CodigoEmpresa = user.EmpresaId
        };

        ViewBag.IsLockedOut = user.LockoutEnd > DateTimeOffset.UtcNow;

        return View(viewModel);
    }

    public async Task<IActionResult> EditarUsuario(string id)
    {
        if (string.IsNullOrEmpty(id)) return NotFound();

        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id);

        if (user == null) return NotFound();

        var roles = await _userManager.GetRolesAsync(user);

        var viewModel = new UsuarioViewModel
        {
            Id = user.Id,
            UserName = user.UserName ?? "",
            Email = user.Email ?? "",
            Perfil = roles.FirstOrDefault() ?? "",
            CodigoEmpresa = user.EmpresaId
        };

        ViewBag.EmpresaId = new SelectList(_context.Empresas, "Id", "RazaoSocial", user.EmpresaId);
        ViewBag.Roles = new SelectList(await _context.Roles.ToListAsync(), "Name", "Name", viewModel.Perfil);

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditarUsuario(UsuarioViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.EmpresaId = new SelectList(_context.Empresas, "Id", "Fantasia", model.CodigoEmpresa);
            ViewBag.Roles = new SelectList(await _context.Roles.ToListAsync(), "Name", "Name", model.Perfil);
            return View(model);
        }

        var user = await _userManager.FindByIdAsync(model.Id);
        if (user == null) return NotFound();

        user.UserName = model.UserName;
        user.Email = model.Email;
        user.EmpresaId = (int)model.CodigoEmpresa!;

        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            foreach (var error in updateResult.Errors)
                ModelState.AddModelError("", error.Description);

            ViewBag.EmpresaId = new SelectList(_context.Empresas, "Id", "Fantasia", model.CodigoEmpresa);
            ViewBag.Roles = new SelectList(await _context.Roles.ToListAsync(), "Name", "Name", model.Perfil);
            return View(model);
        }

        if (!string.IsNullOrEmpty(model.NovaSenha))
        {
            var removePasswordResult = await _userManager.RemovePasswordAsync(user);

            if (removePasswordResult.Succeeded)
            {
                var addPasswordResult = await _userManager.AddPasswordAsync(user, model.NovaSenha);

                if (!addPasswordResult.Succeeded)
                {
                    foreach (var error in addPasswordResult.Errors)
                        ModelState.AddModelError("", $"Erro na senha: {error.Description}");

                    ViewBag.EmpresaId = new SelectList(_context.Empresas, "Id", "Fantasia", model.CodigoEmpresa);
                    ViewBag.Roles = new SelectList(await _context.Roles.ToListAsync(), "Name", "Name", model.Perfil);
                    return View(model);
                }
            }
            else
            {
                foreach (var error in removePasswordResult.Errors)
                    ModelState.AddModelError("", $"Erro ao resetar senha: {error.Description}");

                ViewBag.EmpresaId = new SelectList(_context.Empresas, "Id", "Fantasia", model.CodigoEmpresa);
                ViewBag.Roles = new SelectList(await _context.Roles.ToListAsync(), "Name", "Name", model.Perfil);
                return View(model);
            }
        }

        var currentRoles = await _userManager.GetRolesAsync(user);
        if (!currentRoles.Contains(model.Perfil))
        {
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!string.IsNullOrEmpty(model.Perfil))
                await _userManager.AddToRoleAsync(user, model.Perfil);
        }

        return RedirectToAction(nameof(ListarUsuarios));
    }

    public async Task<IActionResult> ExcluirUsuario(string id)
    {
        if (string.IsNullOrEmpty(id)) return NotFound();

        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        var roles = await _userManager.GetRolesAsync(user);
        var viewModel = new UsuarioViewModel
        {
            Id = user.Id,
            UserName = user.UserName ?? "-",
            Email = user.Email ?? "-",
            Perfil = roles.FirstOrDefault() ?? "Sem Perfil"
        };

        return View(viewModel);
    }

    [HttpPost, ActionName("ExcluirUsuario")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmaExclusao(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        await _userManager.DeleteAsync(user);
        return RedirectToAction(nameof(ListarUsuarios));
    }

    public IActionResult Error(string message)
    {
        var viewModel = new ErrorViewModel
        {
            Message = message,
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        };
        return View(viewModel);
    }
}