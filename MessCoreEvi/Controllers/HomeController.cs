using Microsoft.AspNetCore.Mvc;

namespace MessCoreEvi.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
