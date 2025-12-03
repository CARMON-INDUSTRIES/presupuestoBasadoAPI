using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using presupuestoBasadoAPI.Models;
using System.Security.Claims;

namespace presupuestoBasadoAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Asegura que todas las rutas requieran autenticación
    public class PlanMunicipalController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PlanMunicipalController(AppDbContext context)
        {
            _context = context;
        }

        // Método helper para obtener el EntidadId del usuario actual
        private async Task<int?> GetUsuarioEntidadId()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return null;

            var usuario = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            return usuario?.EntidadId;
        }

        // GET: api/PlanMunicipal/acuerdos
        [HttpGet("acuerdos")]
        public async Task<ActionResult<IEnumerable<AcuerdoMunicipal>>> GetAcuerdos()
        {
            var entidadId = await GetUsuarioEntidadId();

            if (entidadId == null)
                return Unauthorized(new { message = "Usuario no tiene entidad asignada" });

            var acuerdos = await _context.AcuerdoMunicipal
                .Where(a => a.EntidadId == entidadId.Value)
                .ToListAsync();

            return Ok(acuerdos);
        }

        // GET: api/PlanMunicipal/acuerdo/1/objetivos
        [HttpGet("acuerdo/{acuerdoId}/objetivos")]
        public async Task<ActionResult<IEnumerable<ObjetivoMunicipal>>> GetObjetivos(int acuerdoId)
        {
            var entidadId = await GetUsuarioEntidadId();

            if (entidadId == null)
                return Unauthorized(new { message = "Usuario no tiene entidad asignada" });

            // Verificar que el acuerdo pertenece a la entidad del usuario
            var acuerdoPertenece = await _context.AcuerdoMunicipal
                .AnyAsync(a => a.Id == acuerdoId && a.EntidadId == entidadId.Value);

            if (!acuerdoPertenece)
                return Forbid(); // 403 Forbidden

            var objetivos = await _context.ObjetivoMunicipal
                .Where(o => o.AcuerdoMunicipalId == acuerdoId)
                .ToListAsync();

            return Ok(objetivos);
        }

        // GET: api/PlanMunicipal/objetivo/1/estrategias
        [HttpGet("objetivo/{objetivoId}/estrategias")]
        public async Task<ActionResult<IEnumerable<EstrategiaMunicipal>>> GetEstrategias(int objetivoId)
        {
            var entidadId = await GetUsuarioEntidadId();

            if (entidadId == null)
                return Unauthorized(new { message = "Usuario no tiene entidad asignada" });

            // Verificar que el objetivo pertenece a un acuerdo de la entidad del usuario
            var objetivoPertenece = await _context.ObjetivoMunicipal
                .Include(o => o.AcuerdoMunicipal)
                .AnyAsync(o => o.Id == objetivoId && o.AcuerdoMunicipal.EntidadId == entidadId.Value);

            if (!objetivoPertenece)
                return Forbid();

            var estrategias = await _context.EstrategiaMunicipal
                .Where(e => e.ObjetivoMunicipalId == objetivoId)
                .ToListAsync();

            return Ok(estrategias);
        }

        // GET: api/PlanMunicipal/estrategia/1/lineas
        [HttpGet("estrategia/{estrategiaId}/lineas")]
        public async Task<ActionResult<IEnumerable<LineaDeAccionMunicipal>>> GetLineas(int estrategiaId)
        {
            var entidadId = await GetUsuarioEntidadId();

            if (entidadId == null)
                return Unauthorized(new { message = "Usuario no tiene entidad asignada" });

            // Verificar que la estrategia pertenece a la entidad del usuario
            var estrategiaPertenece = await _context.EstrategiaMunicipal
                .Include(e => e.ObjetivoMunicipal)
                    .ThenInclude(o => o.AcuerdoMunicipal)
                .AnyAsync(e => e.Id == estrategiaId &&
                              e.ObjetivoMunicipal.AcuerdoMunicipal.EntidadId == entidadId.Value);

            if (!estrategiaPertenece)
                return Forbid();

            var lineas = await _context.LineaDeAccionMunicipal
                .Where(l => l.EstrategiaMunicipalId == estrategiaId)
                .ToListAsync();

            return Ok(lineas);
        }

        // GET: api/PlanMunicipal/primer-acuerdo
        [HttpGet("primer-acuerdo")]
        public async Task<ActionResult> GetPrimerAcuerdoCompleto()
        {
            var entidadId = await GetUsuarioEntidadId();

            if (entidadId == null)
                return Unauthorized(new { message = "Usuario no tiene entidad asignada" });

            var primerAcuerdo = await _context.AcuerdoMunicipal
                .Where(a => a.EntidadId == entidadId.Value)
                .Include(a => a.Objetivos)
                    .ThenInclude(o => o.Estrategias)
                        .ThenInclude(e => e.LineasDeAccion)
                .OrderBy(a => a.Id)
                .FirstOrDefaultAsync();

            if (primerAcuerdo == null)
                return NotFound(new { message = "No se encontró ningún acuerdo para esta entidad" });

            return Ok(primerAcuerdo);
        }
    }
}