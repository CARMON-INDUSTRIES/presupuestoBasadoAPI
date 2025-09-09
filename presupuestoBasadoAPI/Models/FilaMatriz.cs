namespace presupuestoBasadoAPI.Models
{
    public class FilaMatriz
    {
        public int Id { get; set; }
        public string Nivel { get; set; } = string.Empty;
        public string ResumenNarrativo { get; set; } = string.Empty;
        public string Indicadores { get; set; } = string.Empty;
        public string Medios { get; set; } = string.Empty;
        public string Supuestos { get; set; } = string.Empty;
    }

}
