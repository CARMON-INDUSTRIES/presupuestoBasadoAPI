using static System.Runtime.InteropServices.JavaScript.JSType;

namespace presupuestoBasadoAPI.Models
{
    public class Finalidad
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;

        public virtual ICollection<Funcion> Funciones { get; set; } = new List<Funcion>();
    }
}
