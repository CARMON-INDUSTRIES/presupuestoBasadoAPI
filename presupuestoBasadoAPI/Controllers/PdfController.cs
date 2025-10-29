using iText.Kernel.Pdf;
using iText.Kernel.Font;
using iText.IO.Font.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using presupuestoBasadoAPI.Models;

namespace presupuestoBasadoAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PdfController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public PdfController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet("formato1/{userId}")]
        public IActionResult GenerarFormato1(string userId)
        {
            var user = _userManager.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
                return NotFound("Usuario no encontrado");

            using var ms = new MemoryStream();

            var plantillaPath = @"C:\Users\Ramon\source\repos\presupuestoBasadoAPI\presupuestoBasadoAPI\PDFReports\FormatoAlineacion.pdf";

            var reader = new PdfReader(plantillaPath);
            var writer = new PdfWriter(ms);
            var pdfDoc = new PdfDocument(reader, writer);

            var font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
            var page = pdfDoc.GetFirstPage();
            var canvas = new iText.Kernel.Pdf.Canvas.PdfCanvas(page);

            // === 1.- ANTECEDENTES === primero es Y luego es X
            canvas.BeginText().SetFontAndSize(font, 10).MoveText(250, 150)
                  .ShowText(user.UnidadAdministrativa?.Clave ?? "").EndText();

            canvas.BeginText().SetFontAndSize(font, 10).MoveText(250, 600)
                  .ShowText(user.UnidadesPresupuestales ?? "").EndText();

            canvas.BeginText().SetFontAndSize(font, 10).MoveText(250, 480)
                  .ShowText(user.Cargo ?? "").EndText(); // <- o ProgramaSectorial cuando lo tengas

            canvas.BeginText().SetFontAndSize(font, 10).MoveText(250, 470)
                  .ShowText(user.ProgramaPresupuestario ?? "").EndText();

            canvas.BeginText().SetFontAndSize(font, 10).MoveText(200, 460)
                  .ShowText(user.NombreMatriz ?? "").EndText();

           

            pdfDoc.Close();

            return File(ms.ToArray(), "application/pdf", "FormatoAlineacion.pdf");
        }
    }
}
