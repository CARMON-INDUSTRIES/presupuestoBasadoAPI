using System.ComponentModel.DataAnnotations;

namespace presupuestoBasadoAPI.Models
{
    public class FichaIndicador
    {
        [Key]
        public int Id { get; set; }

        public string UserId { get; set; } = string.Empty;

        // Datos del encabezado
        public string ClaveIndicador { get; set; } = string.Empty;
        public string TipoIndicador { get; set; } = string.Empty;
        public string UnidadResponsable { get; set; } = string.Empty;
        public string UnidadPresupuestal { get; set; } = string.Empty;
        public string ProgramaPresupuestario { get; set; } = string.Empty;
        public string ResponsableMIR { get; set; } = string.Empty;

        public ICollection<IndicadorDetalle> Indicadores { get; set; } = new List<IndicadorDetalle>();
        public ICollection<MetaProgramada> MetasProgramadas { get; set; } = new List<MetaProgramada>();
        public ICollection<LineaAccion> LineasAccion { get; set; } = new List<LineaAccion>();
    }
}
