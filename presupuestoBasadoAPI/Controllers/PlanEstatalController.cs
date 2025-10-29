using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using presupuestoBasadoAPI.Models;

namespace presupuestoBasadoAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlanEstatalController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PlanEstatalController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("acuerdos")]
        public async Task<ActionResult<IEnumerable<AcuerdoEstatal>>> GetAcuerdos()
        {
            return await _context.AcuerdoEstatal.ToListAsync();
        }

        [HttpGet("acuerdo/{acuerdoId}/objetivos")]
        public async Task<ActionResult<IEnumerable<ObjetivoEstatal>>> GetObjetivos(int acuerdoId)
        {
            return await _context.ObjetivoEstatal
                .Where(o => o.AcuerdoEstatalId == acuerdoId)
                .ToListAsync();
        }

        [HttpGet("objetivo/{objetivoId}/estrategias")]
        public async Task<ActionResult<IEnumerable<EstrategiaEstatal>>> GetEstrategias(int objetivoId)
        {
            return await _context.EstrategiaEstatal
                .Where(e => e.ObjetivoEstatalId == objetivoId)
                .ToListAsync();
        }

        [HttpGet("estrategia/{estrategiaId}/lineas")]
        public async Task<ActionResult<IEnumerable<LineaDeAccionEstatal>>> GetLineas(int estrategiaId)
        {
            return await _context.LineaDeAccionEstatal
                .Where(l => l.EstrategiaEstatalId == estrategiaId)
                .ToListAsync();
        }

        [HttpGet("primer-acuerdo")]
        public async Task<ActionResult> GetPrimerAcuerdoCompleto()
        {
            var primerAcuerdo = await _context.AcuerdoEstatal
                .Include(a => a.Objetivos)
                    .ThenInclude(o => o.Estrategias)
                        .ThenInclude(e => e.LineasDeAccion)
                .OrderBy(a => a.Id)
                .FirstOrDefaultAsync();

            if (primerAcuerdo == null)
                return NotFound();

            return Ok(primerAcuerdo);
        }
    }
}
