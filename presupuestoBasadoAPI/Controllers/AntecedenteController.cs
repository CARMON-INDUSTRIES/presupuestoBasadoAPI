using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using presupuestoBasadoAPI.Dto;
using presupuestoBasadoAPI.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace presupuestoBasadoAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // 🔒 proteger con JWT
    public class AntecedenteController : ControllerBase
    {
        private readonly IAntecedenteService _service;
        private readonly IUsuarioActualService _usuarioActualService;

        public AntecedenteController(IAntecedenteService service, IUsuarioActualService usuarioActualService)
        {
            _service = service;
            _usuarioActualService = usuarioActualService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AntecedenteDto>>> Get()
        {
            var userId = await _usuarioActualService.ObtenerUserIdAsync();
            var list = await _service.GetAllAsync(userId!);
            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AntecedenteDto>> Get(int id)
        {
            var userId = await _usuarioActualService.ObtenerUserIdAsync();
            var item = await _service.GetByIdAsync(id, userId!);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        public async Task<ActionResult<AntecedenteDto>> Post([FromBody] AntecedenteDto dto)
        {
            var userId = await _usuarioActualService.ObtenerUserIdAsync();
            var created = await _service.CreateAsync(dto, userId!);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] AntecedenteDto dto)
        {
            var userId = await _usuarioActualService.ObtenerUserIdAsync();
            var updated = await _service.UpdateAsync(id, dto, userId!);
            if (!updated) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = await _usuarioActualService.ObtenerUserIdAsync();
            var deleted = await _service.DeleteAsync(id, userId!);
            if (!deleted) return NotFound();
            return NoContent();
        }

        [HttpGet("ultimo")]
        public async Task<ActionResult<AntecedenteDto>> GetUltimo()
        {
            var userId = await _usuarioActualService.ObtenerUserIdAsync();
            var ultimo = await _service.GetUltimoAsync(userId!);
            if (ultimo == null) return NotFound();
            return Ok(ultimo);
        }
    }
}
