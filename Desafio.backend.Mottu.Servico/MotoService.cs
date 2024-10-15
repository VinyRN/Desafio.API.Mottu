using Desafio.backend.Mottu.Dominio.Dto;
using Desafio.backend.Mottu.Dominio.Entidades;
using Desafio.backend.Mottu.Dominio.Interfaces;
using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace Desafio.backend.Mottu.Servico
{
    public class MotoService : IMotoService
    {
        private readonly IMotoRepository _motoRepository;
        private readonly RabbitMQ.Client.IConnection _rabbitConnection;

        public MotoService(IMotoRepository motoRepository, RabbitMQ.Client.IConnection rabbitConnection)
        {
            _motoRepository = motoRepository;
            _rabbitConnection = rabbitConnection;
        }

        /// <summary>
        /// Grava uma nova moto no banco de dados através do repositório.
        /// </summary>
        /// <param name="moto">Instância da entidade Moto contendo os dados a serem gravados.</param>
        /// <returns>Task representando a operação assíncrona.</returns>
        public async Task GravarMotoAsync(Moto moto)
        {
            await _motoRepository.GravarMotoAsync(moto);

            // Publicar o evento de MotoCadastrada
            var evento = new MotoCadastradaEvento
            {
                IdMoto = moto.IdMoto,
                Ano = moto.Ano,
                Modelo = moto.Modelo,
                Placa = moto.Placa
            };

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(evento));

            using var channel = _rabbitConnection.CreateModel();
            channel.QueueDeclare(queue: "moto_cadastrada_queue", durable: true, exclusive: false, autoDelete: false, arguments: null);
            channel.BasicPublish(exchange: "", routingKey: "moto_cadastrada_queue", basicProperties: null, body: body);
        }

        /// <summary>
        /// Atualiza os dados de uma moto existente através do repositório.
        /// </summary>
        /// <param name="id">Identificador da moto a ser atualizada.</param>
        /// <param name="motoAtualizada">Instância da entidade Moto contendo os novos dados da moto.</param>
        /// <returns>Task representando a operação assíncrona.</returns>
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

            await _motoRepository.AtualizarMotoAsync(id, motoAtualizada);
        }
        public async Task<Moto?> ObterMotoPorIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("O identificador da moto não pode ser nulo ou vazio.", nameof(id));
            }

            return await _motoRepository.ObterMotoPorIdAsync(id);
        }
        public async Task<IEnumerable<Moto>> ConsultarMotosAsync(string? placa = null)
        {
            return await _motoRepository.ConsultarMotosAsync(placa);
        }
        public async Task AtualizarPlacaAsync(string id, string novaPlaca)
        {
            var motoExistente = await _motoRepository.ObterMotoPorIdAsync(id);
            if (motoExistente == null)
            {
                throw new KeyNotFoundException("Moto não encontrada.");
            }

            motoExistente.Placa = novaPlaca;

            await _motoRepository.AtualizarMotoAsync(id, motoExistente);
        }
        public async Task RemoverMotoAsync(string id)
        {
            // Verificar se a moto tem registros de locação
            var temLocacoes = await _motoRepository.VerificarLocacoesAsync(id);
            if (temLocacoes)
            {
                throw new InvalidOperationException("Não é possível remover a moto. Existem registros de locação associados.");
            }

            // Remover a moto
            await _motoRepository.RemoverMotoAsync(id);
        }
    }
}
