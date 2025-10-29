namespace presupuestoBasadoAPI.Models
{
    public class LineaDeAccionEstatal
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;

        public int EstrategiaEstatalId { get; set; }
        public EstrategiaEstatal EstrategiaEstatal { get; set; }
    }
}
