using Desafio.backend.Mottu.Dominio.Entidades;

namespace Desafio.backend.Mottu.Dominio.Dto.LocacaoEvento
{
    /// <summary>
    /// Representa um evento de criação de locação.
    /// Este evento é usado para notificar outros serviços sobre a criação de uma nova locação de moto.
    /// </summary>
    public class LocacaoCriadaEvento
    {
        /// <summary>
        /// Identificador único da locação.
        /// </summary>
        public string IdLocacao { get; set; } 

        /// <summary>
        /// Identificador do entregador que realizou a locação.
        /// </summary>
        public string EntregadorId { get; set; }

        /// <summary>
        /// Identificador da moto alugada.
        /// </summary>
        public string IdMoto { get; set; }

        /// <summary>
        /// Data de início da locação.
        /// </summary>
        public DateTime DataInicio { get; set; }

        /// <summary>
        /// Data de término prevista para a locação.
        /// </summary>
        public DateTime DataTermino { get; set; }

        /// <summary>
        /// Plano de locação escolhido para a moto.
        /// </summary>
        public PlanoLocacao Plano { get; set; }

        /// <summary>
        /// Valor total calculado para a locação.
        /// </summary>
        public decimal ValorTotal { get; set; }
    }
}
