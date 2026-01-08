using Microsoft.AspNetCore.Mvc;
using presupuestoBasadoAPI.Models;
using presupuestoBasadoAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace presupuestoBasadoAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UnidadAdministrativaController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UnidadAdministrativaController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UnidadAdministrativa>>> GetUnidades()
        {
            return await _context.UnidadAdministrativas.ToListAsync();
        }
    }
}
