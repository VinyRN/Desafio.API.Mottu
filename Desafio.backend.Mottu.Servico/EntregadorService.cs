using AutoMapper;
using Desafio.backend.Mottu.Dominio.Dto;
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

        public EntregadorService(IEntregadorRepository entregadorRepository, IMapper mapper, IMensageriaService mensageriaService)
        {
            _entregadorRepository = entregadorRepository;
            _mapper = mapper;
            _mensageriaService = mensageriaService;
        }

        public async Task CadastrarEntregadorAsync(EntregadorRequestDTO entregadorRequestDTO)
        {
            // Mapear o DTO para a entidade Entregador
            var entregador = _mapper.Map<Entregador>(entregadorRequestDTO);

            // Verificar se o tipo da CNH é inválido (C, D, etc.)
            if (entregador.TipoCNHInvalido())
            {
                throw new InvalidOperationException("Tipo de CNH inválido. Tipos C e D não são aceitos.");
            }

            // Verificar se o tipo da CNH é válido
            if (!entregador.TipoCNHValido())
            {
                throw new InvalidOperationException("Tipo de CNH inválido. Tipos válidos são: A, B ou A+B.");
            }

            // Verificar unicidade do CNPJ
            var cnpjExistente = await _entregadorRepository.CNPJExistenteAsync(entregador.CNPJ);
            if (cnpjExistente)
            {
                throw new InvalidOperationException("CNPJ já cadastrado.");
            }

            // Verificar unicidade do número da CNH
            var cnhExistente = await _entregadorRepository.CNHExistenteAsync(entregador.NumeroCNH);
            if (cnhExistente)
            {
                throw new InvalidOperationException("Número de CNH já cadastrado.");
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
        }

        public async Task<bool> AtualizarCnhAsync(string entregadorId, string cnhBase64)
        {
            // Busca o entregador pelo ID
            var entregador = await _entregadorRepository.ObterPorIdAsync(entregadorId);
            if (entregador == null)
                return false;

            // Atualiza a CNH em base64
            entregador.CnhBase64 = cnhBase64;

            // Publicar o evento de EntregadorCadastrado
            var evento = new EntregadorCadastradoEvento
            {
                IdEntregador = entregador.IdEntregador,
                Nome = entregador.Nome,
                CNPJ = entregador.CNPJ,
                TipoCNH = entregador.TipoCNH
            };

            _mensageriaService.PublicarMensagem(evento, "entregador_atualizado_queue");

            // Salva a atualização no banco de dados
            return await _entregadorRepository.AtualizarAsync(entregador);
        }
    }
}
