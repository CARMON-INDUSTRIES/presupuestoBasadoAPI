using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using presupuestoBasadoAPI.Models;
using presupuestoBasadoAPI.Dto;
using System.Security.Claims;

namespace presupuestoBasadoAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EfectoSuperiorController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EfectoSuperiorController(AppDbContext context)
        {
            _context = context;
        }

        // Método auxiliar para obtener el ID del usuario autenticado
        private string GetUserId() =>
            User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

        // POST api/EfectoSuperior
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] EfectoSuperiorDto dto)
        {
            if (dto == null)
                return BadRequest(new { message = "El objeto dto es requerido" });

            var userId = GetUserId();

            var nuevo = new EfectoSuperior
            {
                Descripcion = dto.Descripcion,
                FechaRegistro = DateTime.UtcNow,
                UserId = userId
            };

            _context.EfectosSuperiores.Add(nuevo);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Efecto Superior guardado correctamente", nuevo.Id });
        }

        // GET api/EfectoSuperior
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EfectoSuperior>>> GetTodos()
        {
            var userId = GetUserId();

            var efectos = await _context.EfectosSuperiores
                .Where(e => e.UserId == userId)
                .OrderByDescending(e => e.FechaRegistro)
                .ToListAsync();

            return Ok(efectos);
        }

        // GET api/EfectoSuperior/ultimo
        [HttpGet("ultimo")]
        public async Task<ActionResult<EfectoSuperior>> Ultimo()
        {
            var userId = GetUserId();

            var ultimo = await _context.EfectosSuperiores
                .Where(e => e.UserId == userId)
                .OrderByDescending(e => e.FechaRegistro)
                .FirstOrDefaultAsync();

            if (ultimo == null)
                return NotFound(new { message = "No hay registros de efecto superior para este usuario." });

            return Ok(ultimo);
        }

        // PUT api/EfectoSuperior/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] EfectoSuperiorDto dto)
        {
            if (dto == null)
                return BadRequest(new { message = "El objeto dto es requerido" });

            var userId = GetUserId();

            var efecto = await _context.EfectosSuperiores
                .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);

            if (efecto == null)
                return NotFound(new { message = $"Efecto Superior con ID {id} no encontrado para este usuario" });

            efecto.Descripcion = dto.Descripcion;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Efecto Superior actualizado correctamente" });
        }

        // DELETE api/EfectoSuperior/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var userId = GetUserId();

            var efecto = await _context.EfectosSuperiores
                .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);

            if (efecto == null)
                return NotFound(new { message = $"Efecto Superior con ID {id} no encontrado para este usuario" });

            _context.EfectosSuperiores.Remove(efecto);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Efecto Superior eliminado correctamente" });
        }
    }
}
