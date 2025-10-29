namespace presupuestoBasadoAPI.Models
{
    public class AnalisisEntorno
    {
        public int Id { get; set; }
        public string FactoresInternos { get; set; }            // Capacidades o limitaciones propias
        public string FactoresExternos { get; set; }            // Económicos, políticos, sociales, etc.
        public string RiesgosOportunidades { get; set; }        // Riesgos o fortalezas detectadas
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public string UserId { get; set; } = string.Empty;

    }

}
