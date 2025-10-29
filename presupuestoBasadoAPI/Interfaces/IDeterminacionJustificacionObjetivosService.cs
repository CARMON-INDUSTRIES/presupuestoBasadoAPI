using presupuestoBasadoAPI.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace presupuestoBasadoAPI.Services
{
    public interface IDeterminacionJustificacionObjetivosService
    {
        Task<IEnumerable<DeterminacionJustificacionObjetivosDto>> GetAllAsync(string userId);
        Task<DeterminacionJustificacionObjetivosDto> GetByIdAsync(int id, string userId);
        Task<DeterminacionJustificacionObjetivosDto> CreateAsync(DeterminacionJustificacionObjetivosDto dto, string userId);
        Task<bool> UpdateAsync(int id, DeterminacionJustificacionObjetivosDto dto, string userId);
        Task<bool> DeleteAsync(int id, string userId);
        Task<DeterminacionJustificacionObjetivosDto> GetUltimoAsync(string userId);
    }
}
