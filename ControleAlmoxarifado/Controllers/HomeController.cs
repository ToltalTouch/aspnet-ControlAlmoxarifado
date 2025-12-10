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

    public async Task<IActionResult> Index(string? q,
                                        string? size,
                                        string? category = null,
                                        string? status = null,
                                        string? gender = null,
                                        int page = 1,
                                        int pageSize = 10)
    {
        var baseQuery = _db.Itens.AsNoTracking();

        // popular listas de filtro (como já estava)
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

        // popular tamanhos e gêneros para os selects (valores distintos presentes no banco)
        var sizes = await baseQuery
            .Select(i => i.Tamanho)
            .Where(t => t != null && t != "")
            .Select(t => t!.Trim())
            .Distinct()
            .OrderBy(t => t)
            .ToListAsync();

        var genders = await baseQuery
            .Select(i => i.Genero)
            .Where(g => g != null && g != "")
            .Select(g => g!.Trim())
            .Distinct()
            .OrderBy(g => g)
            .ToListAsync();

    // AQUI: construir query aplicando filtros E busca antes da paginação
    // start with IQueryable so Where calls keep the type; apply OrderBy after filters
    IQueryable<Itens> query = baseQuery;

        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(i => i.Item == category); // ajuste se tiver campo Categoria

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
            query = query.Where(i => i.Genero == gender);

        // filtrar por tamanho se informado
        if (!string.IsNullOrWhiteSpace(size))
        {
            var sizeTrim = size.Trim();
            query = query.Where(i => i.Tamanho != null && i.Tamanho.Trim() == sizeTrim);
        }

        // BUSCA: ajustar nomes de propriedades conforme seu modelo (Sku, Item, NumeroSerie)
        if (!string.IsNullOrWhiteSpace(q))
        {
            var qLower = q.ToLower();
            query = query.Where(i =>
                (i.Item != null && i.Item.ToLower().Contains(qLower))
                // uncomment/adjust these if your entity has Sku / NumeroSerie properties:
                // || (i.Sku != null && i.Sku.ToLower().Contains(qLower))
                // || (i.NumeroSerie != null && i.NumeroSerie.ToLower().Contains(qLower))
            );
        }

    // apply ordering after filtering so OrderBy returns an IOrderedQueryable
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
            SizeOptions = sizes,
            GenderOptions = genders,
            SelectedCategory = category,
            SelectedStatus = status,
            SelectedSize = size,
            SelectedGender = gender,
            SearchQuery = q // <-- repassa para a View
        };

        return View(vm);
    }
}
