using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ControleAlmoxarifado.Models
{
    public class NewEntryModel
    {
        public int? Id { get; set; }
        [Required(ErrorMessage = "Informe o item.")]
        [StringLength(200)]
        [Display(Name = "Item")]
        public string Item { get; set; }

        [Required(ErrorMessage = "Selecione uma categoria.")]
        [Display(Name = "Categoria")]
        public string Categoria { get; set; }
        public IEnumerable<string> Categories { get; set; } = Enumerable.Empty<string>();

        [Display(Name = "Tamanho")]
        public string Tamanho { get; set; }
        public IEnumerable<string> TamanhoOptions { get; set; } = Enumerable.Empty<string>();

        [Display(Name = "Gênero")]
        public string Genero { get; set; }
        public IEnumerable<string> GeneroOptions { get; set; } = Enumerable.Empty<string>();

        [Required(ErrorMessage = "Informe a quantidade.")]
        [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero.")]
        [Display(Name = "Quantidade")]
        public int Quantidade { get; set; } = 1;

        [Display(Name = "Imagem")]
        public IFormFile Image { get; set; }
        public string ImageUrl { get; set; }

        [Display(Name = "Local")]
        public string Local { get; set; }
        public IEnumerable<string> LocalOptions { get; set; } = Enumerable.Empty<string>();
    }
}