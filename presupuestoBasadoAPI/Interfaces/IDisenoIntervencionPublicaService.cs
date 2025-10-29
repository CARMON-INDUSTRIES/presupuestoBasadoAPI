using presupuestoBasadoAPI.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace presupuestoBasadoAPI.Services
{
    public interface IDisenoIntervencionPublicaService
    {
        Task<IEnumerable<DisenoIntervencionPublicaDto>> GetAllAsync(string userId);
        Task<DisenoIntervencionPublicaDto?> GetByIdAsync(int id, string userId);
        Task<DisenoIntervencionPublicaDto> CreateAsync(DisenoIntervencionPublicaDto dto, string userId);
        Task<bool> UpdateAsync(int id, DisenoIntervencionPublicaDto dto, string userId);
        Task<bool> DeleteAsync(int id, string userId);
        Task<DisenoIntervencionPublicaDto?> GetUltimoAsync(string userId);
    }
}
