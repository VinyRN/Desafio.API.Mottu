using AutoMapper;
using Desafio.backend.Mottu.Dominio.Dto;
using Desafio.backend.Mottu.Dominio.Entidades;
using Desafio.backend.Mottu.Dominio.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Desafio.backend.Mottu.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MotoController : ControllerBase
    {
        private readonly IMotoService _motoService;
        private readonly IMapper _mapper;

        public MotoController(IMotoService motoService, IMapper mapper)
        {
            _motoService = motoService;
            _mapper = mapper;
        }

        /// <summary>
        ///  Verificação de saúde da API. Remover se não for necessário.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult HealthCheck()
        {
            return Ok("MotoController está operacional.");
        }

        /// <summary>
        /// Atualiza as informações de uma moto.
        /// </summary>
        /// <param name="motoRequisicaoDto">Dados da moto a serem atualizados.</param>
        /// <param name="id">Identificador da moto.</param>
        /// <returns>Retorna o status da operação.</returns>
        [HttpPut]
        [Route("/moto/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AtualizarMotoAsync([FromBody] MotoRequisicaoDto motoRequisicaoDto, string id)
        {
            if (motoRequisicaoDto is null)
            {
                var detalhesDoProblema = new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Type = "BadRequest",
                    Title = "Registro não pode ser nulo",
                    Detail = "Os dados da moto não podem ser vazios ou nulos.",
                    Instance = HttpContext.Request.Path
                };
                return BadRequest(detalhesDoProblema);
            }

            var moto = _mapper.Map<Moto>(motoRequisicaoDto);
            await _motoService.AtualizarMotoAsync(id,moto);

            return NoContent();
        }

        /// <summary>
        /// Obtém as informações de uma moto por ID.
        /// </summary>
        /// <param name="id">Identificador da moto.</param>
        /// <returns>Retorna a moto encontrada ou um erro caso não seja encontrada.</returns>
        [HttpGet]
        [Route("/moto/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObterMotoPorIdAsync(string id)
        {
            var moto = await _motoService.ObterMotoPorIdAsync(id);

            if (moto == null)
            {
                var detalhesDoProblema = new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Type = "NotFound",
                    Title = "Moto não encontrada",
                    Detail = "Não foi encontrada uma moto com o ID fornecido.",
                    Instance = HttpContext.Request.Path
                };
                return NotFound(detalhesDoProblema);
            }

            return Ok(moto);
        }
        [HttpGet("consultar")]
        public async Task<IActionResult> ConsultarMotos([FromQuery] string? placa = null)
        {
            var motos = await _motoService.ConsultarMotosAsync(placa);
            return Ok(motos);
        }
        [HttpPut("{id}/alterar-placa")]
        public async Task<IActionResult> AtualizarPlaca(string id, [FromBody] string novaPlaca)
        {
            if (string.IsNullOrWhiteSpace(novaPlaca))
            {
                return BadRequest("A placa não pode estar vazia.");
            }

            await _motoService.AtualizarPlacaAsync(id, novaPlaca);
            return Ok("Placa atualizada com sucesso.");
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoverMoto(string id)
        {
            try
            {
                await _motoService.RemoverMotoAsync(id);
                return Ok("Moto removida com sucesso.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
