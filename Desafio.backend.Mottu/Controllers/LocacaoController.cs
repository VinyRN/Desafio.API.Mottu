using Desafio.backend.Mottu.Dominio.Entidades;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class LocacaoController : ControllerBase
{
    private readonly LocacaoService _locacaoService;

    public LocacaoController(LocacaoService locacaoService)
    {
        _locacaoService = locacaoService;
    }

    [HttpPost("alugar")]
    public async Task<IActionResult> AlugarMoto(string entregadorId, string motoId, PlanoLocacao plano)
    {
        try
        {
            var locacaoId = await _locacaoService.AlugarMotoAsync(entregadorId, motoId, plano);
            return Ok(new { LocacaoId = locacaoId });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Erro = ex.Message });
        }
    }
}
