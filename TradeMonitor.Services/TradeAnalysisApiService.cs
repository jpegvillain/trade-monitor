using System.Net.Http;
using System.Net.Http.Json;
using TradeMonitor.Core.Dtos;

namespace TradeMonitor.Services
{
    public class TradeAnalysisApiService
    {
        private readonly HttpClient _httpClient;

        public TradeAnalysisApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<TradeAnalysisResponseDto?> AnalyseTradeAsync(TradeAnalysisRequestDto request)
        {
            var response = await _httpClient.PostAsJsonAsync("api/trade-analysis", request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<TradeAnalysisResponseDto>();
        }
    }
}