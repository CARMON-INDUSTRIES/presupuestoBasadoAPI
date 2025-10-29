namespace presupuestoBasadoAPI.Models
{
    public class Resultado
    {
        public int Id { get; set; }
        public string Descripcion { get; set; } = string.Empty;

        public int ComponenteId { get; set; }
        public Componente Componente { get; set; } = null!;
        public string UserId { get; set; } = string.Empty;

    }
}
