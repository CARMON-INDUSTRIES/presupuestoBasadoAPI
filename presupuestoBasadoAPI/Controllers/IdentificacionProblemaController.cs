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
    [Authorize] // 🔹 Requiere token válido
    public class IdentificacionProblemaController : ControllerBase
    {
        private readonly IIdentificacionProblemaService _service;

        public IdentificacionProblemaController(IIdentificacionProblemaService service)
        {
            _service = service;
        }

        private string GetUserId() => User.Identity?.Name ?? "";

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] IdentificacionProblemaDto dto)
        {
            var resultado = await _service.CrearAsync(dto, GetUserId());
            return Ok(resultado);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<IdentificacionProblema>>> GetAll()
        {
            var lista = await _service.ObtenerTodosAsync(GetUserId());
            return Ok(lista);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<IdentificacionProblema>> GetById(int id)
        {
            var resultado = await _service.ObtenerPorIdAsync(id, GetUserId());
            if (resultado == null)
                return NotFound();
            return Ok(resultado);
        }

        [HttpGet("ultimo")]
        public async Task<ActionResult<IdentificacionProblema>> ObtenerUltimo()
        {
            var ultimo = await _service.ObtenerUltimoAsync(GetUserId());
            if (ultimo == null)
                return NotFound("No se encontró ningún registro.");
            return Ok(ultimo);
        }
    }
}
