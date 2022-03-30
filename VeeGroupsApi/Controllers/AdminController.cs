using Microsoft.AspNetCore.Mvc;

namespace VeeGroupsApi.Controllers
{
    [Route("/{action = Index}")]
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return base.Content("Service is working");
        }
        public string Admin(){
            return "admin page is in the development, or will be soon :]";
        }
    }
}
