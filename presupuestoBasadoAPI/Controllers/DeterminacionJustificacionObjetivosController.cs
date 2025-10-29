using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using presupuestoBasadoAPI.Dto;
using presupuestoBasadoAPI.Services;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace presupuestoBasadoAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // 🔒 protege con JWT
    public class DeterminacionJustificacionObjetivosController : ControllerBase
    {
        private readonly IDeterminacionJustificacionObjetivosService _service;

        public DeterminacionJustificacionObjetivosController(IDeterminacionJustificacionObjetivosService service)
        {
            _service = service;
        }

        // Método para extraer el userId del token
        private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DeterminacionJustificacionObjetivosDto>>> Get()
        {
            var list = await _service.GetAllAsync(GetUserId());
            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DeterminacionJustificacionObjetivosDto>> Get(int id)
        {
            var item = await _service.GetByIdAsync(id, GetUserId());
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpGet("ultimo")]
        public async Task<ActionResult<DeterminacionJustificacionObjetivosDto>> GetUltimo()
        {
            var ultimo = await _service.GetUltimoAsync(GetUserId());
            if (ultimo == null) return NotFound();
            return Ok(ultimo);
        }

        [HttpPost]
        public async Task<ActionResult<DeterminacionJustificacionObjetivosDto>> Post([FromBody] DeterminacionJustificacionObjetivosDto dto)
        {
            var created = await _service.CreateAsync(dto, GetUserId());
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] DeterminacionJustificacionObjetivosDto dto)
        {
            var updated = await _service.UpdateAsync(id, dto, GetUserId());
            if (!updated) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id, GetUserId());
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
