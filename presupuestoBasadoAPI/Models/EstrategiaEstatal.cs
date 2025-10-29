namespace presupuestoBasadoAPI.Models
{
    public class EstrategiaEstatal
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;

        public int ObjetivoEstatalId { get; set; }
        public ObjetivoEstatal ObjetivoEstatal { get; set; }


        public virtual ICollection<LineaDeAccionEstatal> LineasDeAccion { get; set; } = new List<LineaDeAccionEstatal>();
    }
}
