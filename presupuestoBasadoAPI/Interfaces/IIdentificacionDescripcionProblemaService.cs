using presupuestoBasadoAPI.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace presupuestoBasadoAPI.Services
{
    public interface IIdentificacionDescripcionProblemaService
    {
        Task<IEnumerable<IdentificacionDescripcionProblemaDto>> GetAllAsync(string userId);
        Task<IdentificacionDescripcionProblemaDto> GetByIdAsync(int id, string userId);
        Task<IdentificacionDescripcionProblemaDto> CreateAsync(IdentificacionDescripcionProblemaDto dto, string userId);
        Task<bool> UpdateAsync(int id, IdentificacionDescripcionProblemaDto dto, string userId);
        Task<bool> DeleteAsync(int id, string userId);
        Task<IdentificacionDescripcionProblemaDto> GetUltimoAsync(string userId);
    }
}
