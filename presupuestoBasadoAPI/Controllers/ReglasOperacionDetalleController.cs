using Microsoft.AspNetCore.Mvc;
using presupuestoBasadoAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace presupuestoBasadoAPI.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class ReglasOperacionDetalleController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReglasOperacionDetalleController(AppDbContext context)
        {
            _context = context;
        }

        // Obtener el último registro del usuario
        [HttpGet("ultimo")]
        public async Task<ActionResult<ReglasOperacionDetalle>> GetUltimo()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var detalle = await _context.ReglasOperacionDetalles
                .Where(d => d.UserId == userId) // 🔹 filtramos por usuario
                .OrderByDescending(d => d.Id)
                .FirstOrDefaultAsync();

            if (detalle == null) return NotFound("No hay registros");

            return detalle;
        }

        // Crear nuevo
        [HttpPost]
        public async Task<ActionResult<ReglasOperacionDetalle>> Crear(ReglasOperacionDetalle detalle)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); //  asignamos usuario
            detalle.UserId = userId;

            _context.ReglasOperacionDetalles.Add(detalle);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetUltimo), new { id = detalle.Id }, detalle);
        }

        // Actualizar existente
        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(int id, ReglasOperacionDetalle detalle)
        {
            if (id != detalle.Id) return BadRequest();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            detalle.UserId = userId; // 🔹 opcional: mantener usuario actualizado

            _context.Entry(detalle).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
