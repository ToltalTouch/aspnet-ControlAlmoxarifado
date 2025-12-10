using System.Collections.Generic;
using System;

namespace ControleAlmoxarifado.Models
{
    public class HomeIndexViewModel
    {
        public List<Itens> Items { get; set; } = new List<Itens>();

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalItems { get; set; } = 0;
        public int TotalPages => PageSize == 0 ? 0 : (int)Math.Ceiling((double)TotalItems / PageSize);

        // Filtros / seleção usados pela view
        public IEnumerable<string> Categories { get; set; } = Array.Empty<string>();
        public IEnumerable<string> Statuses { get; set; } = Array.Empty<string>();
        public IEnumerable<string> SizeOptions { get; set; } = Array.Empty<string>();
        public IEnumerable<string> GenderOptions { get; set; } = Array.Empty<string>();
        public string? SelectedCategory { get; set; }
        public string? SelectedStatus { get; set; }
        public string? SelectedSize { get; set; }
        public string? SelectedGender { get; set; }
    }
}
