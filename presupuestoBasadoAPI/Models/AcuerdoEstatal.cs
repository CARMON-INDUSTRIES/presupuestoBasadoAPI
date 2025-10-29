namespace presupuestoBasadoAPI.Models
{
    public class AcuerdoEstatal
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;

        public string UserId { get; set; } = string.Empty;

        public virtual ICollection<ObjetivoEstatal> Objetivos { get; set; } = new List<ObjetivoEstatal>();
    }
}
