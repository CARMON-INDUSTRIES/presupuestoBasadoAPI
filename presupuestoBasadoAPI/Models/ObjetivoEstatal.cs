namespace presupuestoBasadoAPI.Models
{
    public class ObjetivoEstatal
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;

        public int AcuerdoEstatalId { get; set; }
        public AcuerdoEstatal AcuerdoEstatal { get; set; }

        public virtual ICollection<EstrategiaEstatal> Estrategias { get; set; } = new List<EstrategiaEstatal>();
    }
}
