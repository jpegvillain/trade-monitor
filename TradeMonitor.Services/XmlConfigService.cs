using System.Xml.Serialization;
using TradeMonitor.Services.Configuration;

namespace TradeMonitor.Services
{
    public class XmlConfigService
    {
        public void SaveConfig(string filePath, TradeMonitorConfig config)
        {
            var serializer = new XmlSerializer(typeof(TradeMonitorConfig));

            using var stream = new FileStream(filePath, FileMode.Create);
            serializer.Serialize(stream, config);
        }

        public TradeMonitorConfig LoadConfig(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return new TradeMonitorConfig();
            }

            var serializer = new XmlSerializer(typeof(TradeMonitorConfig));

            using var stream = new FileStream(filePath, FileMode.Open);
            return (TradeMonitorConfig)serializer.Deserialize(stream)!;
        }
    }
}