namespace presupuestoBasadoAPI.Dto
{
    public class PadronBeneficiariosDto
    {
        public bool TienePadron { get; set; }
        public IFormFile? Archivo { get; set; } // Archivo enviado desde frontend
        public string? ArchivoAdjunto { get; set; } // URL de Cloudinary
        public string? LigaInternet { get; set; }
        public string UserId { get; set; } = string.Empty; // 🔹 para asociar al usuario
    }
}
