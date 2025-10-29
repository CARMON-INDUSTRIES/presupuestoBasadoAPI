using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using presupuestoBasadoAPI.Dto;
using presupuestoBasadoAPI.Interfaces;
using presupuestoBasadoAPI.Models;

namespace presupuestoBasadoAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PoblacionObjetivoController : ControllerBase
    {
        private readonly IPoblacionObjetivoService _service;

        public PoblacionObjetivoController(IPoblacionObjetivoService service)
        {
            _service = service;
        }

        private string GetUserId() => User.Identity?.Name ?? "";

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] PoblacionObjetivoDto dto)
        {
            dto.UserId = GetUserId(); // 🔹 asociar al usuario
            var resultado = await _service.CrearAsync(dto);
            return Ok(resultado);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PoblacionObjetivo>>> GetAll()
        {
            var userId = GetUserId();
            var lista = await _service.ObtenerTodosAsync(userId); // 🔹 filtrar por usuario
            return Ok(lista);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PoblacionObjetivo>> GetById(int id)
        {
            var resultado = await _service.ObtenerPorIdAsync(id);
            if (resultado == null)
                return NotFound();
            return Ok(resultado);
        }

        [HttpGet("ultimo")]
        public async Task<ActionResult<PoblacionObjetivo>> ObtenerUltimo()
        {
            var userId = GetUserId();
            var ultimo = await _service.ObtenerUltimoAsync(userId); // 🔹 filtrar por usuario

            if (ultimo == null)
                return NotFound("No se encontró ningún registro para el usuario.");

            return Ok(ultimo);
        }
    }
}
