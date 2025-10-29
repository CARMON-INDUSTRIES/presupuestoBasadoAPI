namespace presupuestoBasadoAPI.Models
{
    public class ReglasOperacion
    {
        public int Id { get; set; }
        public bool TieneReglasOperacion { get; set; }
        public string? ArchivoAdjunto { get; set; } = string.Empty; // URL o path temporal
        public string? LigaInternet { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;

    }
}
