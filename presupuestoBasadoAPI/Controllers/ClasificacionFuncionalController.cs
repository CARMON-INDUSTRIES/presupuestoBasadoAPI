using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using presupuestoBasadoAPI.Data;
using presupuestoBasadoAPI.Models;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ClasificacionFuncionalController : ControllerBase
{
    private readonly AppDbContext _context;

    public ClasificacionFuncionalController(AppDbContext context)
    {
        _context = context;
    }

    private string GetUserId() =>
        User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ClasificacionFuncional>>> Get()
    {
        var userId = GetUserId();
        return await _context.ClasificacionesFuncionales
            .Where(c => c.UserId == userId)
            .ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<ClasificacionFuncional>> Post([FromBody] ClasificacionFuncional clasificacion)
    {
        var currentYear = DateTime.Now.Year;
        if (clasificacion.AnioOperando < currentYear - 1 || clasificacion.AnioOperando > currentYear + 1)
        {
            return BadRequest("El campo 'AnioOperando' debe ser el año actual, el anterior o el siguiente.");
        }

        clasificacion.UserId = GetUserId();

        _context.ClasificacionesFuncionales.Add(clasificacion);
        await _context.SaveChangesAsync();
        return Ok(clasificacion);
    }

    [HttpGet("ultimo")]
    public async Task<ActionResult<ClasificacionFuncional>> GetUltimo()
    {
        var userId = GetUserId();
        var ultimo = await _context.ClasificacionesFuncionales
            .Where(c => c.UserId == userId)
            .OrderByDescending(x => x.Id)
            .FirstOrDefaultAsync();

        if (ultimo == null) return NotFound();
        return Ok(ultimo);
    }
}
