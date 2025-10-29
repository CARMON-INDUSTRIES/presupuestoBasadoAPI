namespace presupuestoBasadoAPI.Dto
{
    public class CoberturaDto
    {
        public int Id { get; set; }
        // 4.1 y 4.2
        public string IdentificacionCaracterizacionPoblacionPotencial { get; set; } = string.Empty;
        public string IdentificacionCaracterizacionPoblacionObjetivo { get; set; } = string.Empty;

        // 4.3 Nueva unidad de medida
        public string UnidadMedida { get; set; } = string.Empty;

        //  Cuantificación de las poblaciones (numéricas)
        public int CuantificacionPoblacionPotencial { get; set; } // 4.3.1
        public int CuantificacionPoblacionObjetivo { get; set; } // 4.3.2
        public int CuantificacionPoblacionAtendidaAnterior { get; set; } // 4.3.3

        // 4.4 "Frecuencia de Actualizacion" lo pondras como texto plano
        // 4.4.1 Un solo campo de frecuencia (dropdown)
        public string FrecuenciaActualizacion { get; set; } = string.Empty;

        // 4.5 Procesos texto plano
        public string ProcesoIdentificacionPoblacionPotencial { get; set; } = string.Empty; // 4.5.1
        public string ProcesoIdentificacionPoblacionObjetivo { get; set; } = string.Empty; // 4.5.2
    }
}
