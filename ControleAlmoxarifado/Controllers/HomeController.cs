using System.Diagnostics;
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

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
    {
        var query = _db.Itens.AsNoTracking().OrderBy(i => i.Id); // ajuste a ordenação conforme necessário
        var total = await query.CountAsync();
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        var vm = new HomeIndexViewModel
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalItems = total
        };

        return View(vm);
    }
}
