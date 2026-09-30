namespace presupuestoBasadoAPI.Dto
{
    public class IAConvertirTextoDto
    {
        public string Id { get; set; } = string.Empty;
        public string TextoBase { get; set; } = string.Empty;
        public string Nivel { get; set; } = string.Empty;
    }

    public record IAResultadoDto(string Id, string TextoPositivo);
}
