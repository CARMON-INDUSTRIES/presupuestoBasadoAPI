using iText.Kernel.Pdf;
using iText.Kernel.Font;
using iText.IO.Font.Constants;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using presupuestoBasadoAPI.Models;
using presupuestoBasadoAPI.Services;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Pdf.Canvas;
using iText.IO.Image;
using System.IO;

namespace presupuestoBasadoAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FormatoArbolDeProblemasController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IUsuarioActualService _usuarioActualService;
        private readonly string _imagesPath;

        public FormatoArbolDeProblemasController(AppDbContext context, IUsuarioActualService usuarioActualService)
        {
            _context = context;
            _usuarioActualService = usuarioActualService;
            _imagesPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Images");
        }

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
        public async Task<IActionResult> GenerarPdf()
        {
            var userId = await _usuarioActualService.ObtenerUserIdAsync();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("No se pudo obtener el usuario actual.");

            var problema = await _context.IdentificacionDescripcionProblemas
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.Id)
                .FirstOrDefaultAsync();

            var efectoSuperior = await _context.EfectosSuperiores
                .Where(e => e.UserId == userId)
                .OrderByDescending(e => e.Id)
                .FirstOrDefaultAsync();

            var diseno = await _context.DisenoIntervencionPublicas
                .Where(d => d.UserId == userId)
                .OrderByDescending(d => d.Id)
                .FirstOrDefaultAsync();

            if (problema == null || efectoSuperior == null || diseno == null)
                return NotFound("Faltan registros del problema, efecto superior o diseño.");

            var componentes = await _context.Componentes
                .Where(c => c.DisenoIntervencionPublicaId == diseno.Id)
                .Include(c => c.Acciones)
                .Include(c => c.Resultado)
                .ToListAsync();

            using var ms = new MemoryStream();
            using var writer = new PdfWriter(ms);
            using var pdfDoc = new PdfDocument(writer);
            var doc = new iText.Layout.Document(pdfDoc, PageSize.LETTER);
            var font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);

            var colorFondo = new DeviceRgb(255, 255, 255);
            var colorHeaderGris = new DeviceRgb(100, 100, 100);
            var colorFondoGris = new DeviceRgb(255, 255, 255);

            const float lineHeight = 10f;
            const float padding = 6f;

            var grupos = componentes
                .Select((c, i) => new { c, i })
                .GroupBy(x => x.i / 2)
                .Select(g => g.Select(x => x.c).ToList())
                .ToList();

            float CalcularAlturaCaja(string texto, float anchoMax)
            {
                float alturaHeader = 18f;
                if (string.IsNullOrWhiteSpace(texto)) return alturaHeader + 20f;
                int maxChars = (int)(anchoMax / 4.2f);
                int lineas = (int)Math.Ceiling((double)texto.Length / maxChars);
                return alturaHeader + (lineas * lineHeight) + 2 * padding;
            }

            List<string> DividirTexto(string texto, int maxChars)
            {
                var result = new List<string>();
                if (string.IsNullOrEmpty(texto)) return new List<string> { "" };
                for (int i = 0; i < texto.Length; i += maxChars)
                    result.Add(texto.Substring(i, Math.Min(maxChars, texto.Length - i)));
                return result;
            }

            void EscribirCaja(PdfCanvas canvas, PdfFont f, float x, float y, float w, float h, string titulo, string texto, bool esGris)
            {
                float alturaHeader = 18f;

                var colorHeader = esGris ? colorHeaderGris : new DeviceRgb(102, 0, 51);
                var colorFondoCaja = esGris ? colorFondoGris : colorFondo;

                canvas.SaveState();
                canvas.SetFillColor(colorFondoCaja);
                canvas.Rectangle(x, y, w, h);
                canvas.Fill();
                canvas.RestoreState();

                canvas.SaveState();
                canvas.SetFillColor(colorHeader);
                canvas.Rectangle(x, y + h - alturaHeader, w, alturaHeader);
                canvas.Fill();
                canvas.RestoreState();

                canvas.SetStrokeColor(DeviceRgb.BLACK);
                canvas.Rectangle(x, y, w, h);
                canvas.Stroke();

                canvas.BeginText()
                      .SetFontAndSize(f, 9)
                      .SetFillColor(DeviceRgb.WHITE)
                      .MoveText(x + 4, y + h - alturaHeader + 4)
                      .ShowText(titulo)
                      .EndText();

                if (!string.IsNullOrWhiteSpace(texto))
                {
                    int maxChars = (int)(w / 4.2f);
                    var lines = DividirTexto(texto, maxChars);
                    float offsetY = y + h - alturaHeader - 10;
                    foreach (var line in lines)
                    {
                        canvas.BeginText()
                              .SetFontAndSize(f, 9)
                              .SetFillColor(DeviceRgb.BLACK)
                              .MoveText(x + 4, offsetY)
                              .ShowText(line)
                              .EndText();
                        offsetY -= lineHeight;
                    }
                }
            }

            void DibujarFlechaVertical(PdfCanvas canvas, float x, float yDesde, float yHasta)
            {
                float margen = 5f;
                float y1 = yDesde + margen;
                float y2 = yHasta - margen;
                canvas.MoveTo(x, y1).LineTo(x, y2).Stroke();

                float size = 4f;
                canvas.MoveTo(x, y2)
                      .LineTo(x - size, y2 - size)
                      .LineTo(x + size, y2 - size)
                      .LineTo(x, y2)
                      .Fill();
            }

            var emblemaPath = GetEmblemaPath(userId);

            for (int i = 0; i < grupos.Count; i++)
            {
                var grupo = grupos[i];
                var page = pdfDoc.AddNewPage();
                var canvas = new PdfCanvas(page);
                var rect = page.GetPageSize();
                var layout = new iText.Layout.Canvas(canvas, rect);

                float yHeader = rect.GetHeight() - 40;
                layout.ShowTextAligned(
                    new Paragraph("Anexo 4 - Árbol de Problemas")
                        .SetFont(font).SetFontSize(14).SetBold(),
                    40, yHeader, TextAlignment.LEFT);

                if (System.IO.File.Exists(emblemaPath))
                {
                    try
                    {
                        var emblemaData = ImageDataFactory.Create(emblemaPath);
                        var emblema = new iText.Layout.Element.Image(emblemaData)
                            .ScaleAbsolute(60, 60)
                            .SetFixedPosition(rect.GetWidth() - 80, yHeader - 30);
                        layout.Add(emblema);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[PDF] Error al cargar emblema: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine($"⚠️ No se encontró el emblema en: {emblemaPath}");
                }

                float centerX = rect.GetWidth() / 2;
                float yTop = rect.GetHeight() - 120;

                float hEfecto = CalcularAlturaCaja(efectoSuperior.Descripcion, 240);
                EscribirCaja(canvas, font, centerX - 120, yTop, 240, hEfecto, "Efecto", efectoSuperior.Descripcion, false);
                float yEfectoBase = yTop;

                float yResultados = yEfectoBase - hEfecto - 80;
                float xResultados = centerX - grupo.Count * 100f;
                foreach (var comp in grupo)
                {
                    string textoResultado = comp.Resultado?.Descripcion ?? "(sin resultado)";
                    float hRes = CalcularAlturaCaja(textoResultado, 160);
                    EscribirCaja(canvas, font, xResultados, yResultados, 160, hRes, "Resultado", textoResultado, true);
                    DibujarFlechaVertical(canvas, xResultados + 80, yResultados + hRes, yEfectoBase);
                    xResultados += 200f;
                }

                float hProblema = CalcularAlturaCaja(problema.ProblemaCentral, 240);
                float yProblema = yResultados - hProblema - 80;
                EscribirCaja(canvas, font, centerX - 120, yProblema, 240, hProblema, "Problema", problema.ProblemaCentral, false);
                DibujarFlechaVertical(canvas, centerX, yProblema + hProblema, yResultados);

                float yCompBase = yProblema - 100;
                float xCompStart = centerX - grupo.Count * 100f;
                foreach (var comp in grupo)
                {
                    float hComp = CalcularAlturaCaja(comp.Nombre, 160);
                    EscribirCaja(canvas, font, xCompStart, yCompBase, 160, hComp, "Componente", comp.Nombre, true);
                    DibujarFlechaVertical(canvas, xCompStart + 80, yCompBase + hComp, yProblema);

                    if (comp.Acciones != null && comp.Acciones.Any())
                    {
                        float yAccion = yCompBase - hComp - 40;
                        foreach (var accion in comp.Acciones)
                        {
                            float hAccion = CalcularAlturaCaja(accion.Descripcion, 160);
                            EscribirCaja(canvas, font, xCompStart, yAccion, 160, hAccion, "Acción", accion.Descripcion, true);
                            DibujarFlechaVertical(canvas, xCompStart + 80, yAccion + hAccion, yCompBase);
                            yAccion -= hAccion + 20;
                        }
                    }

                    xCompStart += 200f;
                }

                layout.Close();
            }

            doc.Close();
            return File(ms.ToArray(), "application/pdf", "FormatoArbolDeProblemas.pdf");
        }
    }
}
