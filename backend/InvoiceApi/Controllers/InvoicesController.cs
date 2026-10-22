using InvoicesApi.Models;
using InvoicesApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace InvoicesApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InvoicesController : ControllerBase
{
    private readonly IInvoiceService _invoiceService;

    public InvoicesController(IInvoiceService invoiceService)
    {
        _invoiceService = invoiceService;
    }

    [HttpGet]
    public async Task<ActionResult<List<Invoice>>> Get()
    {
        var invoices = await _invoiceService.GetAllAsync();
        return Ok(invoices);
    }

    [HttpGet("customer/{customerId}")]
    public async Task<ActionResult<List<Invoice>>> GetByCustomer(string customerId)
    {
        var invoices = await _invoiceService.GetByCustomerAsync(customerId);
        return Ok(invoices);
    }

    [HttpPost("process-reminders/{customerId}")]
    public async Task<IActionResult> ProcessRemindersByCustomer(string customerId)
    {
        int processed = await _invoiceService.ProcessRemindersByCustomerAsync(customerId);
        return Ok(new { 
            message = $"Proceso finalizado para el cliente {customerId}", 
            customerId = customerId,
            processedInvoices = processed 
        });
    }
}