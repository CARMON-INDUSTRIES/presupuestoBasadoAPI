using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using presupuestoBasadoAPI.Models;
using System.Security.Claims;

namespace presupuestoBasadoAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] 
    public class MatrizIndicadoresController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MatrizIndicadoresController(AppDbContext context)
        {
            _context = context;
        }

        private string GetUserId() =>
            User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MatrizIndicadores>>> GetAll()
        {
            var userId = GetUserId();
            var lista = await _context.MatricesIndicadores
                                      .Where(m => m.UserId == userId)
                                      .Include(m => m.Filas)
                                      .ToListAsync();
            return Ok(lista);
        }

        [HttpGet("ultimo")]
        public async Task<ActionResult<MatrizIndicadores>> GetUltimo()
        {
            var userId = GetUserId();
            var ultimo = await _context.MatricesIndicadores
                                       .Where(m => m.UserId == userId)
                                       .Include(m => m.Filas)
                                       .OrderByDescending(m => m.Id)
                                       .FirstOrDefaultAsync();

            if (ultimo == null) return NotFound();
            return Ok(ultimo);
        }

        [HttpPost]
        public async Task<ActionResult<MatrizIndicadores>> Post([FromBody] MatrizIndicadores matriz)
        {
            var userId = GetUserId();

            matriz.UserId = userId;

            foreach (var fila in matriz.Filas)
            {
                fila.UserId = userId;
            }

            _context.MatricesIndicadores.Add(matriz);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUltimo), new { id = matriz.Id }, matriz);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] MatrizIndicadores matriz)
        {
            if (id != matriz.Id) return BadRequest();

            var userId = GetUserId();
            matriz.UserId = userId;

            foreach (var fila in matriz.Filas)
            {
                fila.UserId = userId;
            }

            _context.Entry(matriz).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetUserId();
            var matriz = await _context.MatricesIndicadores
                                       .Where(m => m.Id == id && m.UserId == userId)
                                       .Include(m => m.Filas)
                                       .FirstOrDefaultAsync();

            if (matriz == null) return NotFound();

            _context.MatricesIndicadores.Remove(matriz);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        [HttpPost("borrador")]
        public async Task<ActionResult<MatrizIndicadores>> GuardarBorrador([FromBody] MatrizIndicadores dto)
        {
            if (dto == null)
                return BadRequest(new { message = "El DTO es requerido" });

            var userId = GetUserId();

            var existente = await _context.MatricesIndicadores
                .Where(m => m.UserId == userId)
                .Include(m => m.Filas)
                .OrderByDescending(m => m.Id)
                .FirstOrDefaultAsync();

            // Crear borrador si no hay ninguno
            if (existente == null)
            {
                dto.Id = 0;
                dto.UserId = userId;

                foreach (var fila in dto.Filas)
                {
                    fila.Id = 0;
                    fila.UserId = userId;
                }

                _context.MatricesIndicadores.Add(dto);
                await _context.SaveChangesAsync();
                return Ok(dto);
            }

            // Actualizar campos básicos
            existente.UnidadResponsable = dto.UnidadResponsable;
            existente.UnidadPresupuestal = dto.UnidadPresupuestal;
            existente.ProgramaSectorial = dto.ProgramaSectorial;
            existente.ProgramaPresupuestario = dto.ProgramaPresupuestario;
            existente.ResponsableMIR = dto.ResponsableMIR;

            // ❗ ELIMINAR FILAS ANTERIORES EN BD (la forma correcta)
            _context.FilaMatriz.RemoveRange(existente.Filas);
            await _context.SaveChangesAsync();

            // ❗ AGREGAR NUEVAS FILAS
            existente.Filas = new List<FilaMatriz>();

            foreach (var fila in dto.Filas)
            {
                existente.Filas.Add(new FilaMatriz
                {
                    Id = 0,
                    Nivel = fila.Nivel,
                    ResumenNarrativo = fila.ResumenNarrativo,
                    Indicadores = fila.Indicadores,
                    Medios = fila.Medios,
                    Supuestos = fila.Supuestos,
                    UserId = userId
                });
            }

            await _context.SaveChangesAsync();

            return Ok(existente);
        }




        [HttpPut("autosave")]
        public async Task<IActionResult> AutoSave([FromBody] MatrizIndicadores dto)
        {
            if (dto == null)
                return BadRequest(new { message = "El DTO es requerido" });

            var userId = GetUserId();

            var existente = await _context.MatricesIndicadores
                .Where(m => m.UserId == userId)
                .Include(m => m.Filas)
                .OrderByDescending(m => m.Id)
                .FirstOrDefaultAsync();

            // --- PRIMER BORRADOR ---
            if (existente == null)
            {
                dto.Id = 0;
                dto.UserId = userId;

                foreach (var fila in dto.Filas)
                {
                    fila.Id = 0;
                    fila.UserId = userId;
                }

                _context.MatricesIndicadores.Add(dto);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Autosave creado", id = dto.Id });
            }

            // --- ACTUALIZACIÓN ---
            existente.UnidadResponsable = dto.UnidadResponsable;
            existente.UnidadPresupuestal = dto.UnidadPresupuestal;
            existente.ProgramaSectorial = dto.ProgramaSectorial;
            existente.ProgramaPresupuestario = dto.ProgramaPresupuestario;
            existente.ResponsableMIR = dto.ResponsableMIR;

            // 🔥 limpiar filas pero SIN USAR RemoveRange
            existente.Filas.Clear();
            await _context.SaveChangesAsync();   // <-- EF ya soltó las referencias

            existente.Filas = new List<FilaMatriz>();

            foreach (var fila in dto.Filas)
            {
                existente.Filas.Add(new FilaMatriz
                {
                    Id = 0,
                    Nivel = fila.Nivel,
                    ResumenNarrativo = fila.ResumenNarrativo,
                    Indicadores = fila.Indicadores,
                    Medios = fila.Medios,
                    Supuestos = fila.Supuestos,
                    UserId = userId
                });
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Autosave actualizado", id = existente.Id });
        }



    }
}
