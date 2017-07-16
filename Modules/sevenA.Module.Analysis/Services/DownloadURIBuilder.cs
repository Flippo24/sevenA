using System;
using sevenA.Module.Analysis.Properties;

namespace sevenA.Module.Analysis.Services
{
    public static class DownloadURIBuilder
    {
        private const int INTERVAL_TO_DOWNLOAD_HISTORICAL_DATA_WITH_GETPRICES = 24 * 3600;

        public static string GetPricesUrlToDownloadAllData(string exchange, string tickerName, DateTime lastDate)
        {
            return GetPricesUri(exchange, tickerName, INTERVAL_TO_DOWNLOAD_HISTORICAL_DATA_WITH_GETPRICES, GetPeriodToDownloadAllData(lastDate));
        }

        public static string GetPricesUrlForRecentData(string exchange, string tickerName, DateTime startDate, DateTime endDate)
        {
            return GetPricesUri(exchange, tickerName, INTERVAL_TO_DOWNLOAD_HISTORICAL_DATA_WITH_GETPRICES, GetPeriod(startDate, endDate));
        }

        public static string GetPricesUrlForLastQuote(string exchange, string tickerName)
        {
            return GetPricesUri(exchange, tickerName, 60, "1d");
        }

        public static string GetPricesUri(string exchange, string tickerName, int interval, string period)
        {
            if (String.IsNullOrEmpty(period))
            {
                throw new ArgumentException("No 'period'.");
            }

            var formatURI = Settings.Default.GET_PRICES_METHOD_URI_BEGINNING + "?q={0}{1}&i={2}&p={3}&f=d,c,h,l,o,v";

            var exchangeString = string.Empty;
            if (!String.IsNullOrEmpty(exchange))
            {
                exchangeString = "&x=" + exchange;
            }

            return String.Format(formatURI, tickerName, exchangeString, interval, period);
        }

        public static Tuple<string, string> SplitExchangeTickerName(string stockName)
        {
            if (!stockName.Contains(":"))
            {
                throw new ArgumentException($"{stockName} not valid");
            }

            return Tuple.Create(stockName.Split(':')[0], stockName.Split(':')[1]);
        }

        private static string GetPeriodToDownloadAllData(DateTime lastDate)
        {
            var year = lastDate.Year;
            return year - 1970 + "Y";
        }

        private static string GetPeriod(DateTime startDate, DateTime endDate)
        {
            if (endDate.Date < startDate.Date)
            {
                throw new ArgumentException("The ending date can't be lower than the starting date.");
            }

            if (endDate.Year > startDate.Year)
            {
                return endDate.Year - startDate.Year + 1 + "Y";
            }

            var span = endDate - startDate;

            if (span.Days > 50)
            {
                return span.Days % 25 + 1 + "M";
            }

            return (span.Days + 1) + "d";
        }
    }
}