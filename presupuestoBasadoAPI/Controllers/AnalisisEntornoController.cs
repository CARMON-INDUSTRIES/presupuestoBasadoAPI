using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using presupuestoBasadoAPI.Dto;
using presupuestoBasadoAPI.Interfaces;
using presupuestoBasadoAPI.Models;
using System.Security.Claims;

namespace presupuestoBasadoAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // 🔐 JWT obligatorio
    public class AnalisisEntornoController : ControllerBase
    {
        private readonly IAnalisisEntornoService _service;
        private readonly AppDbContext _context;

        public AnalisisEntornoController(IAnalisisEntornoService service, AppDbContext context)
        {
            _service = service;
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] AnalisisEntornoDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            dto.UserId = userId; // 🔗 Se asocia al usuario actual

            var resultado = await _service.CrearAsync(dto, userId);
            return Ok(resultado);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AnalisisEntornoDto>>> GetAll()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var lista = await _service.ObtenerTodosAsync(userId); // 🔍 Filtra por usuario
            return Ok(lista);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AnalisisEntornoDto>> GetById(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var resultado = await _service.ObtenerPorIdAsync(id, userId);

            if (resultado == null)
                return NotFound();

            return Ok(resultado);
        }

        [HttpGet("ultimo")]
        public async Task<ActionResult<AnalisisEntornoDto>> ObtenerUltimo()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var ultimo = await _context.AnalisisEntorno
                .Where(p => p.UserId == userId) // 🔍 Filtrado por usuario
                .OrderByDescending(p => p.Id)
                .FirstOrDefaultAsync();

            if (ultimo == null)
                return NotFound("No se encontró ningún registro.");

            return Ok(ultimo);
        }
    }
}
