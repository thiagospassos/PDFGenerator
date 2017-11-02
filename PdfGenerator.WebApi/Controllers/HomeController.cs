using System.Web.Mvc;

namespace PdfGenerator.WebApi.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return Redirect("~/swagger/");
        }
    }
}