namespace presupuestoBasadoAPI.Interfaces
{
    public interface IIAService
    {
        Task<string> ConvertirAPositivoAsync(string textoBase, string nivel, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<presupuestoBasadoAPI.Dto.IAResultadoDto>> ConvertirArbolAsync(
            IReadOnlyList<presupuestoBasadoAPI.Dto.IAConvertirTextoDto> nodos,
            CancellationToken cancellationToken = default);
    }
}
