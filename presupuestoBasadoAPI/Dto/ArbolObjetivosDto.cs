namespace presupuestoBasadoAPI.Dto
{
    public class ArbolObjetivosDto
    {
        public int Id { get; set; }
        public string Fin { get; set; }
        public string ObjetivoCentral { get; set; }
        public List<ComponenteObjetivoDto> Componentes { get; set; } = new();
        public string UserId { get; set; } = string.Empty;
    }

    public class ComponenteObjetivoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public List<string> Medios { get; set; } = new();
        public List<string> Resultados { get; set; } = new();
        public string UserId { get; set; } = string.Empty;

    }
}
