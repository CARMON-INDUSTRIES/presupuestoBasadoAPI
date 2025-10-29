namespace presupuestoBasadoAPI.Models
{
    public class Entidad
    {
        public int Id { get; set; }
        public string Nombre { get; set; }

        public ICollection<ApplicationUser> Usuarios { get; set; }
    }
}
