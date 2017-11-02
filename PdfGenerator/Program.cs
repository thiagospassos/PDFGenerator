using Autofac;
using PdfGenerator.Autofac;
using ManyConsole;
using Serilog;
using SerilogTimings;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace PdfGenerator
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            // Handle the exceptions that crashes the console application.
            AppDomain.CurrentDomain.UnhandledException += UnhadledExceptions;

            // All settings are in the App.config.
            // Using is used to make sure that all of the logs are being flushed by the time the console app closes. (like sending logs to Seq)
            using (var logger = new LoggerConfiguration().ReadFrom.AppSettings().CreateLogger())
            {
                Log.Logger = logger.ForContext("Version", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());

                try
                {
                    Log.Information("{ApplicationName} starting with arguments: " + string.Join(" ", args));

                    using (var container = AutofacContainerFactory.CreateContainer())
                    {
                        using (var scope = container.BeginLifetimeScope())
                        {
                            var commands = container.Resolve<IEnumerable<ConsoleCommand>>();

                            int result;
                            using (Operation.Time("Command for `" + string.Join(" ", args) + "`"))
                            {
                                result = ConsoleCommandDispatcher.DispatchCommand(commands, args, Console.Out);
                            }

                            if (System.Diagnostics.Debugger.IsAttached)
                            {
                                Console.WriteLine("Press enter to continue...");
                                Console.ReadLine();
                            }

                            Log.Information("{ApplicationName} returns {ReturnCode}", ConfigurationManager.AppSettings["serilog:enrich:with-property:ApplicationName"], result);

                            return result;
                        }
                    }
                }
                catch (Exception e)
                {
                    UnhandledExceptions(e);
                }
            }

            return -1;
        }

        private static void UnhadledExceptions(object sender, UnhandledExceptionEventArgs e)
        {
            if (Log.Logger != null && e.ExceptionObject is Exception)
            {
                UnhandledExceptions((Exception)e.ExceptionObject);

                if (e.IsTerminating)
                {
                    Log.CloseAndFlush();
                }
            }
        }

        private static void UnhandledExceptions(Exception e)
        {
            Log.Logger?.Error(e, "Console application crashed");
        }
    }
}
