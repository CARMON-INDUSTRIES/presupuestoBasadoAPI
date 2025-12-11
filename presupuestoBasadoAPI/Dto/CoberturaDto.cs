namespace presupuestoBasadoAPI.Dto
{
    public class CoberturaDto
    {
        public int Id { get; set; }
        
        public string IdentificacionCaracterizacionPoblacionPotencial { get; set; } = string.Empty;
        public string IdentificacionCaracterizacionPoblacionObjetivo { get; set; } = string.Empty;

        public string UnidadMedida { get; set; } = string.Empty;
        
        public int CuantificacionPoblacionPotencial { get; set; }
        public int CuantificacionPoblacionObjetivo { get; set; } 
        public int CuantificacionPoblacionAtendidaAnterior { get; set; } 

        public string FrecuenciaActualizacion { get; set; } = string.Empty;

        public string ProcesoIdentificacionPoblacionPotencial { get; set; } = string.Empty; 
        public string ProcesoIdentificacionPoblacionObjetivo { get; set; } = string.Empty; 
    }
}
