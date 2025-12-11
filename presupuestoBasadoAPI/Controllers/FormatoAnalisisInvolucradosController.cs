using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using presupuestoBasadoAPI.Models;
using System.IO;
using System.Linq;

namespace presupuestoBasadoAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FormatoAnalisisInvolucradosController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly string _imagesPath;

        public FormatoAnalisisInvolucradosController(AppDbContext context)
        {
            _context = context;
            _imagesPath = Path.Combine(Directory.GetCurrentDirectory(), "Images");
        }

        private string GetUserId()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "";

            var token = HttpContext.Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(token))
                Console.WriteLine(" [FormatoAnalisisInvolucrados] No se recibió token en la cabecera Authorization.");
            else
                Console.WriteLine($" [FormatoAnalisisInvolucrados] Token recibido: {token.Substring(0, Math.Min(token.Length, 40))}...");

            if (string.IsNullOrEmpty(userId))
                Console.WriteLine(" [FormatoAnalisisInvolucrados] No se pudo extraer el UserId del token.");
            else
                Console.WriteLine($" [FormatoAnalisisInvolucrados] UserId extraído del token: {userId}");

            return userId;
        }

        private string GetEmblemaPath(string userId)
        {
            var usuario = _context.Users.Include(u => u.Entidad).FirstOrDefault(u => u.Id == userId);

            string emblemaFileName = "emblema.png"; // por defecto

            if (usuario?.Entidad != null && !string.IsNullOrEmpty(usuario.Entidad.Nombre))
            {
                string entidadNombre = usuario.Entidad.Nombre.Trim().ToLower();
                string posiblePath = Path.Combine(_imagesPath, $"{entidadNombre}.png");

                if (System.IO.File.Exists(posiblePath))
                    emblemaFileName = $"{entidadNombre}.png";
                else
                    Console.WriteLine($" [PDF] No se encontró imagen para la entidad '{entidadNombre}', usando emblema por defecto.");
            }

            return Path.Combine(_imagesPath, emblemaFileName);
        }

        [HttpGet("ultimo")]
        public IActionResult GenerarPdf()
        {
            var userId = GetUserId();
            var emblemaPath = GetEmblemaPath(userId);

            var analisis = _context.AnalisisAlternativas
                .Include(a => a.Alternativas)
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.Id)
                .FirstOrDefault();

            if (analisis == null || !analisis.Alternativas.Any())
            {
                Console.WriteLine($"⚠️ [FormatoAnalisisInvolucrados] No se encontraron registros para el usuario: {userId}");
                return NotFound("No se encontraron alternativas para el usuario actual.");
            }

            Console.WriteLine($" [FormatoAnalisisInvolucrados] Generando PDF para usuario: {userId}");

            var alternativas = analisis.Alternativas.ToList();

            using var ms = new MemoryStream();
            var writer = new PdfWriter(ms);
            var pdfDoc = new PdfDocument(writer);
            var document = new Document(pdfDoc);
            var font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
            var colorVino = new DeviceRgb(102, 0, 0);

            // === ENCABEZADO CON EMBLEMA ===
            var encabezadoTabla = new Table(UnitValue.CreatePercentArray(new float[] { 75, 25 }))
                .UseAllAvailableWidth()
                .SetBorder(Border.NO_BORDER);

            var celdaTitulo = new Cell()
                .Add(new Paragraph("Anexo 6\nAnálisis de Alternativas")
                    .SetFont(font)
                    .SetFontSize(14)
                    .SetBold())
                .SetBorder(Border.NO_BORDER)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                .SetPadding(0);
            encabezadoTabla.AddCell(celdaTitulo);

            if (System.IO.File.Exists(emblemaPath))
            {
                var emblema = new Image(ImageDataFactory.Create(emblemaPath))
                    .SetWidth(75)
                    .SetAutoScale(true)
                    .SetHorizontalAlignment(HorizontalAlignment.RIGHT);

                var celdaEmblema = new Cell()
                    .Add(emblema)
                    .SetBorder(Border.NO_BORDER)
                    .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                    .SetTextAlignment(TextAlignment.RIGHT)
                    .SetPaddingRight(10);

                encabezadoTabla.AddCell(celdaEmblema);
            }
            else
            {
                encabezadoTabla.AddCell(new Cell().SetBorder(Border.NO_BORDER));
                Console.WriteLine($"⚠️ No se encontró el emblema en: {emblemaPath}");
            }

            document.Add(encabezadoTabla);
            document.Add(new Paragraph("\n"));

            // === TABLA DE ALTERNATIVAS ===
            float[] columnas = { 120, 70, 70, 70, 70, 70, 70, 70, 70, 60 };
            var tabla = new Table(UnitValue.CreatePointArray(columnas))
                .SetWidth(UnitValue.CreatePercentValue(100));

            string[] headers = {
                "Alternativas (componentes)",
                "a) Facultad Jurídica",
                "b) Presupuesto disponible",
                "c) Realizable en corto plazo",
                "d) Disponibilidad Total de Recursos Técnicos",
                "e) Disponibilidad de Recursos Administrativos",
                "f) Cultural y socialmente Aceptable",
                "g) Estudio de Impacto",
                "h) Probabilidad De Éxito",
                "Total"
            };

            foreach (var h in headers)
            {
                tabla.AddHeaderCell(new Cell()
                    .Add(new Paragraph(h)
                        .SetFont(font)
                        .SetFontSize(8)
                        .SetFontColor(ColorConstants.WHITE)
                        .SetTextAlignment(TextAlignment.CENTER))
                    .SetBackgroundColor(colorVino)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                    .SetBorder(new SolidBorder(ColorConstants.BLACK, 1)));
            }

            foreach (var alt in alternativas)
            {
                tabla.AddCell(CeldaNormal(alt.Nombre));
                tabla.AddCell(CeldaNormal(ValorTexto(alt.Facultad)));
                tabla.AddCell(CeldaNormal(ValorTexto(alt.Presupuesto)));
                tabla.AddCell(CeldaNormal(ValorTexto(alt.CortoPlazo)));
                tabla.AddCell(CeldaNormal(ValorTexto(alt.RecursosTecnicos)));
                tabla.AddCell(CeldaNormal(ValorTexto(alt.RecursosAdministrativos)));
                tabla.AddCell(CeldaNormal(ValorTexto(alt.CulturalSocial)));
                tabla.AddCell(CeldaNormal(ValorTexto(alt.Impacto)));
                tabla.AddCell(CeldaNormal(ValorTexto(alt.Analisis.Probabilidad)));
                tabla.AddCell(CeldaNormal(alt.Total.ToString()));
            }

            document.Add(tabla);
            document.Add(new Paragraph("\nEscala: 3 = Viabilidad Alta, 2 = Viabilidad Media, 1 = Viabilidad Baja, N/A = No aplica")
                .SetFont(font)
                .SetFontSize(9));

            document.Close();
            pdfDoc.Close();

            Console.WriteLine($" [FormatoAnalisisInvolucrados] PDF generado correctamente para {userId}");

            return File(ms.ToArray(), "application/pdf", "Anexo6_AnalisisInvolucrados.pdf");
        }

        private static Cell CeldaNormal(string texto)
        {
            return new Cell()
                .Add(new Paragraph(texto ?? "")
                    .SetFontSize(8)
                    .SetTextAlignment(TextAlignment.CENTER))
                .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                .SetBorder(new SolidBorder(ColorConstants.BLACK, 1));
        }

        private static string ValorTexto(int valor)
        {
            return valor == 0 ? "N/A" : valor.ToString();
        }
    }
}
