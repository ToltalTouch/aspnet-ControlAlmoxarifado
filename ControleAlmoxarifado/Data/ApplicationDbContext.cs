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

        // DbSets
        public DbSet<Itens> Itens { get; set; } = default!;
    }
}
