using ManyConsole;
using Pechkin;
using Serilog;
using System;
using System.IO;
using System.Text;

namespace PdfGenerator.Commands
{
    public class GeneratePdfCommand : ConsoleCommand
    {
        private readonly ILogger _logger;

        public GeneratePdfCommand(ILogger logger, string alias = "generate-pdf")
        {
            _logger = logger;
            IsCommand(alias, "Generate a PDF from HTML");

            HasRequiredOption("n|name=", "Document name", p => DocumentName = p);
            HasRequiredOption("s|source=", "Source HTML file path", p => SourceFile = p);
            HasRequiredOption("t|target=", "Target PDF file path", p => TargetFile = p);
        }

        public string DocumentName { get; set; }

        public string SourceFile { get; set; }

        public string TargetFile { get; set; }

        public override int Run(string[] remainingArguments)
        {
            var logger = _logger
                .ForContext("SourceFile", SourceFile)
                .ForContext("TargetFile", TargetFile)
                .ForContext("DocumentName", DocumentName);

            if (string.IsNullOrWhiteSpace(DocumentName) || string.IsNullOrWhiteSpace(SourceFile) || string.IsNullOrWhiteSpace(TargetFile))
            {
                logger.Warning("Document name, Source File and Target File should not be empty!");
                return Responses.Failure;
            }

            DocumentName = DocumentName.Trim('"', '\'', '`');
            SourceFile = SourceFile.Trim('"', '\'', '`');
            TargetFile = TargetFile.Trim('"', '\'', '`');

#pragma warning disable Serilog003 // Property binding verifier
            logger.Information("Generating document {DocumentName}");
#pragma warning restore Serilog003 // Property binding verifier

            if (!File.Exists(SourceFile))
            {
                logger.Warning("Unable to find source file {SouceFile}", SourceFile);
                return Responses.Failure;
            }

            try
            {
                string html = File.ReadAllText(SourceFile);

                // create global configuration object
                GlobalConfig gc = new GlobalConfig();

                // set it up using fluent notation
                gc.SetDocumentTitle(DocumentName)
                  .SetPaperSize(System.Drawing.Printing.PaperKind.A4)
                  .SetPaperOrientation(false);
                //... etc

                // create converter
                IPechkin pechkin = new SimplePechkin(gc);

                // create document configuration object
                ObjectConfig oc = new ObjectConfig();

                // and set it up using fluent notation too
                oc.SetCreateExternalLinks(true)
                  .SetPrintBackground(true)
                  .SetFallbackEncoding(Encoding.ASCII)
                  // Zoom factor of 2+ and intelligent shrinking fixes an issue where document gets shrink at random times.
                  // https://github.com/gmanny/Pechkin/issues/11
                  .SetZoomFactor(2.0)
                  .SetIntelligentShrinking(true)
                  .SetLoadImages(true);
                oc.Footer.SetContentSpacing(50);
                oc.Footer.SetLeftText("[date]")
                         .SetCenterText("[doctitle]")
                         .SetRightText("Page [page] of [toPage]")
                         .SetFontSize(8);

                oc.Header.SetContentSpacing(50);

                // convert document
                byte[] pdfBuf = pechkin.Convert(oc, html);
                if (pdfBuf == null)
                {
                    logger.Warning("PDF data is null");
                    return Responses.Failure;
                }

                File.WriteAllBytes(TargetFile, pdfBuf);

                logger.Information("PDF successfully generated with {PdfFileSize}B", pdfBuf.Length);

                return Responses.Success;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Failed to generate the PDF file.");
            }

            return Responses.Failure;
        }
    }
}
