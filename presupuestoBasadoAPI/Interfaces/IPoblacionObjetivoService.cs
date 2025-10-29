using presupuestoBasadoAPI.Dto;
using presupuestoBasadoAPI.Models;

namespace presupuestoBasadoAPI.Interfaces
{
    public interface IPoblacionObjetivoService
    {
        Task<PoblacionObjetivo> CrearAsync(PoblacionObjetivoDto dto);
        Task<List<PoblacionObjetivo>> ObtenerTodosAsync(string userId);  // 🔹 filtrar por usuario
        Task<PoblacionObjetivo?> ObtenerPorIdAsync(int id);
        Task<PoblacionObjetivo?> ObtenerUltimoAsync(string userId);      // 🔹 filtrar por usuario
    }
}
