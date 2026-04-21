using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using presupuestoBasadoAPI.Data;
using presupuestoBasadoAPI.Dto;
using presupuestoBasadoAPI.Models;
using System.Security.Claims;

namespace presupuestoBasadoAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] 
    public class AnalisisAlternativasController : ControllerBase
    {
        private readonly AppDbContext _ctx;
        public AnalisisAlternativasController(AppDbContext ctx) => _ctx = ctx;

        [HttpGet("ultimo")]
        public async Task<IActionResult> GetUltimo()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var item = await _ctx.AnalisisAlternativas
                .Include(a => a.Alternativas)
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.Id)
                .FirstOrDefaultAsync();

            if (item is null) return NotFound();
            return Ok(item);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var item = await _ctx.AnalisisAlternativas
                .Include(a => a.Alternativas)
                .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

            if (item is null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] AnalisisAlternativasDto dto)
        {
            if (dto?.Alternativas is null || dto.Alternativas.Count == 0)
                return BadRequest("Se requieren alternativas.");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var analisis = new AnalisisAlternativas
            {
                UserId = userId 
            };

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

        [HttpPost("borrador")]
        public async Task<IActionResult> GuardarBorrador([FromBody] AnalisisAlternativasDto dto)
        {
            if (dto?.Alternativas == null)
                return BadRequest(new { message = "Las alternativas son requeridas" });

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var existente = await _ctx.AnalisisAlternativas
                .Include(a => a.Alternativas)
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.Id)
                .FirstOrDefaultAsync();

            if (existente == null)
            {
                var nuevo = new AnalisisAlternativas
                {
                    UserId = userId
                };

                foreach (var alt in dto.Alternativas)
                {
                    var total = alt.Facultad + alt.Presupuesto + alt.CortoPlazo +
                                alt.RecursosTecnicos + alt.RecursosAdministrativos +
                                alt.CulturalSocial + alt.Impacto;

                    nuevo.Alternativas.Add(new AlternativaEvaluacion
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

                    nuevo.TotalObtenido += total;
                }

                const int criterios = 7;
                nuevo.TotalMaximo = nuevo.Alternativas.Count * criterios * 3;
                nuevo.Probabilidad = nuevo.TotalMaximo > 0
                    ? (int)Math.Round(nuevo.TotalObtenido * 100.0 / nuevo.TotalMaximo)
                    : 0;

                _ctx.AnalisisAlternativas.Add(nuevo);
                await _ctx.SaveChangesAsync();

                return Ok(new { message = "Borrador creado", id = nuevo.Id });
            }

            // eliminar alternativas actuales
            _ctx.RemoveRange(existente.Alternativas);
            await _ctx.SaveChangesAsync();

            existente.Alternativas = new List<AlternativaEvaluacion>();
            existente.TotalObtenido = 0;

            foreach (var alt in dto.Alternativas)
            {
                var total = alt.Facultad + alt.Presupuesto + alt.CortoPlazo +
                            alt.RecursosTecnicos + alt.RecursosAdministrativos +
                            alt.CulturalSocial + alt.Impacto;

                existente.Alternativas.Add(new AlternativaEvaluacion
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

                existente.TotalObtenido += total;
            }

            const int criteriosUpdate = 7;
            existente.TotalMaximo = existente.Alternativas.Count * criteriosUpdate * 3;
            existente.Probabilidad = existente.TotalMaximo > 0
                ? (int)Math.Round(existente.TotalObtenido * 100.0 / existente.TotalMaximo)
                : 0;

            await _ctx.SaveChangesAsync();

            return Ok(new { message = "Borrador actualizado", id = existente.Id });
        }
    }
}
