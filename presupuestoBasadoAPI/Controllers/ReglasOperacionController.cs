using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using presupuestoBasadoAPI.Dto;
using presupuestoBasadoAPI.Interfaces;
using presupuestoBasadoAPI.Models;
using presupuestoBasadoAPI.Services;
using System.Security.Claims;
using System.Threading.Tasks;

namespace presupuestoBasadoAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReglasOperacionController : ControllerBase
    {
        private readonly IReglasOperacionService _service;
        private readonly CloudinaryService _cloudinaryService;

        public ReglasOperacionController(
            IReglasOperacionService service,
            CloudinaryService cloudinaryService)
        {
            _service = service;
            _cloudinaryService = cloudinaryService;
        }

        private string GetUserId() =>
            User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

        [HttpPost]
        public async Task<ActionResult<ReglasOperacion>> Crear([FromForm] ReglasOperacionDto dto)
        {
            if (dto == null)
                return BadRequest(new { message = "El objeto dto es requerido" });

            if (dto.Archivo != null)
            {
                var url = await _cloudinaryService.SubirArchivoAsync(dto.Archivo);
                dto.ArchivoAdjunto = url;
            }

            dto.UserId = GetUserId();

            var nuevo = await _service.CrearAsync(dto, GetUserId());
            return Ok(nuevo);
        }

        [HttpGet("ultimo")]
        public async Task<ActionResult<ReglasOperacion?>> ObtenerUltimo()
        {
            var userId = GetUserId();

            var ultimo = await _service.ObtenerUltimoAsync(userId);

            if (ultimo == null)
                return NotFound(new { message = "No hay registros para este usuario" });

            return Ok(ultimo);
        }
    }
}
