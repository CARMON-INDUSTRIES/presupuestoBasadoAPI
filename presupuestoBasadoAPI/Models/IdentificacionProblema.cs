namespace presupuestoBasadoAPI.Models
{
    public class IdentificacionProblema
    {
        public int Id { get; set; }
        public string DiagnosticoSituacionActual { get; set; }  // Texto narrativo
        public string ProblemaCentral { get; set; }             // Frase concreta
        public string EvidenciaProblema { get; set; }           // Datos, fuentes, etc.
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public string UserId { get; set; } = string.Empty;

    }

}
