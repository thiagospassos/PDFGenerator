using Serilog;
using SerilogWeb.Classic.Enrichers;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace PdfGenerator.WebApi
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.AppSettings()
                .Enrich.WithMachineName()
                .Enrich.With<HttpRequestIdEnricher>()
                .Enrich.FromLogContext()
                .CreateLogger();

            Log.Information("{ApplicationName} starting");

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}
