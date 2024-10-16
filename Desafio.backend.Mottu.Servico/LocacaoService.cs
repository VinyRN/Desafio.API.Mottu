using Desafio.backend.Mottu.Dominio.Interfaces;
using Desafio.backend.Mottu.Dominio.Entidades;
using Desafio.backend.Mottu.Queue.Interfaces;
using Desafio.backend.Mottu.Servico;
using Desafio.backend.Mottu.Dominio.Dto.LocacaoEvento;

public class LocacaoService : ILocacaoService
{
    private readonly IEntregadorRepository _entregadorRepository;
    private readonly IMotoRepository _motoRepository;
    private readonly IMensageriaService _mensageriaService;
    private readonly ILogService _logService;

    public LocacaoService(IEntregadorRepository entregadorRepository, IMotoRepository motoRepository, IMensageriaService mensageriaService, ILogService logService)
    {
        _entregadorRepository = entregadorRepository;
        _motoRepository = motoRepository;
        _mensageriaService = mensageriaService;
        _logService = logService;
    }

    public async Task<string> AlugarMotoAsync(string entregadorId, string motoId, PlanoLocacao plano)
    {
        // Verificar se o entregador está habilitado na categoria A
        var entregador = await _entregadorRepository.ObterPorIdAsync(entregadorId);
        if (entregador == null || !entregador.TipoCNH.Contains("A"))
        {
            await _logService.LogErrorAsync("Erro ao alugar moto: Somente entregadores habilitados na categoria A podem efetuar uma locação.");
            return null;
        }

        // Configuração das datas
        var dataCriacao = DateTime.UtcNow;
        var dataInicio = dataCriacao.AddDays(1);  // A locação começa no dia seguinte à criação
        var dataTermino = dataInicio.AddDays((int)plano);
        var previsaoTermino = dataTermino.AddDays(3);

        // Calcular o valor da locação com base no plano
        var valorTotal = CalcularValorLocacao(plano);

        // Criar a locação
        var locacao = new Locacao
        {
            IdLocacao = Guid.NewGuid().ToString(),
            EntregadorId = entregadorId,
            IdMoto = motoId,
            DataLocacao = dataCriacao,
            DataInicio = dataInicio,
            DataTermino = dataTermino,
            PrevisaoTermino = previsaoTermino,
            Plano = plano,
            ValorTotal = valorTotal
        };

        // Salvar a locação (simulação de persistência)

        // Publicar evento de locação criada
        var evento = new LocacaoCriadaEvento
        {
            IdLocacao = locacao.IdLocacao,
            EntregadorId = locacao.EntregadorId,
            IdMoto = locacao.IdMoto,
            DataInicio = locacao.DataInicio,
            DataTermino = locacao.DataTermino,
            Plano = locacao.Plano,
            ValorTotal = locacao.ValorTotal
        };

        _mensageriaService.PublicarMensagem(evento, "locacao_criada_queue");
        await _logService.LogInfoAsync($"Locação criada: {locacao.IdLocacao}");

        return locacao.IdLocacao; // Retorna o ID da locação criada
    }

    // Método para calcular o valor total com base no plano escolhido e nas diárias
    private decimal CalcularValorLocacao(PlanoLocacao plano)
    {
        return plano switch
        {
            PlanoLocacao.Plano7Dias => 30m * 7,
            PlanoLocacao.Plano15Dias => 28m * 15,
            PlanoLocacao.Plano30Dias => 22m * 30,
            PlanoLocacao.Plano45Dias => 20m * 45,
            PlanoLocacao.Plano50Dias => 18m * 50,
            _ => throw new ArgumentOutOfRangeException(nameof(plano), "Plano de locação inválido.")
        };
    }

    public decimal CalcularValorTotalComDevolucao(Locacao locacao, DateTime dataDevolucao)
    {
        decimal valorTotal = CalcularValorLocacao(locacao.Plano);

        if (dataDevolucao < locacao.PrevisaoTermino)
        {
            int diasNaoEfetivados = (locacao.PrevisaoTermino - dataDevolucao).Days;
            decimal multa = CalcularMulta(locacao.Plano, diasNaoEfetivados);
            valorTotal += multa;

            _logService.LogInfoAsync($"Multa aplicada para locação: {locacao.IdLocacao} com {diasNaoEfetivados} dias não efetivados.").Wait();
        }
        else if (dataDevolucao > locacao.PrevisaoTermino)
        {
            int diasAdicionais = (dataDevolucao - locacao.PrevisaoTermino).Days;
            decimal valorAdicional = diasAdicionais * 50m;
            valorTotal += valorAdicional;

            _logService.LogInfoAsync($"Diárias adicionais cobradas para locação: {locacao.IdLocacao} com {diasAdicionais} dias adicionais.").Wait();
        }

        return valorTotal;
    }

    private decimal CalcularMulta(PlanoLocacao plano, int diasNaoEfetivados)
    {
        decimal valorMulta = 0;

        switch (plano)
        {
            case PlanoLocacao.Plano7Dias:
                valorMulta = 30m * diasNaoEfetivados * 0.20m;
                break;
            case PlanoLocacao.Plano15Dias:
                valorMulta = 28m * diasNaoEfetivados * 0.40m;
                break;
        }

        return valorMulta;
    }
}
