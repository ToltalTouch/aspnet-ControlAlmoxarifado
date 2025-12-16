using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ControleAlmoxarifado.Models;
using ControleAlmoxarifado.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography.X509Certificates;

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

    [HttpGet]
    public async Task<IActionResult> Sizes(string itemName)
    {
        if (string.IsNullOrWhiteSpace(itemName))
            return BadRequest();

        var sizes = await _db.Itens
            .AsNoTracking()
            .Where(i => i.Item == itemName && !string.IsNullOrWhiteSpace(i.Tamanho))
            .Select(i => i.Tamanho!.Trim())
            .Distinct()
            .OrderBy(t => t)
            .ToListAsync();

        ViewBag.ItemName = itemName;
        return PartialView("_SizesPartial", sizes);
    }

    [HttpGet]
    public async Task<IActionResult> SizeDetails(string itemName, string size, string? gender = null)
    {
        if (string.IsNullOrWhiteSpace(itemName) || string.IsNullOrWhiteSpace(size))
        {
            _logger.LogWarning("SizeDetails called with empty parameters: itemName='{itemName}', size='{size}'", itemName, size);
            return BadRequest();
        }

        var sizeTrim = size.Trim();
        _logger.LogInformation("SizeDetails called for item='{itemName}', size='{size}'", itemName, sizeTrim);

        IQueryable<Itens> vquery = _db.Itens.AsNoTracking().Where(i => i.Item == itemName);

        if (!string.Equals(sizeTrim, "ALL", StringComparison.OrdinalIgnoreCase))
        {
            vquery = vquery.Where(i => i.Tamanho != null && i.Tamanho.Trim() == sizeTrim);
        }

        if (!string.IsNullOrWhiteSpace(gender))
        {
            var gTrim = gender.Trim();
            vquery = vquery.Where(i => i.Genero != null && i.Genero.Trim() == gTrim);
        }

        var variants = await vquery.OrderBy(i => i.Id).ToListAsync();

        _logger.LogInformation("SizeDetails found {count} variants for item='{itemName}', size='{size}'", variants.Count, itemName, sizeTrim);

        ViewBag.ItemName = itemName;
        ViewBag.Size = sizeTrim;
        return PartialView("_SizeDetailsPartial", variants);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public async Task<IActionResult> Index(string? q, string? size, string? category = null, string? status = null,
                                        string? gender = null, int page = 1, int pageSize = 10)
    {
        var baseQuery = _db.Itens.AsNoTracking();

        // build filter option lists
        var categories = await baseQuery
            .Where(i => !string.IsNullOrWhiteSpace(i.Item))
            .Select(i => i.Item!.Trim())
            .Distinct()
            .OrderBy(s => s)
            .ToListAsync();

        var sizes = await baseQuery
            .Where(i => !string.IsNullOrWhiteSpace(i.Tamanho))
            .Select(i => i.Tamanho!.Trim())
            .Distinct()
            .OrderBy(s => s)
            .ToListAsync();

        var genders = await baseQuery
            .Where(i => !string.IsNullOrWhiteSpace(i.Genero))
            .Select(i => i.Genero!)
            .Distinct()
            .OrderBy(g => g)
            .ToListAsync();

        var statuses = new List<string> { "Vazio", "Baixo", "Ok" };

        // threshold used for "Baixo"/"Ok" status
        const int lowThreshold = 5;

        // apply filters to the base query
        IQueryable<Itens> query = baseQuery;

        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(i => i.Item != null && i.Item.Trim() == category.Trim());

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

        if (!string.IsNullOrWhiteSpace(size))
        {
            var sizeTrim = size.Trim();
            query = query.Where(i => i.Tamanho != null && i.Tamanho.Trim() == sizeTrim);
        }

        if (!string.IsNullOrWhiteSpace(q))
        {
            var qLower = q.ToLower();
            query = query.Where(i => (i.Item != null && i.Item.ToLower().Contains(qLower)));
        }

        // group by Item so the list shows one row per item (useful for expand-on-click sizes)
        var grouped = query
            .GroupBy(i => i.Item ?? string.Empty)
            .Select(g => new {
                ItemName = g.Key,
                RepresentativeId = g.Min(i => i.Id),
                TotalQuantity = g.Sum(x => x.Quantidade)
            });

        var totalItems = await grouped.CountAsync();
        var pageData = await grouped
            .OrderBy(x => x.ItemName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        // fetch representative rows for the current page
        var repIds = pageData.Select(p => p.RepresentativeId).ToList();
        var reps = await _db.Itens
            .AsNoTracking()
            .Where(i => repIds.Contains(i.Id))
            .ToDictionaryAsync(i => i.Id);

        var itemsList = pageData.Select(x => {
            reps.TryGetValue(x.RepresentativeId, out var rep);
            return new Itens {
                Id = x.RepresentativeId,
                Item = x.ItemName ?? string.Empty,
                ImagemUrl = rep?.ImagemUrl ?? string.Empty,
                Genero = rep?.Genero ?? string.Empty,
                Local = rep?.Local ?? string.Empty,
                Quantidade = x.TotalQuantity,
                Tamanho = string.Empty
            };
        }).ToList();

        var vm = new HomeIndexViewModel
        {
            Items = itemsList,
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems,
            Categories = categories,
            Statuses = statuses,
            SizeOptions = sizes,
            GenderOptions = genders,
            SelectedCategory = category,
            SelectedStatus = status,
            SelectedSize = size,
            SelectedGender = gender,
            SearchQuery = q
        };

        return View(vm);
    }
}
