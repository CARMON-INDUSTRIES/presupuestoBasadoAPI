using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using presupuestoBasadoAPI.Data;
using presupuestoBasadoAPI.Models;

namespace presupuestoBasadoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IndicadorController : ControllerBase
    {
        private readonly AppDbContext _context;

        public IndicadorController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<Indicador>> PostIndicador([FromBody] Indicador indicador)
        {
            if (indicador == null)
                return BadRequest("El indicador no puede ser nulo");

            try
            {
                _context.Indicadores.Add(indicador);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetIndicador), new { id = indicador.Id }, indicador);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al guardar el indicador: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Indicador>> GetIndicador(int id)
        {
            var indicador = await _context.Indicadores
                .Include(i => i.Metas)
                .Include(i => i.LineaBase)
                .Include(i => i.ProgramacionMetas)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (indicador == null)
                return NotFound();

            return indicador;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Indicador>>> GetIndicadores()
        {
            return await _context.Indicadores
                .Include(i => i.Metas)
                .Include(i => i.LineaBase)
                .Include(i => i.ProgramacionMetas)
                .ToListAsync();
        }
    }
}
