namespace presupuestoBasadoAPI.Models
{
    public class LineaDeAccionMunicipal
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;

        public int EstrategiaMunicipalId { get; set; }
        public EstrategiaMunicipal EstrategiaMunicipal { get; set; }
    }
}
