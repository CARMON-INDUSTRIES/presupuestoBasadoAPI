using Microsoft.AspNetCore.Mvc;
using presupuestoBasadoAPI.Dto;
using presupuestoBasadoAPI.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace presupuestoBasadoAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class IdentificacionDescripcionProblemaController : ControllerBase
    {
        private readonly IIdentificacionDescripcionProblemaService _service;
        private readonly IUsuarioActualService _usuarioActualService;

        public IdentificacionDescripcionProblemaController(
            IIdentificacionDescripcionProblemaService service,
            IUsuarioActualService usuarioActualService)
        {
            _service = service;
            _usuarioActualService = usuarioActualService;
        }

        private async Task<string> GetUserIdAsync()
        {
            var userId = await _usuarioActualService.ObtenerUserIdAsync();
            return userId ?? string.Empty;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<IdentificacionDescripcionProblemaDto>>> Get()
        {
            var list = await _service.GetAllAsync(await GetUserIdAsync());
            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<IdentificacionDescripcionProblemaDto>> Get(int id)
        {
            var item = await _service.GetByIdAsync(id, await GetUserIdAsync());
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpGet("ultimo")]
        public async Task<ActionResult<IdentificacionDescripcionProblemaDto>> GetUltimo()
        {
            var ultimo = await _service.GetUltimoAsync(await GetUserIdAsync());
            if (ultimo == null) return NotFound();
            return Ok(ultimo);
        }

        [HttpPost]
        public async Task<ActionResult<IdentificacionDescripcionProblemaDto>> Post([FromBody] IdentificacionDescripcionProblemaDto dto)
        {
            var created = await _service.CreateAsync(dto, await GetUserIdAsync());
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] IdentificacionDescripcionProblemaDto dto)
        {
            var updated = await _service.UpdateAsync(id, dto, await GetUserIdAsync());
            if (!updated) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id, await GetUserIdAsync());
            if (!deleted) return NotFound();
            return NoContent();
        }

        [HttpGet("borrador")]
        public async Task<ActionResult<IdentificacionDescripcionProblemaDto>> GetBorrador()
        {
            var userId = await GetUserIdAsync();
            var ultimo = await _service.GetUltimoAsync(userId);

            if (ultimo == null)
            {
                // crear registro vacío
                var nuevo = new IdentificacionDescripcionProblemaDto
                {
                    UserId = userId,
                    ProblemaCentral = "",
                    Involucrados = "",
                    CausaBeneficiados = "",
                    CausaOpositores = "",
                    CausaEjecutores = "",
                    CausaIndiferentes = "",
                    Efectos = "",
                    Evolucion = ""
                };

                var creado = await _service.CreateAsync(nuevo, userId);
                return Ok(creado);
            }

            return Ok(ultimo);
        }

        [HttpPut("autosave")]
        public async Task<ActionResult<IdentificacionDescripcionProblemaDto>> AutoSave(
            [FromBody] IdentificacionDescripcionProblemaDto dto)
        {
            var userId = await GetUserIdAsync();

            var existente = await _service.GetUltimoAsync(userId);

            if (existente == null)
            {
                dto.UserId = userId;
                var creado = await _service.CreateAsync(dto, userId);
                return Ok(creado);
            }

            dto.Id = existente.Id;
            dto.UserId = userId;

            await _service.UpdateAsync(existente.Id, dto, userId);

            var actualizado = await _service.GetByIdAsync(existente.Id, userId);
            return Ok(actualizado);
        }

    }
}
