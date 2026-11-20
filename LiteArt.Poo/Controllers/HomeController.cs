using Microsoft.AspNetCore.Mvc;

namespace LiteArt.Poo.Controllers;

public class HomeController : Controller
{
    public IActionResult Index() => View();
}
