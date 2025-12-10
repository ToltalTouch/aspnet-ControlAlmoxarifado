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

    public async Task<IActionResult> Index(string? size,
                                        string? category = null,
                                        string? status = null,
                                        string? gender = null,
                                        int page = 1,
                                        int pageSize = 10
                                        )
    {
            var baseQuery = _db.Itens.AsNoTracking();

            var categories = await baseQuery
                .Select(i => i.Item)
                .Where(c => c != null && c != "")
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();

            const int lowThreshold = 5;

            var statuses = await baseQuery
                .Select(i => i.Quantidade == 0 ? "Vazio" : (i.Quantidade <= lowThreshold ? "Baixo" : "Ok"))
                .Distinct()
                .OrderBy(s => s)
                .ToListAsync();

            var sizeOptions = await baseQuery
                .Select(i => i.Tamanho)
                .Where(t => t != null && t != "")
                .Distinct()
                .OrderBy(t => t)
                .ToListAsync();

            var genderOptions = await baseQuery
                .Select(i => i.Genero)
                .Where(g => g !=null && g != "")
                .Distinct()
                .OrderBy(g => g)
                .ToListAsync();

        // start with an IQueryable so we can apply filters (Where) which return IQueryable
        IQueryable<Itens> query = baseQuery;
            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(i => i.Item == category);
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                if (status == "Vazio")
                    query = query.Where(i => i.Quantidade == 0);
                else if (status == "Baixo")
                    query = query.Where(i => i.Quantidade > 0 && i.Quantidade <= lowThreshold);
                else if (status == "Ok")
                    query = query.Where(i => i.Quantidade > lowThreshold);
            }

            if (!string.IsNullOrWhiteSpace(gender))
            {
                query = query.Where(i => i.Genero == gender);
            }

        // apply ordering after filtering so OrderBy returns an IOrderedQueryable but stays
        // assignable to IQueryable for subsequent operations
        query = query.OrderBy(i => i.Id);

        var total = await query.CountAsync();
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        var vm = new HomeIndexViewModel
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalItems = total,
            Categories = categories,
            Statuses = statuses,
            SizeOptions = sizeOptions,
                GenderOptions = genderOptions,
            SelectedSize = size,
            SelectedCategory = category,
            SelectedStatus = status
        };
        return View(vm);
    }
}
