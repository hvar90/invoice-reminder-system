using InvoicesApi.Models;
using MongoDB.Driver;

namespace InvoicesApi.Services;

public class InvoiceService : IInvoiceService
{
    private readonly IMongoCollection<Invoice> _invoicesCollection;
    private readonly IMongoCollection<Customer> _customersCollection;
    private readonly IEmailService _emailService;

    public InvoiceService(IMongoDatabase database, IEmailService emailService)
    {
        _invoicesCollection = database.GetCollection<Invoice>("Invoices");
        _customersCollection = database.GetCollection<Customer>("Customers");
        _emailService = emailService;
    }

    public async Task<List<Invoice>> GetAllAsync()
    {
        return await _invoicesCollection.Find(_ => true).ToListAsync();
    }

    public async Task<List<Invoice>> GetByCustomerAsync(string customerId)
    {
        var customer = await _customersCollection.Find(c => c.Id == customerId).FirstOrDefaultAsync();
        if (customer == null)
        {
            throw new KeyNotFoundException($"El cliente con ID '{customerId}' no existe.");
        }

        return await _invoicesCollection.Find(i => i.CustomerId == customerId).ToListAsync();
    }

    public async Task<int> ProcessRemindersByCustomerAsync(string customerId)
    {
        var customer = await _customersCollection.Find(c => c.Id == customerId).FirstOrDefaultAsync();
        if (customer == null)
        {
            throw new KeyNotFoundException($"El cliente con ID '{customerId}' no existe.");
        }

        var filter = Builders<Invoice>.Filter.And(
            Builders<Invoice>.Filter.Eq(i => i.CustomerId, customerId),
            Builders<Invoice>.Filter.In(i => i.Status, new[] { "primerrecordatorio", "segundorecordatorio" })
        );

        var invoicesToProcess = await _invoicesCollection.Find(filter).ToListAsync();
        int processed = 0;

        foreach (var invoice in invoicesToProcess)
        {
            if (invoice.Status == "primerrecordatorio")
            {
                string body = $"Hola {customer.Name}, su factura por ${invoice.Amount} ha pasado a <b>segundo recordatorio</b>.";
                
                // Si este envío falla, se lanza la excepción, se detiene el bucle y NO se ejecuta el UpdateOneAsync
                await _emailService.SendEmailAsync(customer.Email, "Aviso: Segundo Recordatorio de Pago", body);

                var update = Builders<Invoice>.Update.Set(i => i.Status, "segundorecordatorio");
                await _invoicesCollection.UpdateOneAsync(i => i.Id == invoice.Id, update);
                processed++;
            }
            else if (invoice.Status == "segundorecordatorio")
            {
                string body = $"Hola {customer.Name}, su factura por ${invoice.Amount} no ha sido pagada. Su servicio va a ser <b>desactivado</b>.";
                
                // Si este envío falla, se lanza la excepción, se detiene el bucle y NO se ejecuta el UpdateOneAsync
                await _emailService.SendEmailAsync(customer.Email, "Urgente: Servicio a Ser Desactivado", body);

                var update = Builders<Invoice>.Update.Set(i => i.Status, "desactivado");
                await _invoicesCollection.UpdateOneAsync(i => i.Id == invoice.Id, update);
                processed++;
            }
        }

        return processed;
    }
}