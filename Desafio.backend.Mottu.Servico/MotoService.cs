using Desafio.backend.Mottu.Dominio.Entidades;
using Desafio.backend.Mottu.Dominio.Interfaces;
using Desafio.backend.Mottu.Queue.Interfaces;
using Desafio.backend.Mottu.Dominio.Dto.MotoEvento;
using Amazon.Runtime;

namespace Desafio.backend.Mottu.Servico
{
    public class MotoService : IMotoService
    {
        private readonly IMotoRepository _motoRepository;
        private readonly IMensageriaService _mensageriaService;
        private readonly IElasticLogService _logService;
        private readonly IHttpClientFactory _httpClientFactory;


        public MotoService(IMotoRepository motoRepository, IMensageriaService mensageriaService, IElasticLogService logService, IHttpClientFactory httpClientFactory)
        {
            _motoRepository = motoRepository;
            _mensageriaService = mensageriaService;
            _logService = logService;
            _httpClientFactory = httpClientFactory;
        }

        public async Task GravarMotoAsync(Moto moto)
        {
            await _motoRepository.GravarMotoAsync(moto);

            var evento = new MotoCadastradaEvento
            {
                IdMoto = moto.IdMoto,
                Ano = moto.Ano,
                Modelo = moto.Modelo,
                Placa = moto.Placa
            };

            _mensageriaService.PublicarMensagem(evento, "moto_cadastrada_queue");

            await _logService.LogInfoAsync($"Moto cadastrada: {moto.IdMoto}");
        }

        public async Task AtualizarMotoAsync(string id, Moto motoAtualizada)
        {
            if (string.IsNullOrEmpty(id) || motoAtualizada == null)
            {
                await _logService.LogErrorAsync($"Erro ao atualizar moto: ID ou dados inválidos.");
                return;
            }

            await _motoRepository.AtualizarMotoAsync(id, motoAtualizada);

            var evento = new MotoAtualizadaEvento
            {
                IdMoto = motoAtualizada.IdMoto,
                Ano = motoAtualizada.Ano,
                Modelo = motoAtualizada.Modelo,
                Placa = motoAtualizada.Placa
            };

            _mensageriaService.PublicarMensagem(evento, "moto_atualizada_queue");

            await _logService.LogInfoAsync($"Moto atualizada: {motoAtualizada.IdMoto}");
        }

        public async Task<Moto?> ObterMotoPorIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                await _logService.LogErrorAsync("Erro ao obter moto: ID inválido.");
                return null;
            }

            var moto = await _motoRepository.ObterMotoPorIdAsync(id);
            if (moto == null)
            {
                await _logService.LogInfoAsync($"Moto não encontrada: {id}");
            }

            return moto;
        }

        public async Task<IEnumerable<Moto>> ConsultarMotosAsync(string? placa = null)
        {
            var motos = await _motoRepository.ConsultarMotosAsync(placa);
            await _logService.LogInfoAsync("Consulta de motos realizada.");
            return motos;
        }

        public async Task AtualizarPlacaAsync(string id, string novaPlaca)
        {
            var motoExistente = await _motoRepository.ObterMotoPorIdAsync(id);
            if (motoExistente == null)
            {
                await _logService.LogErrorAsync($"Erro ao atualizar placa: Moto não encontrada. ID: {id}");
                return;
            }

            motoExistente.Placa = novaPlaca;
            await _motoRepository.AtualizarMotoAsync(id, motoExistente);

            await _logService.LogInfoAsync($"Placa da moto atualizada: {motoExistente.IdMoto}");
        }

        public async Task RemoverMotoAsync(string id)
        {
            var temLocacoes = await _motoRepository.VerificarLocacoesAsync(id);
            if (temLocacoes)
            {
                await _logService.LogErrorAsync($"Erro ao remover moto: Existem locações associadas. ID: {id}");
                return;
            }

            var moto = await _motoRepository.ObterMotoPorIdAsync(id);
            if (moto == null)
            {
                await _logService.LogInfoAsync($"Moto não encontrada para remoção. ID: {id}");
                return;
            }

            await _motoRepository.RemoverMotoAsync(id);

            var evento = new MotoRemovidaEvento
            {
                IdMoto = moto.IdMoto,
                Ano = moto.Ano,
                Modelo = moto.Modelo,
                Placa = moto.Placa
            };

            _mensageriaService.PublicarMensagem(evento, "moto_removida_queue");

            await _logService.LogInfoAsync($"Moto removida: {moto.IdMoto}");
        }
    }
}
