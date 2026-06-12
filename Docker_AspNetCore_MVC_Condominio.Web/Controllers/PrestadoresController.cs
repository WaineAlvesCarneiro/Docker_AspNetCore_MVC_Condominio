using Docker_AspNetCore_MVC_Condominio.Web.Data;
using Docker_AspNetCore_MVC_Condominio.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Docker_AspNetCore_MVC_Condominio.Web.Controllers;

[Authorize(Roles = "Sindico,Porteiro")]
public class PrestadoresController(ApplicationDbContext context,
                            UserManager<ApplicationUser> userManager) : Controller
{
    private readonly ApplicationDbContext _context = context;
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    [Authorize(Roles = "Sindico,Porteiro")]
    public async Task<IActionResult> ListarPrestadores(string prestador, bool saida)
    {
        ApplicationUser? _applicationUser = await _userManager.GetUserAsync(User);
        int codigo = _applicationUser!.EmpresaId;

        ViewData["prestador"] = prestador;
        ViewData["comDataSaida"] = saida;
        var listImoveis = await LocalizaTodos(prestador, saida, codigo);
        ViewData["prestador"] = null;
        ViewData["comDataSaida"] = null;
        return View(listImoveis);
    }

    public async Task<List<Prestador>> LocalizaTodos(string pessoa, bool saida, int codigo)
    {
        var query = _context.Prestadores
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
    public async Task<IActionResult> DetalharPrestador(int? id)
    {
        if (id == null)
            return RedirectToAction(nameof(Error), new { message = "Prestador de serviço não foi encontrado para visualizar os detalhes" });

        var prestador = await _context.Prestadores
            .FirstOrDefaultAsync(m => m.Id == id);
        var imovel = await _context.Imoveis
            .FirstOrDefaultAsync(obj => obj.Id == prestador!.ImovelId);

        if (prestador == null)
            return RedirectToAction(nameof(Error), new { message = "Prestador de serviço sem dados para visualização de detalhes" });

        var viewModel = new ImovelViewModel
        {
            Prestador = prestador,
            Imovel = imovel
        };
        return View(viewModel);
    }

    [Authorize(Roles = "Sindico,Porteiro")]
    public async Task<IActionResult> CadastrarPrestador(int id)
    {
        ApplicationUser? _applicationUser = await _userManager.GetUserAsync(User);

        var imovel = await _context.Imoveis.FirstOrDefaultAsync(obj => obj.Id == id);

        var viewModel = new ImovelViewModel
        {
            Imovel = imovel
        };
        return View(viewModel);
    }

    [Authorize(Roles = "Sindico,Porteiro")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CadastrarPrestador(Imovel imovel, Prestador prestador)
    {
        ApplicationUser? _applicationUser = await _userManager.GetUserAsync(User);
        if (_applicationUser == null) return Challenge();

        prestador.ImovelId = imovel.Id;
        prestador.EmpresaId = imovel.EmpresaId;

        _context.Add(prestador);
        await _context.SaveChangesAsync();

        return RedirectToAction("DetalharImovel", "Imovels", new { id = prestador.ImovelId });
    }

    [HttpGet]
    public async Task<IActionResult> EditarPrestador(int? id)
    {
        if (id == null)
            return RedirectToAction(nameof(Error), new { message = "Id do Prestador não foi informado para edição." });

        var lista = await _context.Prestadores
            .Include(v => v.Imovel)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (lista == null) return NotFound();

        var viewModel = new ImovelViewModel
        {
            Prestador = lista,
            Imovel = lista.Imovel
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditarPrestador(int id, Prestador prestador)
    {
        if (id != prestador!.Id)
            return RedirectToAction(nameof(Error), new { message = "Id do Prestador inválido." });

        var prestadorBanco = await _context.Prestadores.AsNoTracking().FirstOrDefaultAsync(v => v.Id == id);

        ModelState.Remove(nameof(prestador.Imovel));
        ModelState.Remove(nameof(prestador.Empresa));

        try
        {
            _context.Update(prestador);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!Exists(prestador.Id))
                return RedirectToAction(nameof(Error), new { message = "Erro de concorrência: O morador não existe mais." });
            else
                throw;
        }

        return RedirectToAction(nameof(ListarPrestadores), new { id = prestador.Id });
    }

    [Authorize(Roles = "Sindico")]
    public async Task<IActionResult> ExcluirPrestador(int? id)
    {
        if (id == null)
            return RedirectToAction(nameof(Error), new { message = "Prestador de serviço não foi encontrada para deletar" });

        var prestador = await _context.Prestadores.FirstOrDefaultAsync(m => m.Id == id);
        var imovel = await _context.Imoveis.FirstOrDefaultAsync(obj => obj.Id == prestador!.ImovelId);

        if (prestador == null)
            return RedirectToAction(nameof(Error), new { message = "Esse prestador de serviço esta vazio para deletar" });

        var viewModel = new ImovelViewModel
        {
            Prestador = prestador,
            Imovel = imovel
        };
        return View(viewModel);
    }

    [Authorize(Roles = "Sindico")]
    [HttpPost, ActionName("ExcluirPrestador")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ExcluirPrestadorConfirmed(int id)
    {
        var prestador = await _context.Prestadores.FindAsync(id);
        _context.Prestadores.Remove(prestador!);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(ListarPrestadores));
    }

    private bool Exists(int id)
    {
        return _context.Prestadores.Any(e => e.Id == id);
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