using Microsoft.EntityFrameworkCore;
using ControleAlmoxarifado.Models;

namespace ControleAlmoxarifado.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Item> Items { get; set; }
        public DbSet<InventoryTransaction> InventoryTransactions { get; set; }
    }
}