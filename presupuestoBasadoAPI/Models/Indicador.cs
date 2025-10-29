namespace presupuestoBasadoAPI.Models
{
    public class Indicador
    {
        public int Id { get; set; }
        public string Nivel { get; set; } = "";
        public string ResultadoEsperado { get; set; } = "";
        public string Dimension { get; set; } = "";
        public string Sentido { get; set; } = "";
        public string Definicion { get; set; } = "";
        public string UnidadMedida { get; set; } = "";
        public string RangoValor { get; set; } = "";
        public string FrecuenciaMedicion { get; set; } = "";
        public string Cobertura { get; set; } = "Municipal";

        public string Numerador { get; set; } = "";
        public string Denominador { get; set; } = "";
        public string Descripcion { get; set; } = "";
        public string Fuentes { get; set; } = "";

        // Relaciones
        public List<Meta> Metas { get; set; } = new();
        public LineaBase LineaBase { get; set; } = new();
        public List<ProgramacionMeta> ProgramacionMetas { get; set; } = new();

        public int FichaIndicadorId { get; set; }
        public FichaIndicador FichaIndicador { get; set; }
        public string UserId { get; set; } = string.Empty;

    }
}
