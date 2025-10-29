using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using presupuestoBasadoAPI.Models;

[ApiController]
[Route("api/[controller]")]
public class MunicipalController : ControllerBase
{
    private readonly AppDbContext _context;
    public MunicipalController(AppDbContext context) { _context = context; }

    // crear acuerdo
    [HttpPost("acuerdo")]
    public async Task<IActionResult> CrearAcuerdo([FromBody] AcuerdoMunicipal acuerdo)
    {
        _context.AcuerdoMunicipal.Add(acuerdo);
        await _context.SaveChangesAsync();
        return Ok(acuerdo);
    }

    // crear objetivo
    [HttpPost("objetivo")]
    public async Task<IActionResult> CrearObjetivo([FromBody] ObjetivoMunicipal objetivo)
    {
        if (!await _context.AcuerdoMunicipal.AnyAsync(a => a.Id == objetivo.AcuerdoMunicipalId))
            return NotFound("Acuerdo no encontrado");
        _context.ObjetivoMunicipal.Add(objetivo);
        await _context.SaveChangesAsync();
        return Ok(objetivo);
    }

    // crear estrategia
    [HttpPost("estrategia")]
    public async Task<IActionResult> CrearEstrategia([FromBody] EstrategiaMunicipal estrategia)
    {
        if (!await _context.ObjetivoMunicipal.AnyAsync(o => o.Id == estrategia.ObjetivoMunicipalId))
            return NotFound("Objetivo no encontrado");
        _context.EstrategiaMunicipal.Add(estrategia);
        await _context.SaveChangesAsync();
        return Ok(estrategia);
    }

    // crear linea de acción
    [HttpPost("linea")]
    public async Task<IActionResult> CrearLinea([FromBody] LineaDeAccionMunicipal linea)
    {
        if (!await _context.EstrategiaMunicipal.AnyAsync(e => e.Id == linea.EstrategiaMunicipalId))
            return NotFound("Estrategia no encontrada");
        _context.LineaDeAccionMunicipal.Add(linea);
        await _context.SaveChangesAsync();
        return Ok(linea);
    }

    // obtener acuerdos (para select)
    [HttpGet("acuerdos")]
    public async Task<IActionResult> GetAcuerdos() =>
        Ok(await _context.AcuerdoMunicipal.Select(a => new { a.Id, a.Nombre }).ToListAsync());

    // obtener objetivos por acuerdo
    [HttpGet("objetivos")]
    public async Task<IActionResult> GetObjetivos([FromQuery] int acuerdoId) =>
        Ok(await _context.ObjetivoMunicipal.Where(o => o.AcuerdoMunicipalId == acuerdoId)
                                           .Select(o => new { o.Id, o.Nombre }).ToListAsync());

    // obtener estrategias por objetivo
    [HttpGet("estrategias")]
    public async Task<IActionResult> GetEstrategias([FromQuery] int objetivoId) =>
        Ok(await _context.EstrategiaMunicipal.Where(e => e.ObjetivoMunicipalId == objetivoId)
                                             .Select(e => new { e.Id, e.Nombre }).ToListAsync());

    // obtener lineas por estrategia
    [HttpGet("lineas")]
    public async Task<IActionResult> GetLineas([FromQuery] int estrategiaId) =>
        Ok(await _context.LineaDeAccionMunicipal.Where(l => l.EstrategiaMunicipalId == estrategiaId)
                                                .Select(l => new { l.Id, l.Nombre }).ToListAsync());

    // opcional: obtener todo el árbol
    [HttpGet("todo")]
    public async Task<IActionResult> ObtenerTodo()
    {
        var data = await _context.AcuerdoMunicipal
            .Include(a => a.Objetivos)
                .ThenInclude(o => o.Estrategias)
                    .ThenInclude(e => e.LineasDeAccion)
            .ToListAsync();
        return Ok(data);
    }

    // DTOs para recibir la jerarquía
    public class AcuerdoDTO
    {
        public string Nombre { get; set; } = string.Empty;
        public List<ObjetivoDTO> Hijos { get; set; } = new();
    }

    public class ObjetivoDTO
    {
        public string Nombre { get; set; } = string.Empty;
        public List<EstrategiaDTO> Hijos { get; set; } = new();
    }

    public class EstrategiaDTO
    {
        public string Nombre { get; set; } = string.Empty;
        public List<LineaDTO> Hijos { get; set; } = new();
    }

    public class LineaDTO
    {
        public string Nombre { get; set; } = string.Empty;
    }

    public class PlanMunicipalDto
    {
        public string? Acuerdo { get; set; }
        public string? Objetivo { get; set; }
        public string? Estrategia { get; set; }
        public string? LineaDeAccion { get; set; }
    }


    [HttpPost("bulk-flat")]
    public async Task<IActionResult> GuardarBulkPlano([FromBody] List<PlanMunicipalDto> datos)
    {
        if (datos == null || !datos.Any())
            return BadRequest("No se recibieron datos.");

        AcuerdoMunicipal? acuerdoActual = null;
        ObjetivoMunicipal? objetivoActual = null;
        EstrategiaMunicipal? estrategiaActual = null;

        foreach (var item in datos)
        {
            if (!string.IsNullOrWhiteSpace(item.Acuerdo))
            {
                acuerdoActual = new AcuerdoMunicipal { Nombre = item.Acuerdo };
                _context.AcuerdoMunicipal.Add(acuerdoActual);
                await _context.SaveChangesAsync();
                objetivoActual = null;
                estrategiaActual = null;
            }

            if (!string.IsNullOrWhiteSpace(item.Objetivo))
            {
                if (acuerdoActual == null)
                    return BadRequest("Se encontró un Objetivo sin Acuerdo previo.");

                objetivoActual = new ObjetivoMunicipal
                {
                    Nombre = item.Objetivo,
                    AcuerdoMunicipalId = acuerdoActual.Id
                };
                _context.ObjetivoMunicipal.Add(objetivoActual);
                await _context.SaveChangesAsync();
                estrategiaActual = null;
            }

            if (!string.IsNullOrWhiteSpace(item.Estrategia))
            {
                if (objetivoActual == null)
                    return BadRequest("Se encontró una Estrategia sin Objetivo previo.");

                estrategiaActual = new EstrategiaMunicipal
                {
                    Nombre = item.Estrategia,
                    ObjetivoMunicipalId = objetivoActual.Id
                };
                _context.EstrategiaMunicipal.Add(estrategiaActual);
                await _context.SaveChangesAsync();
            }

            if (!string.IsNullOrWhiteSpace(item.LineaDeAccion))
            {
                if (estrategiaActual == null)
                    return BadRequest("Se encontró una Línea de Acción sin Estrategia previo.");

                var linea = new LineaDeAccionMunicipal
                {
                    Nombre = item.LineaDeAccion,
                    EstrategiaMunicipalId = estrategiaActual.Id
                };
                _context.LineaDeAccionMunicipal.Add(linea);
            }
        }

        await _context.SaveChangesAsync();
        return Ok(new { message = "Datos cargados correctamente" });
    }


}
