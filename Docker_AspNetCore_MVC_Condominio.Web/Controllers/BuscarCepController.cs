using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Docker_AspNetCore_MVC_Condominio.Web.Controllers;

public class BuscarCepController : Controller
{
    [HttpGet]
    public async Task<IActionResult> BuscarCep(string cep)
    {
        if (string.IsNullOrWhiteSpace(cep)) return BadRequest("CEP inválido");

        cep = new string(cep.Where(char.IsDigit).ToArray());
        if (cep.Length != 8) return BadRequest("CEP deve ter 8 dígitos");

        using var client = new HttpClient();
        var response = await client.GetAsync($"https://viacep.com.br/ws/{cep}/json/");
        if (!response.IsSuccessStatusCode) return StatusCode(500, "Erro ao consultar serviço de CEP");

        var json = await response.Content.ReadAsStringAsync();
        var data = JsonSerializer.Deserialize<JsonElement>(json);

        if (data.TryGetProperty("erro", out _))
            return NotFound("CEP não encontrado");

        return Json(new
        {
            uf = data.GetProperty("uf").GetString(),
            cidade = data.GetProperty("localidade").GetString(),
            endereco = data.GetProperty("logradouro").GetString(),
            bairro = data.GetProperty("bairro").GetString()
        });
    }
}
