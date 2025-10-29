using presupuestoBasadoAPI.Dto;

namespace presupuestoBasadoAPI.Interfaces
{
    public interface IProgramaService
    {
        Task<IEnumerable<ProgramaDto>> GetAllAsync(string userId);  // 🔹 filtrado por usuario
        Task<ProgramaDto?> GetByIdAsync(int id, string userId);     // 🔹 filtrado por usuario
        Task<ProgramaDto> CreateAsync(CreateProgramaDto dto, string userId); // 🔹 asignar usuario
        Task<ProgramaDto?> UpdateAsync(int id, UpdateProgramaDto dto);
        Task<bool> DeleteAsync(int id);
    }

}
