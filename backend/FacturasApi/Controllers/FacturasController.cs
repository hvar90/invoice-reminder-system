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

    [HttpPost("procesar-recordatorios")]
    public async Task<IActionResult> ProcesarRecordatorios()
    {
        int procesadas = await _facturaService.ProcesarRecordatoriosAsync();
        return Ok(new { mensaje = "Proceso finalizado con éxito", facturasProcesadas = procesadas });
    }
}