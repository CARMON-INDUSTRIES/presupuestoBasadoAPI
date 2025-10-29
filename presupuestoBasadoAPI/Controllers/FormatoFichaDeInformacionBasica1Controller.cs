using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
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
    public class FormatoFichaDeInformacionBasica1Controller : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly string _imagesPath;


        public FormatoFichaDeInformacionBasica1Controller(AppDbContext context)
        {
            _context = context;
            _imagesPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Images");

        }
        private readonly string _emblemaPath = System.IO.Path.Combine(
            System.IO.Directory.GetCurrentDirectory(), "Images", "emblema.png");


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


            var usuario = _context.Users
                .Include(u => u.UnidadAdministrativa)
                .FirstOrDefault(u => u.Id == userId);

            // === Recuperar datos ===
            var antecedente = _context.Antecedentes
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.Id)
                .FirstOrDefault();

            var problema = _context.IdentificacionDescripcionProblemas
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.Id)
                .FirstOrDefault();

            var determinacion = _context.DeterminacionJustificacionObjetivo
                .Where(d => d.UserId == userId)
                .OrderByDescending(d => d.Id)
                .FirstOrDefault();

            var cobertura = _context.Coberturas
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.Id)
                .FirstOrDefault();

            var diseno = _context.DisenoIntervencionPublicas
                .Include(d => d.Componentes)
                    .ThenInclude(c => c.Acciones)
                .Include(d => d.Componentes)
                    .ThenInclude(c => c.Resultado)
                .Where(d => d.UserId == userId)
                .OrderByDescending(d => d.Id)
                .FirstOrDefault();

            var detalleReglas = _context.ReglasOperacionDetalles
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.Id)
                .FirstOrDefault();

            var programaSocial = _context.ProgramaSocial
                .Include(p => p.Categorias)
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.Id)
                .FirstOrDefault();

            var padron = _context.PadronBeneficiarios
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.Id)
                .FirstOrDefault();

            var regla = _context.ReglasOperacion
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.Id)
                .FirstOrDefault();

            if (antecedente == null && problema == null && determinacion == null && cobertura == null && diseno == null &&
                detalleReglas == null && programaSocial == null && padron == null && regla == null)
            {
                return NotFound("No se encontraron registros para generar la ficha completa.");
            }

            using var ms = new MemoryStream();
            var writer = new PdfWriter(ms);
            var pdf = new PdfDocument(writer);
            var pageSize = PageSize.LETTER;
            var doc = new Document(pdf, pageSize);
            doc.SetMargins(40, 40, 40, 40);

            var font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
            var colorInstitucional = new DeviceRgb(105, 27, 49);

            // --- ENCABEZADO ---
            var firstPage = pdf.AddNewPage(pageSize);
            var canvas = new PdfCanvas(firstPage);

            float pageWidth = pageSize.GetWidth();
            float pageHeight = pageSize.GetHeight();
            float headerX = 50;
            float headerY = pageHeight - 90;
            float headerW = pageWidth - 2 * headerX;

            

            // === ENCABEZADO COMO TABLA PARA ALINEAR TEXTO Y LOGO ===
            var encabezadoTabla = new Table(UnitValue.CreatePercentArray(new float[] { 75, 25 }))
                .UseAllAvailableWidth()
                .SetFixedPosition(headerX, headerY - 60, headerW)
                .SetBorder(Border.NO_BORDER)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE);

            // --- Columna 1: Texto del encabezado ---
            var textoEncabezado = new Paragraph()
                .Add(new Text("Anexo 1\n")
                    .SetFont(font)
                    .SetBold()
                    .SetFontSize(14))
                .Add(new Text("Ficha de Información Básica del Programa Presupuestario")
                    .SetFont(font)
                    .SetFontSize(11))
                .SetMargin(0)
                .SetTextAlignment(TextAlignment.LEFT);

            encabezadoTabla.AddCell(new Cell()
                .Add(textoEncabezado)
                .SetBorder(Border.NO_BORDER)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                .SetPaddingLeft(10)
                .SetPaddingTop(5));

            // --- Columna 2: Imagen del emblema ---
            if (System.IO.File.Exists(emblemaPath))
            {
                var emblema = new Image(ImageDataFactory.Create(emblemaPath))
                    .SetWidth(85)
                    .SetAutoScale(true)
                    .SetFixedPosition(pageWidth - 100 - 85, pageHeight - 150);
                doc.Add(emblema);
            }

            // Agregar tabla de encabezado al documento
            doc.Add(encabezadoTabla);



            doc.Add(new Paragraph("\n\n\n\n\n\n\n"));

            // === SECCIÓN 1: ANTECEDENTES ===
            doc.Add(SeccionTitulo("1.- ANTECEDENTES", font, colorInstitucional));
            var tablaAntecedentes = new Table(UnitValue.CreatePercentArray(new float[] { 100 })).UseAllAvailableWidth();
            tablaAntecedentes.AddCell(CeldaAreaLibre(antecedente?.DescripcionPrograma ?? string.Empty, font));
            tablaAntecedentes.AddCell(CeldaAreaLibre(antecedente?.ContextoHistoricoNormativo ?? string.Empty, font));
            tablaAntecedentes.AddCell(CeldaAreaLibre(antecedente?.ProblematicaOrigen ?? string.Empty, font));
            tablaAntecedentes.AddCell(CeldaAreaLibre(antecedente?.ExperienciasPrevias ?? string.Empty, font));
            doc.Add(tablaAntecedentes);

            // === SECCIÓN 2 ===
            doc.Add(SeccionTitulo("2.- IDENTIFICACIÓN Y DESCRIPCIÓN DEL PROBLEMA", font, colorInstitucional));
            var tablaProblema = new Table(UnitValue.CreatePercentArray(new float[] { 100 })).UseAllAvailableWidth();
            tablaProblema.AddCell(CeldaAreaLibre(problema?.ProblemaCentral ?? string.Empty, font));
            tablaProblema.AddCell(CeldaAreaLibre(problema?.CausaBeneficiados ?? string.Empty, font));
            doc.Add(tablaProblema);

            var causasRow = new Table(UnitValue.CreatePercentArray(new float[] { 25, 25, 25, 25 })).UseAllAvailableWidth();
            causasRow.AddCell(CeldaAreaLibre(problema?.CausaBeneficiados ?? string.Empty, font));
            causasRow.AddCell(CeldaAreaLibre(problema?.CausaOpositores ?? string.Empty, font));
            causasRow.AddCell(CeldaAreaLibre(problema?.CausaEjecutores ?? string.Empty, font));
            causasRow.AddCell(CeldaAreaLibre(problema?.CausaIndiferentes ?? string.Empty, font));
            doc.Add(causasRow);

            tablaProblema.AddCell(CeldaAreaLibre(problema?.Efectos ?? string.Empty, font));
            tablaProblema.AddCell(CeldaAreaLibre(problema?.Evolucion ?? string.Empty, font));
            tablaProblema.AddCell(CeldaAreaLibre(problema?.Involucrados ?? string.Empty, font));
            doc.Add(tablaProblema);

            // === SECCIÓN 3 ===
            doc.Add(SeccionTitulo("3.- DETERMINACIÓN Y JUSTIFICACIÓN DE LOS OBJETIVOS DE LA INTERVENCIÓN", font, colorInstitucional));
            var tablaDet = new Table(UnitValue.CreatePercentArray(new float[] { 100 })).UseAllAvailableWidth();
            tablaDet.AddCell(CeldaAreaLibre(determinacion?.ObjetivosEspecificos ?? string.Empty, font));
            tablaDet.AddCell(CeldaAreaLibre(determinacion?.RelacionOtrosProgramas ?? string.Empty, font));
            doc.Add(tablaDet);

            // === SECCIÓN 4 ===
            doc.Add(SeccionTitulo("4.- COBERTURA", font, colorInstitucional));
            var tablaCobertura = new Table(UnitValue.CreatePercentArray(new float[] { 100 })).UseAllAvailableWidth();
            tablaCobertura.AddCell(CeldaAreaLibre(cobertura?.IdentificacionCaracterizacionPoblacionPotencial ?? string.Empty, font));
            tablaCobertura.AddCell(CeldaAreaLibre(cobertura?.IdentificacionCaracterizacionPoblacionObjetivo ?? string.Empty, font));
            tablaCobertura.AddCell(CeldaAreaLibre(cobertura?.UnidadMedida ?? string.Empty, font));
            doc.Add(tablaCobertura);

            var tablaMagnitud = new Table(UnitValue.CreatePercentArray(new float[] { 33, 33, 34 })).UseAllAvailableWidth();
            tablaMagnitud.AddHeaderCell(CeldaEncabezado("POBLACIÓN POTENCIAL", font, colorInstitucional));
            tablaMagnitud.AddHeaderCell(CeldaEncabezado("POBLACIÓN OBJETIVO", font, colorInstitucional));
            tablaMagnitud.AddHeaderCell(CeldaEncabezado("POBLACIÓN ATENDIDA DEL EJERCICIO FISCAL ANTERIOR", font, colorInstitucional));
            tablaMagnitud.AddCell(CeldaAreaLibre(cobertura?.CuantificacionPoblacionPotencial.ToString() ?? string.Empty, font));
            tablaMagnitud.AddCell(CeldaAreaLibre(cobertura?.CuantificacionPoblacionObjetivo.ToString() ?? string.Empty, font));
            tablaMagnitud.AddCell(CeldaAreaLibre(cobertura?.CuantificacionPoblacionAtendidaAnterior.ToString() ?? string.Empty, font));
            tablaMagnitud.AddCell(new Cell(1, 3).Add(CeldaAreaLibre(cobertura?.FrecuenciaActualizacion ?? string.Empty, font)).SetBorder(new SolidBorder(colorInstitucional, 1)));
            doc.Add(tablaMagnitud);

            // === SECCIÓN 5 ===
            doc.Add(SeccionTitulo("5.- DISEÑO DE LA INTERVENCIÓN PÚBLICA", font, colorInstitucional));
            if (diseno != null)
            {
                doc.Add(CeldaAreaLibre(diseno.EtapasIntervencion ?? string.Empty, font));
                doc.Add(CeldaAreaLibre(diseno.EscenariosFuturosEsperar ?? string.Empty, font));
                foreach (var comp in diseno.Componentes ?? Enumerable.Empty<dynamic>())
                {
                    doc.Add(new Paragraph($"Componente: {comp.Nombre}").SetFont(font).SetFontSize(10).SetBold().SetMarginTop(6));
                    foreach (var accion in comp.Acciones ?? Enumerable.Empty<dynamic>())
                    {
                        doc.Add(new Paragraph($"• {accion.Descripcion} (Cantidad: {accion.Cantidad})")
                            .SetFont(font).SetFontSize(9).SetMarginLeft(12));
                    }
                    if (comp.Resultado != null)
                    {
                        doc.Add(new Paragraph($"Resultado: {comp.Resultado.Descripcion}")
                            .SetFont(font).SetFontSize(9).SetMarginLeft(12).SetItalic());
                    }
                }
            }

            // === SECCIÓN 6: ¿ES UN PROGRAMA SOCIAL? ===
            // === SECCIÓN 6: ¿ES UN PROGRAMA SOCIAL? ===
            doc.Add(SeccionTitulo("6.- ¿ES UN PROGRAMA SOCIAL?", font, colorInstitucional));

            var marcoPrograma = new Table(UnitValue.CreatePercentArray(new float[] { 100 })).UseAllAvailableWidth();
            marcoPrograma.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));
            marcoPrograma.AddCell(CeldaTexto("Marque según corresponda (Sí / No):", font, true));

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
                var inner = new Table(UnitValue.CreatePercentArray(new float[] { 100 })).UseAllAvailableWidth();

                inner.AddCell(new Cell()
                    .Add(new Paragraph($"{i + 1}")
                    .SetFont(font).SetBold().SetTextAlignment(TextAlignment.CENTER))
                    .SetBorder(new SolidBorder(ColorConstants.BLACK, 1)));

                inner.AddCell(new Cell()
                    .Add(new Paragraph(valores[i])
                    .SetFont(font).SetTextAlignment(TextAlignment.CENTER))
                    .SetBorder(new SolidBorder(ColorConstants.BLACK, 1)));

                inner.AddCell(new Cell()
                    .Add(new Paragraph(etiquetas[i])
                    .SetFont(font).SetFontSize(9).SetTextAlignment(TextAlignment.CENTER))
                    .SetBorder(Border.NO_BORDER));

                tablaRecuadros.AddCell(new Cell().Add(inner).SetBorder(Border.NO_BORDER));
            }

            marcoPrograma.AddCell(new Cell().Add(tablaRecuadros).SetBorder(Border.NO_BORDER));
            doc.Add(marcoPrograma);

            // === 6.1 Vinculación a derechos sociales ===
            doc.Add(SeccionTitulo("6.1 Vinculación a los derechos sociales y dimensión de bienestar económico", font, colorInstitucional));

            if (programaSocial?.Categorias != null && programaSocial.Categorias.Count > 0)
            {
                var tabla = new Table(UnitValue.CreatePercentArray(new float[] { 60, 40 })).UseAllAvailableWidth();
                tabla.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));
                tabla.AddHeaderCell(CeldaEncabezado("Derecho Social", font, colorInstitucional));
                tabla.AddHeaderCell(CeldaEncabezado("Tipo", font, colorInstitucional));

                foreach (var cat in programaSocial.Categorias)
                {
                    tabla.AddCell(CeldaTexto(cat.Nombre, font));
                    tabla.AddCell(CeldaTexto(cat.Tipo, font));
                }
                doc.Add(tabla);
            }
            else
            {
                doc.Add(CeldaTexto("Sin vinculación registrada.", font));
            }

            // === SECCIÓN 7 ===
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

            // === SECCIÓN 8 ===
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


            // === CERRAR ===
            doc.Close();
            var usuarioNombre = usuario?.UserName ?? usuario?.NombreCompleto ?? "usuario";
            var fileName = $"FichaBasicaCompleta_{usuarioNombre}_{DateTime.Now:yyyyMMdd_HHmm}.pdf";
            return File(ms.ToArray(), "application/pdf", fileName);
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
                .SetMarginTop(8)
                .SetMarginBottom(6);
        }

        private Cell CeldaAreaLibre(string texto, PdfFont font)
        {
            var p = new Paragraph(texto ?? string.Empty)
                .SetFont(font)
                .SetFontSize(10)
                .SetMultipliedLeading(1.15f);
            return new Cell().Add(p)
                             .SetBorder(new SolidBorder(0.5f))
                             .SetPadding(6)
                             .SetTextAlignment(TextAlignment.LEFT);
        }

        private Cell CeldaEncabezado(string texto, PdfFont font, DeviceRgb color)
        {
            return new Cell()
                .Add(new Paragraph(texto).SetFont(font).SetFontSize(9).SetBold().SetTextAlignment(TextAlignment.CENTER))
                .SetBackgroundColor(color)
                .SetFontColor(ColorConstants.WHITE)
                .SetPadding(6)
                .SetBorder(new SolidBorder(color, 1));
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
