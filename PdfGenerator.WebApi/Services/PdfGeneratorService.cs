using Serilog;
using SerilogTimings;
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace PdfGenerator.WebApi.Services
{
    public class PdfGeneratorService : IPdfGeneratorService
    {
        private readonly ILogger _log = Log.ForContext<IPdfGeneratorService>();

        public byte[] GeneratePdf(string documentName, byte[] data)
        {
            try
            {
                string html = Encoding.UTF8.GetString(data);

                _log.Information("Started creating PDF document: {@document}", documentName);
                using (Operation.Time("PDF for {DocumentName} generating", documentName))
                {

                    string folder = Path.Combine(Path.GetTempPath(), "BCE");
                    if (!Directory.Exists(folder))
                    {
                        Directory.CreateDirectory(folder);
                    }

                    string sourceFileName = $"{Path.Combine(folder, Guid.NewGuid().ToString())}.html";
                    string targetFileName = $"{Path.Combine(folder, Guid.NewGuid().ToString())}.pdf";

                    File.WriteAllText(sourceFileName, html);

                    var info = new ProcessStartInfo();

                    byte[] pdfData = null;
                    using (Process process = new Process())
                    {
                        process.StartInfo.FileName = GetApplicationPath();

                        process.StartInfo.Arguments = string.Join(" ", new string[]
                        {
                            "generate-pdf",
                            $"--name:\"{documentName}\"",
                            $"--source:\"{sourceFileName}\"",
                            $"--target:\"{targetFileName}\""
                        });

                        process.StartInfo.RedirectStandardOutput = true;
                        process.StartInfo.RedirectStandardError = true;
                        process.StartInfo.UseShellExecute = false;
                        process.Start();

                        // Wait up to 5 minutes
                        process.WaitForExit(5 * 60 * 1000);

                        if (process.ExitCode == 0 && File.Exists(targetFileName))
                        {
                            pdfData = File.ReadAllBytes(targetFileName);
                        }
                        else if (process.ExitCode != 0)
                        {
                            Log.Logger.Warning("PDF generation exited with error code {PdfGenerationExitCode}",
                                process.ExitCode);
                        }
                        else
                        {
                            Log.Logger.Warning("PDF generation was success but the file is missing");
                        }
                    }

                    if (File.Exists(sourceFileName))
                    {
                        File.Delete(sourceFileName);
                    }

                    if (File.Exists(targetFileName))
                    {
                        File.Delete(targetFileName);
                    }

                    _log.Information("Finished creating PDF document: {@document}", documentName);
                    return pdfData;
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex,"Failed while creating PDF document: {@document}", documentName);
                throw;
            }
        }

        private string GetApplicationPath()
        {
            string filePath = ConfigurationManager.AppSettings["PdfGeneratorApp"];
            if (filePath.IndexOf(":", StringComparison.InvariantCultureIgnoreCase) < 0)
            {
                string root = AppDomain.CurrentDomain.BaseDirectory;
                filePath = Path.Combine(root, filePath);
            }

            return filePath;
        }
    }
}