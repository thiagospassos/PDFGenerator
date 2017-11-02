using ManyConsole;
using Serilog;

namespace PdfGenerator.Commands
{
    public class VersionCommand : ConsoleCommand
    {
        private readonly ILogger _logger;

        public VersionCommand(ILogger logger, string alias = "version")
        {
            _logger = logger;
            IsCommand(alias, "Get CLI version");
        }

        public override int Run(string[] remainingArguments)
        {
            _logger.Information("{Version}", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);
            return Responses.Success;
        }
    }
}
