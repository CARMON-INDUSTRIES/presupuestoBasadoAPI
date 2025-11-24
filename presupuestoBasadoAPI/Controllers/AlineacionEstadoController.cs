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
    public class AlineacionEstadoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AlineacionEstadoController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AlineacionEstado>>> GetAll()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return await _context.AlineacionesEstado
                                 .Where(x => x.UserId == userId)
                                 .ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult> Crear(AlineacionEstado modelo)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            modelo.UserId = userId;

            _context.AlineacionesEstado.Add(modelo);
            await _context.SaveChangesAsync();
            return Ok(modelo);
        }

        [HttpGet("ultimo")]
        public async Task<ActionResult<AlineacionEstado>> GetUltimo()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var ultimo = await _context.AlineacionesEstado
                                       .Where(x => x.UserId == userId)
                                       .OrderByDescending(x => x.Id)
                                       .FirstOrDefaultAsync();

            if (ultimo == null) return NotFound();
            return Ok(ultimo);
        }
    }
}
