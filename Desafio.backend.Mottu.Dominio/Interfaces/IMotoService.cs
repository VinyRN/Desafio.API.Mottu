using Desafio.backend.Mottu.Dominio.Entidades;

namespace Desafio.backend.Mottu.Dominio.Interfaces
{
    /// <summary>
    /// Interface que define os serviços relacionados a operações com motos.
    /// </summary>
    public interface IMotoService
    {
        /// <summary>
        /// Assinatura de método responsável por gravar uma moto no sistema de forma assíncrona.
        /// </summary>
        /// <param name="moto">Instância da entidade <see cref="Moto"/> contendo os dados da moto a serem gravados.</param>
        /// <returns>Task que representa a operação assíncrona de gravação da moto.</returns>
        Task GravarMotoAsync(Moto moto);

        /// <summary>
        /// Assinatura de método responsável por atualizar uma moto no sistema de forma assíncrona.
        /// </summary>
        /// <param name="id">Identificador da moto a ser atualizada.</param>
        /// <param name="motoAtualizada">Instância da entidade <see cref="Moto"/> contendo os novos dados da moto.</param>
        /// <returns>Task que representa a operação assíncrona de atualização da moto.</returns>
        Task AtualizarMotoAsync(string id, Moto motoAtualizada);

        /// <summary>
        /// Obtém uma moto pelo seu identificador.
        /// </summary>
        /// <param name="id">Identificador da moto.</param>
        /// <returns>Instância da moto, se encontrada; caso contrário, null.</returns>
        Task<Moto?> ObterMotoPorIdAsync(string id);

        /// <summary>
        /// Consultar motos com a possibilidade de filtrar pela placa.
        /// </summary>
        /// <param name="placa">Placa da moto para filtro (opcional).</param>
        /// <returns>Lista de motos que correspondem ao filtro.</returns>
        Task<IEnumerable<Moto>> ConsultarMotosAsync(string? placa = null);

        /// <summary>
        /// Atualiza apenas a placa de uma moto já cadastrada.
        /// </summary>
        /// <param name="id">Identificador da moto a ser atualizada.</param>
        /// <param name="novaPlaca">Nova placa da moto.</param>
        /// <returns>Task representando a operação assíncrona de atualização.</returns>
        Task AtualizarPlacaAsync(string id, string novaPlaca);

        /// <summary>
        /// Remove uma moto pelo seu identificador, desde que não tenha registros de locação.
        /// </summary>
        /// <param name="id">Identificador da moto.</param>
        /// <returns>Task representando a operação assíncrona de remoção.</returns>
        Task RemoverMotoAsync(string id);
    }
}
