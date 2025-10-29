namespace presupuestoBasadoAPI.Dto
{
    public class FichaIndicadorDTO
    {
        public string ClaveIndicador { get; set; } = string.Empty;
        public string TipoIndicador { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;

        public List<IndicadorDetalleDTO> Indicadores { get; set; } = new();
        public List<MetaProgramadaDto> MetasProgramadas { get; set; } = new();
        public List<LineaAccionDTO> LineasAccion { get; set; } = new();
    }
}
