using System.Xml.Serialization;

namespace TradeMonitor.Services.Configuration
{
    [XmlRoot("TradeMonitorConfig")]
    public class TradeMonitorConfig
    {
        public string SearchText { get; set; } = string.Empty;
        public string SelectedStatus { get; set; } = "All";
        public string SelectedAssetClass { get; set; } = "All";
        public string SelectedSortOption { get; set; } = "Trade Date (Newest)";
    }
}