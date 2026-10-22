using FacturasApi.Models;
using MongoDB.Driver;

namespace FacturasApi.Services;

public class FacturaService : IFacturaService
{
    private readonly IMongoCollection<Factura> _facturasCollection;
    private readonly IMongoCollection<Cliente> _clientesCollection;
    private readonly IEmailService _emailService;

    public FacturaService(IMongoDatabase database, IEmailService emailService)
    {
        _facturasCollection = database.GetCollection<Factura>("Facturas");
        _clientesCollection = database.GetCollection<Cliente>("Clientes");
        _emailService = emailService;
    }

    public async Task<List<Factura>> ObtenerTodasAsync()
    {
        return await _facturasCollection.Find(_ => true).ToListAsync();
    }

    public async Task<List<Factura>> ObtenerPorClienteAsync(string clienteId)
    {
        var cliente = await _clientesCollection.Find(c => c.Id == clienteId).FirstOrDefaultAsync();
        if (cliente == null)
        {
            throw new KeyNotFoundException($"El cliente con ID '{clienteId}' no existe.");
        }

        return await _facturasCollection.Find(f => f.ClienteId == clienteId).ToListAsync();
    }

    public async Task<int> ProcesarRecordatoriosPorClienteAsync(string clienteId)
    {
        var cliente = await _clientesCollection.Find(c => c.Id == clienteId).FirstOrDefaultAsync();
        if (cliente == null)
        {
            throw new KeyNotFoundException($"El cliente con ID '{clienteId}' no existe.");
        }

        var filter = Builders<Factura>.Filter.And(
            Builders<Factura>.Filter.Eq(f => f.ClienteId, clienteId),
            Builders<Factura>.Filter.In(f => f.Estado, new[] { "primerrecordatorio", "segundorecordatorio" })
        );

        var facturasAProcesar = await _facturasCollection.Find(filter).ToListAsync();
        int procesadas = 0;

        foreach (var factura in facturasAProcesar)
        {
            if (factura.Estado == "primerrecordatorio")
            {
                string cuerpo = $"Hola {cliente.Nombre}, su factura por ${factura.Monto} ha pasado a <b>segundo recordatorio</b>.";
                
                // Si este envío falla, se lanza la excepción, se detiene el bucle y NO se ejecuta el UpdateOneAsync
                await _emailService.SendEmailAsync(cliente.Email, "Aviso: Segundo Recordatorio de Pago", cuerpo);

                var update = Builders<Factura>.Update.Set(f => f.Estado, "segundorecordatorio");
                await _facturasCollection.UpdateOneAsync(f => f.Id == factura.Id, update);
                procesadas++;
            }
            else if (factura.Estado == "segundorecordatorio")
            {
                string cuerpo = $"Hola {cliente.Nombre}, su factura por ${factura.Monto} no ha sido pagada. Su servicio va a ser <b>desactivado</b>.";
                
                // Si este envío falla, se lanza la excepción, se detiene el bucle y NO se ejecuta el UpdateOneAsync
                await _emailService.SendEmailAsync(cliente.Email, "Urgente: Servicio a Ser Desactivado", cuerpo);

                var update = Builders<Factura>.Update.Set(f => f.Estado, "desactivado");
                await _facturasCollection.UpdateOneAsync(f => f.Id == factura.Id, update);
                procesadas++;
            }
        }

        return procesadas;
    }
}