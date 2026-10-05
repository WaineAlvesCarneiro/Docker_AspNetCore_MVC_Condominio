using Docker_AspNetCore_MVC_Condominio.Web.Data;
using Docker_AspNetCore_MVC_Condominio.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Docker_AspNetCore_MVC_Condominio.Web.Controllers;

[Authorize(Roles = "Sindico,Porteiro")]
public class VisitantesController(ApplicationDbContext context,
                            UserManager<ApplicationUser> userManager) : Controller
{
    private readonly ApplicationDbContext _context = context;
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    [Authorize(Roles = "Sindico,Porteiro")]
    public async Task<IActionResult> ListarVisitantes(string visitante, bool saida)
    {
        ApplicationUser? _applicationUser = await _userManager.GetUserAsync(User);
        int codigo = _applicationUser!.EmpresaId;

        ViewData["visitante"] = visitante;
        ViewData["comDataSaida"] = saida;
        var listImoveis = await LocalizaTodos(visitante, saida, codigo);
        ViewData["visitante"] = null;
        ViewData["comDataSaida"] = null;
        return View(listImoveis);
    }

    public async Task<List<Visitante>> LocalizaTodos(string pessoa, bool saida, int codigo)
    {
        var query = _context.Visitantes
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
    public async Task<IActionResult> DetalharVisitante(int? id)
    {
        if (id == null)
            return RedirectToAction(nameof(Error), new { message = "Id do visitante não foi informado." });

        ApplicationUser? _applicationUser = await _userManager.GetUserAsync(User);
        int codigo = _applicationUser!.EmpresaId;

        var visitante = await _context.Visitantes
            .Include(m => m.Imovel)
            .FirstOrDefaultAsync(m => m.Id == id && m.EmpresaId == codigo);

        if (visitante == null)
            return RedirectToAction(nameof(Error), new { message = "Visitante não encontrado." });

        var imovel = await _context.Imoveis.FirstOrDefaultAsync(obj => obj.Id == visitante.ImovelId);
        if (imovel == null) return NotFound();

        var viewModel = new ImovelViewModel
        {
            Visitante = visitante,
            Imovel = imovel
        };
        return View(viewModel);
    }

    [Authorize(Roles = "Sindico,Porteiro")]
    public async Task<IActionResult> CadastrarVisitante(int id)
    {
        ApplicationUser? _applicationUser = await _userManager.GetUserAsync(User);

        var imovel = await _context.Imoveis.FirstOrDefaultAsync(obj => obj.Id == id);

        var viewModel = new ImovelViewModel
        {
            Imovel = imovel!
        };
        return View(viewModel);
    }

    [Authorize(Roles = "Sindico,Porteiro")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CadastrarVisitante(Imovel imovel, Visitante visitante)
    {
        ApplicationUser? _applicationUser = await _userManager.GetUserAsync(User);
        if (_applicationUser == null) return Challenge();

        visitante.ImovelId = imovel.Id;
        visitante.EmpresaId = imovel.EmpresaId;

        _context.Add(visitante);
        await _context.SaveChangesAsync();

        return RedirectToAction("DetalharImovel", "Imovels", new { id = visitante.ImovelId });
    }

    [HttpGet]
    public async Task<IActionResult> EditarVisitante(int? id)
    {
        if (id == null)
            return RedirectToAction(nameof(Error), new { message = "Id do visitante não foi informado para edição." });

        var lista = await _context.Visitantes
            .Include(v => v.Imovel)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (lista == null) return NotFound();

        var viewModel = new ImovelViewModel
        {
            Visitante = lista,
            Imovel = lista.Imovel
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditarVisitante(int id, Visitante visitante)
    {
        if (id != visitante!.Id)
            return RedirectToAction(nameof(Error), new { message = "Id do visitante inválido." });

        var visitanteBanco = await _context.Visitantes.AsNoTracking().FirstOrDefaultAsync(v => v.Id == id);

        ModelState.Remove(nameof(visitante.Imovel));
        ModelState.Remove(nameof(visitante.Empresa));

        try
        {
            _context.Update(visitante);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!Exists(visitante.Id))
                return RedirectToAction(nameof(Error), new { message = "Erro de concorrência: O morador não existe mais." });
            else
                throw;
        }

        return RedirectToAction(nameof(ListarVisitantes), new { id = visitante.Id });
    }

    [Authorize(Roles = "Sindico")]
    public async Task<IActionResult> ExcluirVisitante(int? id)
    {
        if (id == null)
            return RedirectToAction(nameof(Error), new { message = "Visitante ão foi encontrada para deletar" });

        var visitante = await _context.Visitantes.FirstOrDefaultAsync(m => m.Id == id);
        var imovel = await _context.Imoveis.FirstOrDefaultAsync(obj => obj.Id == visitante!.ImovelId);

        if (visitante == null)
            return RedirectToAction(nameof(Error), new { message = "Esse visitante esta vazio para deletar" });

        var viewModel = new ImovelViewModel
        {
            Visitante = visitante,
            Imovel = imovel
        };
        return View(viewModel);
    }

    [Authorize(Roles = "Sindico")]
    [HttpPost, ActionName("ExcluirVisitante")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ExcluirVisitanteConfirmed(int id)
    {
        var visitante = await _context.Visitantes.FindAsync(id);
        _context.Visitantes.Remove(visitante!);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(ListarVisitantes));
    }

    private bool Exists(int id)
    {
        return _context.Visitantes.Any(e => e.Id == id);
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
