namespace presupuestoBasadoAPI.Models
{
    public class IdentificacionDescripcionProblema
    {
        public int Id { get; set; }
        public string ProblemaCentral { get; set; } = string.Empty;
        public string Involucrados { get; set; } = string.Empty;
        public string CausaBeneficiados { get; set; } = string.Empty;
        public string CausaOpositores { get; set; } = string.Empty;
        public string CausaEjecutores { get; set; } = string.Empty;
        public string CausaIndiferentes { get; set; } = string.Empty;
        public string Efectos { get; set; } = string.Empty;
        public string Evolucion { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;

    }
}
