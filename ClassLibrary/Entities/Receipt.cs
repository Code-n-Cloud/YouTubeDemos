using Azure.AI.DocumentIntelligence;

namespace ClassLibrary.Entities
{
    public class Receipt
    {
        public string CountryRegion { get; set; }
        public List<ReceiptItem> Items { get; set; }
        public AddressValue MerchantAddress { get; set; }
        public string MerchantName { get; set; }
        public string ReceiptType { get; set; }
        public CurrencyValue SubTotal { get; set; }
        public CurrencyValue Tip { get; set; }
        public CurrencyValue Total { get; set; }
        public CurrencyValue TotalTax { get; set; }
        public DateTimeOffset TransactionDate { get; set; }
        public string TransactionTime { get; set; }
        public string MerchantPhoneNumber { get; set; }
    }
}
