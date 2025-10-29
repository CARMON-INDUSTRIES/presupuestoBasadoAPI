using Microsoft.AspNetCore.Mvc;
using presupuestoBasadoAPI.Dto;
using presupuestoBasadoAPI.Interfaces;
using System.Security.Claims;

namespace presupuestoBasadoAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProgramaSocialController : ControllerBase
    {
        private readonly IProgramaSocialService _service;

        public ProgramaSocialController(IProgramaSocialService service)
        {
            _service = service;
        }

        // POST
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] ProgramaSocialDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // 🔹 obtenemos el usuario actual
            var creado = await _service.CrearAsync(dto, userId);
            return Ok(creado);
        }

        // GET ULTIMO
        [HttpGet("ultimo")]
        public async Task<IActionResult> ObtenerUltimo()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // 🔹 filtramos por usuario
            var data = await _service.ObtenerUltimoAsync(userId);
            if (data == null) return NotFound();
            return Ok(data);
        }
    }
}
