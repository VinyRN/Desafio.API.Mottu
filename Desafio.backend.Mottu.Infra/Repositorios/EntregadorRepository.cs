using Desafio.backend.Mottu.Dominio.Entidades;
using Desafio.backend.Mottu.Dominio.Interfaces;
using MongoDB.Driver;

namespace Desafio.backend.Mottu.Infraestrutura.Repositorios
{
    public class EntregadorRepository : IEntregadorRepository
    {
        private readonly IMongoCollection<Entregador> _entregadorCollection;

        public EntregadorRepository(IMongoDatabase database)
        {
            _entregadorCollection = database.GetCollection<Entregador>("entregadores");
        }

        public async Task GravarEntregadorAsync(Entregador entregador)
        {
            await _entregadorCollection.InsertOneAsync(entregador);
        }

        public async Task<bool> CNPJExistenteAsync(string cnpj)
        {
            var filtro = Builders<Entregador>.Filter.Eq(e => e.CNPJ, cnpj);
            return await _entregadorCollection.Find(filtro).AnyAsync();
        }

        public async Task<bool> CNHExistenteAsync(string numeroCNH)
        {
            var filtro = Builders<Entregador>.Filter.Eq(e => e.NumeroCNH, numeroCNH);
            return await _entregadorCollection.Find(filtro).AnyAsync();
        }
        /// <summary>
        /// Busca um entregador no banco de dados pelo seu identificador único.
        /// </summary>
        /// <param name="entregadorId">O identificador único do entregador.</param>
        /// <returns>Retorna o objeto <see cref="Entregador"/> se encontrado; caso contrário, retorna null.</returns>
        public async Task<Entregador> ObterPorIdAsync(string entregadorId)
        {
            return await _entregadorCollection
                .Find(e => e.IdEntregador == entregadorId)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Atualiza as informações de um entregador no banco de dados.
        /// </summary>
        /// <param name="entregador">O objeto <see cref="Entregador"/> com os dados atualizados.</param>
        /// <returns>Retorna true se a atualização for bem-sucedida; caso contrário, false.</returns>
        public async Task<bool> AtualizarAsync(Entregador entregador)
        {
            var resultado = await _entregadorCollection
                .ReplaceOneAsync(e => e.IdEntregador == entregador.IdEntregador, entregador);

            return resultado.ModifiedCount > 0;
        }
    }
}
