using InvoicesApi.Models;

namespace InvoicesApi.Services;

public interface IInvoiceService
{
    Task<List<Invoice>> GetAllAsync();
    Task<List<Invoice>> GetByCustomerAsync(string customerId);
    Task<int> ProcessRemindersByCustomerAsync(string customerId);
}