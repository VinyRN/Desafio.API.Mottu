namespace Desafio.backend.Mottu.Queue.Interfaces
{
    /// <summary>
    /// Interface para o serviço de mensageria utilizando RabbitMQ.
    /// Define operações para publicação de mensagens em filas específicas.
    /// </summary>
    public interface IMensageriaService
    {
        /// <summary>
        /// Publica uma mensagem em uma fila específica.
        /// </summary>
        /// <typeparam name="T">O tipo do objeto que será publicado como mensagem.</typeparam>
        /// <param name="mensagem">A mensagem a ser publicada na fila.</param>
        /// <param name="queueName">O nome da fila onde a mensagem será publicada.</param>
        void PublicarMensagem<T>(T mensagem, string queueName);
    }
}
