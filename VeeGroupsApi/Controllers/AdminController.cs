using Microsoft.AspNetCore.Mvc;

namespace VeeGroupsApi.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
