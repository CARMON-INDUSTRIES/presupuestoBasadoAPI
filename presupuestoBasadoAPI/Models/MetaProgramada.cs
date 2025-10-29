using presupuestoBasadoAPI.Models;
using System.ComponentModel.DataAnnotations;

public class MetaProgramada
{
    [Key]
    public int Id { get; set; }

    public int FichaIndicadorId { get; set; }
    public FichaIndicador FichaIndicador { get; set; } = null!;

    public string MetaProgramadaNombre { get; set; } = string.Empty;
    public double Cantidad { get; set; }
    public string PeriodoCumplimiento { get; set; } = string.Empty;
    public int Mes { get; set; }
    public double CantidadEsperada { get; set; }
    public double Alcanzado { get; set; }


}
