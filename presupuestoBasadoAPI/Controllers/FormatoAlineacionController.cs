using iText.Kernel.Pdf;
using iText.Kernel.Font;
using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Layout.Borders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using presupuestoBasadoAPI.Models;
using System.IO;
using System;
using System.Linq;
using iText.Kernel.Colors;

namespace presupuestoBasadoAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FormatoAlineacionController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly string _imagesFolder = Path.Combine(
            Directory.GetCurrentDirectory(), "Images");

        public FormatoAlineacionController(AppDbContext context)
        {
            _context = context;
        }

        private string GetUserId()
        {
            return User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "";
        }

        [HttpGet("ultimo")]
        public IActionResult GenerarPdf()
        {
            var userId = GetUserId();

            var usuario = _context.Users
                .Include(u => u.UnidadAdministrativa)
                .Include(u => u.Entidad) // 🔹 Importante: incluir la entidad
                .FirstOrDefault(u => u.Id == userId);

            var alineacionMunicipio = _context.AlineacionesMunicipio
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.Id)
                .FirstOrDefault();

            var alineacionEstado = _context.AlineacionesEstado
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.Id)
                .FirstOrDefault();

            var clasificacion = _context.ClasificacionesFuncionales
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.Id)
                .FirstOrDefault();

            if (usuario == null || (alineacionMunicipio == null && alineacionEstado == null) || clasificacion == null)
                return NotFound("Faltan registros necesarios para generar el PDF");

            // === Determinar emblema según la entidad ===
            string entidadNombre = usuario.Entidad?.Nombre?.ToLower() ?? "";
            string emblemaPath = Path.Combine(_imagesFolder, "emblema.png"); // valor por defecto

            if (!string.IsNullOrEmpty(entidadNombre))
            {
                string posibleEmblema = Path.Combine(_imagesFolder, $"{entidadNombre}.png");
                if (System.IO.File.Exists(posibleEmblema))
                    emblemaPath = posibleEmblema;
            }

            using var ms = new MemoryStream();
            var writer = new PdfWriter(ms);
            var pdf = new PdfDocument(writer);
            var doc = new Document(pdf);
            var font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
            doc.SetMargins(30, 40, 30, 40);

            var colorInstitucional = new DeviceRgb(105, 27, 49);

            // === ENCABEZADO ===
            var encabezadoTabla = new Table(UnitValue.CreatePercentArray(new float[] { 75, 25 }))
                .UseAllAvailableWidth()
                .SetBorder(Border.NO_BORDER);

            var encabezadoTexto = new Paragraph()
                .Add(new Text("Formato 1\n").SetFont(font).SetBold().SetFontSize(14))
                .Add(new Text("Alineación de la Matriz de Indicadores\nPara Resultados").SetFont(font).SetFontSize(11))
                .SetTextAlignment(TextAlignment.LEFT);

            encabezadoTabla.AddCell(new Cell().Add(encabezadoTexto)
                .SetBorder(Border.NO_BORDER)
                .SetPaddingLeft(10)
                .SetPaddingTop(5));

            // === Emblema dinámico según entidad ===
            if (System.IO.File.Exists(emblemaPath))
            {
                try
                {
                    var emblema = new Image(ImageDataFactory.Create(emblemaPath))
                        .SetWidth(85)
                        .SetAutoScale(true)
                        .SetHorizontalAlignment(HorizontalAlignment.RIGHT);

                    encabezadoTabla.AddCell(new Cell()
                        .Add(emblema)
                        .SetBorder(Border.NO_BORDER)
                        .SetTextAlignment(TextAlignment.RIGHT)
                        .SetPaddingRight(10));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[PDF] Error al cargar emblema: {ex.Message}");
                    encabezadoTabla.AddCell(new Cell().SetBorder(Border.NO_BORDER));
                }
            }
            else
            {
                Console.WriteLine($"[PDF] No se encontró el emblema: {emblemaPath}");
                encabezadoTabla.AddCell(new Cell().SetBorder(Border.NO_BORDER));
            }

            doc.Add(encabezadoTabla);
            doc.Add(new Paragraph("\n"));

            // === 1. ANTECEDENTES ===
            doc.Add(SeccionTitulo("1.- ANTECEDENTES", font, colorInstitucional));
            var antecedentes = new Table(UnitValue.CreatePercentArray(new float[] { 30, 70 })).UseAllAvailableWidth();
            antecedentes.AddCell(CeldaEtiqueta("Unidad responsable", font));
            antecedentes.AddCell(CeldaDato(usuario.UnidadAdministrativa?.Unidad ?? "", font));
            antecedentes.AddCell(CeldaEtiqueta("Programa presupuestario", font));
            antecedentes.AddCell(CeldaDato(usuario.ProgramaPresupuestario ?? "", font));
            antecedentes.AddCell(CeldaEtiqueta("Entidad", font));
            antecedentes.AddCell(CeldaDato(usuario.Entidad?.Nombre ?? "", font));
            doc.Add(antecedentes);

            // === 2. ALINEACIÓN ESTATAL (si existe) ===
            if (alineacionEstado != null)
            {
                doc.Add(SeccionTitulo("ALINEACIÓN ESTATAL AL PLAN ESTATAL DE DESARROLLO", font, colorInstitucional));

                var tablaEstado = new Table(UnitValue.CreatePercentArray(new float[] { 30, 70 })).UseAllAvailableWidth();
                tablaEstado.AddCell(CeldaEtiqueta("Tipo de Alineación", font));
                tablaEstado.AddCell(CeldaDato("Estatal", font));
                tablaEstado.AddCell(CeldaEtiqueta("Acuerdo", font));
                tablaEstado.AddCell(CeldaDato(alineacionEstado.Acuerdo ?? "", font));
                tablaEstado.AddCell(CeldaEtiqueta("Objetivo", font));
                tablaEstado.AddCell(CeldaDato(alineacionEstado.Objetivo ?? "", font));
                tablaEstado.AddCell(CeldaEtiqueta("Estrategia", font));
                tablaEstado.AddCell(CeldaDato(alineacionEstado.Estrategia ?? "", font));
                tablaEstado.AddCell(CeldaEtiqueta("Línea de Acción", font));
                tablaEstado.AddCell(CeldaDato(alineacionEstado.LineaAccion ?? "", font));
                doc.Add(tablaEstado);
            }

            // === 3. ALINEACIÓN MUNICIPAL (si existe) ===
            if (alineacionMunicipio != null)
            {
                doc.Add(SeccionTitulo("ALINEACIÓN MUNICIPAL AL PLAN MUNICIPAL DE DESARROLLO", font, colorInstitucional));

                var tablaMunicipio = new Table(UnitValue.CreatePercentArray(new float[] { 30, 70 })).UseAllAvailableWidth();
                tablaMunicipio.AddCell(CeldaEtiqueta("Tipo de Alineación", font));
                tablaMunicipio.AddCell(CeldaDato("Municipal", font));
                tablaMunicipio.AddCell(CeldaEtiqueta("Acuerdo", font));
                tablaMunicipio.AddCell(CeldaDato(alineacionMunicipio.Acuerdo ?? "", font));
                tablaMunicipio.AddCell(CeldaEtiqueta("Objetivo", font));
                tablaMunicipio.AddCell(CeldaDato(alineacionMunicipio.Objetivo ?? "", font));
                tablaMunicipio.AddCell(CeldaEtiqueta("Estrategia", font));
                tablaMunicipio.AddCell(CeldaDato(alineacionMunicipio.Estrategia ?? "", font));
                tablaMunicipio.AddCell(CeldaEtiqueta("Línea de Acción", font));
                tablaMunicipio.AddCell(CeldaDato(alineacionMunicipio.LineaAccion ?? "", font));
                tablaMunicipio.AddCell(CeldaEtiqueta("Ramo", font));
                tablaMunicipio.AddCell(CeldaDato(alineacionMunicipio.Ramo ?? "", font));
                doc.Add(tablaMunicipio);
            }

            // === 4. CLASIFICACIÓN FUNCIONAL ===
            doc.Add(SeccionTitulo("CLASIFICACIÓN FUNCIONAL", font, colorInstitucional));
            var clasif = new Table(UnitValue.CreatePercentArray(new float[] { 30, 70 })).UseAllAvailableWidth();
            clasif.AddCell(CeldaEtiqueta("Finalidad", font));
            clasif.AddCell(CeldaDato(clasificacion.Finalidad ?? "", font));
            clasif.AddCell(CeldaEtiqueta("Función", font));
            clasif.AddCell(CeldaDato(clasificacion.Funcion ?? "", font));
            clasif.AddCell(CeldaEtiqueta("Subfunción", font));
            clasif.AddCell(CeldaDato(clasificacion.Subfuncion ?? "", font));
            doc.Add(clasif);

            // === 5. OTROS DATOS ===
            doc.Add(SeccionTitulo("OTROS DATOS", font, colorInstitucional));
            var otros = new Table(UnitValue.CreatePercentArray(new float[] { 30, 70 })).UseAllAvailableWidth();
            otros.AddCell(CeldaEtiqueta("Año operando", font));
            otros.AddCell(CeldaDato(clasificacion.AnioOperando.ToString(), font));
            doc.Add(otros);

            // === ENTREGA DE BIENES Y SERVICIOS ===
            string entregaBienes = clasificacion.EntregaBienes ?? "";
            var entrega = new Paragraph()
                .Add(new Text("El programa presupuestario entrega bienes y servicios a: ")
                    .SetFont(font).SetFontSize(10))
                .Add(new Text(entregaBienes)
                    .SetFont(font).SetFontSize(10).SetBold().SetUnderline());
            doc.Add(entrega);

            // === FIRMAS ===
            var firmas = new Table(UnitValue.CreatePercentArray(new float[] { 50, 50 })).UseAllAvailableWidth();
            firmas.AddCell(CeldaFirma($"\n\n\n\n(Nombre y firma)\n{usuario.NombreCompleto ?? usuario.UserName ?? "Responsable de la MIR"}", font));
            firmas.AddCell(CeldaFirma("\n\n\n\n(Nombre y firma)\nM.R.H. Daniela López Hernández", font));
            doc.Add(firmas);

            doc.Close();
            return File(ms.ToArray(), "application/pdf", "FormatoAlineacion.pdf");
        }

        // ======= MÉTODOS AUXILIARES =======
        private static Cell CeldaEtiqueta(string texto, PdfFont font) =>
            new Cell().Add(new Paragraph(texto).SetFont(font).SetBold().SetFontSize(10))
                      .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
                      .SetBorder(new SolidBorder(0.5f))
                      .SetPadding(5);

        private static Cell CeldaDato(string texto, PdfFont font) =>
            new Cell().Add(new Paragraph(texto).SetFont(font).SetFontSize(10))
                      .SetBorder(new SolidBorder(0.5f))
                      .SetPadding(5);

        private static Cell CeldaFirma(string texto, PdfFont font) =>
            new Cell().Add(new Paragraph(texto)
                .SetFont(font)
                .SetFontSize(10)
                .SetTextAlignment(TextAlignment.CENTER))
                .SetBorder(Border.NO_BORDER);

        private static Paragraph SeccionTitulo(string texto, PdfFont font, DeviceRgb color) =>
            new Paragraph(texto)
                .SetFont(font)
                .SetBold()
                .SetBackgroundColor(color)
                .SetFontColor(ColorConstants.WHITE)
                .SetPadding(5)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetMarginTop(10);
    }
}
