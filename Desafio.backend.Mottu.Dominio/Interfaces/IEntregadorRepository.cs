using Desafio.backend.Mottu.Dominio.Entidades;
using System.Threading.Tasks;

namespace Desafio.backend.Mottu.Dominio.Interfaces
{
    /// <summary>
    /// Interface que define os métodos de acesso ao repositório de entregadores.
    /// </summary>
    public interface IEntregadorRepository
    {
        /// <summary>
        /// Grava um novo entregador no banco de dados.
        /// </summary>
        /// <param name="entregador">O objeto <see cref="Entregador"/> a ser gravado.</param>
        /// <returns>Task representando a operação assíncrona.</returns>
        Task GravarEntregadorAsync(Entregador entregador);

        /// <summary>
        /// Verifica se um CNPJ já existe no banco de dados.
        /// </summary>
        /// <param name="cnpj">O CNPJ do entregador.</param>
        /// <returns>Retorna true se o CNPJ já existir; caso contrário, false.</returns>
        Task<bool> CNPJExistenteAsync(string cnpj);

        /// <summary>
        /// Verifica se uma CNH já existe no banco de dados.
        /// </summary>
        /// <param name="numeroCNH">O número da CNH do entregador.</param>
        /// <returns>Retorna true se o número da CNH já existir; caso contrário, false.</returns>
        Task<bool> CNHExistenteAsync(string numeroCNH);

        /// <summary>
        /// Busca um entregador pelo seu identificador único.
        /// </summary>
        /// <param name="entregadorId">O identificador único do entregador.</param>
        /// <returns>Retorna o objeto <see cref="Entregador"/> se encontrado; caso contrário, retorna null.</returns>
        Task<Entregador> ObterPorIdAsync(string entregadorId);

        /// <summary>
        /// Atualiza as informações de um entregador no banco de dados.
        /// </summary>
        /// <param name="entregador">O objeto <see cref="Entregador"/> com os dados atualizados.</param>
        /// <returns>Retorna true se a atualização for bem-sucedida; caso contrário, false.</returns>
        Task<bool> AtualizarAsync(Entregador entregador);
    }
}
