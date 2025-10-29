using presupuestoBasadoAPI.Dto;

namespace presupuestoBasadoAPI.Interfaces
{
    public interface IArbolObjetivosService
    {
        Task<ArbolObjetivosDto?> GetUltimoAsync(string userId);
        Task<ArbolObjetivosDto> CrearAsync(ArbolObjetivosDto dto, string userId);
    }
}
