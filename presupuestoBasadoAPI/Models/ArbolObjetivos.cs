namespace presupuestoBasadoAPI.Models
{
    public class ArbolObjetivos
    {
        public int Id { get; set; }

        // Fin superior
        public string Fin { get; set; }

        // Objetivo central
        public string ObjetivoCentral { get; set; }
        public string UserId { get; set; } = string.Empty;

        // Colección de componentes
        public ICollection<ComponenteObjetivo> Componentes { get; set; } = new List<ComponenteObjetivo>();
    }

    public class ComponenteObjetivo
    {
        public int Id { get; set; }
        public string Nombre { get; set; }

        // Cada componente tiene medios (acciones) y fines intermedios (resultados)
        public ICollection<string> Medios { get; set; } = new List<string>();
        public ICollection<string> Resultados { get; set; } = new List<string>();
        public string UserId { get; set; } = string.Empty;

    }
}
