using System.Collections.Generic;

namespace ControleAlmoxarifado.Models
{
    public class HomeIndexViewModel
    {
        public IEnumerable<Itens> Items { get; set; } = new List<Itens>();
    }
}
