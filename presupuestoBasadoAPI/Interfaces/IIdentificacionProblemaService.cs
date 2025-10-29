using presupuestoBasadoAPI.Dto;
using presupuestoBasadoAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace presupuestoBasadoAPI.Interfaces
{
    public interface IIdentificacionProblemaService
    {
        Task<IdentificacionProblema> CrearAsync(IdentificacionProblemaDto dto, string userId);
        Task<List<IdentificacionProblema>> ObtenerTodosAsync(string userId);
        Task<IdentificacionProblema?> ObtenerPorIdAsync(int id, string userId);
        Task<IdentificacionProblema?> ObtenerUltimoAsync(string userId);
    }
}
