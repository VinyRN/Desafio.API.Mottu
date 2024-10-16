using System.Text;
using System.Text.Json;

namespace Desafio.backend.Mottu.Servico
{
    public interface ILogService
    {
        Task LogInfoAsync(string message);
        Task LogErrorAsync(string message);
    }

    public class ElasticLogService : ILogService
    {
        private readonly HttpClient _httpClient;
        private readonly string _elasticUrl;

        public ElasticLogService(HttpClient httpClient, string elasticUrl)
        {
            _httpClient = httpClient;
            _elasticUrl = elasticUrl;
        }

        public async Task LogInfoAsync(string message)
        {
            await LogAsync("info", message);
        }

        public async Task LogErrorAsync(string message)
        {
            await LogAsync("error", message);
        }

        private async Task LogAsync(string logLevel, string message)
        {
            var logEntry = new
            {
                Level = logLevel,
                Message = message,
                Timestamp = DateTime.UtcNow
            };

            var content = new StringContent(JsonSerializer.Serialize(logEntry), Encoding.UTF8, "application/json");
            await _httpClient.PostAsync($"{_elasticUrl}/logs/_doc", content);
        }
    }
}
