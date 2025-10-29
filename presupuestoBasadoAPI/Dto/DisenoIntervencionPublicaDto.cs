

namespace presupuestoBasadoAPI.Dto
{
    public class DisenoIntervencionPublicaDto
    {
        public int Id { get; set; }
        public List<ComponenteDto> Componentes { get; set; } = new();
        public string EtapasIntervencion { get; set; } = string.Empty;
        public string EscenariosFuturosEsperar { get; set; } = string.Empty;
    }
}
