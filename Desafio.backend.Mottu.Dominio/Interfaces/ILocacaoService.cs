using System.Threading.Tasks;
using Desafio.backend.Mottu.Dominio.Entidades;

namespace Desafio.backend.Mottu.Dominio.Interfaces
{
    /// <summary>
    /// Interface que define os serviços relacionados a locações de motos.
    /// </summary>
    public interface ILocacaoService
    {
        /// <summary>
        /// Realiza a locação de uma moto para um entregador.
        /// </summary>
        /// <param name="entregadorId">O identificador do entregador.</param>
        /// <param name="motoId">O identificador da moto a ser alugada.</param>
        /// <param name="plano">O plano de locação escolhido.</param>
        /// <returns>Retorna o identificador da locação criada.</returns>
        Task<string> AlugarMotoAsync(string entregadorId, string motoId, PlanoLocacao plano);
    }
}
