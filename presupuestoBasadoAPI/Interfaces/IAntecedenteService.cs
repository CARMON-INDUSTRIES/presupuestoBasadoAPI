using presupuestoBasadoAPI.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace presupuestoBasadoAPI.Services
{
    public interface IAntecedenteService
    {
        Task<IEnumerable<AntecedenteDto>> GetAllAsync(string userId);
        Task<AntecedenteDto?> GetByIdAsync(int id, string userId);
        Task<AntecedenteDto> CreateAsync(AntecedenteDto dto, string userId);
        Task<bool> UpdateAsync(int id, AntecedenteDto dto, string userId);
        Task<bool> DeleteAsync(int id, string userId);
        Task<AntecedenteDto?> GetUltimoAsync(string userId);
    }
}
