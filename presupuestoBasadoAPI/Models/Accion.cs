namespace presupuestoBasadoAPI.Models
{
    public class Accion
    {
        public int Id { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public int Cantidad { get; set; } 

        public int ComponenteId { get; set; }
        public Componente Componente { get; set; }

        public string UserId { get; set; } = string.Empty;

    }
}
