using Desafio.backend.Mottu.Dominio.Dto.MotoEvento;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Desafio.backend.Mottu.Infraestrutura.Mensageria
{
    public class MotoCadastradaConsumidor : IHostedService
    {
        private readonly IMongoCollection<MotoCadastradaEvento> _eventoCollection;
        private readonly IConnection _rabbitConnection;
        private IModel _channel;

        public MotoCadastradaConsumidor(IMongoDatabase database, IConnection rabbitConnection)
        {
            _eventoCollection = database.GetCollection<MotoCadastradaEvento>("moto_cadastrada_eventos");
            _rabbitConnection = rabbitConnection;
        }

        public void StartConsuming()
        {
            using (var channel = _rabbitConnection.CreateModel())
            {
                channel.QueueDeclare(queue: "moto_cadastrada_queue", durable: true, exclusive: false, autoDelete: false, arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += async (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var mensagem = Encoding.UTF8.GetString(body);
                    var evento = JsonSerializer.Deserialize<MotoCadastradaEvento>(mensagem);

                    if (evento?.Ano == 2024)
                    {
                        // Lógica para notificar se o ano da moto for "2024"
                        Console.WriteLine($"Moto do ano 2024 cadastrada: {evento.Modelo} - {evento.Placa}");

                        // Armazenar o evento no banco de dados para consulta futura
                        await _eventoCollection.InsertOneAsync(evento);
                    }
                };

                channel.BasicConsume(queue: "moto_cadastrada_queue", autoAck: true, consumer: consumer);
            }
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _channel = _rabbitConnection.CreateModel();
            _channel.QueueDeclare(queue: "moto_cadastrada_queue", durable: true, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var mensagem = Encoding.UTF8.GetString(body);
                var evento = JsonSerializer.Deserialize<MotoCadastradaEvento>(mensagem);

                if (evento?.Ano == 2024)
                {
                    // Lógica para notificar e armazenar o evento
                    Console.WriteLine($"Moto do ano 2024 cadastrada: {evento.Modelo} - {evento.Placa}");
                    await _eventoCollection.InsertOneAsync(evento);
                }
            };

            _channel.BasicConsume(queue: "moto_cadastrada_queue", autoAck: true, consumer: consumer);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _channel?.Close();
            _channel?.Dispose();
            return Task.CompletedTask;
        }
    }
}
