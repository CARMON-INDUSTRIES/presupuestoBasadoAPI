using System.ComponentModel.DataAnnotations;

namespace presupuestoBasadoAPI.Models
{
    public class AlternativaEvaluacion
    {
        public int Id { get; set; }

        public int AnalisisAlternativasId { get; set; }
        public AnalisisAlternativas Analisis { get; set; } = default!;

        public string Nombre { get; set; } = string.Empty;

        // Criterios (1–3; N/A = 0)
        public int Facultad { get; set; }
        public int Presupuesto { get; set; }
        public int CortoPlazo { get; set; }
        public int RecursosTecnicos { get; set; }
        public int RecursosAdministrativos { get; set; }
        public int CulturalSocial { get; set; }
        public int Impacto { get; set; }

        // Conveniencia
        public int Total { get; set; }
        public string UserId { get; set; } = string.Empty;

    }
}
