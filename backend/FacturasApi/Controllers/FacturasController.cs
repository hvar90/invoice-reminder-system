using FacturasApi.Models;
using FacturasApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace FacturasApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FacturasController : ControllerBase
{
    private readonly IFacturaService _facturaService;

    public FacturasController(IFacturaService facturaService)
    {
        _facturaService = facturaService;
    }

    [HttpGet]
    public async Task<ActionResult<List<Factura>>> Get()
    {
        var facturas = await _facturaService.ObtenerTodasAsync();
        return Ok(facturas);
    }

    [HttpGet("cliente/{clienteId}")]
    public async Task<ActionResult<List<Factura>>> GetPorCliente(string clienteId)
    {
        var facturas = await _facturaService.ObtenerPorClienteAsync(clienteId);
        return Ok(facturas);
    }

    [HttpPost("procesar-recordatorios/{clienteId}")]
    public async Task<IActionResult> ProcesarRecordatoriosPorCliente(string clienteId)
    {
        int procesadas = await _facturaService.ProcesarRecordatoriosPorClienteAsync(clienteId);
        return Ok(new { 
            mensaje = $"Proceso finalizado para el cliente {clienteId}", 
            clienteId = clienteId,
            facturasProcesadas = procesadas 
        });
    }
}