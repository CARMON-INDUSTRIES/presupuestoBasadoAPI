using System;
using System.IO;
using System.Linq;
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
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using presupuestoBasadoAPI.Models;

namespace presupuestoBasadoAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FormatoEstructuraAnaliticaController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly string _imagesPath;

        public FormatoEstructuraAnaliticaController(AppDbContext context)
        {
            _context = context;
            _imagesPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Images");
        }

        // Obtener UserId del token
        private string GetUserId() =>
            User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "";

        // Obtener ruta del emblema según la entidad del usuario
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

            // === Cargar datos del usuario ===
            var problema = _context.IdentificacionDescripcionProblemas
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.Id)
                .FirstOrDefault();

            var objetivo = _context.ArbolObjetivos
                .Include(o => o.Componentes)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.Id)
                .FirstOrDefault();

            var efectoSuperior = _context.EfectosSuperiores
                .Where(e => e.UserId == userId)
                .OrderByDescending(e => e.Id)
                .FirstOrDefault();

            var diseno = _context.DisenoIntervencionPublicas
                .Include(d => d.Componentes)
                    .ThenInclude(c => c.Resultado)
                .Where(d => d.UserId == userId)
                .OrderByDescending(d => d.Id)
                .FirstOrDefault();

            var cobertura = _context.Coberturas
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.Id)
                .FirstOrDefault();

            if (problema == null && objetivo == null && efectoSuperior == null && diseno == null && cobertura == null)
                return NotFound("No se encontraron datos para generar el formato.");

            // === Crear PDF ===
            using var ms = new MemoryStream();
            var writer = new PdfWriter(ms);
            var pdf = new PdfDocument(writer);
            var pageSize = PageSize.LETTER;
            var doc = new Document(pdf, pageSize);
            doc.SetMargins(30, 30, 30, 30);

            var font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
            var colorInstitucional = new DeviceRgb(105, 27, 49);
            var thinBorder = new SolidBorder(new DeviceRgb(120, 120, 120), 0.5f);

            float pageWidth = pageSize.GetWidth();
            float pageHeight = pageSize.GetHeight();

            // === Título ===
            var titulo = new Paragraph()
                .Add(new Text("Anexo 7\n").SetFont(font).SetBold().SetFontSize(14))
                .Add(new Text("Estructura analítica del Programa Presupuestario")
                    .SetFont(font).SetFontSize(11))
                .SetFixedPosition(36, pageHeight - 60, pageWidth - 72);
            doc.Add(titulo);

            // === Emblema dinámico ===
            if (System.IO.File.Exists(emblemaPath))
            {
                var emblema = new Image(ImageDataFactory.Create(emblemaPath))
                    .SetWidth(60)
                    .SetFixedPosition(pageWidth - 50 - 50, pageHeight - 70);
                doc.Add(emblema);
            }

            doc.Add(new Paragraph("\n\n\n\n\n\n"));

            // === Tabla Problemática / Solución ===
            var tablaTopo = new Table(UnitValue.CreatePercentArray(new float[] { 50, 50 })).UseAllAvailableWidth();
            tablaTopo.SetBorder(new SolidBorder(ColorConstants.BLACK, 0.5f));

            tablaTopo.AddHeaderCell(EncabezadoColumna(
                "PROBLEMÁTICA CENTRAL (PROVIENE DEL ÁRBOL DEL PROBLEMA)", font, colorInstitucional));
            tablaTopo.AddHeaderCell(EncabezadoColumna(
                "SOLUCIÓN (PROVIENE DEL ÁRBOL DEL OBJETIVO)", font, colorInstitucional));

            var cellIzq = new Cell()
                .Add(new Paragraph(problema?.ProblemaCentral ?? "").SetFont(font).SetFontSize(10))
                .SetBorder(thinBorder)
                .SetPadding(8)
                .SetHeight(120)
                .SetVerticalAlignment(VerticalAlignment.TOP);

            var cellDer = new Cell()
                .Add(new Paragraph(objetivo?.ObjetivoCentral ?? "").SetFont(font).SetFontSize(10))
                .SetBorder(thinBorder)
                .SetPadding(8)
                .SetHeight(120)
                .SetVerticalAlignment(VerticalAlignment.TOP);

            tablaTopo.AddCell(cellIzq);
            tablaTopo.AddCell(cellDer);
            doc.Add(tablaTopo);

            // === EFECTOS / FINES ===
            doc.Add(new Paragraph("\n"));
            var tablaEF = new Table(UnitValue.CreatePercentArray(new float[] { 50, 50 })).UseAllAvailableWidth();
            tablaEF.AddHeaderCell(EncabezadoColumna("EFECTOS", font, colorInstitucional));
            tablaEF.AddHeaderCell(EncabezadoColumna("FINES", font, colorInstitucional));

            var efectos = new System.Collections.Generic.List<string>();
            if (diseno?.Componentes != null)
                foreach (var c in diseno.Componentes)
                    if (c?.Resultado?.Descripcion != null)
                        efectos.Add(c.Resultado.Descripcion);

            var fines = new System.Collections.Generic.List<string>();
            if (!string.IsNullOrWhiteSpace(efectoSuperior?.Descripcion))
                fines.Add(efectoSuperior.Descripcion);

            if (objetivo?.Componentes != null)
                foreach (var comp in objetivo.Componentes)
                    if (comp?.Resultados != null)
                        foreach (var r in comp.Resultados)
                            if (!string.IsNullOrWhiteSpace(r))
                                fines.Add(r);

            int filasEF = Math.Max(Math.Max(efectos.Count, fines.Count), 6);
            for (int i = 0; i < filasEF; i++)
            {
                string txtE = i < efectos.Count ? efectos[i] : "";
                string txtF = i < fines.Count ? fines[i] : "";

                tablaEF.AddCell(new Cell().Add(new Paragraph(txtE).SetFont(font).SetFontSize(10))
                    .SetBorder(thinBorder).SetPadding(6));
                tablaEF.AddCell(new Cell().Add(new Paragraph(txtF).SetFont(font).SetFontSize(10))
                    .SetBorder(thinBorder).SetPadding(6));
            }
            doc.Add(tablaEF);

            // === MAGNITUD ===
            doc.Add(new Paragraph("\n"));
            var tablaMagnitud = new Table(UnitValue.CreatePercentArray(new float[] { 50, 50 })).UseAllAvailableWidth();
            tablaMagnitud.AddHeaderCell(EncabezadoColumna("MAGNITUD (LÍNEA BASE)", font, colorInstitucional));
            tablaMagnitud.AddHeaderCell(EncabezadoColumna("MAGNITUD (RESULTADO ESPERADO)", font, colorInstitucional));

            var magIzq = cobertura?.CuantificacionPoblacionAtendidaAnterior.ToString() ?? "";
            var magDer = cobertura?.CuantificacionPoblacionObjetivo.ToString() ?? "";

            tablaMagnitud.AddCell(new Cell().Add(new Paragraph(magIzq).SetFont(font))
                .SetBorder(thinBorder).SetPadding(8));
            tablaMagnitud.AddCell(new Cell().Add(new Paragraph(magDer).SetFont(font))
                .SetBorder(thinBorder).SetPadding(8));

            doc.Add(tablaMagnitud);

            // === CAUSAS / MEDIOS ===
            doc.Add(new Paragraph("\n"));
            var tablaCM = new Table(UnitValue.CreatePercentArray(new float[] { 50, 50 })).UseAllAvailableWidth();
            tablaCM.AddHeaderCell(EncabezadoColumna("CAUSAS", font, colorInstitucional));
            tablaCM.AddHeaderCell(EncabezadoColumna("MEDIOS", font, colorInstitucional));

            var causas = new[] {
                problema?.CausaBeneficiados ?? "",
                problema?.CausaOpositores ?? "",
                problema?.CausaEjecutores ?? "",
                problema?.CausaIndiferentes ?? ""
            };

            var medios = new System.Collections.Generic.List<string>();
            if (objetivo?.Componentes != null)
                foreach (var comp in objetivo.Componentes)
                    if (comp?.Medios != null)
                        foreach (var m in comp.Medios)
                            if (!string.IsNullOrWhiteSpace(m))
                                medios.Add(m);

            int filasCM = Math.Max(Math.Max(causas.Length, medios.Count), 6);
            for (int i = 0; i < filasCM; i++)
            {
                string txtC = i < causas.Length ? causas[i] : "";
                string txtM = i < medios.Count ? medios[i] : "";

                tablaCM.AddCell(new Cell().Add(new Paragraph(txtC).SetFont(font).SetFontSize(10))
                    .SetBorder(thinBorder).SetPadding(6));
                tablaCM.AddCell(new Cell().Add(new Paragraph(txtM).SetFont(font).SetFontSize(10))
                    .SetBorder(thinBorder).SetPadding(6));
            }
            doc.Add(tablaCM);

            // === Finalizar PDF ===
            doc.Close();

            var filename = $"EstructuraAnalitica_{userId}_{DateTime.Now:yyyyMMdd_HHmm}.pdf";
            return File(ms.ToArray(), "application/pdf", filename);
        }

        // === Helper para encabezado de tabla ===
        private Cell EncabezadoColumna(string texto, PdfFont font, DeviceRgb color)
        {
            return new Cell()
                .Add(new Paragraph(texto)
                    .SetFont(font)
                    .SetFontSize(9)
                    .SetBold()
                    .SetTextAlignment(TextAlignment.CENTER))
                .SetBackgroundColor(color)
                .SetFontColor(ColorConstants.WHITE)
                .SetPadding(6)
                .SetBorder(new SolidBorder(new DeviceRgb(80, 80, 80), 0.5f));
        }
    }
}
