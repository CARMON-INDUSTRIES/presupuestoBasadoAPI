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
    [Authorize] 
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

        [HttpGet("borrador")]
        public async Task<ActionResult<DeterminacionJustificacionObjetivosDto>> GetBorrador()
        {
            var userId = GetUserId();
            var ultimo = await _service.GetUltimoAsync(userId);

            if (ultimo == null)
            {
                var nuevo = new DeterminacionJustificacionObjetivosDto
                {
                    ObjetivosEspecificos = "",
                    RelacionOtrosProgramas = ""
                };

                var creado = await _service.CreateAsync(nuevo, userId);
                return Ok(creado);
            }

            return Ok(ultimo);
        }

        // PUT /autosave  -----------------------------------------
        [HttpPut("autosave")]
        public async Task<ActionResult<DeterminacionJustificacionObjetivosDto>> AutoSave(
            [FromBody] DeterminacionJustificacionObjetivosDto dto)
        {
            var userId = GetUserId();
            var existente = await _service.GetUltimoAsync(userId);

            if (existente == null)
            {
                var creado = await _service.CreateAsync(dto, userId);
                return Ok(creado);
            }

            dto.Id = existente.Id;

            await _service.UpdateAsync(existente.Id, dto, userId);

            var actualizado = await _service.GetByIdAsync(existente.Id, userId);

            return Ok(actualizado);
        }
    }
}
