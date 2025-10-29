using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using presupuestoBasadoAPI.Models;

[ApiController]
[Route("api/[controller]")]
public class EstatalController : ControllerBase
{
    private readonly AppDbContext _context;
    public EstatalController(AppDbContext context) { _context = context; }

    // crear acuerdo
    [HttpPost("acuerdo")]
    public async Task<IActionResult> CrearAcuerdo([FromBody] AcuerdoEstatal acuerdo)
    {
        _context.AcuerdoEstatal.Add(acuerdo);
        await _context.SaveChangesAsync();
        return Ok(acuerdo);
    }

    // crear objetivo
    [HttpPost("objetivo")]
    public async Task<IActionResult> CrearObjetivo([FromBody] ObjetivoEstatal objetivo)
    {
        if (!await _context.AcuerdoEstatal.AnyAsync(a => a.Id == objetivo.AcuerdoEstatalId))
            return NotFound("Acuerdo no encontrado");
        _context.ObjetivoEstatal.Add(objetivo);
        await _context.SaveChangesAsync();
        return Ok(objetivo);
    }

    // crear estrategia
    [HttpPost("estrategia")]
    public async Task<IActionResult> CrearEstrategia([FromBody] EstrategiaEstatal estrategia)
    {
        if (!await _context.ObjetivoEstatal.AnyAsync(o => o.Id == estrategia.ObjetivoEstatalId))
            return NotFound("Objetivo no encontrado");
        _context.EstrategiaEstatal.Add(estrategia);
        await _context.SaveChangesAsync();
        return Ok(estrategia);
    }

    // crear linea de acción
    [HttpPost("linea")]
    public async Task<IActionResult> CrearLinea([FromBody] LineaDeAccionEstatal linea)
    {
        if (!await _context.EstrategiaEstatal.AnyAsync(e => e.Id == linea.EstrategiaEstatalId))
            return NotFound("Estrategia no encontrada");
        _context.LineaDeAccionEstatal.Add(linea);
        await _context.SaveChangesAsync();
        return Ok(linea);
    }

    // obtener acuerdos (para select)
    [HttpGet("acuerdos")]
    public async Task<IActionResult> GetAcuerdos() =>
        Ok(await _context.AcuerdoEstatal.Select(a => new { a.Id, a.Nombre }).ToListAsync());

    // obtener objetivos por acuerdo
    [HttpGet("objetivos")]
    public async Task<IActionResult> GetObjetivos([FromQuery] int acuerdoId) =>
        Ok(await _context.ObjetivoEstatal.Where(o => o.AcuerdoEstatalId == acuerdoId)
                                         .Select(o => new { o.Id, o.Nombre }).ToListAsync());

    // obtener estrategias por objetivo
    [HttpGet("estrategias")]
    public async Task<IActionResult> GetEstrategias([FromQuery] int objetivoId) =>
        Ok(await _context.EstrategiaEstatal.Where(e => e.ObjetivoEstatalId == objetivoId)
                                           .Select(e => new { e.Id, e.Nombre }).ToListAsync());

    // obtener lineas por estrategia
    [HttpGet("lineas")]
    public async Task<IActionResult> GetLineas([FromQuery] int estrategiaId) =>
        Ok(await _context.LineaDeAccionEstatal.Where(l => l.EstrategiaEstatalId == estrategiaId)
                                              .Select(l => new { l.Id, l.Nombre }).ToListAsync());

    // opcional: obtener todo el árbol
    [HttpGet("todo")]
    public async Task<IActionResult> ObtenerTodo()
    {
        var data = await _context.AcuerdoEstatal
            .Include(a => a.Objetivos)
                .ThenInclude(o => o.Estrategias)
                    .ThenInclude(e => e.LineasDeAccion)
            .ToListAsync();
        return Ok(data);
    }
}
