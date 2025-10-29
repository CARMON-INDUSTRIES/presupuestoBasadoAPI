namespace presupuestoBasadoAPI.Models
{
    public class ObjetivoMunicipal
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;

        public int AcuerdoMunicipalId { get; set; }
        public AcuerdoMunicipal AcuerdoMunicipal { get; set; }

        public virtual ICollection<EstrategiaMunicipal> Estrategias { get; set; } = new List<EstrategiaMunicipal>();
    }
}
