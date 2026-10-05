using Docker_AspNetCore_MVC_Condominio.Web.Data;
using Docker_AspNetCore_MVC_Condominio.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Docker_AspNetCore_MVC_Condominio.Web.Controllers;

[Authorize(Roles = "Sindico,Porteiro")]
public class EncomendasRecebidasController(ApplicationDbContext context,
                            UserManager<ApplicationUser> userManager) : Controller
{
    private readonly ApplicationDbContext _context = context;
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    [Authorize(Roles = "Sindico,Porteiro")]
    public async Task<IActionResult> ListarRecebidas(string codigorastreio, bool entregue)
    {
        ApplicationUser? _applicationUser = await _userManager.GetUserAsync(User);
        int codigo = _applicationUser!.EmpresaId;

        ViewData["codigorastreio"] = codigorastreio;
        ViewData["comdataentregue"] = entregue;
        var listImoveis = await LocalizaTodos(codigorastreio, entregue, codigo);
        ViewData["codigorastreio"] = null;
        ViewData["comdataentregue"] = null;
        return View(listImoveis);
    }

    public async Task<List<EncomendasRecebidas>> LocalizaTodos(string codigorastreio, bool? entregue, int codigo)
    {
        var query = _context.EncomendasRecebidas
                            .Where(x => x.EmpresaId == codigo)
                            .Include(i => i.Imovel)
                            .Include(m => m.Morador)
                            .Where(x => x.Imovel!.EmpresaId == codigo
                                 && x.Morador!.EmpresaId == codigo
                                 && x.Morador!.DataSaida == null)
                            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(codigorastreio))
            query = query.Where(x => x.CodigoRastreio == codigorastreio);

        if (entregue.HasValue)
            query = query.Where(x => x.Entregue == entregue.Value);

        return await query.ToListAsync();
    }

    [Authorize(Roles = "Sindico,Porteiro")]
    public async Task<IActionResult> DetalharRecebida(int? id)
    {
        if (id == null)
            return RedirectToAction(nameof(Error), new { message = "Encomenda Entregue não foi encontrada para visualizar os detalhes" });

        var EncomendasRecebidas = await _context.EncomendasRecebidas.FirstOrDefaultAsync(m => m.Id == id);
        var imovel = await _context.Imoveis.FirstOrDefaultAsync(obj => obj.Id == EncomendasRecebidas!.ImovelId);
        var morador = await _context.Moradores.FirstOrDefaultAsync(obj => obj.Id == EncomendasRecebidas!.MoradorId);

        if (EncomendasRecebidas == null)
            return RedirectToAction(nameof(Error), new { message = "Essa Encomenda Recebida esta vazio para deletar" });

        var viewModel = new ImovelViewModel
        {
            EncomendasRecebidas = EncomendasRecebidas,
            Imovel = imovel,
            Morador = morador
        };
        return View(viewModel);
    }

    [Authorize(Roles = "Sindico,Porteiro")]
    public async Task<IActionResult> CadastrarRecebida(int id)
    {
        ApplicationUser? _applicationUser = await _userManager.GetUserAsync(User);
        int codigo = _applicationUser!.EmpresaId;

        var imovel = await _context.Imoveis.FirstOrDefaultAsync(obj => obj.Id == id);
        var listMoradores = await ListMoradoresImovelIdAsync(imovel!.Id, codigo);

        var viewModel = new ImovelViewModel
        {
            Imovel = imovel,
            Moradores = listMoradores
        };
        return View(viewModel);
    }

    [Authorize(Roles = "Sindico,Porteiro")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CadastrarRecebida(ImovelViewModel model)
    {
        ApplicationUser? _applicationUser = await _userManager.GetUserAsync(User);
        if (_applicationUser == null) return Challenge();

        var encomenda = model.EncomendasRecebidas;

        encomenda.ImovelId = model.Imovel!.Id;
        encomenda.EmpresaId = model.Imovel.EmpresaId;

        _context.Add(encomenda);
        await _context.SaveChangesAsync();

        return RedirectToAction("DetalharImovel", "Imovels", new { id = encomenda.ImovelId });
    }

    [HttpGet]
    public async Task<IActionResult> EditarRecebida(int? id)
    {
        if (id == null)
            return RedirectToAction(nameof(Error), new { message = "Id do Recebida não foi informado para edição." });

        var lista = await _context.EncomendasRecebidas
            .Include(v => v.Imovel)
            .Include(m => m.Morador)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (lista == null) return NotFound();

        var viewModel = new ImovelViewModel
        {
            EncomendasRecebidas = lista,
            Imovel = lista.Imovel,
            Morador = lista.Morador,
            Moradores = await _context.Moradores
                                      .Where(x => x.EmpresaId == lista.EmpresaId && x.DataSaida == null)
                                      .ToListAsync()
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditarRecebida(int id, EncomendasRecebidas EncomendasRecebidas)
    {
        if (id != EncomendasRecebidas!.Id)
            return RedirectToAction(nameof(Error), new { message = "Id do EncomendasRecebidas inválido." });

        var EncomendasRecebidasBanco = await _context.EncomendasRecebidas.AsNoTracking().FirstOrDefaultAsync(v => v.Id == id);

        ModelState.Remove(nameof(EncomendasRecebidas.Imovel));
        ModelState.Remove(nameof(EncomendasRecebidas.Empresa));

        try
        {
            _context.Update(EncomendasRecebidas);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!EncomendasRecebidaExists(EncomendasRecebidas.Id))
                return RedirectToAction(nameof(Error), new { message = "Erro de concorrência: O morador não existe mais." });
            else
                throw;
        }

        return RedirectToAction(nameof(ListarRecebidas), new { id = EncomendasRecebidas.Id });
    }

    [Authorize(Roles = "Sindico")]
    public async Task<IActionResult> ExcluirRecebida(int? id)
    {
        if (id == null)
            return RedirectToAction(nameof(Error), new { message = "Encomenda Recebida não foi encontrada para deletar" });

        var EncomendasRecebidas = await _context.EncomendasRecebidas.FirstOrDefaultAsync(m => m.Id == id);
        var imovel = await _context.Imoveis.FirstOrDefaultAsync(obj => obj.Id == EncomendasRecebidas!.ImovelId);
        var morador = await _context.Moradores.FirstOrDefaultAsync(obj => obj.Id == EncomendasRecebidas!.MoradorId);
        if (EncomendasRecebidas == null)
            return RedirectToAction(nameof(Error), new { message = "Essa Encomenda Recebida esta vazio para deletar" });

        var viewModel = new ImovelViewModel
        {
            EncomendasRecebidas = EncomendasRecebidas,
            Imovel = imovel,
            Morador = morador
        };
        return View(viewModel);
    }

    [Authorize(Roles = "Sindico")]
    [HttpPost, ActionName("ExcluirRecebida")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ExcluirRecebidaConfirmed(int id)
    {
        var EncomendasRecebidas = await _context.EncomendasRecebidas.FindAsync(id);
        var EncomendasEntregues = await _context.EncomendasEntregues.FirstOrDefaultAsync(m => m.EncomendasRecebidasId == EncomendasRecebidas!.Id);

        _context.EncomendasRecebidas.Remove(EncomendasRecebidas!);
        await _context.SaveChangesAsync();

        if (EncomendasEntregues != null)
        {
            _context.EncomendasEntregues.Remove(EncomendasEntregues);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(ListarRecebidas));
    }

    public async Task<List<Morador>> ListMoradoresImovelIdAsync(int idImovel, int codigo)
    {
        var query = _context.Moradores
                            .Where(x => x.EmpresaId == codigo && x.ImovelId == idImovel && x.DataSaida == null)
                            .Include(i => i.Imovel)
                            .AsQueryable()
                            .OrderBy(x => x.Nome);

        return await query.ToListAsync();
    }

    private bool EncomendasRecebidaExists(int id)
    {
        return _context.EncomendasRecebidas.Any(e => e.Id == id);
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