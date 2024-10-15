using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace Desafio.backend.Mottu.Dominio.Entidades
{
    /// <summary>
    /// Representa uma moto no sistema, incluindo suas informações básicas.
    /// </summary>
    public class Moto
    {
        /// <summary>
        /// Identificador único da moto, gerado automaticamente pelo MongoDB.
        /// </summary>
        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        [BsonRepresentation(BsonType.ObjectId)]
        public string IdMoto { get; set; } = ObjectId.GenerateNewId().ToString();

        /// <summary>
        /// Ano de fabricação da moto.
        /// </summary>
        [BsonElement("ano")]
        [BsonRepresentation(BsonType.String)]
        public int Ano { get; set; }

        /// <summary>
        /// Modelo da moto.
        /// </summary>
        [BsonElement("modelo")]
        [BsonRepresentation(BsonType.String)]
        public required string Modelo { get; set; }

        /// <summary>
        /// Placa da moto, utilizada para identificação no sistema.
        /// </summary>
        [BsonElement("placa")]
        [BsonRepresentation(BsonType.String)]
        public required string Placa { get; set; }
    }
}
