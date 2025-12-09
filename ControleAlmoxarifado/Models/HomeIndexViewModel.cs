using System.Collections.Generic;

namespace ControleAlmoxarifado.Models
{
    public class HomeIndexViewModel
    {
        public List<Itens> Items { get; set; } = new List<Itens>();    
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalItems { get; set; } = 0;
        public int TotalPages => PageSize == 0 ? 0 : (int)Math.Ceiling((double)TotalItems / PageSize);    
    }
}
