namespace presupuestoBasadoAPI.Models
{
    public class AnalisisAlternativas
    {
        public int Id { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        public ICollection<AlternativaEvaluacion> Alternativas { get; set; } = new List<AlternativaEvaluacion>();

        // Resumen
        public int TotalObtenido { get; set; }
        public int TotalMaximo { get; set; }
        public int Probabilidad { get; set; }   // 0–100
    }
}
