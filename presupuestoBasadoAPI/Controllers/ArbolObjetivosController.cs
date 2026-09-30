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

            return await EjecutarIA(async () => new { textoPositivo =
                await iaService.ConvertirAPositivoAsync(dto.TextoBase, dto.Nivel, HttpContext.RequestAborted) });
        }

        [HttpPost("convertir-arbol")]
        [RequestSizeLimit(300000)]
        public Task<IActionResult> ConvertirArbol([FromBody] List<IAConvertirTextoDto> nodos,
            [FromServices] IIAService iaService) => EjecutarIA(async () =>
                await iaService.ConvertirArbolAsync(nodos, HttpContext.RequestAborted));

        private async Task<IActionResult> EjecutarIA(Func<Task<object>> operacion)
        {
            try { return Ok(await operacion()); }
            catch (ArgumentException ex) { return BadRequest(new { mensaje = ex.Message }); }
            catch (HttpRequestException ex)
            {
                var cuota = ex.StatusCode == System.Net.HttpStatusCode.TooManyRequests;
                var mensaje = ex.StatusCode switch
                {
                    System.Net.HttpStatusCode.NotFound => "El modelo configurado en Gemini:Model no está disponible para este proyecto. Actualiza el modelo y reinicia el backend.",
                    System.Net.HttpStatusCode.Unauthorized => "Gemini rechazó la autenticación. Revisa Gemini:ApiKey en los secretos del backend.",
                    System.Net.HttpStatusCode.Forbidden => "Gemini denegó el acceso. Revisa los permisos o restricciones de la clave y del proyecto.",
                    System.Net.HttpStatusCode.BadRequest => "Gemini rechazó la solicitud (400). Revisa la clave y la compatibilidad del modelo con los parámetros enviados.",
                    System.Net.HttpStatusCode.TooManyRequests => "Se alcanzó la cuota de Gemini. Puedes continuar manualmente o reintentar más tarde.",
                    null => "No se pudo conectar con Gemini. Revisa la conexión del servidor.",
                    _ => "Gemini no está disponible temporalmente. Intenta nuevamente más tarde."
                };
                return StatusCode(cuota ? 429 : 502, new { mensaje, codigoProveedor = (int?)ex.StatusCode });
            }
            catch (OperationCanceledException)
            { return StatusCode(504, new { mensaje = "La generación se canceló o tardó demasiado. Puedes reintentar." }); }
            catch (InvalidOperationException ex)
            { return StatusCode(503, new { mensaje = ex.Message }); }
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
