namespace presupuestoBasadoAPI.Models
{
    public class LineaBase
    {
        public int Id { get; set; }
        public decimal Valor { get; set; }
        public string Unidad { get; set; } = "";
        public int Anio { get; set; }
        public string Periodo { get; set; } = "";

        public int? IndicadorId { get; set; }
        public Indicador? Indicador { get; set; }
        public string UserId { get; set; } = string.Empty;

    }
}
