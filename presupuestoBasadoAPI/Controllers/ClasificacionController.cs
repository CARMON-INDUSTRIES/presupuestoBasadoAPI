using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using presupuestoBasadoAPI.Models;

[ApiController]
[Route("api/[controller]")]
public class ClasificacionController : ControllerBase
{
    private readonly AppDbContext _context;
    public ClasificacionController(AppDbContext ctx) { _context = ctx; }

    // Finalidad
    [HttpPost("finalidad")]
    public async Task<IActionResult> CrearFinalidad([FromBody] Finalidad f)
    {
        _context.Finalidad.Add(f);
        await _context.SaveChangesAsync();
        return Ok(f);
    }

    [HttpGet("finalidades")]
    public async Task<IActionResult> GetFinalidades() =>
        Ok(await _context.Finalidad.Select(x => new { x.Id, x.Nombre }).ToListAsync());

    // Función
    [HttpPost("funcion")]
    public async Task<IActionResult> CrearFuncion([FromBody] Funcion fn)
    {
        if (!await _context.Finalidad.AnyAsync(x => x.Id == fn.FinalidadId))
            return NotFound("Finalidad no encontrada");

        _context.Funcion.Add(fn);
        await _context.SaveChangesAsync();
        return Ok(fn);
    }

    [HttpGet("funciones")]
    public async Task<IActionResult> GetFunciones([FromQuery] int finalidadId) =>
        Ok(await _context.Funcion.Where(f => f.FinalidadId == finalidadId)
                                 .Select(f => new { f.Id, f.Nombre }).ToListAsync());

    // SubFunción
    [HttpPost("subfuncion")]
    public async Task<IActionResult> CrearSubFuncion([FromBody] Subfuncion s)
    {
        if (!await _context.Funcion.AnyAsync(x => x.Id == s.FuncionId))
            return NotFound("Función no encontrada");

        _context.SubFuncion.Add(s);
        await _context.SaveChangesAsync();
        return Ok(s);
    }

    [HttpGet("subfunciones")]
    public async Task<IActionResult> GetSubFunciones([FromQuery] int funcionId) =>
        Ok(await _context.SubFuncion.Where(s => s.FuncionId == funcionId)
                                    .Select(s => new { s.Id, s.Nombre }).ToListAsync());
}
