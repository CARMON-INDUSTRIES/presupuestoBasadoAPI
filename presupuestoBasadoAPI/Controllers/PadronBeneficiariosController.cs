using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using presupuestoBasadoAPI.Dto;
using presupuestoBasadoAPI.Interfaces;
using presupuestoBasadoAPI.Services;
using System.Security.Claims;
using System.Threading.Tasks;

namespace presupuestoBasadoAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PadronBeneficiariosController : ControllerBase
    {
        private readonly IPadronBeneficiariosService _service;
        private readonly CloudinaryService _cloudinaryService;

        public PadronBeneficiariosController(
            IPadronBeneficiariosService service,
            CloudinaryService cloudinaryService)
        {
            _service = service;
            _cloudinaryService = cloudinaryService;
        }

        // ✅ Obtener el ID real del usuario autenticado desde las claims del token JWT
        private string GetUserId() =>
            User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

        [HttpPost]
        public async Task<IActionResult> Crear([FromForm] PadronBeneficiariosDto dto)
        {
            if (dto == null)
                return BadRequest(new { message = "El objeto dto es requerido" });

            // 🔹 Subir archivo a Cloudinary si existe
            if (dto.Archivo != null)
            {
                var url = await _cloudinaryService.SubirArchivoAsync(dto.Archivo);
                dto.ArchivoAdjunto = url;
            }

            // 🔹 Asociar el registro con el ID del usuario autenticado
            dto.UserId = GetUserId();

            var creado = await _service.CrearAsync(dto);
            return Ok(creado);
        }

        [HttpGet("ultimo")]
        public async Task<IActionResult> ObtenerUltimo()
        {
            var userId = GetUserId();

            // 🔹 Filtrar por el usuario autenticado
            var ultimo = await _service.ObtenerUltimoAsync(userId);

            if (ultimo == null)
                return NotFound(new { message = "No hay registros para este usuario" });

            return Ok(ultimo);
        }
    }
}
