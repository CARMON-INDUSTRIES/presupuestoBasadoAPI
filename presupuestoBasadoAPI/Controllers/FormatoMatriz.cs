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

        // 🔹 Obtener ruta del emblema según la entidad del usuario
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

            // Obtener usuario autenticado (para encabezado)
            var user = _context.Users
                .Include(u => u.UnidadAdministrativa)
                .FirstOrDefault(u => u.Id == userId);

            if (user == null)
                return NotFound("Usuario no encontrado.");

            // Obtener la última matriz del usuario con sus filas
            var matriz = _context.MatricesIndicadores
                .Include(m => m.Filas)
                .Where(m => m.UserId == userId)
                .OrderByDescending(m => m.Id)
                .FirstOrDefault();

            // Obtener el último árbol de objetivos del mismo usuario (para niveles)
            var arbol = _context.ArbolObjetivos
                .Include(a => a.Componentes)
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.Id)
                .FirstOrDefault();

            if (arbol == null && matriz == null)
                return NotFound("No se encontró ni Árbol de Objetivos ni Matriz de Indicadores para este usuario.");

            var filasGuardadas = matriz?.Filas?.ToList() ?? new List<FilaMatriz>();

            // Crear PDF desde cero
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

            // === Título principal ===
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

            // --- Encabezado con datos del usuario ---
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

            // --- Tabla principal: columnas fijas ---
            var tabla = new Table(UnitValue.CreatePercentArray(new float[] { 12, 36, 16, 18, 18 }))
                .UseAllAvailableWidth()
                .SetBorder(new SolidBorder(colorInstitucional, 1f));

            // Encabezados
            tabla.AddHeaderCell(CeldaHeaderTabla("Nivel", font, colorInstitucional));
            tabla.AddHeaderCell(CeldaHeaderTabla("Resumen Narrativo", font, colorInstitucional));
            tabla.AddHeaderCell(CeldaHeaderTabla("Indicadores (solo nombre)", font, colorInstitucional));
            tabla.AddHeaderCell(CeldaHeaderTabla("Medios de verificación", font, colorInstitucional));
            tabla.AddHeaderCell(CeldaHeaderTabla("Supuestos", font, colorInstitucional));

            // Construir la lista de niveles desde ArbolObjetivos
            var niveles = new List<string>();
            if (arbol != null)
            {
                niveles.Add($"Fin: ");
                niveles.Add($"Propósito: ");

                if (arbol.Componentes != null)
                {
                    foreach (var comp in arbol.Componentes)
                    {
                        niveles.Add($"Componente: ");
                        if (comp.Medios != null)
                        {
                            foreach (var medio in comp.Medios)
                            {
                                niveles.Add($"Actividad: ");
                            }
                        }
                    }
                }
            }
            else
            {
                niveles.AddRange(filasGuardadas.Select(f => f.Nivel).Where(n => !string.IsNullOrWhiteSpace(n)));
            }

            // Para cada nivel, buscar la fila guardada
            foreach (var nivelTexto in niveles)
            {
                var fila = BuscarFilaPorNivel(nivelTexto, filasGuardadas);

                tabla.AddCell(CeldaContenidoTabla(nivelTexto, font));

                if (fila != null)
                {
                    tabla.AddCell(CeldaContenidoTabla(fila.ResumenNarrativo, font));
                    tabla.AddCell(CeldaContenidoTabla(fila.Indicadores, font));
                    tabla.AddCell(CeldaContenidoTabla(fila.Medios, font));
                    tabla.AddCell(CeldaContenidoTabla(fila.Supuestos, font));
                }
                else
                {
                    tabla.AddCell(CeldaContenidoTabla(string.Empty, font));
                    tabla.AddCell(CeldaContenidoTabla(string.Empty, font));
                    tabla.AddCell(CeldaContenidoTabla(string.Empty, font));
                    tabla.AddCell(CeldaContenidoTabla(string.Empty, font));
                }
            }

            if (!niveles.Any())
            {
                var empty = new Cell(1, 5)
                    .Add(new Paragraph("No hay información disponible para la Matriz.").SetFont(font).SetFontSize(10))
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

        // ---------- Helpers de celdas ----------
        private FilaMatriz? BuscarFilaPorNivel(string nivelTexto, List<FilaMatriz> filas)
        {
            if (filas == null || !filas.Any()) return null;
            var exact = filas.FirstOrDefault(f => string.Equals((f.Nivel ?? "").Trim(), nivelTexto.Trim(), StringComparison.OrdinalIgnoreCase));
            if (exact != null) return exact;

            var idx = nivelTexto.IndexOf(':');
            if (idx >= 0)
            {
                var prefix = nivelTexto.Substring(0, idx + 1).Trim();
                var byPrefix = filas.FirstOrDefault(f => (f.Nivel ?? "").TrimStart().StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
                if (byPrefix != null) return byPrefix;
            }

            var contains = filas.FirstOrDefault(f => (f.Nivel ?? "").IndexOf(nivelTexto, StringComparison.OrdinalIgnoreCase) >= 0);
            if (contains != null) return contains;

            var label = idx >= 0 ? nivelTexto.Substring(0, idx).Trim() : nivelTexto.Trim();
            return filas.FirstOrDefault(f => (f.Nivel ?? "").StartsWith(label, StringComparison.OrdinalIgnoreCase));
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
