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
    public class CoberturaController : ControllerBase
    {
        private readonly ICoberturaService _service;

        public CoberturaController(ICoberturaService service)
        {
            _service = service;
        }

        private string GetUserId() =>
            User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CoberturaDto>>> Get()
        {
            var userId = GetUserId();
            var list = await _service.GetAllAsync(userId);
            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CoberturaDto>> Get(int id)
        {
            var userId = GetUserId();
            var item = await _service.GetByIdAsync(id, userId);
            if (item == null)
                return NotFound(new { message = $"Cobertura con ID {id} no encontrada" });

            return Ok(item);
        }

        [HttpGet("ultimo")]
        public async Task<ActionResult<CoberturaDto>> GetUltimo()
        {
            var userId = GetUserId();
            var ultimo = await _service.GetUltimoAsync(userId);
            if (ultimo == null)
                return NotFound(new { message = "No hay registros en Cobertura" });

            return Ok(ultimo);
        }

        [HttpPost]
        public async Task<ActionResult<CoberturaDto>> Post([FromBody] CoberturaDto dto)
        {
            if (dto == null)
                return BadRequest(new { message = "El objeto dto es requerido" });

            var userId = GetUserId();
            var created = await _service.CreateAsync(dto, userId);

            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] CoberturaDto dto)
        {
            if (dto == null)
                return BadRequest(new { message = "El objeto dto es requerido" });

            var userId = GetUserId();
            var updated = await _service.UpdateAsync(id, dto, userId);
            if (!updated)
                return NotFound(new { message = $"Cobertura con ID {id} no encontrada" });

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetUserId();
            var deleted = await _service.DeleteAsync(id, userId);
            if (!deleted)
                return NotFound(new { message = $"Cobertura con ID {id} no encontrada" });

            return NoContent();
        }

        [HttpGet("borrador")]
        public async Task<ActionResult<CoberturaDto>> GetBorrador()
        {
            var userId = GetUserId();
            var borrador = await _service.GetUltimoAsync(userId);

            if (borrador == null)
            {
                return Ok(new CoberturaDto()); // siempre devuelve algo
            }

            return Ok(borrador);
        }

        [HttpPut("autosave")]
        public async Task<IActionResult> AutoSave([FromBody] CoberturaDto dto)
        {
            if (dto == null)
                return BadRequest(new { message = "El objeto dto es requerido" });

            var userId = GetUserId();
            var ok = await _service.AutoSaveAsync(dto, userId);

            if (!ok)
                return BadRequest(new { message = "No se pudo guardar el autosave" });

            return Ok(new { message = "Autosave realizado correctamente" });
        }
    }
}
