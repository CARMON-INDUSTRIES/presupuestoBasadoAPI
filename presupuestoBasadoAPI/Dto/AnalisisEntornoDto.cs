namespace presupuestoBasadoAPI.Dto
{
    public class AnalisisEntornoDto
    {
        public string FactoresInternos { get; set; }
        public string FactoresExternos { get; set; }
        public string RiesgosOportunidades { get; set; }

        public string UserId {  get; set; } = string.Empty;
    }

}
