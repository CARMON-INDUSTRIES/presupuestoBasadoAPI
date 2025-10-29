using presupuestoBasadoAPI.Dto;
using presupuestoBasadoAPI.Models;
using System.Threading.Tasks;

namespace presupuestoBasadoAPI.Interfaces
{
    public interface IReglasOperacionService
    {
        Task<ReglasOperacion> CrearAsync(ReglasOperacionDto dto, string userId);
        Task<ReglasOperacion?> ObtenerUltimoAsync(string userId);
    }
}
