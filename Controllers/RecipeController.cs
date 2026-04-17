using Microsoft.AspNetCore.Mvc;

namespace TheDigitalCookbook.Controllers;

public class RecipeController : Controller
{
    public IActionResult Index()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var username = HttpContext.Session.GetString("Username");

        if (userId == null)
        {
            return RedirectToAction("Login", "Auth");
        }

        ViewBag.Username = username ?? "User";
        return View();
    }
}