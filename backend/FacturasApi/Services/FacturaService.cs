using FacturasApi.Models;
using MongoDB.Driver;

namespace FacturasApi.Services;

public class FacturaService : IFacturaService
{
    private readonly IMongoCollection<Factura> _facturasCollection;
    private readonly IEmailService _emailService;

    public FacturaService(IMongoDatabase database, IEmailService emailService)
    {
        _facturasCollection = database.GetCollection<Factura>("Facturas");
        _emailService = emailService;
    }

    public async Task<List<Factura>> ObtenerTodasAsync()
    {
        return await _facturasCollection.Find(_ => true).ToListAsync();
    }

    public async Task<int> ProcesarRecordatoriosAsync()
    {
        // Filtrar estados que requieren procesamiento
        var filter = Builders<Factura>.Filter.In(f => f.Estado, new[] { "primerrecordatorio", "segundorecordatorio" });
        var facturasAProcesar = await _facturasCollection.Find(filter).ToListAsync();

        int procesadas = 0;

        foreach (var factura in facturasAProcesar)
        {
            if (factura.Estado == "primerrecordatorio")
            {
                string cuerpo = $"Hola {factura.NombreCliente}, su factura por ${factura.Monto} ha pasado a <b>segundo recordatorio</b>.";
                await _emailService.SendEmailAsync(factura.EmailCliente, "Aviso: Segundo Recordatorio de Pago", cuerpo);

                var update = Builders<Factura>.Update.Set(f => f.Estado, "segundorecordatorio");
                await _facturasCollection.UpdateOneAsync(f => f.Id == factura.Id, update);
                procesadas++;
            }
            else if (factura.Estado == "segundorecordatorio")
            {
                string cuerpo = $"Hola {factura.NombreCliente}, su factura por ${factura.Monto} no ha sido pagada. Su servicio va a ser <b>desactivado</b>.";
                await _emailService.SendEmailAsync(factura.EmailCliente, "Urgente: Servicio a Ser Desactivado", cuerpo);

                var update = Builders<Factura>.Update.Set(f => f.Estado, "desactivado");
                await _facturasCollection.UpdateOneAsync(f => f.Id == factura.Id, update);
                procesadas++;
            }
        }

        return procesadas;
    }
}