namespace presupuestoBasadoAPI.Models
{
    public class Funcion
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;

        public int FinalidadId { get; set; }
        public Finalidad Finalidad { get; set; }

        public virtual ICollection<Subfuncion> SubFunciones { get; set; } = new List<Subfuncion>();
    }
}
