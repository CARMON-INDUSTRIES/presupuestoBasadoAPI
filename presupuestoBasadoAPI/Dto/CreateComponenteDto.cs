namespace presupuestoBasadoAPI.Dto
{
    public class CreateComponenteDto
    {
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int ProgramaPresupuestarioId { get; set; }
    }
}
