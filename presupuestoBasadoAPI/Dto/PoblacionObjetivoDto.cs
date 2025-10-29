namespace presupuestoBasadoAPI.Dto
{
    public class PoblacionObjetivoDto
    {
        public string GrupoPoblacional { get; set; }
        public string AmbitoGeografico { get; set; }
        public string CriteriosSeleccion { get; set; }
        public string UserId { get; set; } = string.Empty;// 🔹 para asociar al usuario
    }

}
