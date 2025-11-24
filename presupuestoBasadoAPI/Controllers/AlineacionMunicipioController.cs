using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using presupuestoBasadoAPI.Models;
using Microsoft.AspNetCore.Identity;
using presupuestoBasadoAPI.Services;

namespace presupuestoBasadoAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] 
    public class AlineacionMunicipioController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager; 
        private readonly IUsuarioActualService _usuarioActualService;

        public AlineacionMunicipioController(
            AppDbContext context,
            UserManager<ApplicationUser> userManager, 
            IUsuarioActualService usuarioActualService)
        {
            _context = context;
            _userManager = userManager;
            _usuarioActualService = usuarioActualService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AlineacionMunicipio>>> GetAll()
        {
            var userId = await _usuarioActualService.ObtenerUserIdAsync();
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            return await _context.AlineacionesMunicipio
                                 .Where(x => x.UserId == userId)
                                 .ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult> Crear(AlineacionMunicipio modelo)
        {
            var userId = await _usuarioActualService.ObtenerUserIdAsync();
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            modelo.UserId = userId; 

            _context.AlineacionesMunicipio.Add(modelo);
            await _context.SaveChangesAsync();
            return Ok(modelo);
        }

        [HttpGet("ultimo")]
        public async Task<ActionResult<AlineacionMunicipio>> GetUltimo()
        {
            var userId = await _usuarioActualService.ObtenerUserIdAsync();
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var ultimo = await _context.AlineacionesMunicipio
                                       .Where(x => x.UserId == userId)
                                       .OrderByDescending(x => x.Id)
                                       .FirstOrDefaultAsync();

            if (ultimo == null) return NotFound();
            return Ok(ultimo);
        }
    }
}
