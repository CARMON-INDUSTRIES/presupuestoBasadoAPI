namespace presupuestoBasadoAPI.Models
{
    public class Ramo
    {
        public int Id { get; set; }
        public string RecursoPropio { get; set; }
        public string RecursoEstatal { get; set; }
        public string RecursoFederal { get; set; }
        public string UserId { get; set; } = string.Empty;


    }
}
