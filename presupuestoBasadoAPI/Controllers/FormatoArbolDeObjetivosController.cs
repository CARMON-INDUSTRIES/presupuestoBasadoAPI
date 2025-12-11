using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
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
    public class FormatoArbolDeObjetivosController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly string _imagesPath;

        public FormatoArbolDeObjetivosController(AppDbContext context)
        {
            _context = context;
            _imagesPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Images");
        }

        private string GetUserId() =>
            User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

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
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("No se pudo obtener el usuario actual.");

            var arbol = await _context.ArbolObjetivos
                .Include(a => a.Componentes)
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.Id)
                .FirstOrDefaultAsync();

            if (arbol == null)
                return NotFound("No se encontró un registro de Árbol de Objetivos.");

            var emblemaPath = GetEmblemaPath(userId);

            using var ms = new MemoryStream();
            using var writer = new PdfWriter(ms);
            using var pdfDoc = new PdfDocument(writer);
            var doc = new Document(pdfDoc, PageSize.LETTER);
            var font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
            var fontBold = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);

            // Colores
            var colorVino = new DeviceRgb(102, 0, 51);
            var colorBlanco = new DeviceRgb(255, 255, 255);
            var colorHeaderGris = new DeviceRgb(100, 100, 100);

            doc.SetMargins(70, 40, 40, 40);

            float pageWidth = PageSize.LETTER.GetWidth();
            float pageHeight = PageSize.LETTER.GetHeight();
            float headerY = pageHeight - 80;

            // === ENCABEZADO ===
            if (System.IO.File.Exists(emblemaPath))
            {
                try
                {
                    var emblemaData = ImageDataFactory.Create(emblemaPath);
                    var emblema = new Image(emblemaData)
                        .SetWidth(100)
                        .SetFixedPosition(pageWidth - 130, headerY - 10);
                    doc.Add(emblema);
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

            var encabezado = new Paragraph("Anexo 5 - Árbol de Objetivos")
                .SetFont(fontBold)
                .SetFontSize(12)
                .SetFixedPosition(50, headerY + 10, 400)
                .SetMargin(0);
            doc.Add(encabezado);

            // === SECCIÓN PRINCIPAL ===
            float centerX = pageWidth / 2;
            const float padding = 6f;
            const float lineHeight = 10f;
            const float margenSuperior = 140f;

            var componentes = arbol.Componentes.ToList();
            int totalPaginas = (int)Math.Ceiling(componentes.Count / 2.0);
            int compPorPagina = 2;

            for (int p = 0; p < totalPaginas; p++)
            {
                if (p > 0) pdfDoc.AddNewPage();
                var page = pdfDoc.GetPage(p + 1);
                var canvas = new iText.Kernel.Pdf.Canvas.PdfCanvas(page);

                // === FIN ===
                float yFin = pageHeight - margenSuperior;
                float hFin = CalcularAlturaCaja(arbol.Fin, 240, lineHeight, padding);
                EscribirCajaConEncabezado(canvas, font, centerX - 120, yFin, 240, hFin, "Fin", arbol.Fin, colorVino, colorBlanco);

                // === RESULTADOS ===
                float yResultado = yFin - hFin - 80;
                float hResultado = 0f;
                var compsPagina = componentes.Skip(p * compPorPagina).Take(compPorPagina).ToList();
                var resultados = compsPagina.SelectMany(c => c.Resultados).ToList();

                if (resultados.Any())
                {
                    float totalWidth = resultados.Count * 160f;
                    float xStartRes = centerX - totalWidth / 2;

                    foreach (var resultado in resultados)
                    {
                        hResultado = CalcularAlturaCaja(resultado, 140, lineHeight, padding);
                        EscribirCajaConEncabezado(canvas, font, xStartRes, yResultado, 140, hResultado, "Resultado", resultado, colorHeaderGris, colorBlanco);
                        DibujarFlecha(canvas, xStartRes + 70, yResultado + hResultado, yFin, colorVino);
                        xStartRes += 160f;
                    }
                }

                // === PROPÓSITO CENTRAL ===
                float yProposito = yResultado - (hResultado > 0 ? hResultado + 80 : 100);
                float hProposito = CalcularAlturaCaja(arbol.ObjetivoCentral, 240, lineHeight, padding);
                EscribirCajaConEncabezado(canvas, font, centerX - 120, yProposito, 240, hProposito, "Propósito", arbol.ObjetivoCentral, colorVino, colorBlanco);
                DibujarFlecha(canvas, centerX, yProposito + hProposito, resultados.Any() ? yResultado : yFin, colorVino);

                // === COMPONENTES ===
                float totalWidthComp = compsPagina.Count * 140f;
                float xStart = centerX - totalWidthComp / 2;
                float yComponente = yProposito - 130;

                foreach (var comp in compsPagina)
                {
                    float hComp = CalcularAlturaCaja(comp.Nombre, 140, lineHeight, padding);
                    EscribirCajaConEncabezado(canvas, font, xStart, yComponente, 140, hComp, "Componente", comp.Nombre, colorHeaderGris, colorBlanco);
                    DibujarFlecha(canvas, xStart + 70, yComponente + hComp, yProposito, colorVino);

                    // === MEDIOS ===
                    float yMedio = yComponente - 110;
                    if (comp.Medios != null && comp.Medios.Any())
                    {
                        foreach (var medio in comp.Medios)
                        {
                            float hMedio = CalcularAlturaCaja(medio, 120, lineHeight, padding);
                            EscribirCajaConEncabezado(canvas, font, xStart, yMedio, 120, hMedio, "Medio", medio, colorHeaderGris, colorBlanco);
                            DibujarFlecha(canvas, xStart + 60, yMedio + hMedio, yComponente, colorVino);
                            yMedio -= hMedio + 10;
                        }
                    }

                    xStart += 160f;
                }
            }

            doc.Close();
            return File(ms.ToArray(), "application/pdf", "FormatoArbolDeObjetivos.pdf");
        }

        // ==== FUNCIONES AUXILIARES ====

        private float CalcularAlturaCaja(string texto, float anchoMaximo, float lineHeight, float padding)
        {
            float alturaHeader = 18f;
            if (string.IsNullOrWhiteSpace(texto)) return alturaHeader + 20f;
            int maxCharsPorLinea = (int)(anchoMaximo / 4.2f);
            int lineas = (int)Math.Ceiling((double)texto.Length / maxCharsPorLinea);
            return alturaHeader + (lineas * lineHeight) + 2 * padding;
        }

        private void EscribirCajaConEncabezado(iText.Kernel.Pdf.Canvas.PdfCanvas canvas, PdfFont font,
                                               float x, float y, float w, float h,
                                               string titulo, string texto, DeviceRgb colorHeader, DeviceRgb colorFondo)
        {
            float alturaHeader = 18f;
            canvas.SaveState();
            canvas.SetFillColor(colorFondo);
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
                  .SetFontAndSize(font, 9)
                  .SetFillColor(DeviceRgb.WHITE)
                  .MoveText(x + 4, y + h - alturaHeader + 4)
                  .ShowText(titulo)
                  .EndText();

            if (!string.IsNullOrWhiteSpace(texto))
            {
                var maxCharsPorLinea = (int)(w / 4.2f);
                var lines = DividirTexto(texto, maxCharsPorLinea);
                float offsetY = y + h - alturaHeader - 10;
                foreach (var line in lines)
                {
                    canvas.BeginText()
                          .SetFontAndSize(font, 9)
                          .SetFillColor(DeviceRgb.BLACK)
                          .MoveText(x + 4, offsetY)
                          .ShowText(line)
                          .EndText();
                    offsetY -= 10;
                }
            }
        }

        private List<string> DividirTexto(string texto, int maxChars)
        {
            var result = new List<string>();
            for (int i = 0; i < texto.Length; i += maxChars)
                result.Add(texto.Substring(i, Math.Min(maxChars, texto.Length - i)));
            return result;
        }

        private void DibujarFlecha(iText.Kernel.Pdf.Canvas.PdfCanvas canvas, float x, float yDesde, float yHasta, DeviceRgb color)
        {
            float margen = 5f;
            float y1 = yDesde + margen;
            float y2 = yHasta - margen;
            canvas.SetStrokeColor(color);
            canvas.MoveTo(x, y1);
            canvas.LineTo(x, y2);
            canvas.Stroke();

            float size = 4f;
            canvas.SetFillColor(color);
            canvas.MoveTo(x, y2);
            canvas.LineTo(x - size, y2 - size);
            canvas.LineTo(x + size, y2 - size);
            canvas.ClosePathFillStroke();
        }
    }
}
