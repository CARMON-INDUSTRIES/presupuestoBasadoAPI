using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using presupuestoBasadoAPI.Interfaces;
using presupuestoBasadoAPI.Dto;
using System.Security.Claims;

namespace presupuestoBasadoAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ArbolObjetivosController : ControllerBase
    {
        private readonly IArbolObjetivosService _service;

        public ArbolObjetivosController(IArbolObjetivosService service)
        {
            _service = service;
        }

        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier)
                   ?? throw new UnauthorizedAccessException("No se pudo obtener el UserId del token.");
        }

        [HttpGet("ultimo")]
        public async Task<IActionResult> GetUltimo()
        {
            var userId = GetUserId();
            var result = await _service.GetUltimoAsync(userId);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] ArbolObjetivosDto dto)
        {
            var userId = GetUserId();
            var result = await _service.CrearAsync(dto, userId);
            return Ok(result);
        }

        [HttpPost("convertir-positivo")]
        public async Task<IActionResult> ConvertirTextoAPositivo(
    [FromBody] IAConvertirTextoDto dto,
    [FromServices] IIAService iaService)
        {
            if (string.IsNullOrWhiteSpace(dto.TextoBase))
                return BadRequest("El texto base es obligatorio.");

            var nivelesValidos = new[]
            {
        "FIN",
        "OBJETIVO_CENTRAL",
        "COMPONENTE",
        "RESULTADO",
        "MEDIO"
    };

            if (!nivelesValidos.Contains(dto.Nivel))
                return BadRequest("Nivel de árbol no válido.");

            var resultado = await iaService.ConvertirAPositivoAsync(
                dto.TextoBase,
                dto.Nivel
            );

            return Ok(new { textoPositivo = resultado });
        }

    }
}
