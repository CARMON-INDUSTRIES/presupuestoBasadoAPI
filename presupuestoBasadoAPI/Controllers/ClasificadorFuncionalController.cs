using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using presupuestoBasadoAPI.Models;

namespace presupuestoBasadoAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClasificadorFuncionalController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ClasificadorFuncionalController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("finalidades")]
        public async Task<ActionResult<IEnumerable<Finalidad>>> GetFinalidades()
        {
            return await _context.Finalidad.ToListAsync();
        }

        [HttpGet("finalidad/{finalidadId}/funciones")]
        public async Task<ActionResult<IEnumerable<Funcion>>> GetFunciones(int finalidadId)
        {
            return await _context.Funcion
                .Where(f => f.FinalidadId == finalidadId)
                .ToListAsync();
        }

        [HttpGet("funcion/{funcionId}/subfunciones")]
        public async Task<ActionResult<IEnumerable<Subfuncion>>> GetSubFunciones(int funcionId)
        {
            return await _context.SubFuncion
                .Where(s => s.FuncionId == funcionId)
                .ToListAsync();
        }

        [HttpGet("primer-finalidad")]
        public async Task<ActionResult> GetPrimerFinalidadCompleta()
        {
            var primerFinalidad = await _context.Finalidad
                .Include(f => f.Funciones)
                    .ThenInclude(fn => fn.SubFunciones)
                .OrderBy(f => f.Id)
                .FirstOrDefaultAsync();

            if (primerFinalidad == null)
                return NotFound();

            return Ok(primerFinalidad);
        }

        // Devuelve todas las subfunciones
        [HttpGet("subfunciones")]
        public async Task<ActionResult<IEnumerable<Subfuncion>>> GetAllSubfunciones()
        {
            return await _context.SubFuncion
                .Include(s => s.Funcion)
                    .ThenInclude(f => f.Finalidad)
                .ToListAsync();
        }

        // Devuelve la función y finalidad asociadas a una subfunción
        [HttpGet("subfuncion/{subfuncionId}/jerarquia")]
        public async Task<ActionResult> GetJerarquiaPorSubfuncion(int subfuncionId)
        {
            var sub = await _context.SubFuncion
                .Include(s => s.Funcion)
                    .ThenInclude(f => f.Finalidad)
                .FirstOrDefaultAsync(s => s.Id == subfuncionId);

            if (sub == null)
                return NotFound();

            return Ok(new
            {
                subfuncionId = sub.Id,
                subfuncionNombre = sub.Nombre,
                funcionId = sub.Funcion.Id,
                funcionNombre = sub.Funcion.Nombre,
                finalidadId = sub.Funcion.Finalidad.Id,
                finalidadNombre = sub.Funcion.Finalidad.Nombre
            });
        }
    }
}

