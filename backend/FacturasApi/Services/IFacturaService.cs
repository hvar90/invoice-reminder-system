using FacturasApi.Models;

namespace FacturasApi.Services;

public interface IFacturaService
{
    Task<List<Factura>> ObtenerTodasAsync();
    Task<int> ProcesarRecordatoriosAsync();
}