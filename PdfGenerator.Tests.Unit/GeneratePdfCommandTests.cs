using ManyConsole;
using Serilog;
using System.IO;
using PdfGenerator.Commands;
using Xunit;

namespace PdfGenerator.Tests.Unit
{
    public class GeneratePdfCommandTests
    {
        [Fact]
        public void GeneratePdfShouldFailIfNotArguments()
        {
            var logger = new LoggerConfiguration().CreateLogger();
            var generatePdfCommand = new GeneratePdfCommand(logger);

            TextWriter output = new StringWriter();
            var exitCode = ConsoleCommandDispatcher.DispatchCommand(generatePdfCommand, new string[0], output);

            Assert.Equal(-1, exitCode);
        }

        // Due the issues with wkhtmltopdf DLL the unit test might hang forever on second attempt.
        // The unit test is enabled for Release mode so that the build server fails if something is wrong.
#if !DEBUG
        [Fact]
#endif
        public void GeneratePdfShouldGenerateFile()
        {
            var logger = new LoggerConfiguration().CreateLogger();
            var generatePdfCommand = new GeneratePdfCommand(logger);

            string targetFile = Path.GetFullPath(@"Templates\CaseReport.pdf");
            if (File.Exists(targetFile))
            {
                File.Delete(targetFile);
            }

            TextWriter output = new StringWriter();
            string[] arguments = {
                "--name:\"Test document\"",
                $"--source:\"{Path.GetFullPath(@"Templates\CaseReport.html")}\"",
                $"--target:\"{Path.GetFullPath(@"Templates\CaseReport.pdf")}\""
            };

            int exitCode = ConsoleCommandDispatcher.DispatchCommand(generatePdfCommand, arguments, output);
            bool targetFileExists = File.Exists(targetFile);

            Assert.Equal(Responses.Success, exitCode);
            Assert.True(targetFileExists);

            if (targetFileExists)
            {
                File.Delete(targetFile);
            }
        }


        [Fact]
        public void GeneratePdfShouldFailIfIncorrectSourceFile()
        {
            var logger = new LoggerConfiguration().CreateLogger();
            var generatePdfCommand = new GeneratePdfCommand(logger);

            TextWriter output = new StringWriter();
            string[] arguments = {
                "--name:\"Test document\"",
                $"--source:\"{Path.GetFullPath(@"Templates\MissingFile.html")}\"",
                $"--target:\"{Path.GetFullPath(@"Templates\CaseReport.pdf")}\""
            };

            var exitCode = ConsoleCommandDispatcher.DispatchCommand(generatePdfCommand, arguments, output);

            Assert.Equal(Responses.Failure, exitCode);
        }
    }
}
