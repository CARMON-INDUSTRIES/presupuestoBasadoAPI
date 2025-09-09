namespace presupuestoBasadoAPI.Dto
{
    public class AlternativaEvaluacionDto
    {
        public string Nombre { get; set; } = string.Empty;
        public int Facultad { get; set; }
        public int Presupuesto { get; set; }
        public int CortoPlazo { get; set; }
        public int RecursosTecnicos { get; set; }
        public int RecursosAdministrativos { get; set; }
        public int CulturalSocial { get; set; }
        public int Impacto { get; set; }
    }
}
