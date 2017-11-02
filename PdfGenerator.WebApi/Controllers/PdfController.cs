using System;
using PdfGenerator.WebApi.Models;
using PdfGenerator.WebApi.Services;
using System.Web.Http;
using Serilog;

namespace PdfGenerator.WebApi.Controllers
{
    [RoutePrefix("api/pdf")]
    public class PdfController : ApiController
    {
        private readonly IPdfGeneratorService _pdfGenerator = new PdfGeneratorService();
        private readonly ILogger _log = Log.ForContext<PdfController>();

        [HttpPost]
        [Route("generate")]
        public byte[] Generate(GeneratePdfRequest request)
        {
            try
            {
                _log.Information("Getting PDF generation request: {@request}", request);
                return _pdfGenerator.GeneratePdf(request.DocumentName, request.Html);
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Error while processing PDF generation {@request}", request);
                throw;
            }
        }
    }
}