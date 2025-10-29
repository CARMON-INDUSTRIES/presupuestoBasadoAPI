using presupuestoBasadoAPI.Dto;
using presupuestoBasadoAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace presupuestoBasadoAPI.Interfaces
{
    public interface IJustificacionProgramaService
    {
        Task<JustificacionPrograma> CrearAsync(JustificacionProgramaDto dto, string userId);
        Task<List<JustificacionPrograma>> ObtenerTodosAsync(string userId);
        Task<JustificacionPrograma?> ObtenerPorIdAsync(int id, string userId);
        Task<JustificacionPrograma?> ObtenerUltimoAsync(string userId);
    }
}
