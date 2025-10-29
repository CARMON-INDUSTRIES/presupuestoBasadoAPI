namespace presupuestoBasadoAPI.Models
{
    public class AcuerdoMunicipal
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;

        public string UserId { get; set; } = string.Empty;

        public virtual ICollection<ObjetivoMunicipal> Objetivos { get; set; } = new List<ObjetivoMunicipal>();
    }
}
