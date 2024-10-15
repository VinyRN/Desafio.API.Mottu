using Desafio.backend.Mottu.Dominio.Entidades;
using Desafio.backend.Mottu.Dominio.Interfaces;
using MongoDB.Driver;

namespace Desafio.backend.Mottu.Infraestrutura.Repositorios
{
    /// <summary>
    /// Implementação do repositório de motos, utilizando MongoDB como banco de dados.
    /// </summary>
    public class MotoRepository : IMotoRepository
    {
        private readonly IMongoCollection<Moto> _motoCollection;
        private readonly IMongoCollection<Locacao> _locacaoCollection;

        /// <summary>
        /// Construtor do repositório de motos, recebendo a instância do MongoDB.
        /// </summary>
        /// <param name="database">Instância do banco de dados MongoDB.</param>
        public MotoRepository(IMongoDatabase database)
        {
            _motoCollection = database.GetCollection<Moto>("motos");
            _locacaoCollection = database.GetCollection<Locacao>("locacoes");

            var indexKeysDefinition = Builders<Moto>.IndexKeys.Ascending(m => m.Placa);
            var indexOptions = new CreateIndexOptions { Unique = true };
            var indexModel = new CreateIndexModel<Moto>(indexKeysDefinition, indexOptions);
            _motoCollection.Indexes.CreateOne(indexModel);
        }


        /// <summary>
        /// Grava uma nova moto no banco de dados MongoDB.
        /// </summary>
        /// <param name="moto">Instância da entidade Moto contendo os dados a serem gravados.</param>
        /// <returns>Task representando a operação assíncrona de gravação.</returns>
        public async Task GravarMotoAsync(Moto moto)
        {
            if (moto is null)
            {
                throw new ArgumentNullException(nameof(moto), "A moto não pode ser nula.");
            }

            // Verificar se já existe uma moto com a mesma placa
            var filtroPlaca = Builders<Moto>.Filter.Eq(m => m.Placa, moto.Placa);
            var motoExistente = await _motoCollection.Find(filtroPlaca).FirstOrDefaultAsync();

            if (motoExistente != null)
            {
                throw new InvalidOperationException($"Já existe uma moto cadastrada com a placa {moto.Placa}.");
            }

            await _motoCollection.InsertOneAsync(moto);
        }


        /// <summary>
        /// Atualiza os dados de uma moto existente no banco de dados MongoDB.
        /// </summary>
        /// <param name="id">Identificador da moto a ser atualizada.</param>
        /// <param name="motoAtualizada">Instância da entidade Moto com os novos dados a serem atualizados.</param>
        /// <returns>Task representando a operação assíncrona de atualização.</returns>
        public async Task AtualizarMotoAsync(string id, Moto motoAtualizada)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("O identificador da moto não pode ser nulo ou vazio.", nameof(id));
            }

            if (motoAtualizada == null)
            {
                throw new ArgumentNullException(nameof(motoAtualizada), "A moto atualizada não pode ser nula.");
            }

            // Verificar se já existe outra moto com a mesma placa
            var filtroPlaca = Builders<Moto>.Filter.Eq(m => m.Placa, motoAtualizada.Placa) &
                              Builders<Moto>.Filter.Ne(m => m.IdMoto, id); // Excluir a própria moto do filtro
            var motoExistente = await _motoCollection.Find(filtroPlaca).FirstOrDefaultAsync();

            if (motoExistente != null)
            {
                throw new InvalidOperationException($"Já existe uma moto cadastrada com a placa {motoAtualizada.Placa}.");
            }

            var filtro = Builders<Moto>.Filter.Eq(m => m.IdMoto, id);
            await _motoCollection.ReplaceOneAsync(filtro, motoAtualizada);
        }
        public async Task<Moto?> ObterMotoPorIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("O identificador da moto não pode ser nulo ou vazio.", nameof(id));
            }

            var filtro = Builders<Moto>.Filter.Eq(m => m.IdMoto, id);
            return await _motoCollection.Find(filtro).FirstOrDefaultAsync();
        }
        public async Task<IEnumerable<Moto>> ConsultarMotosAsync(string? placa = null)
        {
            var filtro = placa == null
                ? Builders<Moto>.Filter.Empty
                : Builders<Moto>.Filter.Eq(m => m.Placa, placa);

            return await _motoCollection.Find(filtro).ToListAsync();
        }
        public async Task RemoverMotoAsync(string id)
        {
            var filtro = Builders<Moto>.Filter.Eq(m => m.IdMoto, id);
            await _motoCollection.DeleteOneAsync(filtro);
        }

        public async Task<bool> VerificarLocacoesAsync(string id)
        {
            // Verificar se há locações associadas à moto
            var filtro = Builders<Locacao>.Filter.Eq(l => l.IdMoto, id);
            return await _locacaoCollection.Find(filtro).AnyAsync();
        }
    }
}
