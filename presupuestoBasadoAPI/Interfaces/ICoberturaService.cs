using presupuestoBasadoAPI.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace presupuestoBasadoAPI.Services
{
    public interface ICoberturaService
    {
        Task<IEnumerable<CoberturaDto>> GetAllAsync(string userId);
        Task<CoberturaDto?> GetByIdAsync(int id, string userId);
        Task<CoberturaDto> CreateAsync(CoberturaDto dto, string userId);
        Task<bool> UpdateAsync(int id, CoberturaDto dto, string userId);
        Task<bool> DeleteAsync(int id, string userId);
        Task<CoberturaDto?> GetUltimoAsync(string userId);
        Task<bool> AutoSaveAsync(CoberturaDto dto, string userId);

    }
}
