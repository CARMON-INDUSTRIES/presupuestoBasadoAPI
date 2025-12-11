using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using presupuestoBasadoAPI.Models;
using System.Security.Claims;

namespace presupuestoBasadoAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FormatoFichaFinalController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly string _imagesPath;

        public FormatoFichaFinalController(AppDbContext context)
        {
            _context = context;
            _imagesPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Images");
        }

        private readonly string _emblemaPath = System.IO.Path.Combine(
           System.IO.Directory.GetCurrentDirectory(), "Images", "emblema.png");

        private string GetUserId() =>
            User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

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
        public async Task<IActionResult> GenerarFichaFinal([FromQuery] int? indicadorId = null)
        {
            var userId = GetUserId();
            var emblemaPath = GetEmblemaPath(userId);

            var usuario = await _context.Users
                .Where(u => u.Id == userId)
                .Select(u => new {
                u.NombreCompleto,
                u.Cargo,
                u.Coordinador,
                u.UnidadesPresupuestales,
                u.ProgramaPresupuestario,
                u.NombreMatriz
        })
            .FirstOrDefaultAsync();


            var ficha = await _context.Fichas
                .Include(f => f.Indicadores)
                .Include(f => f.MetasProgramadas)
                .Where(f => f.UserId == userId)
                .OrderByDescending(f => f.Id)
                .FirstOrDefaultAsync();

            if (ficha == null)
                return NotFound("No se encontró ninguna ficha registrada para este usuario.");

            var indicadores = indicadorId.HasValue
                ? ficha.Indicadores.Where(i => i.Id == indicadorId.Value).ToList()
                : ficha.Indicadores.ToList();

            if (!indicadores.Any())
                return NotFound("No se encontró el indicador solicitado para esta ficha.");

            using var ms = new MemoryStream();
            var writer = new PdfWriter(ms);
            var pdf = new PdfDocument(writer);
            var document = new Document(pdf);
            var pageSize = PageSize.LETTER;
            var doc = new Document(pdf, pageSize);

            var font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
            var fontBold = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
            var colorRojo = new DeviceRgb(105, 27, 49);
            var colorGris = new DeviceRgb(245, 245, 245);

            float pageWidth = pdf.GetDefaultPageSize().GetWidth();
            float pageHeight = pdf.GetDefaultPageSize().GetHeight();

            // === Emblema superior derecho ===
            if (System.IO.File.Exists(emblemaPath))
            {
                var emblema = new Image(ImageDataFactory.Create(emblemaPath))
                    .SetWidth(85)
                    .SetAutoScale(true)
                    .SetFixedPosition(pageWidth - 100 - 85, pageHeight - 95);
                doc.Add(emblema);
            }

            // === Título principal (sin rectángulo) ===
            document.Add(new Paragraph("Ficha Técnica del Indicador")
                .SetFont(fontBold)
                .SetFontSize(14)
                .SetTextAlignment(TextAlignment.LEFT)
                .SetMarginBottom(29));

            // === I. DATOS DE IDENTIFICACIÓN DEL PROGRAMA ===
            document.Add(SeccionTitulo("I. DATOS DE IDENTIFICACIÓN DEL PROGRAMA", colorRojo));

            var tProg = new Table(UnitValue.CreatePercentArray(new float[] { 2, 3, 2, 3 })).UseAllAvailableWidth();
            tProg.AddCell(Celda("Nombre completo:", true, colorGris));
            tProg.AddCell(Celda(usuario.NombreCompleto));
            tProg.AddCell(Celda("Cargo:", true, colorGris));
            tProg.AddCell(Celda(usuario.Cargo));
            tProg.AddCell(Celda("Coordinador:", true, colorGris));
            tProg.AddCell(Celda(usuario.Coordinador));
            tProg.AddCell(Celda("Unidad presupuestal:", true, colorGris));
            tProg.AddCell(Celda(usuario.UnidadesPresupuestales));
            tProg.AddCell(Celda("Programa presupuestario:", true, colorGris));
            tProg.AddCell(Celda(usuario.ProgramaPresupuestario));
            tProg.AddCell(Celda("Nombre de la matriz:", true, colorGris));
            tProg.AddCell(Celda(usuario.NombreMatriz));
            document.Add(tProg);


            // === II. DATOS DEL INDICADOR SELECCIONADO ===
            var indicador = indicadores.First();
            document.Add(SeccionTitulo("II. DATOS DE IDENTIFICACIÓN DEL INDICADOR", colorRojo));

            var tIdent = new Table(UnitValue.CreatePercentArray(new float[] { 2, 3, 2, 3 })).UseAllAvailableWidth();
            tIdent.AddCell(Celda("Nivel:", true, colorGris));
            tIdent.AddCell(Celda(indicador.Nivel));
            tIdent.AddCell(Celda("Dimensión a medir:", true, colorGris));
            tIdent.AddCell(Celda(indicador.Dimension));
            tIdent.AddCell(Celda("Sentido:", true, colorGris));
            tIdent.AddCell(Celda(indicador.Sentido));
            tIdent.AddCell(Celda("Definición:", true, colorGris));
            tIdent.AddCell(Celda(indicador.Definicion));
            document.Add(tIdent);

            // === III. DATOS DEL INDICADOR ===
            document.Add(SeccionTitulo("III. DATOS DEL INDICADOR", colorRojo));

            var tDatos = new Table(UnitValue.CreatePercentArray(new float[] { 2, 3, 2, 3 })).UseAllAvailableWidth();
            tDatos.AddCell(Celda("Fórmula del indicador:", true, colorGris));
            tDatos.AddCell(new Cell(1, 3)
                .Add(new Paragraph($"{indicador.Numerador} / {indicador.Denominador}").SetFontSize(9))
                .SetBorder(new iText.Layout.Borders.SolidBorder(ColorConstants.BLACK, 0.5f)));

            tDatos.AddCell(Celda("Unidad de medida:", true, colorGris));
            tDatos.AddCell(Celda(indicador.UnidadMedida));
            tDatos.AddCell(Celda("Rango de valor:", true, colorGris));
            tDatos.AddCell(Celda(indicador.RangoValor));
            tDatos.AddCell(Celda("Frecuencia de medición:", true, colorGris));
            tDatos.AddCell(Celda(indicador.FrecuenciaMedicion));
            tDatos.AddCell(Celda("Cobertura:", true, colorGris));
            tDatos.AddCell(Celda(indicador.Cobertura));
            tDatos.AddCell(Celda("Fuentes:", true, colorGris));
            tDatos.AddCell(new Cell(1, 3)
                .Add(new Paragraph(
                    $"Resultado: {indicador.FuenteResultado}\n" +
                    $"Numerador: {indicador.FuenteNumerador}\n" +
                    $"Denominador: {indicador.FuenteDenominador}")
                    .SetFontSize(9))
                .SetBorder(new iText.Layout.Borders.SolidBorder(ColorConstants.BLACK, 0.5f)));
            document.Add(tDatos);

            // === LÍNEA BASE ===
            document.Add(SeccionTitulo("Línea Base", colorRojo));

            var tLinea = new Table(UnitValue.CreatePercentArray(new float[] { 2, 2, 2, 2 })).UseAllAvailableWidth();
            tLinea.AddCell(Celda("Valor:", true, colorGris));
            tLinea.AddCell(Celda(indicador.LineaBaseValor?.ToString() ?? ""));
            tLinea.AddCell(Celda("Año:", true, colorGris));
            tLinea.AddCell(Celda(indicador.LineaBaseAnio));
            tLinea.AddCell(Celda("Unidad:", true, colorGris));
            tLinea.AddCell(Celda(indicador.LineaBaseUnidad));
            tLinea.AddCell(Celda("Periodo:", true, colorGris));
            tLinea.AddCell(Celda(indicador.LineaBasePeriodo));
            document.Add(tLinea);

            // === METAS PROGRAMADAS ===
            document.Add(SeccionTitulo("Determinación de Metas", colorRojo));

            var metas = ficha.MetasProgramadas
                .Where(m => m.FichaIndicadorId == ficha.Id)
                .ToList();

            var tMetas = new Table(UnitValue.CreatePercentArray(new float[] { 2, 2, 2, 2, 2 })).UseAllAvailableWidth();
            tMetas.AddHeaderCell(Celda("Meta Programada", true, colorGris));
            tMetas.AddHeaderCell(Celda("Periodo", true, colorGris));
            tMetas.AddHeaderCell(Celda("Mes", true, colorGris));
            tMetas.AddHeaderCell(Celda("Esperado", true, colorGris));
            tMetas.AddHeaderCell(Celda("Alcanzado", true, colorGris));

            if (metas.Any())
            {
                foreach (var m in metas)
                {
                    tMetas.AddCell(Celda(m.MetaProgramadaNombre));
                    tMetas.AddCell(Celda(m.PeriodoCumplimiento));
                    tMetas.AddCell(Celda(m.Mes.ToString()));
                    tMetas.AddCell(Celda(m.CantidadEsperada.ToString("N2")));
                    tMetas.AddCell(Celda(m.Alcanzado.ToString("N2")));
                }
            }
            else
            {
                tMetas.AddCell(new Cell(1, 5)
                    .Add(new Paragraph("No hay metas registradas"))
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(9));
            }
            document.Add(tMetas);

            //document.Add(SeccionTitulo("IV. CREMA", colorRojo));

            //var cremaTexto = indicador.Crema != null
            //    ? string.Join(", ", indicador.Crema.Select(c => $"{c.Key}: {c.Value}"))
            //    : "N/A";

            //document.Add(new Paragraph($"Crema: {cremaTexto}"));

            // === IV. LÍNEA DE ACCIÓN (solo la última capturada) ===
            document.Add(SeccionTitulo("V. LÍNEA DE ACCIÓN", colorRojo));

            var ultimaMunicipal = await _context.AlineacionesMunicipio
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.Id)
                .FirstOrDefaultAsync();

            var ultimaEstatal = await _context.AlineacionesEstado
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.Id)
                .FirstOrDefaultAsync();

            // Si existe estatal se usa esa; si no, la municipal
            var lineaSeleccionada = ultimaEstatal?.LineaAccion ?? ultimaMunicipal?.LineaAccion;


            if (!string.IsNullOrWhiteSpace(lineaSeleccionada))
            {
                var tLa = new Table(UnitValue.CreatePercentArray(new float[] { 1 })).UseAllAvailableWidth();
                tLa.AddCell(new Cell()
                    .Add(new Paragraph(lineaSeleccionada).SetFontSize(9))
                    .SetBorder(new iText.Layout.Borders.SolidBorder(ColorConstants.BLACK, 0.5f))
                    .SetPadding(4));
                document.Add(tLa);
            }
            else
            {
                document.Add(new Paragraph("No hay línea de acción registrada para este usuario.")
                    .SetFontSize(9));
            }


            document.Close();
            return File(ms.ToArray(), "application/pdf", $"FichaIndicador_{ficha.Id}_{indicador.Id}.pdf");
        }

        // === Helpers ===
        private static Paragraph SeccionTitulo(string texto, Color colorFondo)
        {
            return new Paragraph(texto)
                .SetBackgroundColor(colorFondo)
                .SetFontColor(ColorConstants.WHITE)
                .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD))
                .SetTextAlignment(TextAlignment.CENTER)
                .SetPadding(4)
                .SetMarginTop(10);
        }

        private static Cell Celda(string texto, bool negrita = false, Color? bg = null)
        {
            var c = new Cell()
                .Add(new Paragraph(texto ?? "").SetFontSize(9))
                .SetBorder(new iText.Layout.Borders.SolidBorder(ColorConstants.BLACK, 0.5f))
                .SetPadding(3);

            if (negrita)
                c.SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD));
            if (bg != null)
                c.SetBackgroundColor(bg);
            return c;
        }
    }
}
