namespace presupuestoBasadoAPI.Dto
{
    public class RegisterDto
    {
        public string? User { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public string? Password { get; set; } = string.Empty;
        public string? NombreCompleto { get; set; } = string.Empty;
        public string? Cargo { get; set; } = string.Empty;
        public string? Coordinador { get; set; } = string.Empty;
        public string? UnidadesPresupuestales { get; set; } = string.Empty;
        public string? ProgramaPresupuestario { get; set; } = string.Empty;
        public string? NombreMatriz { get; set; } = string.Empty;
        public string? Rol { get; set; } = string.Empty;

        public int UnidadAdministrativaId { get; set; }
        public int EntidadId { get; set; }

    }
}
