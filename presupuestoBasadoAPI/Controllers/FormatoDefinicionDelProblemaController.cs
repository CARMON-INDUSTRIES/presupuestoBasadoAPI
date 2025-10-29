using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using presupuestoBasadoAPI.Models;
using System;
using System.IO;
using System.Linq;

namespace presupuestoBasadoAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FormatoDefinicionDelProblemaController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly string _imagesPath;

        public FormatoDefinicionDelProblemaController(AppDbContext context)
        {
            _context = context;
            _imagesPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Images");
        }

        private string GetUserId()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "";
            var token = HttpContext.Request.Headers["Authorization"].ToString();

            if (string.IsNullOrEmpty(token))
                Console.WriteLine(" [FormatoDefinicionDelProblema] No se recibió token en la cabecera Authorization.");
            else
                Console.WriteLine($" [FormatoDefinicionDelProblema] Token recibido: {token.Substring(0, Math.Min(token.Length, 40))}...");

            if (string.IsNullOrEmpty(userId))
                Console.WriteLine(" [FormatoDefinicionDelProblema] No se pudo extraer el UserId del token.");
            else
                Console.WriteLine($" [FormatoDefinicionDelProblema] UserId extraído del token: {userId}");

            return userId;
        }

        // 🔰 FUNCIÓN PARA OBTENER EL EMBLEMA SEGÚN LA ENTIDAD DEL USUARIO
        private string GetEmblemaPath(string userId)
        {
            var usuario = _context.Users.Include(u => u.Entidad).FirstOrDefault(u => u.Id == userId);

            string emblemaFileName = "emblema.png"; // por defecto

            if (usuario?.Entidad != null && !string.IsNullOrEmpty(usuario.Entidad.Nombre))
            {
                string entidadNombre = usuario.Entidad.Nombre.Trim().ToLower();
                string posiblePath = System.IO.Path.Combine(_imagesPath, $"{entidadNombre}.png");

                if (System.IO.File.Exists(posiblePath))
                    emblemaFileName = $"{entidadNombre}.png";
                else
                    Console.WriteLine($" [PDF] No se encontró imagen para la entidad '{entidadNombre}', usando emblema por defecto.");
            }

            return System.IO.Path.Combine(_imagesPath, emblemaFileName);
        }

        [HttpGet("ultimo")]
        public IActionResult GenerarPdf()
        {
            var userId = GetUserId();
            var emblemaPath = GetEmblemaPath(userId);

            var identificacion = _context.IdentificacionDescripcionProblemas
                .Where(i => i.UserId == userId)
                .OrderByDescending(i => i.Id)
                .FirstOrDefault();

            var cobertura = _context.Coberturas
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.Id)
                .FirstOrDefault();

            var efectoSuperior = _context.EfectosSuperiores
                .Where(e => e.UserId == userId)
                .OrderByDescending(e => e.Id)
                .FirstOrDefault();

            if (identificacion == null || cobertura == null)
            {
                Console.WriteLine($" [FormatoDefinicionDelProblema] Faltan registros para el usuario {userId}");
                return NotFound("Faltan registros de Identificación o Cobertura");
            }

            using var ms = new MemoryStream();
            using var writer = new PdfWriter(ms);
            using var pdf = new PdfDocument(writer);
            var doc = new Document(pdf, PageSize.LETTER);
            doc.SetMargins(40, 40, 40, 40);

            var font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
            var colorInstitucional = new DeviceRgb(105, 27, 49);
            var bordeNegro = new SolidBorder(ColorConstants.BLACK, 1);

            // === ENCABEZADO CON EMBLEMA ===
            var encabezado = new Table(UnitValue.CreatePercentArray(new float[] { 80, 20 }))
                .UseAllAvailableWidth();

            // Lado izquierdo (texto)
            encabezado.AddCell(new Cell()
                .Add(new Paragraph("Anexo 2").SetFont(font).SetFontSize(14).SetBold())
                .Add(new Paragraph("Definición del Problema").SetFont(font).SetFontSize(12))
                .SetBorder(Border.NO_BORDER)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE));

            // Lado derecho (imagen si existe)
            if (System.IO.File.Exists(emblemaPath))
            {
                var emblema = new Image(ImageDataFactory.Create(emblemaPath))
                    .SetHeight(60)
                    .SetAutoScale(true)
                    .SetHorizontalAlignment(HorizontalAlignment.RIGHT);
                encabezado.AddCell(new Cell()
                    .Add(emblema)
                    .SetBorder(Border.NO_BORDER)
                    .SetTextAlignment(TextAlignment.RIGHT)
                    .SetVerticalAlignment(VerticalAlignment.MIDDLE));
            }
            else
            {
                encabezado.AddCell(new Cell().SetBorder(Border.NO_BORDER)); // espacio vacío
                Console.WriteLine($"⚠️ No se encontró el emblema en: {emblemaPath}");
            }

            doc.Add(encabezado);
            doc.Add(new Paragraph("\n"));

            // === SECCIÓN 1 ===
            doc.Add(ContenedorSeccion("1.- POBLACIÓN Ó ÁREA DE ENFOQUE POTENCIAL",
                cobertura.IdentificacionCaracterizacionPoblacionPotencial, font, colorInstitucional, bordeNegro));

            // === SECCIÓN 2 ===
            doc.Add(ContenedorSeccion("2.- POBLACIÓN Ó ÁREA DE ENFOQUE OBJETIVO",
                cobertura.IdentificacionCaracterizacionPoblacionObjetivo, font, colorInstitucional, bordeNegro));

            // === SECCIÓN 3 ===
            doc.Add(ContenedorSeccion("3.- PROBLEMÁTICA CENTRAL (PROPÓSITO)",
                identificacion.ProblemaCentral, font, colorInstitucional, bordeNegro));

            // === SECCIÓN 4 ===
            var divMagnitud = new Div().SetBorder(bordeNegro).SetPadding(5).SetMarginBottom(10);
            divMagnitud.Add(SeccionTitulo("4.- MAGNITUD DEL PROBLEMA", font, colorInstitucional));

            var tablaMagnitud = new Table(UnitValue.CreatePercentArray(new float[] { 33, 33, 34 })).UseAllAvailableWidth();
            tablaMagnitud.AddHeaderCell(CeldaEncabezado("4.1 POBLACIÓN POTENCIAL", font, colorInstitucional));
            tablaMagnitud.AddHeaderCell(CeldaEncabezado("4.2 POBLACIÓN OBJETIVO", font, colorInstitucional));
            tablaMagnitud.AddHeaderCell(CeldaEncabezado("4.3 POBLACIÓN ATENDIDA DEL EJERCICIO FISCAL ANTERIOR", font, colorInstitucional));

            tablaMagnitud.AddCell(CeldaDato(cobertura.CuantificacionPoblacionPotencial.ToString() ?? "", font, colorInstitucional));
            tablaMagnitud.AddCell(CeldaDato(cobertura.CuantificacionPoblacionObjetivo.ToString() ?? "", font, colorInstitucional));
            tablaMagnitud.AddCell(CeldaDato(cobertura.CuantificacionPoblacionAtendidaAnterior.ToString() ?? "", font, colorInstitucional));

            divMagnitud.Add(tablaMagnitud);

            // === NUEVA FILA: UNIDAD DE MEDIDA ===
            var tablaUnidad = new Table(UnitValue.CreatePercentArray(new float[] { 30, 70 })).UseAllAvailableWidth();
            tablaUnidad.AddCell(CeldaEncabezado("Unidad de Medida", font, colorInstitucional));
            tablaUnidad.AddCell(CeldaDato(cobertura.UnidadMedida ?? "No especificada", font, colorInstitucional));

            divMagnitud.Add(new Paragraph("\n"));
            divMagnitud.Add(tablaUnidad);

            doc.Add(divMagnitud);

            // === SECCIÓN 5 ===
            doc.Add(ContenedorSeccion("5.- EFECTO SUPERIOR (FIN)",
                efectoSuperior?.Descripcion ?? "", font, colorInstitucional, bordeNegro));

            doc.Close();

            Console.WriteLine($" [FormatoDefinicionDelProblema] PDF generado correctamente para {userId}");

            return File(ms.ToArray(), "application/pdf", "FormatoDefinicionDelProblema.pdf");
        }

        // === FUNCIONES AUXILIARES ===

        private Div ContenedorSeccion(string titulo, string texto, PdfFont font, DeviceRgb color, Border borde)
        {
            var div = new Div()
                .SetBorder(borde)
                .SetPadding(5)
                .SetMarginBottom(10);

            div.Add(SeccionTitulo(titulo, font, color));
            div.Add(CampoTexto(texto, font));

            return div;
        }

        private Paragraph SeccionTitulo(string texto, PdfFont font, DeviceRgb color)
        {
            return new Paragraph(texto)
                .SetFont(font)
                .SetFontSize(11)
                .SetBold()
                .SetFontColor(ColorConstants.WHITE)
                .SetBackgroundColor(color)
                .SetPadding(5)
                .SetMarginTop(5)
                .SetMarginBottom(3);
        }

        private Paragraph CampoTexto(string texto, PdfFont font)
        {
            return new Paragraph(texto ?? "")
                .SetFont(font)
                .SetFontSize(10)
                .SetTextAlignment(TextAlignment.LEFT)
                .SetMarginBottom(5);
        }

        private Cell CeldaEncabezado(string texto, PdfFont font, DeviceRgb color)
        {
            return new Cell()
                .Add(new Paragraph(texto)
                .SetFont(font).SetFontSize(9).SetBold().SetFontColor(ColorConstants.BLACK))
                .SetTextAlignment(TextAlignment.CENTER)
                .SetBackgroundColor(ColorConstants.WHITE)
                .SetBorder(new SolidBorder(color, 1))
                .SetPadding(5);
        }

        private Cell CeldaDato(string texto, PdfFont font, DeviceRgb color)
        {
            return new Cell()
                .Add(new Paragraph(texto)
                .SetFont(font).SetFontSize(9))
                .SetTextAlignment(TextAlignment.CENTER)
                .SetBorder(new SolidBorder(color, 1))
                .SetPadding(5);
        }
    }
}
