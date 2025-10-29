namespace presupuestoBasadoAPI.Dto
{
    public class ReglasOperacionDto
    {
        public bool TieneReglasOperacion { get; set; }
        public IFormFile? Archivo { get; set; }      
        public string? ArchivoAdjunto { get; set; } 
        public string? LigaInternet { get; set; }
        public string UserId { get; set; } = string.Empty;

    }
}
