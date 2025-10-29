

namespace presupuestoBasadoAPI.Dto
{
    public class ComponenteDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;

        public List<AccionDto> Acciones { get; set; } = new();  
        public ResultadoDto? Resultado { get; set; } = new();
    }
}
