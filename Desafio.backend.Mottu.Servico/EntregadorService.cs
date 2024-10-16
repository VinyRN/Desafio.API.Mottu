using AutoMapper;
using Desafio.backend.Mottu.Dominio.Dto.EntregadorEvento;
using Desafio.backend.Mottu.Dominio.Dto.Request;
using Desafio.backend.Mottu.Dominio.Entidades;
using Desafio.backend.Mottu.Dominio.Interfaces;
using Desafio.backend.Mottu.Queue.Interfaces;

namespace Desafio.backend.Mottu.Servico
{
    public class EntregadorService : IEntregadorService
    {
        private readonly IEntregadorRepository _entregadorRepository;
        private readonly IMapper _mapper;
        private readonly IMensageriaService _mensageriaService;
        private readonly IElasticLogService _logService;

        public EntregadorService(IEntregadorRepository entregadorRepository, IMapper mapper, IMensageriaService mensageriaService, IElasticLogService logService)
        {
            _entregadorRepository = entregadorRepository;
            _mapper = mapper;
            _mensageriaService = mensageriaService;
            _logService = logService;
        }

        public async Task CadastrarEntregadorAsync(EntregadorRequestDTO entregadorRequestDTO)
        {
            // Mapear o DTO para a entidade Entregador
            var entregador = _mapper.Map<Entregador>(entregadorRequestDTO);

            // Verificar se o tipo da CNH é inválido
            if (entregador.TipoCNHInvalido())
            {
                await _logService.LogErrorAsync("Erro ao cadastrar entregador: Tipo de CNH inválido.");
                return;
            }

            // Verificar se o tipo da CNH é válido
            if (!entregador.TipoCNHValido())
            {
                await _logService.LogErrorAsync("Erro ao cadastrar entregador: Tipo de CNH inválido. Tipos válidos são: A, B ou A+B.");
                return;
            }

            // Verificar unicidade do CNPJ
            var cnpjExistente = await _entregadorRepository.CNPJExistenteAsync(entregador.CNPJ);
            if (cnpjExistente)
            {
                await _logService.LogErrorAsync("Erro ao cadastrar entregador: CNPJ já cadastrado.");
                return;
            }

            // Verificar unicidade do número da CNH
            var cnhExistente = await _entregadorRepository.CNHExistenteAsync(entregador.NumeroCNH);
            if (cnhExistente)
            {
                await _logService.LogErrorAsync("Erro ao cadastrar entregador: Número de CNH já cadastrado.");
                return;
            }

            // Gravar o entregador
            await _entregadorRepository.GravarEntregadorAsync(entregador);

            // Publicar o evento de EntregadorCadastrado
            var evento = new EntregadorCadastradoEvento
            {
                IdEntregador = entregador.IdEntregador,
                Nome = entregador.Nome,
                CNPJ = entregador.CNPJ,
                TipoCNH = entregador.TipoCNH
            };

            _mensageriaService.PublicarMensagem(evento, "entregador_cadastrado_queue");
            await _logService.LogInfoAsync($"Entregador cadastrado com sucesso: {entregador.IdEntregador}");
        }

        public async Task<bool> AtualizarCnhAsync(string entregadorId, string cnhBase64)
        {
            // Busca o entregador pelo ID
            var entregador = await _entregadorRepository.ObterPorIdAsync(entregadorId);
            if (entregador == null)
            {
                await _logService.LogErrorAsync($"Erro ao atualizar CNH: Entregador não encontrado. ID: {entregadorId}");
                return false;
            }

            // Atualiza a CNH em base64
            entregador.CnhBase64 = cnhBase64;

            // Publicar o evento de EntregadorAtualizado
            var evento = new EntregadorCadastradoEvento
            {
                IdEntregador = entregador.IdEntregador,
                Nome = entregador.Nome,
                CNPJ = entregador.CNPJ,
                TipoCNH = entregador.TipoCNH
            };

            _mensageriaService.PublicarMensagem(evento, "entregador_atualizado_queue");
            await _logService.LogInfoAsync($"CNH do entregador atualizada: {entregador.IdEntregador}");

            // Salva a atualização no banco de dados
            return await _entregadorRepository.AtualizarAsync(entregador);
        }
    }
}
