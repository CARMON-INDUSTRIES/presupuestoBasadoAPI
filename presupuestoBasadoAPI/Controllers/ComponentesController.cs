using Microsoft.AspNetCore.Mvc;
using presupuestoBasadoAPI.Dto;
using presupuestoBasadoAPI.Interfaces;

namespace presupuestoBasadoAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ComponentesController : ControllerBase
    {
        private readonly IComponenteService _service;

        public ComponentesController(IComponenteService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var componentes = await _service.GetAllAsync();
            return Ok(componentes);
        }

        [HttpGet("por-programa/{programaId}")]
        public async Task<IActionResult> GetByProgramaId(int programaId)
        {
            var componentes = await _service.GetByProgramaIdAsync(programaId);
            return Ok(componentes);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var componente = await _service.GetByIdAsync(id);
            if (componente == null) return NotFound();
            return Ok(componente);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateComponenteDto dto)
        {
            var nuevo = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = nuevo.Id }, nuevo);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _service.DeleteAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}
