using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using presupuestoBasadoAPI.Models;
using Microsoft.EntityFrameworkCore;
using presupuestoBasadoAPI.Data;


namespace presupuestoBasadoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EntidadController : ControllerBase
    {
        private readonly AppDbContext _context;
        public EntidadController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Entidad>>> GetEntidades()
        {
            return await _context.Entidad.ToListAsync();
        }
    }

 }
