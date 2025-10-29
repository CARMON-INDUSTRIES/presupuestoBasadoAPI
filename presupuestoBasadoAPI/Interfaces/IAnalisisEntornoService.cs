using presupuestoBasadoAPI.Dto;
using presupuestoBasadoAPI.Models;

namespace presupuestoBasadoAPI.Interfaces
{
    public interface IAnalisisEntornoService
    {
        Task<AnalisisEntorno> CrearAsync(AnalisisEntornoDto dto, string userId);
        Task<List<AnalisisEntorno>> ObtenerTodosAsync(string userId);
        Task<AnalisisEntorno?> ObtenerPorIdAsync(int id, string userId);
    }
}
