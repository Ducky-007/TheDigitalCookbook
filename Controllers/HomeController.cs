using Microsoft.AspNetCore.Mvc;

namespace TheDigitalCookbook.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}