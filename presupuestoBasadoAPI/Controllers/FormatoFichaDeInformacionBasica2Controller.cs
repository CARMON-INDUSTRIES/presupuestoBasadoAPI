using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using iText.IO.Font.Constants;
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
    public class FormatoFichaDeInformacionBasica2Controller : ControllerBase
    {
        private readonly AppDbContext _context;

        public FormatoFichaDeInformacionBasica2Controller(AppDbContext context)
        {
            _context = context;
        }

        private string GetUserId() =>
            User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "";

        [HttpGet("ultimo")]
        public IActionResult GenerarPdf()
        {
            try
            {
                var userId = GetUserId();

                // === Recuperar datos ===
                var detalleReglas = _context.ReglasOperacionDetalles
                    .Where(r => r.UserId == userId)
                    .OrderByDescending(r => r.Id)
                    .FirstOrDefault();

                var padron = _context.PadronBeneficiarios
                    .OrderByDescending(p => p.Id)
                    .FirstOrDefault();

                var regla = _context.ReglasOperacion
                    .OrderByDescending(r => r.Id)
                    .FirstOrDefault();

                if (detalleReglas == null && padron == null && regla == null)
                    return NotFound("No se encontraron datos para generar el PDF.");

                // === Crear documento PDF desde cero ===
                using var ms = new MemoryStream();
                using var writer = new PdfWriter(ms);
                using var pdf = new PdfDocument(writer);
                var doc = new Document(pdf, PageSize.LETTER);
                doc.SetMargins(40, 40, 40, 40);

                var font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
                var colorInstitucional = new DeviceRgb(105, 27, 49);

                // === SECCIÓN 6: ¿ES UN PROGRAMA SOCIAL? ===
                doc.Add(SeccionTitulo("6.- ¿ES UN PROGRAMA SOCIAL?", font, colorInstitucional));

                var marcoPrograma = new Table(UnitValue.CreatePercentArray(new float[] { 100 })).UseAllAvailableWidth();
                marcoPrograma.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));

                marcoPrograma.AddCell(CeldaTexto(
                    "Marque según corresponda (Sí / No):", font, true));

                // === Recuadros numerados del 1 al 4 ===
                var tablaRecuadros = new Table(UnitValue.CreatePercentArray(new float[] { 25, 25, 25, 25 }))
                    .UseAllAvailableWidth();

                string[] etiquetas = {
                    "1) Sujetos a Reglas de operación",
                    "2) Otros Subsidios",
                    "3) Prestación de servicios públicos",
                    "4) Provisión de bienes públicos"
                };

                string[] valores = {
                    detalleReglas?.SujetoReglasOperacion ?? "No",
                    detalleReglas?.OtrosSubsidios ?? "No",
                    detalleReglas?.PrestacionServiciosPublicos ?? "No",
                    detalleReglas?.ProvisionBienesPublicos ?? "No"
                };

                for (int i = 0; i < 4; i++)
                {
                    var innerTable = new Table(UnitValue.CreatePercentArray(new float[] { 100 }))
                        .UseAllAvailableWidth();

                    // Número del recuadro
                    innerTable.AddCell(new Cell()
                        .Add(new Paragraph($"{i + 1}")
                        .SetFont(font)
                        .SetBold()
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetFontSize(12))
                        .SetBorder(new SolidBorder(ColorConstants.BLACK, 1))
                        .SetPadding(6)
                        .SetTextAlignment(TextAlignment.CENTER));

                    // Respuesta (Sí/No)
                    innerTable.AddCell(new Cell()
                        .Add(new Paragraph(valores[i])
                        .SetFont(font)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetFontSize(10))
                        .SetBorder(new SolidBorder(ColorConstants.BLACK, 1))
                        .SetPadding(4));

                    // Etiqueta descriptiva
                    innerTable.AddCell(new Cell()
                        .Add(new Paragraph(etiquetas[i])
                        .SetFont(font)
                        .SetFontSize(9)
                        .SetTextAlignment(TextAlignment.CENTER))
                        .SetBorder(Border.NO_BORDER)
                        .SetPadding(3));

                    tablaRecuadros.AddCell(new Cell()
                        .Add(innerTable)
                        .SetBorder(Border.NO_BORDER)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetPadding(4));
                }

                marcoPrograma.AddCell(new Cell().Add(tablaRecuadros).SetBorder(Border.NO_BORDER));
                doc.Add(marcoPrograma);

                // === 6.1 Vinculación a derechos sociales ===
                doc.Add(new Paragraph("\n"));
                doc.Add(SeccionTitulo("6.1 Vinculación a los derechos sociales y dimensión de bienestar económico", font, colorInstitucional));

                var tablaDerechos = new Table(UnitValue.CreatePercentArray(new float[] { 40, 30, 30 })).UseAllAvailableWidth();
                tablaDerechos.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));

                string[] derechos = {
                    "Alimentación","Educación","Salud","Trabajo","Vivienda",
                    "Seguridad Social","No Discriminación","Medio ambiente sano","Bienestar Económico"
                };

                tablaDerechos.AddHeaderCell(CeldaTexto("Derecho", font, true).SetBorderBottom(new SolidBorder(ColorConstants.BLACK, 1)));
                tablaDerechos.AddHeaderCell(CeldaTexto("Directo", font, true).SetBorderBottom(new SolidBorder(ColorConstants.BLACK, 1)));
                tablaDerechos.AddHeaderCell(CeldaTexto("Indirecto", font, true).SetBorderBottom(new SolidBorder(ColorConstants.BLACK, 1)));

                foreach (var d in derechos)
                {
                    tablaDerechos.AddCell(CeldaTexto(d, font));
                    tablaDerechos.AddCell(CeldaTexto("", font));
                    tablaDerechos.AddCell(CeldaTexto("", font));
                }

                doc.Add(tablaDerechos);

                // === SECCIÓN 7: PADRÓN DE BENEFICIARIOS ===
                doc.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
                doc.Add(SeccionTitulo("7.- PADRÓN DE BENEFICIARIOS", font, colorInstitucional));

                var marcoPadron = new Table(UnitValue.CreatePercentArray(new float[] { 100 })).UseAllAvailableWidth();
                marcoPadron.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));

                if (padron != null)
                {
                    if (!string.IsNullOrWhiteSpace(padron.ArchivoAdjunto))
                        marcoPadron.AddCell(CeldaTexto($"Archivo: {padron.ArchivoAdjunto}", font));

                    if (!string.IsNullOrWhiteSpace(padron.LigaInternet))
                        marcoPadron.AddCell(CeldaTexto($"Liga de Internet: {padron.LigaInternet}", font));
                }
                else
                {
                    marcoPadron.AddCell(CeldaTexto("Sin datos de padrón disponibles.", font));
                }

                doc.Add(marcoPadron);

                // === SECCIÓN 8: REGLAS DE OPERACIÓN ===
                doc.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
                doc.Add(SeccionTitulo("8.- REGLAS DE OPERACIÓN", font, colorInstitucional));

                var marcoReglas = new Table(UnitValue.CreatePercentArray(new float[] { 100 })).UseAllAvailableWidth();
                marcoReglas.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));

                if (regla != null)
                {
                    if (!string.IsNullOrWhiteSpace(regla.ArchivoAdjunto))
                        marcoReglas.AddCell(CeldaTexto($"Archivo: {regla.ArchivoAdjunto}", font));

                    if (!string.IsNullOrWhiteSpace(regla.LigaInternet))
                        marcoReglas.AddCell(CeldaTexto($"Liga de Internet: {regla.LigaInternet}", font));
                }
                else
                {
                    marcoReglas.AddCell(CeldaTexto("Sin reglas de operación registradas.", font));
                }

                doc.Add(marcoReglas);

                // === CERRAR DOCUMENTO ===
                doc.Close();

                var fileName = $"FichaBasica2_{DateTime.Now:yyyyMMdd_HHmm}.pdf";
                return File(ms.ToArray(), "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                var errorDetalle = new
                {
                    Mensaje = "Error generando el PDF",
                    Tipo = ex.GetType().FullName,
                    ex.Message,
                    ex.StackTrace,
                    Inner = ex.InnerException?.Message
                };

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("=== ERROR EN PDF ===");
                Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(errorDetalle, new System.Text.Json.JsonSerializerOptions { WriteIndented = true }));
                Console.ResetColor();

                return StatusCode(500, errorDetalle);
            }
        }

        // ---------- Helpers ----------
        private Paragraph SeccionTitulo(string texto, PdfFont font, DeviceRgb color)
        {
            return new Paragraph(texto)
                .SetFont(font)
                .SetFontSize(11)
                .SetBold()
                .SetFontColor(ColorConstants.WHITE)
                .SetBackgroundColor(color)
                .SetPadding(6)
                .SetMarginTop(10)
                .SetMarginBottom(6);
        }

        private Cell CeldaTexto(string texto, PdfFont font, bool bold = false)
        {
            var p = new Paragraph(texto ?? string.Empty)
                .SetFont(font)
                .SetFontSize(10)
                .SetMultipliedLeading(1.15f);
            if (bold) p.SetBold();

            return new Cell().Add(p)
                             .SetBorder(Border.NO_BORDER)
                             .SetPadding(4)
                             .SetTextAlignment(TextAlignment.LEFT);
        }
    }
}
