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

        [HttpGet("borrador")]
        public async Task<ActionResult<ArbolObjetivosDto>> GetBorrador()
        {
            var userId = GetUserId();
            var ultimo = await _service.GetUltimoAsync(userId);

            if (ultimo == null)
            {
                var nuevo = new ArbolObjetivosDto
                {
                    Fin = "",
                    ObjetivoCentral = "",
                    UserId = userId,
                    Componentes = new List<ComponenteObjetivoDto>()
                };

                var creado = await _service.CrearAsync(nuevo, userId);
                return Ok(creado);
            }

            return Ok(ultimo);
        }

        [HttpPut("autosave")]
        public async Task<ActionResult<ArbolObjetivosDto>> AutoSave(
        [FromBody] ArbolObjetivosDto dto)
        {
            var userId = GetUserId();
            var existente = await _service.GetUltimoAsync(userId);

            dto.UserId = userId;

            if (dto.Componentes != null)
            {
                foreach (var comp in dto.Componentes)
                {
                    comp.UserId = userId;
                }
            }

            if (existente == null)
            {
                var creado = await _service.CrearAsync(dto, userId);
                return Ok(creado);
            }

            dto.Id = existente.Id;

            await _service.UpdateAsync(existente.Id, dto, userId);

            var actualizado = await _service.GetUltimoAsync(userId);

            return Ok(actualizado);
        }

    }
}
