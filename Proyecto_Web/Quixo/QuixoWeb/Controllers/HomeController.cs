using Microsoft.AspNetCore.Mvc;

namespace QuixoWeb.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult SelectMode()
        {
            return View();
        }

    }
}
