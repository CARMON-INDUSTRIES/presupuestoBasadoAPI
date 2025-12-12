using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Layout.Borders;
using presupuestoBasadoAPI.Models;

namespace presupuestoBasadoAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FormatoMatrizController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly string _imagesPath;

        public FormatoMatrizController(AppDbContext context)
        {
            _context = context;
            _imagesPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Images");
        }

        private string GetUserId() =>
            User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "";

        private string GetEmblemaPath(string userId)
        {
            var usuario = _context.Users.Include(u => u.Entidad).FirstOrDefault(u => u.Id == userId);
            string emblemaFileName = "emblema.png"; // por defecto

            if (usuario?.Entidad != null && !string.IsNullOrWhiteSpace(usuario.Entidad.Nombre))
            {
                string entidadNombre = usuario.Entidad.Nombre.Trim().ToLower();
                string posiblePath = System.IO.Path.Combine(_imagesPath, $"{entidadNombre}.png");
                if (System.IO.File.Exists(posiblePath))
                    emblemaFileName = $"{entidadNombre}.png";
            }

            return System.IO.Path.Combine(_imagesPath, emblemaFileName);
        }

        [HttpGet("ultimo")]
        public IActionResult GenerarPdf()
        {
            var userId = GetUserId();
            var emblemaPath = GetEmblemaPath(userId);

            var user = _context.Users
                .Include(u => u.UnidadAdministrativa)
                .FirstOrDefault(u => u.Id == userId);

            if (user == null)
                return NotFound("Usuario no encontrado.");

            var matriz = _context.MatricesIndicadores
                .Include(m => m.Filas)
                .Where(m => m.UserId == userId)
                .OrderByDescending(m => m.Id)
                .FirstOrDefault();

            var arbol = _context.ArbolObjetivos
                .Include(a => a.Componentes)
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.Id)
                .FirstOrDefault();

            if (arbol == null && matriz == null)
                return NotFound("No se encontró ni Árbol de Objetivos ni Matriz de Indicadores para este usuario.");

            var filasGuardadas = matriz?.Filas?.ToList() ?? new List<FilaMatriz>();

            using var ms = new MemoryStream();
            using var writer = new PdfWriter(ms);
            using var pdf = new PdfDocument(writer);
            var pageSize = PageSize.LETTER;
            var doc = new Document(pdf, pageSize);
            doc.SetMargins(40, 36, 36, 36);

            var font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
            var colorInstitucional = new DeviceRgb(105, 27, 49);
            var thinBorder = new SolidBorder(new DeviceRgb(230, 230, 230), 0.5f);

            float pageWidth = pageSize.GetWidth();
            float pageHeight = pageSize.GetHeight();

            var titulo = new Paragraph("Matriz de Indicador Para Resultados")
                .SetFont(font)
                .SetFontSize(14)
                .SetBold()
                .SetTextAlignment(TextAlignment.LEFT)
                .SetFixedPosition(36, pageHeight - 50, pageWidth - 72);
            doc.Add(titulo);

            if (System.IO.File.Exists(emblemaPath))
            {
                var emblema = new Image(ImageDataFactory.Create(emblemaPath))
                    .SetWidth(85)
                    .SetAutoScale(true)
                    .SetFixedPosition(pageWidth - 100 - 85, pageHeight - 100);
                doc.Add(emblema);
            }

            doc.Add(new Paragraph("\n\n\n"));

            var encabezado = new Table(UnitValue.CreatePercentArray(new float[] { 25, 25, 25, 25 })).UseAllAvailableWidth();
            encabezado.AddCell(CeldaEncabezado("Unidad Responsable:", font, colorInstitucional));
            encabezado.AddCell(CeldaDato(user.NombreMatriz, font));
            encabezado.AddCell(CeldaEncabezado("Unidad Presupuestal Responsable:", font, colorInstitucional));
            encabezado.AddCell(CeldaDato(user.UnidadesPresupuestales, font));

            encabezado.AddCell(CeldaEncabezado("Responsable:", font, colorInstitucional));
            encabezado.AddCell(CeldaDato(user.NombreCompleto, font));
            encabezado.AddCell(CeldaEncabezado("Programa Presupuestario:", font, colorInstitucional));
            encabezado.AddCell(CeldaDato(user.ProgramaPresupuestario, font));

            doc.Add(encabezado);
            doc.Add(new Paragraph("\n"));

            var tabla = new Table(UnitValue.CreatePercentArray(new float[] { 12, 36, 16, 18, 18 }))
                .UseAllAvailableWidth()
                .SetBorder(new SolidBorder(colorInstitucional, 1f));

            // Encabezados
            tabla.AddHeaderCell(CeldaHeaderTabla("Nivel", font, colorInstitucional));
            tabla.AddHeaderCell(CeldaHeaderTabla("Resumen Narrativo", font, colorInstitucional));
            tabla.AddHeaderCell(CeldaHeaderTabla("Indicadores (solo nombre)", font, colorInstitucional));
            tabla.AddHeaderCell(CeldaHeaderTabla("Medios de verificación", font, colorInstitucional));
            tabla.AddHeaderCell(CeldaHeaderTabla("Supuestos", font, colorInstitucional));

            // 🔥 NUEVO: filas ordenadas reales, sin duplicados
            var filasOrdenadas = filasGuardadas
                .Where(f => !string.IsNullOrWhiteSpace(f.Nivel))
                .OrderBy(f => f.Id)
                .ToList();

            foreach (var fila in filasOrdenadas)
            {
                tabla.AddCell(CeldaContenidoTabla(fila.Nivel, font));
                tabla.AddCell(CeldaContenidoTabla(fila.ResumenNarrativo, font));
                tabla.AddCell(CeldaContenidoTabla(fila.Indicadores, font));
                tabla.AddCell(CeldaContenidoTabla(fila.Medios, font));
                tabla.AddCell(CeldaContenidoTabla(fila.Supuestos, font));
            }

            if (!filasOrdenadas.Any())
            {
                var empty = new Cell(1, 5)
                    .Add(new Paragraph("No hay información disponible para la Matriz.")
                    .SetFont(font).SetFontSize(10))
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetPadding(8)
                    .SetBorder(thinBorder);

                tabla.AddCell(empty);
            }

            doc.Add(tabla);
            doc.Close();

            var filename = $"MatrizIndicadores_{userId}_{DateTime.Now:yyyyMMdd_HHmm}.pdf";
            return File(ms.ToArray(), "application/pdf", filename);
        }

        private Cell CeldaEncabezado(string texto, PdfFont font, DeviceRgb color)
        {
            return new Cell()
                .Add(new Paragraph(texto).SetFont(font).SetFontSize(9).SetBold().SetFontColor(ColorConstants.WHITE))
                .SetBackgroundColor(color)
                .SetPadding(6)
                .SetBorder(Border.NO_BORDER);
        }

        private Cell CeldaDato(string texto, PdfFont font)
        {
            return new Cell()
                .Add(new Paragraph(texto ?? "").SetFont(font).SetFontSize(9))
                .SetPadding(6)
                .SetBorder(new SolidBorder(new DeviceRgb(230, 230, 230), 0.5f));
        }

        private Cell CeldaHeaderTabla(string texto, PdfFont font, DeviceRgb color)
        {
            return new Cell()
                .Add(new Paragraph(texto).SetFont(font).SetFontSize(9).SetBold().SetFontColor(ColorConstants.WHITE))
                .SetBackgroundColor(color)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetPadding(6);
        }

        private Cell CeldaContenidoTabla(string texto, PdfFont font)
        {
            var p = new Paragraph(texto ?? "")
                        .SetFont(font)
                        .SetFontSize(9)
                        .SetMultipliedLeading(1.15f);

            return new Cell()
                .Add(p)
                .SetPadding(6)
                .SetBorder(new SolidBorder(new DeviceRgb(230, 230, 230), 0.5f));
        }
    }
}
