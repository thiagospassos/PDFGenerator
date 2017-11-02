using Autofac;
using AutofacSerilogIntegration;
using ManyConsole;

namespace PdfGenerator.Autofac
{
    public class PdfGeneratorModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(GetType().Assembly)
                .AssignableTo<ConsoleCommand>()
                .As<ConsoleCommand>()
                .SingleInstance();

            builder.RegisterLogger();
        }
    }
}