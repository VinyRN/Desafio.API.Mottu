namespace Desafio.backend.Mottu.Dominio.Interfaces
{
    public interface IElasticLogService
    {
        Task LogInfoAsync(string message);
        Task LogErrorAsync(string message);
    }
}
