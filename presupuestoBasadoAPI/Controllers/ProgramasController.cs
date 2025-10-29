using Microsoft.AspNetCore.Mvc;
using presupuestoBasadoAPI.Dto;
using presupuestoBasadoAPI.Interfaces;

[ApiController]
[Route("api/[controller]")]
public class ProgramasController : ControllerBase
{
    private readonly IProgramaService _service;
    private readonly IUsuarioActualService _usuarioActualService;

    public ProgramasController(IProgramaService service, IUsuarioActualService usuarioActualService)
    {
        _service = service;
        _usuarioActualService = usuarioActualService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var userId = await _usuarioActualService.ObtenerUserIdAsync();
        var programas = await _service.GetAllAsync(userId);
        return Ok(programas);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var userId = await _usuarioActualService.ObtenerUserIdAsync();
        var programa = await _service.GetByIdAsync(id, userId);
        if (programa == null) return NotFound();
        return Ok(programa);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProgramaDto dto)
    {
        var userId = await _usuarioActualService.ObtenerUserIdAsync();
        var nuevo = await _service.CreateAsync(dto, userId);
        return CreatedAtAction(nameof(GetById), new { id = nuevo.Id }, nuevo);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProgramaDto dto)
    {
        var updated = await _service.UpdateAsync(id, dto);
        if (updated == null) return NotFound();
        return Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _service.DeleteAsync(id);
        if (!success) return NotFound();
        return NoContent();
    }
}
