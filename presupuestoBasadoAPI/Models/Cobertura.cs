namespace presupuestoBasadoAPI.Models
{
    public class Cobertura
    {
        public int Id { get; set; }

        // 4.1 y 4.2
        public string IdentificacionCaracterizacionPoblacionPotencial { get; set; } = string.Empty;
        public string IdentificacionCaracterizacionPoblacionObjetivo { get; set; } = string.Empty;

        // ✅ Nueva unidad de medida
        public string UnidadMedida { get; set; } = string.Empty;

        // 4.3 Cuantificación de las poblaciones (numéricas)
        public int CuantificacionPoblacionPotencial { get; set; }
        public int CuantificacionPoblacionObjetivo { get; set; }
        public int CuantificacionPoblacionAtendidaAnterior { get; set; }

        // 4.4 ✅ Un solo campo de frecuencia (dropdown)
        public string FrecuenciaActualizacion { get; set; } = string.Empty;

        // 4.5 Procesos
        public string ProcesoIdentificacionPoblacionPotencial { get; set; } = string.Empty;
        public string ProcesoIdentificacionPoblacionObjetivo { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;

    }
}
