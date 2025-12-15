namespace presupuestoBasadoAPI.Interfaces
{
    public interface IIAService
    {
        Task<string> ConvertirAPositivoAsync(string textoBase, string nivel);
    }
}
