using Desafio.backend.Mottu.Dominio.Entidades;

namespace Desafio.backend.Mottu.Dominio.Interfaces
{
    /// <summary>
    /// Interface para o repositório de motos, definindo as operações de persistência.
    /// </summary>
    public interface IMotoRepository
    {
        /// <summary>
        /// Grava uma nova moto no banco de dados.
        /// </summary>
        /// <param name="moto">Instância da entidade Moto contendo os dados a serem gravados.</param>
        /// <returns>Task representando a operação assíncrona de gravação.</returns>
        Task GravarMotoAsync(Moto moto);

        /// <summary>
        /// Atualiza uma moto existente no banco de dados.
        /// </summary>
        /// <param name="id">Identificador da moto a ser atualizada.</param>
        /// <param name="motoAtualizada">Instância da entidade Moto com os novos dados a serem atualizados.</param>
        /// <returns>Task representando a operação assíncrona de atualização.</returns>
        Task AtualizarMotoAsync(string id, Moto motoAtualizada);

        /// <summary>
        /// Obtém uma moto pelo seu identificador.
        /// </summary>
        /// <param name="id">Identificador da moto.</param>
        /// <returns>Instância da moto, se encontrada; caso contrário, null.</returns>
        Task<Moto?> ObterMotoPorIdAsync(string id);

        /// <summary>
        /// Obtém uma lista de motos existentes com a possibilidade de filtrar pela placa.
        /// </summary>
        /// <param name="placa">Placa da moto para filtro (opcional).</param>
        /// <returns>Lista de motos que correspondem ao filtro.</returns>
        Task<IEnumerable<Moto>> ConsultarMotosAsync(string? placa = null);

        /// <summary>
        /// Remove uma moto pelo seu identificador.
        /// </summary>
        /// <param name="id">Identificador da moto.</param>
        /// <returns>Task representando a operação assíncrona de remoção.</returns>
        Task RemoverMotoAsync(string id);

        /// <summary>
        /// Verifica se a moto possui registros de locação.
        /// </summary>
        /// <param name="id">Identificador da moto.</param>
        /// <returns>Task representando a verificação, retornando true se a moto tiver registros de locação.</returns>
        Task<bool> VerificarLocacoesAsync(string id);
    }
}
