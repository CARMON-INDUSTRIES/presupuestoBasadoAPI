using presupuestoBasadoAPI.Dto;


namespace presupuestoBasadoAPI.Interfaces
{
    public interface IComponenteService
    {
        Task<IEnumerable<ComponenteDto>> GetAllAsync();
        Task<IEnumerable<ComponenteDto>> GetByProgramaIdAsync(int programaId);
        Task<ComponenteDto?> GetByIdAsync(int id);
        Task<ComponenteDto> CreateAsync(CreateComponenteDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
