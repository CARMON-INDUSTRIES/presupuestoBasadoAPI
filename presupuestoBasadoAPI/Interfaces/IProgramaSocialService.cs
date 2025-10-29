using presupuestoBasadoAPI.Dto;

namespace presupuestoBasadoAPI.Interfaces
{
    public interface IProgramaSocialService
    {
        Task<ProgramaSocialDto> CrearAsync(ProgramaSocialDto dto, string userId);  // 🔹 userId agregado
        Task<ProgramaSocialDto?> ObtenerUltimoAsync(string userId);                  // 🔹 filtrar por userId
    }
}
