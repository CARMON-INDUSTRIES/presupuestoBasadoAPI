namespace presupuestoBasadoAPI.Models
{
    public class Meta
    {
        public int Id { get; set; }
        public string MetaProgramada { get; set; } = "";
        public decimal Cantidad { get; set; }
        public string PeriodoCumplimiento { get; set; } = "";

        public int IndicadorId { get; set; }
        public Indicador Indicador { get; set; }
        public string UserId { get; set; } = string.Empty;

    }
}
