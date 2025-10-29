namespace presupuestoBasadoAPI.Models
{
    public class DisenoIntervencionPublica
    {
        public int Id { get; set; }
        public string EtapasIntervencion { get; set; } = string.Empty;
        public string EscenariosFuturosEsperar { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;


        public ICollection<Componente> Componentes { get; set; } = new List<Componente>();
    }
}
