namespace sevenA.Module.Analysis.Services
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Core.Elements;
    using Models;

    public class YahooFinanceDataService
    {
        private static YahooFinanceDataService instance;

        protected YahooFinanceDataService()
        {
            instance = this;
        }

        public static YahooFinanceDataService Instance => instance ?? new YahooFinanceDataService();

        public Task<List<StockData>> GetHistoricalDataAsync(CancellationToken token, string symbol, DateTime startDate)
        {
            return Task.Factory.StartNew(
                () =>
                    {
                        var url = $"http://ichart.yahoo.com/table.csv?s={symbol}&a={startDate.Month - 1}&b={startDate.Day}&c={startDate.Year}";

                        var result = new List<StockData>();
                        try
                        {
                            string data;
                            using (var webclient = new WebClientExtended())
                            {
                                data = webclient.DownloadString(url);
                            }

                            if (!string.IsNullOrEmpty(data))
                            {
                                var allLines = data.Split(new[] { "\n" }, StringSplitOptions.None).ToList().Skip(1);

                                foreach (var line in allLines)
                                {
                                    var values = line.Split(',');

                                    if (values.Length < 4)
                                    {
                                        continue;
                                    }

                                    result.Add(
                                        new StockData
                                        {
                                            Date = DateTime.ParseExact(values[0], "yyyy-MM-dd", CultureInfo.InvariantCulture),
                                            Open = double.Parse(values[1]),
                                            High = double.Parse(values[2]),
                                            Low = double.Parse(values[3]),
                                            Close = double.Parse(values[4])
                                        });
                                }
                            }
                        }
                        catch (Exception)
                        {
                            // ignored
                        }

                        return result;
                    },
                token);
        }

        public Task<StockData> GetLatestAsync(CancellationToken token, string symbol)
        {
            return Task.Factory.StartNew(
                () =>
                    {
                        var url = $"http://download.finance.yahoo.com/d/quotes.csv?s={symbol}&f=nd1l1vkj";

                        var result = new StockData();
                        try
                        {
                            string data;
                            using (var webclient = new WebClientExtended())
                            {
                                data = webclient.DownloadString(url);
                            }

                            if (!string.IsNullOrEmpty(data))
                            {
                                var values =
                                    data.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                                        .Select(x => x.Trim())
                                        .Skip(1).ToList();
                                try
                                {
                                    result.Date = DateTime.ParseExact(
                                        values.ElementAt(0).Replace("\"", string.Empty),
                                        "M/d/yyyy",
                                        CultureInfo.InvariantCulture);

                                    result.Close = double.Parse(values.ElementAt(1));
                                }
                                catch
                                {
                                    result.Date = DateTime.ParseExact(
                                        values.ElementAt(1).Replace("\"", string.Empty),
                                        "M/d/yyyy",
                                        CultureInfo.InvariantCulture);

                                    result.Close = double.Parse(values.ElementAt(2));
                                }
                            }
                        }
                        catch (Exception)
                        {
                            // ignored
                        }

                        return result;
                    },
                token);
        }

        public string GetYahooSymbol(string morningStarSymbol)
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
    }
}