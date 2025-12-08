using Microsoft.AspNetCore.Mvc;
using ControleAlmoxarifado.Models;

namespace ControleAlmoxarifado.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        // Action to handle item entry
        [HttpPost]
        public IActionResult AddItem(Item item)
        {
            if (ModelState.IsValid)
            {
                // Logic to add item to inventory
                // Example: _context.Items.Add(item);
                // _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(item);
        }

        // Action to handle item exit
        [HttpPost]
        public IActionResult RemoveItem(InventoryTransaction transaction)
        {
            if (ModelState.IsValid)
            {
                // Logic to remove item from inventory
                // Example: _context.InventoryTransactions.Add(transaction);
                // _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(transaction);
        }
    }
}