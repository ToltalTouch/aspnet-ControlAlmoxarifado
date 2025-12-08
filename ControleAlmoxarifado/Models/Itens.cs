using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata;

namespace ControleAlmoxarifado.Models
{
    public class Itens
    {
        public int Id { get; set; }
        public string Item { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public string Tamanho { get; set; } = string.Empty;
        public int Quantidade { get; set; }
        public int Genero { get; set; }
        public string Local { get; set; } = string.Empty;
        public string ImagemUrl { get; set; } = string.Empty;
    }
}
