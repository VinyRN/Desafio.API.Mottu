namespace Desafio.backend.Mottu.Dominio.Entidades
{
    public class Entregador
    {
        /// <summary>
        /// Identificador único do entregador.
        /// </summary>
        public string IdEntregador { get; set; } = string.Empty;

        /// <summary>
        /// Nome completo do entregador.
        /// </summary>
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// CNPJ do entregador, que deve ser único.
        /// </summary>
        public string CNPJ { get; set; } = string.Empty;

        /// <summary>
        /// Data de nascimento do entregador.
        /// </summary>
        public DateTime DataNascimento { get; set; }

        /// <summary>
        /// Número da CNH do entregador, que deve ser único.
        /// </summary>
        public string NumeroCNH { get; set; } = string.Empty;

        /// <summary>
        /// Tipo da CNH (A, B ou A+B).
        /// </summary>
        public string TipoCNH { get; set; } = string.Empty;

        /// <summary>
        /// Imagem da CNH em formato de URL ou base64.
        /// </summary>
        public string ImagemCNH { get; set; } = string.Empty;

        /// <summary>
        /// Valida se o tipo da CNH é válido (A, B, ou A+B), rejeitando categorias C e D.
        /// </summary>
        public bool TipoCNHValido()
        {
            var tiposValidos = new[] { "A", "B", "A+B" };
            return tiposValidos.Contains(TipoCNH);
        }

        /// <summary>
        /// Verifica se o tipo da CNH é inválido (como C ou D).
        /// </summary>
        public bool TipoCNHInvalido()
        {
            var tiposInvalidos = new[] { "C", "D", "C+D", "D+E", "E" };
            return tiposInvalidos.Contains(TipoCNH);
        }

        // Propriedade para armazenar a imagem da CNH em base64
        public string CnhBase64 { get; set; } = string.Empty;
    }
}
