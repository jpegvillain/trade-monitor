using Microsoft.Data.Sqlite;
using System.Diagnostics;
using TradeMonitor.Core.Models;

namespace TradeMonitor.Data
{
    public class TradeRepository
    {
        private readonly string _connectionString;

        public TradeRepository(string databasePath)
        {
            _connectionString = $"Data Source={databasePath}";
        }

        public void InitializeDatabase()
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText =
            @"
                CREATE TABLE IF NOT EXISTS Trades
                (
                    TradeId TEXT PRIMARY KEY,
                    Book TEXT NOT NULL,
                    Counterparty TEXT NOT NULL,
                    AssetClass TEXT NOT NULL,
                    TradeDate TEXT NOT NULL,
                    Notional REAL NOT NULL,
                    Status TEXT NOT NULL,
                    Trader TEXT NOT NULL,
                    Region TEXT NOT NULL,
                    IsHighPriority INTEGER NOT NULL
                );
            ";

            command.ExecuteNonQuery();
        }

        public void SaveTrades(IEnumerable<Trade> trades)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var transaction = connection.BeginTransaction();

            var deleteCommand = connection.CreateCommand();
            deleteCommand.CommandText = "DELETE FROM Trades;";
            deleteCommand.Transaction = transaction;
            deleteCommand.ExecuteNonQuery();

            foreach (var trade in trades)
            {
                var insertCommand = connection.CreateCommand();
                insertCommand.Transaction = transaction;
                insertCommand.CommandText =
                @"
                    INSERT INTO Trades
                    (
                        TradeId,
                        Book,
                        Counterparty,
                        AssetClass,
                        TradeDate,
                        Notional,
                        Status,
                        Trader,
                        Region,
                        IsHighPriority
                    )
                    VALUES
                    (
                        $tradeId,
                        $book,
                        $counterparty,
                        $assetClass,
                        $tradeDate,
                        $notional,
                        $status,
                        $trader,
                        $region,
                        $isHighPriority
                    );
                ";

                insertCommand.Parameters.AddWithValue("$tradeId", trade.TradeId);
                insertCommand.Parameters.AddWithValue("$book", trade.Book);
                insertCommand.Parameters.AddWithValue("$counterparty", trade.Counterparty);
                insertCommand.Parameters.AddWithValue("$assetClass", trade.AssetClass);
                insertCommand.Parameters.AddWithValue("$tradeDate", trade.TradeDate.ToString("yyyy-MM-dd"));
                insertCommand.Parameters.AddWithValue("$notional", trade.Notional);
                insertCommand.Parameters.AddWithValue("$status", trade.Status);
                insertCommand.Parameters.AddWithValue("$trader", trade.Trader);
                insertCommand.Parameters.AddWithValue("$region", trade.Region);
                insertCommand.Parameters.AddWithValue("$isHighPriority", trade.IsHighPriority ? 1 : 0);

                insertCommand.ExecuteNonQuery();
            }

            transaction.Commit();
        }

        public List<Trade> LoadTrades()
        {
            var trades = new List<Trade>();

            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText =
            @"
                SELECT
                    TradeId,
                    Book,
                    Counterparty,
                    AssetClass,
                    TradeDate,
                    Notional,
                    Status,
                    Trader,
                    Region,
                    IsHighPriority
                FROM Trades;
            ";

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                trades.Add(new Trade
                {
                    TradeId = reader.GetString(0),
                    Book = reader.GetString(1),
                    Counterparty = reader.GetString(2),
                    AssetClass = reader.GetString(3),
                    TradeDate = DateTime.Parse(reader.GetString(4)),
                    Notional = Convert.ToDecimal(reader.GetDouble(5)),
                    Status = reader.GetString(6),
                    Trader = reader.GetString(7),
                    Region = reader.GetString(8),
                    IsHighPriority = reader.GetInt32(9) == 1
                });
            }

            return trades;
        }
    }
}