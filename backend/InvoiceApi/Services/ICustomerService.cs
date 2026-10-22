using InvoicesApi.Models;

namespace InvoicesApi.Services;

public interface ICustomerService
{
    Task<List<Customer>> GetAllAsync();
    Task<Customer?> GetByIdAsync(string id);
}