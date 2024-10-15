using Desafio.backend.Mottu.Dominio.Dto.Request;
using Desafio.backend.Mottu.Dominio.Entidades;

namespace Desafio.backend.Mottu.Dominio.Interfaces
{
    public interface IEntregadorService
    {
        Task CadastrarEntregadorAsync(EntregadorRequestDTO entregador);

        Task<bool> AtualizarCnhAsync(string entregadorId, string cnhBase64);
    }
}
