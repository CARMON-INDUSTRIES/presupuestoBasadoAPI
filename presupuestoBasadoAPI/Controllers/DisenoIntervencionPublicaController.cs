using Microsoft.AspNetCore.Mvc;
using presupuestoBasadoAPI.Dto;
using presupuestoBasadoAPI.Services;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace presupuestoBasadoAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Requiere token válido
    public class DisenoIntervencionPublicaController : ControllerBase
    {
        private readonly IDisenoIntervencionPublicaService _service;

    public DisenoIntervencionPublicaController(IDisenoIntervencionPublicaService service)
        {
            _service = service;
        }

        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DisenoIntervencionPublicaDto>>> Get()
        {
            var list = await _service.GetAllAsync(GetUserId());
            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DisenoIntervencionPublicaDto>> Get(int id)
        {
            var item = await _service.GetByIdAsync(id, GetUserId());
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpGet("ultimo")]
        public async Task<ActionResult<DisenoIntervencionPublicaDto>> GetUltimo()
        {
            var ultimo = await _service.GetUltimoAsync(GetUserId());
            if (ultimo == null) return NotFound();
            return Ok(ultimo);
        }

        [HttpPost]
        public async Task<ActionResult<DisenoIntervencionPublicaDto>> Post([FromBody] DisenoIntervencionPublicaDto dto)
        {
            var created = await _service.CreateAsync(dto, GetUserId());
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] DisenoIntervencionPublicaDto dto)
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
