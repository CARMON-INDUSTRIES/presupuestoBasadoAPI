using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace presupuestoBasadoAPI.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string NombreCompleto { get; set; } = string.Empty;
        public string Cargo { get; set; } = string.Empty;
        public string Coordinador { get; set; } = string.Empty;
        public string UnidadesPresupuestales { get; set; } = string.Empty;
        public string ProgramaPresupuestario { get; set; } = string.Empty;
        public string NombreMatriz { get; set; } = string.Empty;

        [ForeignKey("UnidadAdministrativa")]
        [Column("UnidadAdministrativaId")]
        public int? UnidadAdministrativaId { get; set; }

        // UnidadResponsable
        public UnidadAdministrativa UnidadAdministrativa { get; set; }


        [ForeignKey("Entidad")]
        [Column("EntidadId")]
        public int? EntidadId { get; set; }

        public Entidad Entidad { get; set; }
    }
}
