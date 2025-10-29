using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;

namespace presupuestoBasadoAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FormatoConsolidadoController : ControllerBase
    {
        private readonly FormatoAlineacionController _formatoAlineacion;
        private readonly FormatoFichaDeInformacionBasica1Controller _formatoFichaBasica;
        private readonly FormatoDefinicionDelProblemaController _formatoDefinicionProblema;
        private readonly FormatoAnalisisDeInvolucradosController _formatoAnalisisDeInvolucrados;
        private readonly FormatoArbolDeProblemasController _formatoArbolProblemas;
        private readonly FormatoArbolDeObjetivosController _formatoArbolObjetivos;
        private readonly FormatoAnalisisInvolucradosController _formatoAnalisisInvolucrados2;
        private readonly FormatoEstructuraAnaliticaController _formatoEstructuraAnalitica;
        private readonly FormatoMatrizController _formatoMatriz;
        private readonly FormatoFichaFinalController _formatoFichaTecnica;

        public FormatoConsolidadoController(
            FormatoAlineacionController formatoAlineacion,
            FormatoFichaDeInformacionBasica1Controller formatoFichaBasica,
            FormatoDefinicionDelProblemaController formatoDefinicionProblema,
            FormatoAnalisisDeInvolucradosController formatoAnalisisDeInvolucrados,
            FormatoArbolDeProblemasController formatoArbolProblemas,
            FormatoArbolDeObjetivosController formatoArbolObjetivos,
            FormatoAnalisisInvolucradosController formatoAnalisisInvolucrados2,
            FormatoEstructuraAnaliticaController formatoEstructuraAnalitica,
            FormatoMatrizController formatoMatriz,
            FormatoFichaFinalController formatoFichaTecnica
        )
        {
            _formatoAlineacion = formatoAlineacion;
            _formatoFichaBasica = formatoFichaBasica;
            _formatoDefinicionProblema = formatoDefinicionProblema;
            _formatoAnalisisDeInvolucrados = formatoAnalisisDeInvolucrados;
            _formatoArbolProblemas = formatoArbolProblemas;
            _formatoArbolObjetivos = formatoArbolObjetivos;
            _formatoAnalisisInvolucrados2 = formatoAnalisisInvolucrados2;
            _formatoEstructuraAnalitica = formatoEstructuraAnalitica;
            _formatoMatriz = formatoMatriz;
            _formatoFichaTecnica = formatoFichaTecnica;

        }

        [HttpGet("descargar")]
        public IActionResult DescargarTodos()
        {
            using var msFinal = new MemoryStream();
            using var pdfFinal = new PdfDocument(new PdfWriter(msFinal));
            var merger = new PdfMerger(pdfFinal);

            void MergePDFFromController(ControllerBase controller)
            {
                if (controller == null) return;

                // Obtener método GenerarPdf
                MethodInfo? method = controller.GetType().GetMethod("GenerarPdf");
                if (method == null) return;

                var result = method.Invoke(controller, null) as FileContentResult;
                if (result == null) return;

                using var temp = new MemoryStream(result.FileContents);
                using var pdfDoc = new PdfDocument(new PdfReader(temp));
                merger.Merge(pdfDoc, 1, pdfDoc.GetNumberOfPages());
            }

            // Llamamos a cada controlador
            MergePDFFromController(_formatoAlineacion);
            MergePDFFromController(_formatoFichaBasica);
            MergePDFFromController(_formatoDefinicionProblema);
            MergePDFFromController(_formatoAnalisisDeInvolucrados);
            MergePDFFromController(_formatoArbolProblemas);
            MergePDFFromController(_formatoArbolObjetivos);
            MergePDFFromController(_formatoAnalisisInvolucrados2);
            MergePDFFromController(_formatoEstructuraAnalitica);
            MergePDFFromController(_formatoMatriz);
            MergePDFFromController(_formatoFichaTecnica);

            pdfFinal.Close();

            var filename = $"FormatoConsolidado_{User.Identity.Name}_{DateTime.Now:yyyyMMdd_HHmm}.pdf";
            return File(msFinal.ToArray(), "application/pdf", filename);
        }
    }
}
