namespace presupuestoBasadoAPI.Models
{
    public class ReglasOperacionDetalle
    {
        public int Id { get; set; }

        public string? SujetoReglasOperacion { get; set; }

        public string OtrosSubsidios { get; set; } = string.Empty;

        public string PrestacionServiciosPublicos { get; set; } = string.Empty;

        public string ProvisionBienesPublicos { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;

    }
}
