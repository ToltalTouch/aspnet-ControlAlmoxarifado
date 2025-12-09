using System.ComponentModel.DataAnnotations.Schema;

namespace ControleAlmoxarifado.Models
{
    [Table("DB_UNIFORME")]
    public class Itens
    {
        public int Id { get; set; }
        public string Item { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public string Tamanho { get; set; } = string.Empty;
        public int Quantidade { get; set; }
        // Alterado para string pois a tabela importada contém valores textuais (ex: "FEMININO").
        public string Genero { get; set; } = string.Empty;
        public string Local { get; set; } = string.Empty;
        public string ImagemUrl { get; set; } = string.Empty;
    }
}
