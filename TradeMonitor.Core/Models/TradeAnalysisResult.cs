namespace TradeMonitor.Core.Models
{
    public class TradeAnalysisResult
    {
        public string TradeId { get; set; } = string.Empty;
        public decimal RiskScore { get; set; }
        public int LatencyMs { get; set; }
        public string Comment { get; set; } = string.Empty;
    }
}