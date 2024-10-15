using System;
using System.Threading.Tasks;
using Desafio.backend.Mottu.Dominio.Interfaces;
using Desafio.backend.Mottu.Dominio.Entidades;

public class LocacaoService : ILocacaoService
{
    private readonly IEntregadorRepository _entregadorRepository;
    private readonly IMotoRepository _motoRepository;

    public LocacaoService(IEntregadorRepository entregadorRepository, IMotoRepository motoRepository)
    {
        _entregadorRepository = entregadorRepository;
        _motoRepository = motoRepository;
    }

    public async Task<string> AlugarMotoAsync(string entregadorId, string motoId, PlanoLocacao plano)
    {
        // Verificar se o entregador está habilitado na categoria A
        var entregador = await _entregadorRepository.ObterPorIdAsync(entregadorId);
        if (entregador == null || !entregador.TipoCNH.Contains("A"))
        {
            throw new Exception("Somente entregadores habilitados na categoria A podem efetuar uma locação.");
        }

        // Configuração das datas
        var dataCriacao = DateTime.UtcNow;
        var dataInicio = dataCriacao.AddDays(1);  // A locação começa no dia seguinte à criação
        var dataTermino = dataInicio.AddDays((int)plano);
        var previsaoTermino = dataTermino.AddDays(3); // Exemplo de previsão de término (ajustável conforme necessidade)

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

        // (Aqui você adicionaria o código para salvar a locação no banco de dados)

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

    // Novo método para calcular o valor da locação com base na data de devolução
    public decimal CalcularValorTotalComDevolucao(Locacao locacao, DateTime dataDevolucao)
    {
        decimal valorTotal = CalcularValorLocacao(locacao.Plano); // Aqui está a correção

        if (dataDevolucao < locacao.PrevisaoTermino)
        {
            // Se a devolução for antes da previsão de término, aplicar multa
            int diasNaoEfetivados = (locacao.PrevisaoTermino - dataDevolucao).Days;
            decimal multa = CalcularMulta(locacao.Plano, diasNaoEfetivados);
            valorTotal += multa;
        }
        else if (dataDevolucao > locacao.PrevisaoTermino)
        {
            // Se a devolução for após a previsão de término, cobrar diárias extras
            int diasAdicionais = (dataDevolucao - locacao.PrevisaoTermino).Days;
            decimal valorAdicional = diasAdicionais * 50m;  // R$50,00 por dia adicional
            valorTotal += valorAdicional;
        }

        return valorTotal;
    }

    // Método auxiliar para calcular a multa com base no plano
    private decimal CalcularMulta(PlanoLocacao plano, int diasNaoEfetivados)
    {
        decimal valorMulta = 0;

        switch (plano)
        {
            case PlanoLocacao.Plano7Dias:
                valorMulta = 30m * diasNaoEfetivados * 0.20m;  // 20% de multa
                break;
            case PlanoLocacao.Plano15Dias:
                valorMulta = 28m * diasNaoEfetivados * 0.40m;  // 40% de multa
                break;
            case PlanoLocacao.Plano30Dias:
            case PlanoLocacao.Plano45Dias:
            case PlanoLocacao.Plano50Dias:
                // Sem multas específicas para esses planos
                break;
        }

        return valorMulta;
    }
}
