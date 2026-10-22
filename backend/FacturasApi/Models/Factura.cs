using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FacturasApi.Models;

public class Factura
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("clienteId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string ClienteId { get; set; } = string.Empty;

    [BsonElement("monto")]
    public decimal Monto { get; set; }

    [BsonElement("estado")]
    public string Estado { get; set; } = "primerrecordatorio";
}