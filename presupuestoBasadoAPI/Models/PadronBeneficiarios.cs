namespace presupuestoBasadoAPI.Models
{
    public class PadronBeneficiarios
    {
        public int Id { get; set; }
        public bool TienePadron { get; set; } // Sí o No
        public string? ArchivoAdjunto { get; set; } // Ruta o URL al archivo (Cloudinary más adelante)
        public string? LigaInternet { get; set; } // URL de acceso
        public string UserId { get; set; } = string.Empty;

    }
}
