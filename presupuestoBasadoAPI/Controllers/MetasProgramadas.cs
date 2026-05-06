using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using presupuestoBasadoAPI.Models;
using System.Security.Claims;

namespace presupuestoBasadoAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProgramacionMetasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProgramacionMetasController(AppDbContext context)
        {
            _context = context;
        }

        private string GetUserId() =>
            User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

        [HttpGet("ultima")]
        public async Task<IActionResult> GetUltimaFicha()
        {
            var userId = GetUserId();

            var ficha = await _context.Fichas
                .Include(f => f.Indicadores)
                    .ThenInclude(i => i.MetasProgramadas)
                .Where(f => f.UserId == userId)
                .OrderByDescending(f => f.Id)
                .FirstOrDefaultAsync();

            if (ficha == null)
                return NotFound("No hay fichas registradas");

            return Ok(ficha);
        }

        [HttpPut("actualizar-avances")]
        public async Task<IActionResult> ActualizarAvances([FromBody] List<MetaProgramada> metas)
        {
            if (metas == null || !metas.Any())
                return BadRequest("No se recibieron metas");

            foreach (var meta in metas)
            {
                var metaDb = await _context.Set<MetaProgramada>()
                    .FirstOrDefaultAsync(m => m.Id == meta.Id);

                if (metaDb != null)
                {
                    metaDb.Alcanzado = meta.Alcanzado;
                }
            }

            await _context.SaveChangesAsync();

            return Ok("Avances actualizados");
        }
    }
}