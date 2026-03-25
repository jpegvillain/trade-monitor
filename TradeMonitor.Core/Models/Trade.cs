namespace TradeMonitor.Core.Models
{
    public class Trade
    {
        public string TradeId { get; set; } = string.Empty;
        public string Book { get; set; } = string.Empty;
        public string Counterparty { get; set; } = string.Empty;
        public string AssetClass { get; set; } = string.Empty;
        public DateTime TradeDate { get; set; }
        public decimal Notional { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Trader { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
        public bool IsHighPriority { get; set; }
    }
}