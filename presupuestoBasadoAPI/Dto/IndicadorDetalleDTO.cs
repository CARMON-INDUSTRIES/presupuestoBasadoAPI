namespace presupuestoBasadoAPI.Dto
{
    public class IndicadorDetalleDTO
    {
        public string Nivel { get; set; } = string.Empty;
        public string ResultadoEsperado { get; set; } = string.Empty;
        public string Dimension { get; set; } = string.Empty;
        public string Sentido { get; set; } = string.Empty;
        public string Definicion { get; set; } = string.Empty;
        public string UnidadMedida { get; set; } = string.Empty;
        public string RangoValor { get; set; } = string.Empty;
        public string FrecuenciaMedicion { get; set; } = string.Empty;
        public string Cobertura { get; set; } = string.Empty;
        public string Numerador { get; set; } = string.Empty;
        public string Denominador { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string FuenteResultado { get; set; } = string.Empty;
        public string FuenteNumerador { get; set; } = string.Empty;
        public string FuenteDenominador { get; set; } = string.Empty;
        public double? LineaBaseValor { get; set; }
        public string LineaBaseUnidad { get; set; } = string.Empty;
        public string LineaBaseAnio { get; set; } = string.Empty;
        public string LineaBasePeriodo { get; set; } = string.Empty;
        public Dictionary<string, string> Crema { get; set; } = new();
    }
}
