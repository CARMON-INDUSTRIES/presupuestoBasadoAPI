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
    public class FormatoAnalisisDeInvolucradosController : ControllerBase
    {
        private readonly AppDbContext _context;

        // Ruta base de imágenes
        private readonly string _imagesPath = System.IO.Path.Combine(
            System.IO.Directory.GetCurrentDirectory(), "Images");

        public FormatoAnalisisDeInvolucradosController(AppDbContext context)
        {
            _context = context;
        }

        private string GetUserId()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "";

            if (string.IsNullOrEmpty(userId))
                Console.WriteLine(" [FormatoAnalisisDeInvolucrados] No se pudo extraer el UserId del token.");
            else
                Console.WriteLine($" [FormatoAnalisisDeInvolucrados] UserId: {userId}");

            return userId;
        }

        [HttpGet("ultimo")]
        public IActionResult GenerarPdf()
        {
            var userId = GetUserId();

            // 🔹 Obtener usuario con su entidad
            var usuario = _context.Users
                .Include(u => u.Entidad)
                .FirstOrDefault(u => u.Id == userId);

            var problema = _context.IdentificacionDescripcionProblemas
                                   .Where(p => p.UserId == userId)
                                   .OrderByDescending(p => p.Id)
                                   .FirstOrDefault();

            if (problema == null)
            {
                Console.WriteLine($" [FormatoAnalisisDeInvolucrados] No se encontró registro de problema para el usuario {userId}");
                return NotFound("No se encontró un registro en IdentificacionDescripcionProblemas para este usuario");
            }

            using var ms = new MemoryStream();
            using var writer = new PdfWriter(ms);
            using var pdf = new PdfDocument(writer);
            var doc = new Document(pdf, PageSize.LETTER);
            doc.SetMargins(40, 40, 40, 40);

            var font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
            var colorInstitucional = new DeviceRgb(105, 27, 49);

            // === 🔹 Determinar emblema según la entidad del usuario ===
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

            string emblemaPath = System.IO.Path.Combine(_imagesPath, emblemaFileName);

            // === ENCABEZADO ===
            var encabezado = new Table(UnitValue.CreatePercentArray(new float[] { 80, 20 }))
                .UseAllAvailableWidth();

            // Lado izquierdo (texto)
            encabezado.AddCell(new Cell()
                .Add(new Paragraph("Anexo 3")
                    .SetFont(font).SetFontSize(14).SetBold())
                .Add(new Paragraph("Análisis de Involucrados")
                    .SetFont(font).SetFontSize(12))
                .SetBorder(Border.NO_BORDER)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE));

            // Lado derecho (emblema)
            float pageWidth = pdf.GetDefaultPageSize().GetWidth();
            float pageHeight = pdf.GetDefaultPageSize().GetHeight();

            if (System.IO.File.Exists(emblemaPath))
            {
                var emblema = new Image(ImageDataFactory.Create(emblemaPath))
                    .SetWidth(85)
                    .SetAutoScale(true)
                    .SetFixedPosition(pageWidth - 100 - 85, pageHeight - 93);
                doc.Add(emblema);

            }

            else
            {
                encabezado.AddCell(new Cell().SetBorder(Border.NO_BORDER)); // espacio vacío
            }

            doc.Add(encabezado);
            doc.Add(new Paragraph("\n"));

            // === TÍTULO DE PROBLEMÁTICA CENTRAL ===
            var titulo = new Table(UnitValue.CreatePercentArray(new float[] { 100 })).UseAllAvailableWidth();
            titulo.AddCell(new Cell()
                .Add(new Paragraph("PROBLEMÁTICA CENTRAL:")
                    .SetFont(font)
                    .SetFontSize(11)
                    .SetBold()
                    .SetFontColor(ColorConstants.WHITE))
                .SetBackgroundColor(colorInstitucional)
                .SetBorder(Border.NO_BORDER)
                .SetPadding(5));
            doc.Add(titulo);

            // === RECUADRO DE PROBLEMÁTICA CENTRAL ===
            var cuadroProblema = new Table(UnitValue.CreatePercentArray(new float[] { 100 })).UseAllAvailableWidth();
            cuadroProblema.AddCell(new Cell()
                .Add(new Paragraph(problema.ProblemaCentral ?? "")
                    .SetFont(font)
                    .SetFontSize(10)
                    .SetFontColor(ColorConstants.WHITE)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetMultipliedLeading(1.2f))
                .SetBackgroundColor(colorInstitucional)
                .SetBorder(new SolidBorder(colorInstitucional, 1))
                .SetPadding(10));
            doc.Add(cuadroProblema);

            doc.Add(new Paragraph("\n"));

            // === TABLA DE ACTORES INVOLUCRADOS ===
            var tabla = new Table(UnitValue.CreatePercentArray(new float[] { 25, 25, 25, 25 }))
                .UseAllAvailableWidth();

            string[] headers = { "BENEFICIARIOS", "OPOSITORES", "EJECUTORES", "INDIFERENTES" };
            foreach (var header in headers)
            {
                tabla.AddCell(new Cell()
                    .Add(new Paragraph(header)
                        .SetFont(font)
                        .SetFontSize(10)
                        .SetBold()
                        .SetFontColor(ColorConstants.BLACK))
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetBackgroundColor(ColorConstants.WHITE)
                    .SetBorder(new SolidBorder(colorInstitucional, 1))
                    .SetPadding(5));
            }

            tabla.AddCell(CeldaContenido(problema.CausaBeneficiados, font, colorInstitucional));
            tabla.AddCell(CeldaContenido(problema.CausaOpositores, font, colorInstitucional));
            tabla.AddCell(CeldaContenido(problema.CausaEjecutores, font, colorInstitucional));
            tabla.AddCell(CeldaContenido(problema.CausaIndiferentes, font, colorInstitucional));

            doc.Add(tabla);

            doc.Close();

            Console.WriteLine($" [FormatoAnalisisDeInvolucrados] PDF generado correctamente para {userId}");

            return File(ms.ToArray(), "application/pdf", "FormatoAnalisisDeInvolucrados.pdf");
        }

        private Cell CeldaContenido(string texto, PdfFont font, DeviceRgb color)
        {
            return new Cell()
                .Add(new Paragraph(texto ?? "")
                    .SetFont(font)
                    .SetFontSize(9)
                    .SetMultipliedLeading(1.2f))
                .SetBorder(new SolidBorder(color, 1))
                .SetPadding(8)
                .SetTextAlignment(TextAlignment.LEFT)
                .SetVerticalAlignment(VerticalAlignment.TOP);
        }
    }
}
