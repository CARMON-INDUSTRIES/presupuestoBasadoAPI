using presupuestoBasadoAPI.Models;
using System.ComponentModel.DataAnnotations;

public class LineaAccion
{
    [Key]
    public int Id { get; set; }

    public int FichaIndicadorId { get; set; }
    public FichaIndicador FichaIndicador { get; set; } = null!;

    public string Acuerdo { get; set; } = string.Empty;
    public string Objetivo { get; set; } = string.Empty;
    public string Estrategia { get; set; } = string.Empty;
    public string LineaAccionTexto { get; set; } = string.Empty;
    public string Ramo { get; set; } = string.Empty;
}
