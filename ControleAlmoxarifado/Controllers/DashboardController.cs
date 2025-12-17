using Microsoft.AspNetCore.Mvc;

namespace ControleAlmoxarifado.Controllers;

public class DashboardController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
