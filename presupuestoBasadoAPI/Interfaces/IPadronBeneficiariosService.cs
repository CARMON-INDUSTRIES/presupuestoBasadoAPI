using presupuestoBasadoAPI.Dto;

namespace presupuestoBasadoAPI.Interfaces
{
    public interface IPadronBeneficiariosService
    {
        Task<PadronBeneficiariosDto> CrearAsync(PadronBeneficiariosDto dto);
        Task<PadronBeneficiariosDto?> ObtenerUltimoAsync(string userId); // 🔹 recibe userId
    }
}
