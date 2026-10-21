using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FacturasApi.Models;

public class Factura
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("clienteId")]
    public string ClienteId { get; set; } = string.Empty;

    [BsonElement("nombreCliente")]
    public string NombreCliente { get; set; } = string.Empty;

    [BsonElement("emailCliente")]
    public string EmailCliente { get; set; } = string.Empty;

    [BsonElement("monto")]
    public decimal Monto { get; set; }

    [BsonElement("estado")]
    public string Estado { get; set; } = "primerrecordatorio";
}