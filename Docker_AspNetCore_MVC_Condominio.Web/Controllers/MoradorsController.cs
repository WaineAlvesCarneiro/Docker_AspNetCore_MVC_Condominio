using Docker_AspNetCore_MVC_Condominio.Web.Data;
using Docker_AspNetCore_MVC_Condominio.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Docker_AspNetCore_MVC_Condominio.Web.Controllers;

[Authorize(Roles = "Sindico,Porteiro")]
public class MoradorsController(ApplicationDbContext context,
                        UserManager<ApplicationUser> userManager) : Controller
{
    private readonly ApplicationDbContext _context = context;
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    [Authorize(Roles = "Sindico,Porteiro")]
    public async Task<IActionResult> ListarMoradores(string morador, bool saida)
    {
        ApplicationUser? _applicationUser = await _userManager.GetUserAsync(User);
        int codigo = _applicationUser!.EmpresaId;

        ViewData["morador"] = morador;
        ViewData["comDataSaida"] = saida;
        var listMoradoes = await LocalizaTodos(morador, saida, codigo);
        ViewData["morador"] = null;
        ViewData["comDataSaida"] = null;
        return View(listMoradoes);
    }
    public async Task<List<Morador>> LocalizaTodos(string pessoa, bool saida, int codigo)
    {
        var query = _context.Moradores
                            .Where(x => x.EmpresaId == codigo)
                            .Include(i => i.Imovel)
                            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(pessoa))
            query = query.Where(x => EF.Functions.Like(x.Nome, $"%{pessoa}%"));

        query = saida
            ? query.Where(x => x.DataSaida != null)
            : query.Where(x => x.DataSaida == null);

        query = query.OrderByDescending(x => x.DataEntrada);

        return await query.ToListAsync();
    }

    [Authorize(Roles = "Sindico,Porteiro")]
    public async Task<IActionResult> DetalharMorador(int? id)
    {
        if (id == null)
            return RedirectToAction(nameof(Error), new { message = "Id do morador não foi informado." });

        ApplicationUser? _applicationUser = await _userManager.GetUserAsync(User);
        int codigo = _applicationUser!.EmpresaId;

        var morador = await _context.Moradores
            .Include(m => m.Imovel)
            .FirstOrDefaultAsync(m => m.Id == id && m.EmpresaId == codigo);

        if (morador == null)
            return RedirectToAction(nameof(Error), new { message = "Morador não encontrado." });

        var imovel = await _context.Imoveis.FirstOrDefaultAsync(obj => obj.Id == morador.ImovelId);
        if (imovel == null) return NotFound();

        var viewModel = new ImovelViewModel
        {
            Morador = morador,
            Imovel = imovel
        };

        return View(viewModel);
    }

    [Authorize(Roles = "Sindico")]
    [HttpGet]
    public async Task<IActionResult> CadastrarMoradores(int id)
    {
        ApplicationUser? _applicationUser = await _userManager.GetUserAsync(User);
        if (_applicationUser == null) return Challenge();

        var imovel = await _context.Imoveis.FirstOrDefaultAsync(obj => obj.Id == id);
        if (imovel == null) return NotFound();

        var viewModel = new ImovelViewModel
        {
            Imovel = imovel
        };

        return View(viewModel);
    }

    [Authorize(Roles = "Sindico")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CadastrarMoradores(ImovelViewModel model)
    {
        ApplicationUser? _applicationUser = await _userManager.GetUserAsync(User);
        if (_applicationUser == null) return Challenge();

        model.Morador!.EmpresaId = _applicationUser.EmpresaId;
        model.Morador!.ImovelId = model.Imovel!.Id;

        _context.Add(model.Morador);
        await _context.SaveChangesAsync();

        return RedirectToAction("DetalharImovel", "Imovels", new { id = model.Imovel.Id });
    }

    [Authorize(Roles = "Sindico")]
    [HttpGet]
    public async Task<IActionResult> EditarMorador(int? id)
    {
        if (id == null)
            return RedirectToAction(nameof(Error), new { message = "Id do morador não foi informado para edição." });

        ApplicationUser? _applicationUser = await _userManager.GetUserAsync(User);
        int codigo = _applicationUser!.EmpresaId;

        var morador = await _context.Moradores.Include(m => m.Imovel).FirstOrDefaultAsync(m => m.Id == id && m.EmpresaId == codigo);

        if (morador == null)
            return RedirectToAction(nameof(Error), new { message = "Morador não encontrado para edição." });

        var viewModel = new ImovelViewModel
        {
            Morador = morador,
            Imovel = morador.Imovel
        };

        return View(viewModel);
    }

    [Authorize(Roles = "Sindico")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditarMorador(int id, Morador morador)
    {
        if (id != morador.Id)
            return RedirectToAction(nameof(Error), new { message = "Id do morador inválido." });

        var moradorBanco = await _context.Moradores
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id && m.EmpresaId == morador.EmpresaId);

        if (moradorBanco == null)
            return RedirectToAction(nameof(Error), new { message = "Morador não encontrado no banco." });

        try
        {
            _context.Update(morador);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!Exists(morador.Id))
                return RedirectToAction(nameof(Error), new { message = "Erro de concorrência: O morador não existe mais." });
            else
                throw;
        }

        return RedirectToAction(nameof(ListarMoradores), new { id = morador.Id });
    }

    [Authorize(Roles = "Sindico")]
    public async Task<IActionResult> ExcluirMorador(int? id)
    {
        if (id == null)
            return RedirectToAction(nameof(Error), new { message = "Morador não foi encontrado para deletar" });

        var morador = await _context.Moradores.FirstOrDefaultAsync(m => m.Id == id);
        var imovel = await _context.Imoveis.FirstOrDefaultAsync(obj => obj.Id == morador!.ImovelId);

        if (morador == null)
            return RedirectToAction(nameof(Error), new { message = "Esse morador esta vazio para deletar" });

        var viewModel = new ImovelViewModel
        {
            Morador = morador,
            Imovel = imovel
        };
        return View(viewModel);
    }

    [Authorize(Roles = "Sindico")]
    [HttpPost, ActionName("ExcluirMorador")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ExcluirMoradorConfirmed(int id)
    {
        var morador = await _context.Moradores.FindAsync(id);
        Imovel? imovel = await _context.Imoveis.FirstOrDefaultAsync(obj => obj.Id == morador!.ImovelId);

        _context.Moradores.Remove(morador!);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(ListarMoradores), imovel);
    }

    private bool Exists(int id)
    {
        return _context.Moradores.Any(e => e.Id == id);
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