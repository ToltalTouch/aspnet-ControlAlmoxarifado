// ...existing code...
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using ControleAlmoxarifado.Data;
using Microsoft.Extensions.DependencyInjection;
using ControleAlmoxarifado.Models;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace ControleAlmoxarifado.Controllers
{
    public class ItemController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<ItemController> _logger;

    // Marca explicitamente o construtor que o DI deve usar para evitar ambiguidade.
    [ActivatorUtilitiesConstructor]
    public ItemController(
            ApplicationDbContext db,
            IConfiguration config,
            IWebHostEnvironment env,
            ILogger<ItemController> logger)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _config = config;
            _env = env;
            _logger = logger;
        }

        // GET: /Item
        public async Task<IActionResult> Index()
        {
            var items = await _db.Itens
                .AsNoTracking()
                .OrderBy(i => i.Id)
                .ToListAsync();

            return View(items);
        }

        // GET: /Item/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _db.Itens.FindAsync(id);
            if (item == null) return NotFound();
            return PartialView("_EditPartial", item);
        }

        // POST: /Item/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Itens model)
        {
            if (id != model.Id) return BadRequest();

            if (!ModelState.IsValid)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return PartialView("_EditPartial", model);
                return View(model);
            }

            // busca a entidade existente e atualiza apenas a Quantidade
            var existing = await _db.Itens.FindAsync(id);
            if (existing == null) return NotFound();

            existing.Quantidade = model.Quantidade;
            // marcar explicitamente apenas a propriedade Quantidade como modificada
            _db.Entry(existing).Property(e => e.Quantidade).IsModified = true;

            try
            {
                await _db.SaveChangesAsync();

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return Json(new { success = true, id = existing.Id, quantidade = existing.Quantidade });

                return RedirectToAction("Index", "Home");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _db.Itens.AnyAsync(e => e.Id == id)) return NotFound();
                throw;
            }
        }
    }
}