namespace presupuestoBasadoAPI.Models
{
    public class EstrategiaMunicipal
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;

        public int ObjetivoMunicipalId { get; set; }
        public ObjetivoMunicipal ObjetivoMunicipal { get; set; }

        public virtual ICollection<LineaDeAccionMunicipal> LineasDeAccion { get; set; } = new List<LineaDeAccionMunicipal>();
    }
}
