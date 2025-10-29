namespace presupuestoBasadoAPI.Models
{
    public class PoblacionObjetivo
    {
        public int Id { get; set; }
        public string GrupoPoblacional { get; set; }            // ¿Quiénes son?
        public string AmbitoGeografico { get; set; }            // Lugar(es) de impacto
        public string CriteriosSeleccion { get; set; }          // Cómo se identifican
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public string UserId { get; set; } = string.Empty;

    }

}
