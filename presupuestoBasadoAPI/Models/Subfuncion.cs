namespace presupuestoBasadoAPI.Models
{
    public class Subfuncion
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;

        public int FuncionId { get; set; }
        public Funcion Funcion { get; set; }
    }
}
