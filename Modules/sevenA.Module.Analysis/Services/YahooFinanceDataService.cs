namespace sevenA.Module.Analysis.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using sevenA.Module.Analysis.Models;

    using YahooFinanceAPI;

    public class YahooFinanceDataService
    {
        private static YahooFinanceDataService instance;

        public static YahooFinanceDataService Instance => instance ?? new YahooFinanceDataService();

        public string GetYahooFinanceSymbol(string morningStarSymbol)
        {
            var symbolUpperCase = morningStarSymbol.ToUpperInvariant();

            if (symbolUpperCase.StartsWith("XSES"))
            {
                return symbolUpperCase.Replace("XSES:", string.Empty) + ".SI";
            }

            if (symbolUpperCase.StartsWith("XLIS"))
            {
                return symbolUpperCase.Replace("XLIS:", string.Empty) + ".LS";
            }

            return symbolUpperCase;
        }

        public async Task<List<StockData>> GetHistoricalDataAsync(
            string stockSymbol,
            DateTime startDate)
        {
            var historicalData = await Historical.GetPriceAsync(stockSymbol, startDate, DateTime.Now);

            return historicalData?.Select(
                    x => new StockData { Date = x.Date, Open = x.Open, Close = x.Close, High = x.High, Low = x.Low })
                .ToList();
        }
    }
}