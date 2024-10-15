namespace Desafio.backend.Mottu.Dominio.Entidades
{
    public class Locacao
    {
        /// <summary>
        /// Identificador único da locação.
        /// </summary>
        public string IdLocacao { get; set; } = string.Empty;

        /// <summary>
        /// Identificador da moto associada à locação.
        /// </summary>
        public string IdMoto { get; set; } = string.Empty;

        /// <summary>
        /// Identificador do entregador que alugou a moto.
        /// </summary>
        public string EntregadorId { get; set; } = string.Empty;

        /// <summary>
        /// Nome do cliente que alugou a moto.
        /// </summary>
        public string Cliente { get; set; } = string.Empty;

        /// <summary>
        /// Data em que a locação foi criada.
        /// </summary>
        public DateTime DataLocacao { get; set; }

        /// <summary>
        /// Data de início da locação.
        /// </summary>
        public DateTime DataInicio { get; set; }

        /// <summary>
        /// Data de término da locação.
        /// </summary>
        public DateTime DataTermino { get; set; }

        /// <summary>
        /// Previsão de data de término da locação (para possíveis atrasos).
        /// </summary>
        public DateTime PrevisaoTermino { get; set; }

        /// <summary>
        /// Data de devolução da moto.
        /// </summary>
        public DateTime? DataDevolucao { get; set; }

        /// <summary>
        /// Plano de locação escolhido (7, 15, 30, 45, 50 dias).
        /// </summary>
        public PlanoLocacao Plano { get; set; }

        /// <summary>
        /// Valor total da locação com base no plano escolhido.
        /// </summary>
        public decimal ValorTotal { get; set; }
    }

    /// <summary>
    /// Enum que define os planos de locação disponíveis.
    /// </summary>
    public enum PlanoLocacao
    {
        Plano7Dias = 7,
        Plano15Dias = 15,
        Plano30Dias = 30,
        Plano45Dias = 45,
        Plano50Dias = 50
    }
}
