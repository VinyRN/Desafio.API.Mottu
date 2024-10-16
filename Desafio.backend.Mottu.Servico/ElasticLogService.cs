using Desafio.backend.Mottu.Dominio.Interfaces;
using System.Text;
using System.Text.Json;

namespace Desafio.backend.Mottu.Servico
{
    public class ElasticLogService : IElasticLogService
    {
        private readonly HttpClient _httpClient;
        private readonly string _elasticUrl;

        public ElasticLogService(IHttpClientFactory httpClientFactory, string elasticUrl)
        {
            _httpClient = httpClientFactory.CreateClient();
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

            // Use diretamente a instância de _httpClient injetada
            var response = await _httpClient.PostAsync($"{_elasticUrl}/logs/_doc", content);

            // Verificar se a resposta foi bem-sucedida
            if (!response.IsSuccessStatusCode)
            {
                // Tratamento de erro: ler a resposta para fins de diagnóstico
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Erro ao enviar log para o Elasticsearch. Status Code: {response.StatusCode}. Resposta: {responseContent}");
            }
        }
    }
}
