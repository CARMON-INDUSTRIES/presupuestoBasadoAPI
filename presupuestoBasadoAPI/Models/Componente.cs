using System;

namespace presupuestoBasadoAPI.Models
{
    public class Componente
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;

        public int DisenoIntervencionPublicaId { get; set; }
        public DisenoIntervencionPublica DisenoIntervencionPublica { get; set; }

        public virtual ICollection<Accion> Acciones { get; set; } = new List<Accion>();
        public virtual Resultado? Resultado { get; set; }
        public string UserId { get; set; } = string.Empty;

    }
}