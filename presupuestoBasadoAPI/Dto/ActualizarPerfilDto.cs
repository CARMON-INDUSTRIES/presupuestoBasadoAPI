using System.ComponentModel.DataAnnotations;

namespace presupuestoBasadoAPI.Dto
{
    public class ActualizarPerfilDto
    {
        [Required]
        public string? User { get; set; } = string.Empty;

        public string? Email { get; set; }
        public string? NombreCompleto { get; set; }
        public string? Cargo { get; set; }
        public string? Coordinador { get; set; }
        public string? UnidadesPresupuestales { get; set; }
        public string? ProgramaPresupuestario { get; set; }
        public string? NombreMatriz { get; set; }
        public int? UnidadAdministrativaId { get; set; }
        public int? EntidadId { get; set; }
    }
}
