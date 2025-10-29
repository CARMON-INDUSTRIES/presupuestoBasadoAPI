namespace presupuestoBasadoAPI.Models
{
    public class JustificacionPrograma
    {
        public int Id { get; set; }
        public string RelevanciaSocial { get; set; }           // ¿Por qué es importante?
        public string AlineacionPlaneacion { get; set; }       // Relación con planes/programas
        public string ContribucionSolucion { get; set; }       // ¿Cómo ayuda el programa?
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public string UserId { get; set; } = string.Empty;

    }

}
