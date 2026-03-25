namespace TradeMonitor.Core.Dtos
{
    public class TradeAnalysisRequestDto
    {
        public string TradeId { get; set; } = string.Empty;
        public string Counterparty { get; set; } = string.Empty;
        public string AssetClass { get; set; } = string.Empty;
        public decimal Notional { get; set; }
        public string Region { get; set; } = string.Empty;
    }
}