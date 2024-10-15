namespace Desafio.backend.Mottu.Dominio.Dto
{
    /// <summary>
    /// DTO que representa o evento de uma moto cadastrada.
    /// </summary>
    public class MotoCadastradaEvento
    {
        /// <summary>
        /// Identificador da moto.
        /// </summary>
        public string IdMoto { get; set; } = string.Empty;

        /// <summary>
        /// Ano de fabricação da moto.
        /// </summary>
        public int Ano { get; set; }

        /// <summary>
        /// Modelo da moto.
        /// </summary>
        public required string Modelo { get; set; }

        /// <summary>
        /// Placa da moto.
        /// </summary>
        public required string Placa { get; set; }
    }
}
