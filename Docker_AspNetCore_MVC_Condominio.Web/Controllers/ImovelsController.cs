using Docker_AspNetCore_MVC_Condominio.Web.Data;
using Docker_AspNetCore_MVC_Condominio.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Docker_AspNetCore_MVC_Condominio.Web.Controllers;

[Authorize(Roles = "Sindico,Porteiro")]
public class ImovelsController(ApplicationDbContext context,
                        UserManager<ApplicationUser> userManager) : Controller
{
    private readonly ApplicationDbContext _context = context;
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    [Authorize(Roles = "Sindico,Porteiro")]
    public async Task<IActionResult> ListarImoveis(string bloco, string apartamento)
    {
        ApplicationUser? _applicationUser = await _userManager.GetUserAsync(User);
        int codigo = _applicationUser!.EmpresaId;

        ViewData["bloco"] = bloco;
        ViewData["apartamento"] = apartamento;
        var imoveis = await LocalizaTodos(bloco, apartamento, codigo);
        ViewData["bloco"] = null;
        ViewData["apartamento"] = null;
        return View(imoveis);
    }

    public async Task<List<Imovel>> LocalizaTodos(string bloco, string apartamento, int codigo)
    {
        var query = _context.Imoveis.Where(x => x.EmpresaId == codigo).AsQueryable();

        if (!string.IsNullOrWhiteSpace(bloco))
            query = query.Where(x => EF.Functions.Like(x.Bloco, $"%{bloco}%"));

        if (!string.IsNullOrWhiteSpace(apartamento))
            query = query.Where(x => EF.Functions.Like(x.Apartamento, $"%{apartamento}%"));

        return await query.ToListAsync();
    }

    [Authorize(Roles = "Sindico,Porteiro")]
    public async Task<IActionResult> DetalharImovel(int? id)
    {
        if (id == null)
            return RedirectToAction(nameof(Error), new { message = "Id do imovel não foi informado" });

        var imovel = await _context.Imoveis.FirstOrDefaultAsync(m => m.Id == id);
        var listMoradores = await ListMoradoresSemDataDeSaidaDoImovelAsync(imovel!.Id, imovel.EmpresaId);
        var viewModel = new ImovelViewModel
        {
            Imovel = imovel,
            Moradores = listMoradores
        };
        return View(viewModel);
    }

    public async Task<List<Morador>> ListMoradoresSemDataDeSaidaDoImovelAsync(int? id, int? empresaId)
    {
        var query = _context.Moradores
                            .Where(x => x.EmpresaId == empresaId && x.DataSaida == null)
                            .Include(i => i.Imovel)
                            .AsQueryable();

        return await query.ToListAsync();
    }

    [Authorize(Roles = "Sindico")]
    public IActionResult CadastrarImovel()
    {
        return View();
    }

    [Authorize(Roles = "Sindico")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CadastrarImovel(Imovel imovel)
    {
        ApplicationUser? _applicationUser = await _userManager.GetUserAsync(User);

        if (!ModelState.IsValid)
        {
            var viewModel = new ImovelViewModel
            {
                Imovel = await _context.Imoveis.FirstOrDefaultAsync(obj => obj.Id == imovel.Id)
            };
            return View(viewModel);
        }

        imovel.EmpresaId = _applicationUser!.EmpresaId;

        _context.Add(imovel);
        await _context.SaveChangesAsync();

        var imovelId = await _context.Imoveis.FirstOrDefaultAsync(m => m.Id == imovel.Id);
        return RedirectToAction(nameof(ListarImoveis));
    }

    [Authorize(Roles = "Sindico")]
    public async Task<IActionResult> EditarImovel(int? id)
    {
        if (id == null)
            return RedirectToAction(nameof(Error), new { message = "Imóvel não foi encontrado para editar" });

        var imovelReturn = await _context.Imoveis.FirstOrDefaultAsync(obj => obj.Id == id);
        if (imovelReturn == null)
            return RedirectToAction(nameof(Error), new { message = "Esse imóvel esta vazio para editar" });

        var viewModel = new ImovelViewModel
        {
            Imovel = imovelReturn
        };
        return View(viewModel);
    }

    [Authorize(Roles = "Sindico")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditarImovel(int id, Imovel imovel)
    {
        if (!ModelState.IsValid)
        {
            var imovelReturn = await _context.Imoveis.FirstOrDefaultAsync(obj => obj.Id == imovel.Id);

            var viewModel = new ImovelViewModel
            {
                Imovel = imovelReturn
            };
            return View(viewModel);
        }
        if (id != imovel.Id)
            return RedirectToAction(nameof(Error), new { message = "Imóvel não foi encontrado para editar" });

        try
        {
            _context.Update(imovel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ListarImoveis), imovel);
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!Exists(imovel.Id))
                return RedirectToAction(nameof(Error), new { message = "Esse imóvel não foi encontrado para salvar/update a edição" });
            else
                throw;
        }
    }

    [Authorize(Roles = "Sindico")]
    public async Task<IActionResult> ExcluirImovel(int? id)
    {
        if (id == null)
            return RedirectToAction(nameof(Error), new { message = "Imovel não foi encontrada para deletar" });

        var imovel = await _context.Imoveis.FirstOrDefaultAsync(m => m.Id == id);
        if (imovel == null)
            return RedirectToAction(nameof(Error), new { message = "Esse Imovel esta vazio para deletar" });

        var listMoradores = await ListMoradoresSemDataDeSaidaDoImovelAsync(imovel.Id, imovel.EmpresaId);
        var viewModel = new ImovelViewModel
        {
            Imovel = imovel,
            Moradores = listMoradores
        };
        return View(viewModel);
    }

    [Authorize(Roles = "Sindico")]
    [HttpPost, ActionName("ExcluirImovel")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ExcluirImovelConfirmed(int id)
    {
        var imovel = await _context.Imoveis.FindAsync(id);

        _context.Imoveis.Remove(imovel!);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(ListarImoveis));
    }

    private bool Exists(int id)
    {
        return _context.Imoveis.Any(e => e.Id == id);
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