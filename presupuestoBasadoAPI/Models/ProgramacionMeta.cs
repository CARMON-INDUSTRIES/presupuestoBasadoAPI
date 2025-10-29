namespace presupuestoBasadoAPI.Models
{
    public class ProgramacionMeta
    {
        public int Id { get; set; }
        public string Mes { get; set; } = "";
        public decimal Cantidad { get; set; }
        public decimal Alcanzado { get; set; }
        public DateTime? Fecha { get; set; }
        public string Semaforo { get; set; } = "";
        public string UserId { get; set; } = string.Empty;


        public int? IndicadorId { get; set; }
        public Indicador? Indicador { get; set; }
    }
}
