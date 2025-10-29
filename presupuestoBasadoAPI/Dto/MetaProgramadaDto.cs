namespace presupuestoBasadoAPI.Dto
{
    public class MetaProgramadaDto
    {
        public string MetaProgramadaNombre { get; set; } = string.Empty;
        public double Cantidad { get; set; }
        public string PeriodoCumplimiento { get; set; } = string.Empty;
        public int Mes { get; set; }
        public double CantidadEsperada { get; set; }
        public double Alcanzado { get; set; }
    }
}
