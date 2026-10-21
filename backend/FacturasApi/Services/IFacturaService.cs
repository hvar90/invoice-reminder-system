using FacturasApi.Models;

namespace FacturasApi.Services;

public interface IFacturaService
{
    Task<List<Factura>> ObtenerTodasAsync();
    Task<List<Factura>> ObtenerPorClienteAsync(string clienteId);
    Task<int> ProcesarRecordatoriosPorClienteAsync(string clienteId);
}