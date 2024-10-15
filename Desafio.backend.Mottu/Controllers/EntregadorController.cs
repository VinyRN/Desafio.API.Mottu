using Desafio.backend.Mottu.Dominio.Dto.Request;
using Desafio.backend.Mottu.Dominio.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Desafio.backend.Mottu.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EntregadorController : ControllerBase
    {
        private readonly IEntregadorService _entregadorService;

        public EntregadorController(IEntregadorService entregadorService)
        {
            _entregadorService = entregadorService;
        }

        [HttpPost("cadastrar")]
        public async Task<IActionResult> CadastrarEntregador([FromBody] EntregadorRequestDTO entregadorRequestDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _entregadorService.CadastrarEntregadorAsync(entregadorRequestDTO);
                return Ok(new { mensagem = "Entregador cadastrado com sucesso." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { erro = ex.Message });
            }
            catch (Exception ex)
            {
                // Retorna uma mensagem genérica para outros tipos de exceções
                return StatusCode(500, new { erro = "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde." });
            }
        }

        [HttpPost("upload-cnh")]
        public async Task<IActionResult> UploadCnh(IFormFile file, [FromQuery] string entregadorId)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Nenhum arquivo foi enviado.");

            // Verifica o formato do arquivo
            var allowedExtensions = new[] { ".png", ".bmp" };
            var fileExtension = Path.GetExtension(file.FileName).ToLower();

            if (!Array.Exists(allowedExtensions, ext => ext == fileExtension))
                return BadRequest("Formato de arquivo inválido. Apenas PNG e BMP são permitidos.");

            // Converte o arquivo para base64
            string base64String;
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                byte[] fileBytes = memoryStream.ToArray();
                base64String = Convert.ToBase64String(fileBytes);
            }

            // Chama o serviço para atualizar a CNH no banco de dados
            var sucesso = await _entregadorService.AtualizarCnhAsync(entregadorId, base64String);

            if (sucesso)
                return Ok("CNH atualizada com sucesso.");
            else
                return StatusCode(500, "Erro ao atualizar a CNH.");
        }
    }
}
