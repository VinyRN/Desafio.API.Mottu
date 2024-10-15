namespace Desafio.backend.Mottu.Dominio.Dto
{
    /// <summary>
    /// DTO de requisição para manipulação de dados da moto.
    /// </summary>
    public class MotoRequisicaoDto
    {
        /// <summary>
        /// Identificador único da moto.
        /// </summary>
        public string IdMoto { get; set; } = string.Empty;

        /// <summary>
        /// Ano de fabricação da moto.
        /// </summary>
        public required int Ano { get; set; }

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
