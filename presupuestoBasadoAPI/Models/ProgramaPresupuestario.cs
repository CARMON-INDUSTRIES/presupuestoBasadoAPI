namespace presupuestoBasadoAPI.Models
{
    public class ProgramaPresupuestario
    {
        public int Id { get; set; }
        public string Clave { get; set; }
        public string Nombre { get; set; }
        public string ObjetivoGeneral { get; set; }
        public string UnidadResponsable { get; set; }
        public string UnidadesIntegrantes { get; set; }
        public string UserId { get; set; } = string.Empty;

    }
}
