using Microsoft.AspNetCore.Mvc;

namespace AdminLayer.Controllers.Staff
{
    public class ContactsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
