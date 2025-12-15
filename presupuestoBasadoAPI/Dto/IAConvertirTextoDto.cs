namespace presupuestoBasadoAPI.Dto
{
    public class IAConvertirTextoDto
    {
        /// <summary>
        /// Texto base del diagnóstico, estrategia o problema
        /// </summary>
        public string TextoBase { get; set; } = string.Empty;

        /// <summary>
        /// Nivel del Árbol de Objetivos:
        /// FIN | OBJETIVO_CENTRAL | COMPONENTE | RESULTADO | MEDIO
        /// </summary>
        public string Nivel { get; set; } = string.Empty;
    }
}
