using Docker_AspNetCore_MVC_Condominio.Web.Data;
using Docker_AspNetCore_MVC_Condominio.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Docker_AspNetCore_MVC_Condominio.Web.Controllers;

[Authorize(Roles = "Suporte")]
public class EmpresasController(ApplicationDbContext context) : Controller
{
    private readonly ApplicationDbContext _context = context;

    [Authorize(Roles = "Suporte")]
    public async Task<IActionResult> ListarEmpresas(string razaosocial, string cnpj)
    {
        ViewData["razaosocial"] = razaosocial;
        ViewData["cnpj"] = cnpj;
        var listEmpresas = await LocalizaTodos(razaosocial, cnpj);
        ViewData["razaosocial"] = null;
        ViewData["cnpj"] = null;
        return View(listEmpresas);
    }

    public async Task<List<Empresa>> LocalizaTodos(string razaosocial, string cnpj)
    {
        var query = _context.Empresas
                            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(razaosocial))
            query = query.Where(x => EF.Functions.Like(x.RazaoSocial, $"%{razaosocial}%"));

        if (!string.IsNullOrWhiteSpace(cnpj))
        {
            cnpj = new string(cnpj.Where(char.IsDigit).ToArray());
            query = query.Where(x => EF.Functions.Like(x.Cnpj, $"%{cnpj}%"));
        }

        return await query.ToListAsync();
    }

    [Authorize(Roles = "Suporte")]
    public async Task<IActionResult> DetalharEmpresa(int? id)
    {
        if (id == null)
            return RedirectToAction(nameof(Error), new { message = "Id do empresa não foi informado." });

        var empresa = await _context.Empresas.FirstOrDefaultAsync(m => m.Id == id);
        if (empresa == null)
            return RedirectToAction(nameof(Error), new { message = "Empresa não foi encontrada." });

        return View(empresa);
    }

    [Authorize(Roles = "Suporte")]
    public IActionResult CadastrarEmpresa()
    {
        return View();
    }

    [Authorize(Roles = "Suporte")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CadastrarEmpresa(Empresa empresa)
    {
        empresa.Cnpj = new string(empresa.Cnpj.Where(char.IsDigit).ToArray());
        empresa.Celular = new string(empresa.Celular.Where(char.IsDigit).ToArray());
        empresa.Cep = new string(empresa.Cep.Where(char.IsDigit).ToArray());

        if (ModelState.IsValid)
        {
            _context.Add(empresa);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ListarEmpresas));
        }
        return View(empresa);
    }

    [Authorize(Roles = "Suporte")]
    public async Task<IActionResult> EditarEmpresa(int? id)
    {
        if (id == null)
            return RedirectToAction(nameof(Error), new { message = "Empresa não foi encontrada para editar" });

        var empresa = await _context.Empresas.FindAsync(id);
        if (empresa == null)
            return RedirectToAction(nameof(Error), new { message = "Essa empresa esta vazia para editar" });

        return View(empresa);
    }

    [Authorize(Roles = "Suporte")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditarEmpresa(int id, Empresa empresa)
    {
        if (id != empresa.Id)
            return RedirectToAction(nameof(Error), new { message = "Empresa não foi encontrada para editar" });

        empresa.Cnpj = new string(empresa.Cnpj.Where(char.IsDigit).ToArray());
        empresa.Celular = new string(empresa.Celular.Where(char.IsDigit).ToArray());
        empresa.Cep = new string(empresa.Cep.Where(char.IsDigit).ToArray());

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(empresa);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Exists(empresa.Id))
                    return RedirectToAction(nameof(Error), new { message = "Essa Empresa não foi encontrada para salva/update a edição" });
                else
                    throw;
            }
            return RedirectToAction(nameof(ListarEmpresas));
        }
        return View(empresa);
    }

    [Authorize(Roles = "Suporte")]
    public async Task<IActionResult> ExcluirEmpresa(int? id)
    {
        if (id == null)
            return RedirectToAction(nameof(Error), new { message = "Empresa não foi encontrada para deletar" });

        var empresa = await _context.Empresas.FirstOrDefaultAsync(m => m.Id == id);
        if (empresa == null)
            return RedirectToAction(nameof(Error), new { message = "Essa Empresa esta vazia para deletar" });

        return View(empresa);
    }

    [Authorize(Roles = "Suporte")]
    [HttpPost, ActionName("ExcluirEmpresa")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ExcluirEmpresaConfirmed(int id)
    {
        var empresa = await _context.Empresas.FindAsync(id);
        _context.Empresas.Remove(empresa!);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(ListarEmpresas));
    }

    private bool Exists(int id)
    {
        return _context.Empresas.Any(e => e.Id == id);
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
