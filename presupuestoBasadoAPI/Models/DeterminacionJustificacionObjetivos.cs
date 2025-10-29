namespace presupuestoBasadoAPI.Models
{
    public class DeterminacionJustificacionObjetivos
    {
        public int Id { get; set; }
        public string ObjetivosEspecificos { get; set; } = string.Empty;
        public string RelacionOtrosProgramas { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;

    }
}
