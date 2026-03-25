using System.Collections.ObjectModel;
using System.IO;
using System.Net.Http;
using System.Windows;
using System.Windows.Input;
using TradeMonitor.App.Commands;
using TradeMonitor.Core.Dtos;
using TradeMonitor.Core.Models;
using TradeMonitor.Data;
using TradeMonitor.Services;
using TradeMonitor.Services.Configuration;

namespace TradeMonitor.App.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        #region Fields
        private TradeViewModel? _selectedTrade;
        private string _searchText = string.Empty;
        private string _selectedStatus = "All";
        private string _selectedAssetClass = "All";
        private string _statusMessage = "Ready";

        private string _selectedSortOption = "Trade Date (Newest)";
        private int _totalTradesCount;
        private int _approvedTradesCount;
        private decimal _totalNotional;
        private string _largestTradeId = "N/A";

        private bool _isBusy;
        private string _progressMessage = "Idle";
        private readonly Random _random = new();

        private ObservableCollection<TradeViewModel> _trades;
        private ObservableCollection<TradeViewModel> _filteredTrades;

        private readonly TradeRepository _tradeRepository;

        private readonly XmlConfigService _xmlConfigService;
        private readonly string _configFilePath;

        private readonly TradeAnalysisApiService _tradeAnalysisApiService;
        #endregion

        #region Properties
        public ObservableCollection<string> StatusOptions { get; set; }
        public ObservableCollection<string> AssetClassOptions { get; set; }
        public ObservableCollection<string> SortOptions { get; set; }

        public ObservableCollection<TradeViewModel> Trades
        {
            get => _trades;
            set => SetProperty(ref _trades, value);
        }

        public ObservableCollection<TradeViewModel> FilteredTrades
        {
            get => _filteredTrades;
            set => SetProperty(ref _filteredTrades, value);
        }

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                if (SetProperty(ref _isBusy, value))
                {
                    RefreshCommandStates();
                }
            }
        }

        public string ProgressMessage
        {
            get => _progressMessage;
            set => SetProperty(ref _progressMessage, value);
        }

        public TradeViewModel? SelectedTrade
        {
            get => _selectedTrade;
            set
            {
                if (SetProperty(ref _selectedTrade, value))
                {
                    RefreshCommandStates();
                }
            }
        }

        public string SearchText
        {
            get => _searchText;
            set => SetProperty(ref _searchText, value);
        }

        public string SelectedStatus
        {
            get => _selectedStatus;
            set
            {
                if (SetProperty(ref _selectedStatus, value))
                {
                    ApplyFilters();
                }
            }
        }

        public string SelectedAssetClass
        {
            get => _selectedAssetClass;
            set
            {
                if (SetProperty(ref _selectedAssetClass, value))
                {
                    ApplyFilters();
                }
            }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public string SelectedSortOption
        {
            get => _selectedSortOption;
            set
            {
                if (SetProperty(ref _selectedSortOption, value))
                {
                    ApplyFilters();
                }
            }
        }

        public int TotalTradesCount
        {
            get => _totalTradesCount;
            set => SetProperty(ref _totalTradesCount, value);
        }

        public int ApprovedTradesCount
        {
            get => _approvedTradesCount;
            set => SetProperty(ref _approvedTradesCount, value);
        }

        public decimal TotalNotional
        {
            get => _totalNotional;
            set => SetProperty(ref _totalNotional, value);
        }

        public string LargestTradeId
        {
            get => _largestTradeId;
            set => SetProperty(ref _largestTradeId, value);
        }
        #endregion

        #region Commands
        public ICommand LoadTradesCommand { get; }
        public ICommand FilterTradesCommand { get; }
        public ICommand AnalyseTradesCommand { get; }
        public ICommand SaveTradesToDatabaseCommand { get; }
        public ICommand LoadTradesFromDatabaseCommand { get; }
        public ICommand SaveConfigCommand { get; }
        public ICommand LoadConfigCommand { get; }
        #endregion

        #region Constructor
        public MainViewModel()
        {
            _xmlConfigService = new XmlConfigService();
            _configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TradeMonitorConfig.xml");

            Trades = new ObservableCollection<TradeViewModel>();
            FilteredTrades = new ObservableCollection<TradeViewModel>();

            var databasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "trades.db");
            _tradeRepository = new TradeRepository(databasePath);
            _tradeRepository.InitializeDatabase();

            StatusOptions = new ObservableCollection<string>
            {
                "All",
                "New",
                "Pending",
                "Approved",
                "Rejected"
            };

            AssetClassOptions = new ObservableCollection<string>
            {
                "All",
                "IRS",
                "FX",
                "FXO",
                "Bonds"
            };

            SortOptions = new ObservableCollection<string>
            {
                "Trade Date (Newest)",
                "Trade Date (Oldest)",
                "Notional (High to Low)",
                "Notional (Low to High)",
                "Counterparty (A-Z)"
            };

            _tradeAnalysisApiService = new TradeAnalysisApiService(
                new HttpClient
                {
                    BaseAddress = new Uri("http://localhost:8080/")
                });

            LoadTradesCommand = new AsyncRelayCommand(_ => LoadLargeTradeDatasetAsync(), _ => !IsBusy);
            FilterTradesCommand = new RelayCommand(_ => ApplyFilters(), _ => !IsBusy);
            AnalyseTradesCommand = new AsyncRelayCommand(_ => AnalyseSelectedTradeAsync(), _ => SelectedTrade != null && !IsBusy);
            SaveTradesToDatabaseCommand = new AsyncRelayCommand(_ => SaveTradesToDatabaseAsync(), _ => Trades.Any() && !IsBusy);
            LoadTradesFromDatabaseCommand = new AsyncRelayCommand(_ => LoadTradesFromDatabaseAsync(), _ => !IsBusy);
            SaveConfigCommand = new AsyncRelayCommand(_ => SaveConfigAsync(), _ => !IsBusy);
            LoadConfigCommand = new AsyncRelayCommand(_ => LoadConfigAsync(), _ => !IsBusy);
        }
        #endregion

        #region Data Loading
        private async Task LoadLargeTradeDatasetAsync()
        {
            try
            {
                IsBusy = true;
                ProgressMessage = "Generating large trade dataset...";
                StatusMessage = "Loading trades...";

                var generatedTrades = await Task.Run(() =>
                {
                    var statuses = new[] { "New", "Pending", "Approved", "Rejected" };
                    var assetClasses = new[] { "IRS", "FX", "FXO", "Bonds" };
                    var books = new[] { "IRS-BOOK-1", "FX-BOOK-2", "FXO-BOOK-3", "BOND-BOOK-4" };
                    var counterparties = new[] { "Goldman Sachs", "JP Morgan", "Barclays", "Citi", "Morgan Stanley", "HSBC" };
                    var traders = new[] { "Charlie", "Alice", "Ben", "Sarah", "James", "Nina" };
                    var regions = new[] { "London", "New York", "Singapore", "Tokyo" };

                    var trades = new List<TradeViewModel>();

                    for (int i = 1; i <= 1000; i++)
                    {
                        trades.Add(new TradeViewModel
                        {
                            TradeId = $"TRD-{1000 + i}",
                            Book = books[_random.Next(books.Length)],
                            Counterparty = counterparties[_random.Next(counterparties.Length)],
                            AssetClass = assetClasses[_random.Next(assetClasses.Length)],
                            TradeDate = DateTime.Today.AddDays(-_random.Next(0, 30)),
                            Notional = _random.Next(100000, 10000000),
                            Status = statuses[_random.Next(statuses.Length)],
                            Trader = traders[_random.Next(traders.Length)],
                            Region = regions[_random.Next(regions.Length)],
                            IsHighPriority = _random.Next(0, 5) == 0
                        });
                    }

                    return trades;
                });

                Trades = new ObservableCollection<TradeViewModel>(generatedTrades);

                ApplyFilters();

                StatusMessage = $"Loaded {Trades.Count:N0} trades";
                ProgressMessage = "Trade load complete";
            }
            finally
            {
                IsBusy = false;
            }
        }
        #endregion

        #region Filtering and Summary
        private void ApplyFilters()
        {
            IEnumerable<TradeViewModel> query = Trades;

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                string search = SearchText.Trim().ToLower();

                query = query.Where(t =>
                    t.TradeId.ToLower().Contains(search) ||
                    t.Book.ToLower().Contains(search) ||
                    t.Counterparty.ToLower().Contains(search) ||
                    t.Trader.ToLower().Contains(search) ||
                    t.Region.ToLower().Contains(search));
            }

            if (SelectedStatus != "All")
            {
                query = query.Where(t => t.Status == SelectedStatus);
            }

            if (SelectedAssetClass != "All")
            {
                query = query.Where(t => t.AssetClass == SelectedAssetClass);
            }

            query = SelectedSortOption switch
            {
                "Trade Date (Newest)" => query
                    .OrderByDescending(t => t.IsHighPriority)
                    .ThenByDescending(t => t.TradeDate),

                "Trade Date (Oldest)" => query
                    .OrderByDescending(t => t.IsHighPriority)
                    .ThenBy(t => t.TradeDate),

                "Notional (High to Low)" => query
                    .OrderByDescending(t => t.IsHighPriority)
                    .ThenByDescending(t => t.Notional),

                "Notional (Low to High)" => query
                    .OrderByDescending(t => t.IsHighPriority)
                    .ThenBy(t => t.Notional),

                "Counterparty (A-Z)" => query
                    .OrderByDescending(t => t.IsHighPriority)
                    .ThenBy(t => t.Counterparty),

                _ => query
                    .OrderByDescending(t => t.IsHighPriority)
                    .ThenByDescending(t => t.TradeDate)
            };

            var results = query.ToList();

            FilteredTrades = new ObservableCollection<TradeViewModel>(results);

            UpdateSummary(results);

            StatusMessage = $"Showing {FilteredTrades.Count} trades";
        }

        private void UpdateSummary(List<TradeViewModel> trades)
        {
            TotalTradesCount = trades.Count;
            ApprovedTradesCount = trades.Count(t => t.Status == "Approved");
            TotalNotional = trades.Sum(t => t.Notional);

            var largestTrade = trades
                .OrderByDescending(t => t.Notional)
                .FirstOrDefault();

            LargestTradeId = largestTrade?.TradeId ?? "N/A";
        }
        #endregion

        #region Analysis
        private async Task AnalyseSelectedTradeAsync()
        {
            if (SelectedTrade == null)
            {
                return;
            }

            try
            {
                IsBusy = true;
                ProgressMessage = $"Sending {SelectedTrade.TradeId} to Java backend...";
                StatusMessage = "Calling backend analysis service...";

                var request = new TradeAnalysisRequestDto
                {
                    TradeId = SelectedTrade.TradeId,
                    Counterparty = SelectedTrade.Counterparty,
                    AssetClass = SelectedTrade.AssetClass,
                    Notional = SelectedTrade.Notional,
                    Region = SelectedTrade.Region
                };

                var result = await _tradeAnalysisApiService.AnalyseTradeAsync(request);

                if (result == null)
                {
                    StatusMessage = "No response returned from backend";
                    ProgressMessage = "Backend call failed";
                    return;
                }

                MessageBox.Show(
                    $"Trade: {result.TradeId}\n" +
                    $"Risk Score: {result.RiskScore}\n" +
                    $"Latency: {result.LatencyMs} ms\n" +
                    $"Comment: {result.Comment}",
                    "Java Backend Analysis Result",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                StatusMessage = $"Backend analysis completed for {result.TradeId}";
                ProgressMessage = "Java backend analysis complete";
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Failed to call Java backend.\n\n{ex.Message}",
                    "Backend Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                StatusMessage = "Backend call failed";
                ProgressMessage = "Error calling Java backend";
            }
            finally
            {
                IsBusy = false;
            }
        }
        #endregion

        #region Command Helpers
        private void RefreshCommandStates()
        {
            if (LoadTradesCommand is AsyncRelayCommand loadCommand)
            {
                loadCommand.RaiseCanExecuteChanged();
            }

            if (AnalyseTradesCommand is AsyncRelayCommand analyseCommand)
            {
                analyseCommand.RaiseCanExecuteChanged();
            }

            if (SaveTradesToDatabaseCommand is AsyncRelayCommand saveCommand)
            {
                saveCommand.RaiseCanExecuteChanged();
            }

            if (LoadTradesFromDatabaseCommand is AsyncRelayCommand loadDbCommand)
            {
                loadDbCommand.RaiseCanExecuteChanged();
            }

            if (SaveConfigCommand is AsyncRelayCommand saveConfigCommand)
            {
                saveConfigCommand.RaiseCanExecuteChanged();
            }

            if (LoadConfigCommand is AsyncRelayCommand loadConfigCommand)
            {
                loadConfigCommand.RaiseCanExecuteChanged();
            }

            CommandManager.InvalidateRequerySuggested();
        }
        #endregion

        #region Database Operations
        private async Task SaveTradesToDatabaseAsync()
        {
            try
            {
                IsBusy = true;
                ProgressMessage = "Saving trades to SQLite database...";
                StatusMessage = "Saving trades...";

                var tradeModels = Trades.Select(t => t.ToModel()).ToList();

                await Task.Run(() =>
                {
                    _tradeRepository.SaveTrades(tradeModels);
                });

                ProgressMessage = "Database save complete";
                StatusMessage = $"Saved {tradeModels.Count:N0} trades to database";
            }
            finally
            {
                IsBusy = false;
            }
        }
        #endregion

        #region Database Operations (continued)
        private async Task LoadTradesFromDatabaseAsync()
        {
            try
            {
                IsBusy = true;
                ProgressMessage = "Loading trades from SQLite database...";
                StatusMessage = "Loading trades from database...";

                var loadedTrades = await Task.Run(() =>
                {
                    return _tradeRepository.LoadTrades();
                });

                var loadedViewModels = loadedTrades
                    .Select(TradeViewModel.FromModel)
                    .ToList();

                Trades = new ObservableCollection<TradeViewModel>(loadedViewModels);
                ApplyFilters();

                ProgressMessage = "Database load complete";
                StatusMessage = $"Loaded {Trades.Count:N0} trades from database";
            }
            finally
            {
                IsBusy = false;
            }
        }

        #endregion

        #region Configuration Operations
        private async Task SaveConfigAsync()
        {
            try
            {
                IsBusy = true;
                ProgressMessage = "Saving XML configuration...";
                StatusMessage = "Saving configuration...";

                var config = new TradeMonitorConfig
                {
                    SearchText = SearchText,
                    SelectedStatus = SelectedStatus,
                    SelectedAssetClass = SelectedAssetClass,
                    SelectedSortOption = SelectedSortOption
                };

                await Task.Run(() =>
                {
                    _xmlConfigService.SaveConfig(_configFilePath, config);
                });

                ProgressMessage = "Configuration save complete";
                StatusMessage = "Configuration saved to XML";
            }
            finally
            {
                IsBusy = false;
            }
        }
        #endregion

        #region Configuration Operations (continued)
        private async Task LoadConfigAsync()
        {
            try
            {
                IsBusy = true;
                ProgressMessage = "Loading XML configuration...";
                StatusMessage = "Loading configuration...";

                var config = await Task.Run(() =>
                {
                    return _xmlConfigService.LoadConfig(_configFilePath);
                });

                SearchText = config.SearchText;
                SelectedStatus = config.SelectedStatus;
                SelectedAssetClass = config.SelectedAssetClass;
                SelectedSortOption = config.SelectedSortOption;

                ApplyFilters();

                ProgressMessage = "Configuration load complete";
                StatusMessage = "Configuration loaded from XML";
            }
            finally
            {
                IsBusy = false;
            }
        }

        #endregion

    }
}