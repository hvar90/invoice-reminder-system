using InvoicesApi.Models;
using InvoicesApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace InvoicesApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomersController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpGet]
    public async Task<ActionResult<List<Customer>>> Get()
    {
        var customers = await _customerService.GetAllAsync();
        return Ok(customers);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Customer>> GetById(string id)
    {
        var customer = await _customerService.GetByIdAsync(id);
        
        if (customer == null)
        {
            return NotFound(new { message = $"Cliente con ID {id} no encontrado." });
        }

        return Ok(customer);
    }
}