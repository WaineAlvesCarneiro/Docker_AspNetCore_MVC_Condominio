using Docker_AspNetCore_MVC_Condominio.Web.Data;
using Docker_AspNetCore_MVC_Condominio.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Docker_AspNetCore_MVC_Condominio.Web.Controllers;

[Authorize(Roles = "Sindico,Porteiro")]
public class EncomendasEntreguesController(ApplicationDbContext context,
                            UserManager<ApplicationUser> userManager) : Controller
{
    private readonly ApplicationDbContext _context = context;
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    [Authorize(Roles = "Sindico,Porteiro")]
    public async Task<IActionResult> ListarEntregas(string codigorastreio, bool entregue)
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

    public async Task<List<EncomendasEntregues>> LocalizaTodos(string codigorastreio, bool? entregue, int codigo)
    {
        var query = _context.EncomendasEntregues
            .Where(x => x.EmpresaId == codigo)
            .Include(i => i.Imovel)
            .Include(m => m.Morador)
            .Include(re => re.EncomendasRecebidas)
                .ThenInclude(r => r.Morador)
            .Where(x => x.Imovel!.EmpresaId == codigo
                 && x.Morador!.EmpresaId == codigo
                 && x.Morador!.DataSaida == null)
            .AsQueryable();


        if (!string.IsNullOrWhiteSpace(codigorastreio))
            query = query.Where(x => x.EncomendasRecebidas!.CodigoRastreio == codigorastreio);

        if (entregue.HasValue)
            query = query.Where(x => x.EncomendasRecebidas!.Entregue == entregue.Value);

        return await query.ToListAsync();
    }

    [Authorize(Roles = "Sindico,Porteiro")]
    public async Task<IActionResult> DetalharEntrega(int? id)
    {
        if (id == null)
            return RedirectToAction(nameof(Error), new { message = "Encomenda Entregue não foi encontrada para visualizar os detalhes" });

        var encomendasEntregues = await _context.EncomendasEntregues.FirstOrDefaultAsync(m => m.Id == id);
        var encomendasRecebidas = await _context.EncomendasRecebidas.FirstOrDefaultAsync(m => m.Id == encomendasEntregues!.EncomendasRecebidasId);

        var moradorNaEncomenda = await _context.Moradores.FirstOrDefaultAsync(obj => obj.Id == encomendasRecebidas!.MoradorId);
        var imovel = await _context.Imoveis.FirstOrDefaultAsync(obj => obj.Id == encomendasEntregues!.ImovelId);

        var moradorRecebeuEntrega = await _context.Moradores.FirstOrDefaultAsync(obj => obj.Id == encomendasEntregues!.MoradorId);

        if (encomendasEntregues == null)
            return RedirectToAction(nameof(Error), new { message = "Encomenda Entregue sem dados para visualização de detalhes" });

        var viewModel = new ImovelViewModel
        {
            EncomendasEntregues = encomendasEntregues,
            EncomendasRecebidas = encomendasRecebidas!,
            Imovel = imovel,
            MoradorRecebeuEntrega = moradorRecebeuEntrega,
            Morador = moradorNaEncomenda
        };
        return View(viewModel);
    }

    [HttpGet]
    [Authorize(Roles = "Sindico,Porteiro")]
    public async Task<IActionResult> CadastrarEntrega(int id)
    {
        ApplicationUser? _applicationUser = await _userManager.GetUserAsync(User);
        if (_applicationUser == null) return Challenge();

        var encomendasRecebidas = await _context.EncomendasRecebidas
            .FirstOrDefaultAsync(m => m.ImovelId == id && m.Entregue == false && m.EmpresaId == _applicationUser.EmpresaId);

        if (encomendasRecebidas == null)
            return RedirectToAction(nameof(Error), new { message = "Não foi encontrada encomenda deste imóvel a ser entregue." });

        var imovel = await _context.Imoveis.FirstOrDefaultAsync(obj => obj.Id == encomendasRecebidas.ImovelId);

        if (imovel == null)
            return RedirectToAction(nameof(Error), new { message = "O Imóvel vinculado a esta encomenda não foi localizado no sistema." });

        var moradorNaEncomenda = await _context.Moradores.FirstOrDefaultAsync(obj => obj.Id == encomendasRecebidas.MoradorId);
        var listMoradores = await ListMoradoresImovelIdAsync(imovel.Id, _applicationUser.EmpresaId);

        var viewModel = new ImovelViewModel
        {
            Moradores = listMoradores ?? new List<Morador>(),
            EncomendasRecebidas = encomendasRecebidas,
            Imovel = imovel,
            Morador = moradorNaEncomenda,
            EncomendasEntregues = new EncomendasEntregues
            {
                EmpresaId = _applicationUser.EmpresaId,
                Empresa = imovel.Empresa,
                ImovelId = imovel.Id,
                Imovel = imovel,
                EncomendasRecebidasId = encomendasRecebidas.Id,
                EncomendasRecebidas = encomendasRecebidas
            }
        };

        return View(viewModel);
    }

    [Authorize(Roles = "Sindico,Porteiro")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CadastrarEntrega(Imovel imovel, EncomendasEntregues encomendasEntregues)
    {
        ApplicationUser? _applicationUser = await _userManager.GetUserAsync(User);
        if (_applicationUser == null) return Challenge();

        encomendasEntregues.ImovelId = imovel.Id;
        encomendasEntregues.EmpresaId = imovel.EmpresaId;

        _context.Add(encomendasEntregues);
        await _context.SaveChangesAsync();

        var encomendaParaAtualizar = await _context.EncomendasRecebidas.FindAsync(encomendasEntregues.EncomendasRecebidasId);
        if (encomendaParaAtualizar != null)
        {
            encomendaParaAtualizar.Entregue = true;

            _context.Update(encomendaParaAtualizar);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("DetalharImovel", "Imovels", new { id = encomendasEntregues.ImovelId });
    }

    [HttpGet]
    public async Task<IActionResult> EditarEntrega(int? id)
    {
        if (id == null)
            return RedirectToAction(nameof(Error), new { message = "Encomenda Entregue não foi encontrada para visualizar os detalhes" });

        var encomendasEntregues = await _context.EncomendasEntregues.FirstOrDefaultAsync(m => m.Id == id);
        var encomendasRecebidas = await _context.EncomendasRecebidas.FirstOrDefaultAsync(m => m.Id == encomendasEntregues!.EncomendasRecebidasId);

        if (encomendasEntregues == null)
            return RedirectToAction(nameof(Error), new { message = "Encomenda Entregue sem dados para visualização de detalhes" });

        var moradorNaEncomenda = await _context.Moradores.FirstOrDefaultAsync(obj => obj.Id == encomendasRecebidas!.MoradorId);
        var imovel = await _context.Imoveis.FirstOrDefaultAsync(obj => obj.Id == encomendasEntregues!.ImovelId);

        var moradorRecebeuEntrega = await _context.Moradores.FirstOrDefaultAsync(obj => obj.Id == encomendasEntregues!.MoradorId);

        var listMoradores = await ListMoradoresImovelIdAsync(imovel!.Id, imovel!.EmpresaId);

        encomendasEntregues.Morador = moradorRecebeuEntrega!;

        var viewModel = new ImovelViewModel
        {
            Moradores = listMoradores ?? new List<Morador>(),
            EncomendasRecebidas = encomendasRecebidas!,
            Imovel = imovel,
            EncomendasEntregues = encomendasEntregues!
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditarEntrega(int id, EncomendasEntregues encomendasEntregues)
    {
        if (id != encomendasEntregues!.Id)
            return RedirectToAction(nameof(Error), new { message = "Id do EncomendasEntregues inválido." });

        var EncomendasEntreguesBanco = await _context.EncomendasEntregues.AsNoTracking().FirstOrDefaultAsync(v => v.Id == id);

        ModelState.Remove(nameof(encomendasEntregues.Imovel));
        ModelState.Remove(nameof(encomendasEntregues.Empresa));

        try
        {
            _context.Update(encomendasEntregues);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!Exists(encomendasEntregues.Id))
                return RedirectToAction(nameof(Error), new { message = "Erro de concorrência: O morador não existe mais." });
            else
                throw;
        }

        return RedirectToAction(nameof(ListarEntregas), new { id = encomendasEntregues.Id });
    }

    [Authorize(Roles = "Sindico")]
    public async Task<IActionResult> ExcluirEntrega(int? id)
    {
        if (id == null)
            return RedirectToAction(nameof(Error), new { message = "Encomenda Recebida não foi encontrada para deletar" });

        var encomendasEntregues = await _context.EncomendasEntregues.FirstOrDefaultAsync(m => m.Id == id);
        var encomendasRecebidas = await _context.EncomendasRecebidas.FirstOrDefaultAsync(m => m.Id == encomendasEntregues!.EncomendasRecebidasId);

        var moradorNaEncomenda = await _context.Moradores.FirstOrDefaultAsync(obj => obj.Id == encomendasRecebidas!.MoradorId);
        var imovel = await _context.Imoveis.FirstOrDefaultAsync(obj => obj.Id == encomendasEntregues!.ImovelId);

        var moradorRecebeuEntrega = await _context.Moradores.FirstOrDefaultAsync(obj => obj.Id == encomendasEntregues!.MoradorId);

        if (encomendasEntregues == null)
            return RedirectToAction(nameof(Error), new { message = "Essa Encomenda Recebida esta vazio para deletar" });

        var viewModel = new ImovelViewModel
        {
            EncomendasEntregues = encomendasEntregues,
            EncomendasRecebidas = encomendasRecebidas!,
            Imovel = imovel,
            MoradorRecebeuEntrega = moradorRecebeuEntrega,
            Morador = moradorNaEncomenda
        };
        return View(viewModel);
    }

    [Authorize(Roles = "Sindico")]
    [HttpPost, ActionName("ExcluirEntrega")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ExcluirEntregaConfirmed(int id)
    {
        var encomendasEntregues = await _context.EncomendasEntregues.FindAsync(id);
        var encomendasRecebidas = await _context.EncomendasRecebidas.FirstOrDefaultAsync(m => m.Id == encomendasEntregues!.EncomendasRecebidasId);

        _context.EncomendasEntregues.Remove(encomendasEntregues!);
        await _context.SaveChangesAsync();

        if (encomendasRecebidas != null)
        {
            _context.EncomendasRecebidas.Remove(encomendasRecebidas);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(ListarEntregas));
    }

    public async Task<List<Morador>> ListMoradoresImovelIdAsync(int idImovel, int codigo)
    {
        var moradores = from obj in _context.Moradores select obj;
        moradores = moradores.Where(x => x.ImovelId == idImovel && x.EmpresaId == codigo)
            .OrderBy(x => x.Nome);

        return await moradores.ToListAsync();
    }

    private bool Exists(int id)
    {
        return _context.EncomendasEntregues.Any(e => e.Id == id);
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