using Autofac;

namespace PdfGenerator.Autofac
{
    public static class AutofacContainerFactory
    {
        public static IContainer CreateContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new PdfGeneratorModule());

            return builder.Build();
        }
    }
}
