using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using Desafio.backend.Mottu.Queue.Interfaces;

namespace Desafio.backend.Mottu.Queue
{
    public class MensageriaService : IMensageriaService
    {
        private readonly IConnection _connection;

        public MensageriaService(IConnection connection)
        {
            _connection = connection;
        }

        public void PublicarMensagem<T>(T mensagem, string queueName)
        {
            using var channel = _connection.CreateModel();
            channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(mensagem));

            channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: body);
        }
    }
}
