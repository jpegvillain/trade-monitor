using TradeMonitor.Core.Models;
using TradeMonitor.Core.Dtos;

namespace TradeMonitor.App.ViewModels
{
    public class TradeViewModel : ViewModelBase
    {
        #region Fields
        private string _tradeId = string.Empty;
        private string _book = string.Empty;
        private string _counterparty = string.Empty;
        private string _assetClass = string.Empty;
        private DateTime _tradeDate;
        private decimal _notional;
        private string _status = string.Empty;
        private string _trader = string.Empty;
        private string _region = string.Empty;
        private bool _isHighPriority;
        #endregion

        #region Properties
        public string TradeId
        {
            get => _tradeId;
            set => SetProperty(ref _tradeId, value);
        }

        public string Book
        {
            get => _book;
            set => SetProperty(ref _book, value);
        }

        public string Counterparty
        {
            get => _counterparty;
            set => SetProperty(ref _counterparty, value);
        }

        public string AssetClass
        {
            get => _assetClass;
            set => SetProperty(ref _assetClass, value);
        }

        public DateTime TradeDate
        {
            get => _tradeDate;
            set => SetProperty(ref _tradeDate, value);
        }

        public decimal Notional
        {
            get => _notional;
            set => SetProperty(ref _notional, value);
        }

        public string Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        public string Trader
        {
            get => _trader;
            set => SetProperty(ref _trader, value);
        }

        public string Region
        {
            get => _region;
            set => SetProperty(ref _region, value);
        }

        public bool IsHighPriority
        {
            get => _isHighPriority;
            set => SetProperty(ref _isHighPriority, value);
        }
        #endregion

        #region Mapping
        public static TradeViewModel FromModel(Trade trade)
        {
            return new TradeViewModel
            {
                TradeId = trade.TradeId,
                Book = trade.Book,
                Counterparty = trade.Counterparty,
                AssetClass = trade.AssetClass,
                TradeDate = trade.TradeDate,
                Notional = trade.Notional,
                Status = trade.Status,
                Trader = trade.Trader,
                Region = trade.Region,
                IsHighPriority = trade.IsHighPriority
            };
        }

        public Trade ToModel()
        {
            return new Trade
            {
                TradeId = TradeId,
                Book = Book,
                Counterparty = Counterparty,
                AssetClass = AssetClass,
                TradeDate = TradeDate,
                Notional = Notional,
                Status = Status,
                Trader = Trader,
                Region = Region,
                IsHighPriority = IsHighPriority
            };
        }

        public TradeDto ToDto()
        {
            return new TradeDto
            {
                TradeId = TradeId,
                Book = Book,
                Counterparty = Counterparty,
                AssetClass = AssetClass,
                TradeDate = TradeDate.ToString("yyyy-MM-dd"),
                Notional = Notional,
                Status = Status,
                Trader = Trader,
                Region = Region,
                IsHighPriority = IsHighPriority
            };
        }

        public static TradeViewModel FromDto(TradeDto dto)
        {
            return new TradeViewModel
            {
                TradeId = dto.TradeId,
                Book = dto.Book,
                Counterparty = dto.Counterparty,
                AssetClass = dto.AssetClass,
                TradeDate = DateTime.Parse(dto.TradeDate),
                Notional = dto.Notional,
                Status = dto.Status,
                Trader = dto.Trader,
                Region = dto.Region,
                IsHighPriority = dto.IsHighPriority
            };
        }
        #endregion
    }
}