using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using presupuestoBasadoAPI.Dto;
using presupuestoBasadoAPI.Models;
using System.Security.Claims;

namespace presupuestoBasadoAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FichaIndicadorController : ControllerBase
    {
        private readonly AppDbContext _context;

        public FichaIndicadorController(AppDbContext context)
        {
            _context = context;
        }

        private string GetUserId() =>
            User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<FichaIndicador>>> GetAll()
        {
            var userId = GetUserId();

            var lista = await _context.Fichas
                .Include(f => f.Indicadores)
                .Include(f => f.MetasProgramadas)
                .Include(f => f.LineasAccion)
                .Where(f => f.UserId == userId)
                .ToListAsync();

            return Ok(lista);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<FichaIndicador>> GetById(int id)
        {
            var userId = GetUserId();

            var ficha = await _context.Fichas
                .Include(f => f.Indicadores)
                .Include(f => f.MetasProgramadas)
                .Include(f => f.LineasAccion)
                .FirstOrDefaultAsync(f => f.Id == id && f.UserId == userId);

            if (ficha == null)
                return NotFound();

            return Ok(ficha);
        }

        [HttpPost]
        public async Task<ActionResult<FichaIndicador>> Post([FromBody] FichaIndicadorDTO model)
        {
            var userId = GetUserId();

            var ficha = new FichaIndicador
            {
                UserId = userId,
                ClaveIndicador = model.ClaveIndicador,
                TipoIndicador = model.TipoIndicador,
                ProgramaPresupuestario = "",
                UnidadPresupuestal = "",
                UnidadResponsable = "",
                ResponsableMIR = "",
                Indicadores = model.Indicadores.Select(i => new IndicadorDetalle
                {
                    Nivel = i.Nivel,
                    ResultadoEsperado = i.ResultadoEsperado,
                    Dimension = i.Dimension,
                    Sentido = i.Sentido,
                    Definicion = i.Definicion,
                    UnidadMedida = i.UnidadMedida,
                    RangoValor = i.RangoValor,
                    FrecuenciaMedicion = i.FrecuenciaMedicion,
                    Cobertura = i.Cobertura,
                    Numerador = i.Numerador,
                    Denominador = i.Denominador,
                    Descripcion = i.Descripcion,
                    FuenteResultado = i.FuenteResultado,
                    FuenteNumerador = i.FuenteNumerador,
                    FuenteDenominador = i.FuenteDenominador,
                    LineaBaseValor = i.LineaBaseValor,
                    LineaBaseUnidad = i.LineaBaseUnidad,
                    LineaBaseAnio = i.LineaBaseAnio,
                    LineaBasePeriodo = i.LineaBasePeriodo,
                    Crema = i.Crema
                }).ToList(),
                MetasProgramadas = model.MetasProgramadas.Select(m => new MetaProgramada
                {
                    MetaProgramadaNombre = m.MetaProgramadaNombre,
                    Cantidad = m.Cantidad,
                    PeriodoCumplimiento = m.PeriodoCumplimiento,
                    Mes = m.Mes,
                    CantidadEsperada = m.CantidadEsperada,
                    Alcanzado = m.Alcanzado
                }).ToList(),
                LineasAccion = model.LineasAccion.Select(l => new LineaAccion
                {
                    Acuerdo = l.Acuerdo,
                    Objetivo = l.Objetivo,
                    Estrategia = l.Estrategia,
                    LineaAccionTexto = l.LineaAccionTexto,
                    Ramo = l.Ramo
                }).ToList()
            };

            _context.Fichas.Add(ficha);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = ficha.Id }, ficha);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] FichaIndicador fichaActualizada)
        {
            var userId = GetUserId();

            var fichaExistente = await _context.Fichas
                .Include(f => f.Indicadores)
                .Include(f => f.MetasProgramadas)
                .Include(f => f.LineasAccion)
                .FirstOrDefaultAsync(f => f.Id == id && f.UserId == userId);

            if (fichaExistente == null)
                return NotFound();

            fichaExistente.ClaveIndicador = fichaActualizada.ClaveIndicador;
            fichaExistente.TipoIndicador = fichaActualizada.TipoIndicador;
            fichaExistente.UnidadResponsable = fichaActualizada.UnidadResponsable;
            fichaExistente.UnidadPresupuestal = fichaActualizada.UnidadPresupuestal;
            fichaExistente.ProgramaPresupuestario = fichaActualizada.ProgramaPresupuestario;
            fichaExistente.ResponsableMIR = fichaActualizada.ResponsableMIR;

            _context.Set<IndicadorDetalle>().RemoveRange(fichaExistente.Indicadores ?? []);
            _context.Set<MetaProgramada>().RemoveRange(fichaExistente.MetasProgramadas ?? []);
            _context.Set<LineaAccion>().RemoveRange(fichaExistente.LineasAccion ?? []);

            fichaExistente.Indicadores = fichaActualizada.Indicadores ?? [];
            fichaExistente.MetasProgramadas = fichaActualizada.MetasProgramadas ?? [];
            fichaExistente.LineasAccion = fichaActualizada.LineasAccion ?? [];

            foreach (var ind in fichaExistente.Indicadores)
                ind.FichaIndicador = fichaExistente;

            foreach (var meta in fichaExistente.MetasProgramadas)
                meta.FichaIndicador = fichaExistente;

            foreach (var linea in fichaExistente.LineasAccion)
                linea.FichaIndicador = fichaExistente;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetUserId();

            var ficha = await _context.Fichas
                .Include(f => f.Indicadores)
                .Include(f => f.MetasProgramadas)
                .Include(f => f.LineasAccion)
                .FirstOrDefaultAsync(f => f.Id == id && f.UserId == userId);

            if (ficha == null)
                return NotFound();

            _context.Set<IndicadorDetalle>().RemoveRange(ficha.Indicadores ?? []);
            _context.Set<MetaProgramada>().RemoveRange(ficha.MetasProgramadas ?? []);
            _context.Set<LineaAccion>().RemoveRange(ficha.LineasAccion ?? []);
            _context.Fichas.Remove(ficha);

            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
