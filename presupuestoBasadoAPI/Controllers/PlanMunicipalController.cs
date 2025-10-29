using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using presupuestoBasadoAPI.Models;

namespace presupuestoBasadoAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlanMunicipalController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PlanMunicipalController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/PlanMunicipal/acuerdos
        [HttpGet("acuerdos")]
        public async Task<ActionResult<IEnumerable<AcuerdoMunicipal>>> GetAcuerdos()
        {
            return await _context.AcuerdoMunicipal.ToListAsync();
        }

        // GET: api/PlanMunicipal/acuerdo/1/objetivos
        [HttpGet("acuerdo/{acuerdoId}/objetivos")]
        public async Task<ActionResult<IEnumerable<ObjetivoMunicipal>>> GetObjetivos(int acuerdoId)
        {
            var objetivos = await _context.ObjetivoMunicipal
                .Where(o => o.AcuerdoMunicipalId == acuerdoId)
                .ToListAsync();

            return objetivos;
        }

        // GET: api/PlanMunicipal/objetivo/1/estrategias
        [HttpGet("objetivo/{objetivoId}/estrategias")]
        public async Task<ActionResult<IEnumerable<EstrategiaMunicipal>>> GetEstrategias(int objetivoId)
        {
            var estrategias = await _context.EstrategiaMunicipal
                .Where(e => e.ObjetivoMunicipalId == objetivoId)
                .ToListAsync();

            return estrategias;
        }

        // GET: api/PlanMunicipal/estrategia/1/lineas
        [HttpGet("estrategia/{estrategiaId}/lineas")]
        public async Task<ActionResult<IEnumerable<LineaDeAccionMunicipal>>> GetLineas(int estrategiaId)
        {
            var lineas = await _context.LineaDeAccionMunicipal
                .Where(l => l.EstrategiaMunicipalId == estrategiaId)
                .ToListAsync();

            return lineas;
        }

        // GET: api/PlanMunicipal/primer-acuerdo
        [HttpGet("primer-acuerdo")]
        public async Task<ActionResult> GetPrimerAcuerdoCompleto()
        {
            var primerAcuerdo = await _context.AcuerdoMunicipal
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
