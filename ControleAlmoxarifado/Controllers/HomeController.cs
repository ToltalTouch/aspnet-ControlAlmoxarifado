using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using ControleAlmoxarifado.Models;
using ControleAlmoxarifado.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ControleAlmoxarifado.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _db;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
    {
        _logger = logger;
        _db = db;
    }

    public IActionResult Index()
    {
        var model = new HomeIndexViewModel
        {
            Items = _db?.Itens?.ToList() ?? new List<Itens>()
        };

        return View(model);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
