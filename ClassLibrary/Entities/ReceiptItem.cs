using Azure.AI.DocumentIntelligence;

namespace ClassLibrary.Entities
{
    public class ReceiptItem
    {
        public string Description { get; set; }
        public int Quantity { get; set; }
        public CurrencyValue Price { get; set; }
    }
}
