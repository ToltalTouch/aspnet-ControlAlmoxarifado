using System;

namespace ControleAlmoxarifado.Models
{
    public class InventoryTransaction
    {
        public int TransactionId { get; set; }
        public int ItemId { get; set; }
        public int Quantity { get; set; }
        public string TransactionType { get; set; } // "Entry" or "Exit"
        public DateTime Date { get; set; }

        public InventoryTransaction()
        {
            Date = DateTime.Now;
        }
    }
}