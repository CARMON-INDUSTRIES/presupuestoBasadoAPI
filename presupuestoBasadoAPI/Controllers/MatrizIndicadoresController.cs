using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using presupuestoBasadoAPI.Models;


namespace presupuestoBasadoAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MatrizIndicadoresController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MatrizIndicadoresController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /MatrizIndicadores
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MatrizIndicadores>>> GetAll()
        {
            return await _context.MatricesIndicadores
                .Include(m => m.Filas)
                .ToListAsync();
        }

        // GET: /MatrizIndicadores/ultimo
        [HttpGet("ultimo")]
        public async Task<ActionResult<MatrizIndicadores>> GetUltimo()
        {
            var ultimo = await _context.MatricesIndicadores
                .Include(m => m.Filas)
                .OrderByDescending(m => m.Id)
                .FirstOrDefaultAsync();

            if (ultimo == null) return NotFound();
            return ultimo;
        }

        // POST: /MatrizIndicadores
        [HttpPost]
        public async Task<ActionResult<MatrizIndicadores>> Post([FromBody] MatrizIndicadores matriz)
        {
            _context.MatricesIndicadores.Add(matriz);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUltimo), new { id = matriz.Id }, matriz);
        }

        // PUT: /MatrizIndicadores/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] MatrizIndicadores matriz)
        {
            if (id != matriz.Id) return BadRequest();

            _context.Entry(matriz).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: /MatrizIndicadores/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var matriz = await _context.MatricesIndicadores.FindAsync(id);
            if (matriz == null) return NotFound();

            _context.MatricesIndicadores.Remove(matriz);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
