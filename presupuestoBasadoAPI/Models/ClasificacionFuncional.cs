namespace presupuestoBasadoAPI.Models
{
    public class ClasificacionFuncional
    {
        public int Id { get; set; }
        public string Finalidad { get; set; }
        public string Funcion { get; set; }
        public string Subfuncion { get; set; }
        public int AnioOperando { get; set; }
        public string EntregaBienes { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;


    }
}
