namespace Desafio.backend.Mottu.Dominio.Dto.EntregadorEvento
{
    /// <summary>
    /// Representa um evento de cadastro de entregador na plataforma.
    /// Este evento é utilizado para notificar outros serviços e sistemas
    /// sobre a criação de um novo entregador.
    /// </summary>
    public class EntregadorCadastradoEvento
    {
        /// <summary>
        /// Identificador único do entregador.
        /// </summary>
        public string IdEntregador { get; set; }

        /// <summary>
        /// Nome do entregador.
        /// </summary>
        public string Nome { get; set; }

        /// <summary>
        /// CNPJ do entregador.
        /// </summary>
        public string CNPJ { get; set; }

        /// <summary>
        /// Tipo da CNH do entregador (ex.: A, B ou A+B).
        /// </summary>
        public string TipoCNH { get; set; }
    }
}
