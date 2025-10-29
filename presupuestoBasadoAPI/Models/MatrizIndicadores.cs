using System.ComponentModel.DataAnnotations;

namespace presupuestoBasadoAPI.Models
{
    public class MatrizIndicadores
    {
        [Key]
        public int Id { get; set; }

        public string UnidadResponsable { get; set; } = string.Empty;
        public string UnidadPresupuestal { get; set; } = string.Empty;
        public string ProgramaSectorial { get; set; } = string.Empty;
        public string ProgramaPresupuestario { get; set; } = string.Empty;
        public string ResponsableMIR { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;


        public ICollection<FilaMatriz> Filas { get; set; } = new List<FilaMatriz>();
    }
}
