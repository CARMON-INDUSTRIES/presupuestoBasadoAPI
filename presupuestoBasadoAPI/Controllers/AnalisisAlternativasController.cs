// Controllers/AnalisisAlternativasController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using presupuestoBasadoAPI.Data;
using presupuestoBasadoAPI.Dto;
using presupuestoBasadoAPI.Models;

namespace presupuestoBasadoAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnalisisAlternativasController : ControllerBase
    {
        private readonly AppDbContext _ctx;
        public AnalisisAlternativasController(AppDbContext ctx) => _ctx = ctx;

        [HttpGet("ultimo")]
        public async Task<IActionResult> GetUltimo()
        {
            var item = await _ctx.AnalisisAlternativas
                .Include(a => a.Alternativas)
                .OrderByDescending(a => a.Id)
                .FirstOrDefaultAsync();

            if (item is null) return NotFound();
            return Ok(item);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _ctx.AnalisisAlternativas
                .Include(a => a.Alternativas)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (item is null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] AnalisisAlternativasDto dto)
        {
            if (dto?.Alternativas is null || dto.Alternativas.Count == 0)
                return BadRequest("Se requieren alternativas.");

            var analisis = new AnalisisAlternativas();

            foreach (var alt in dto.Alternativas)
            {
                var total = alt.Facultad + alt.Presupuesto + alt.CortoPlazo +
                            alt.RecursosTecnicos + alt.RecursosAdministrativos +
                            alt.CulturalSocial + alt.Impacto;

                analisis.Alternativas.Add(new AlternativaEvaluacion
                {
                    Nombre = alt.Nombre,
                    Facultad = alt.Facultad,
                    Presupuesto = alt.Presupuesto,
                    CortoPlazo = alt.CortoPlazo,
                    RecursosTecnicos = alt.RecursosTecnicos,
                    RecursosAdministrativos = alt.RecursosAdministrativos,
                    CulturalSocial = alt.CulturalSocial,
                    Impacto = alt.Impacto,
                    Total = total
                });

                analisis.TotalObtenido += total;
            }

            const int criterios = 7;
            analisis.TotalMaximo = analisis.Alternativas.Count * criterios * 3;
            analisis.Probabilidad = analisis.TotalMaximo > 0
                ? (int)Math.Round(analisis.TotalObtenido * 100.0 / analisis.TotalMaximo)
                : 0;

            _ctx.AnalisisAlternativas.Add(analisis);
            await _ctx.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = analisis.Id }, analisis);
        }
    }
}
