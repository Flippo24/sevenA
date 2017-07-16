using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

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

    public class GoogleFinanceDataService
    {
        private static GoogleFinanceDataService instance;

        private enum Columns { date = 0, close = 1, open = 4, high = 2, low = 3, volume = 5 }
        private const string COLUMNS_ROW_BEGINNING = "COLUMNS=";
        private const string COLUMNS_ROW = COLUMNS_ROW_BEGINNING + "DATE,CLOSE,HIGH,LOW,OPEN,VOLUME";
        private const string TIME_ZONE_OFFSET_ROW_BEGINNING = "TIMEZONE_OFFSET=";
        private const int EXPECTED_NUMBER_OF_FIELDS = 6;
        private const string BEGINNING_OF_DATE = "a";

        protected GoogleFinanceDataService()
        {
            instance = this;
        }

        public static GoogleFinanceDataService Instance => instance ?? new GoogleFinanceDataService();

        public string GetGoogleFinanceSymbol(string morningStarSymbol)
        {
            var symbolUpperCase = morningStarSymbol.ToUpperInvariant();

            if (symbolUpperCase.StartsWith("XSES"))
            {
                return symbolUpperCase.Replace("XSES:", "SGX:");
            }

            if (symbolUpperCase.StartsWith("XLIS"))
            {
                return symbolUpperCase.Replace("XLIS:", "ELI:");
            }

            return symbolUpperCase;
        }
        
        public Task<List<StockData>> GetHistoricalDataAsync(CancellationToken token, string stockSymbol, DateTime startDate)
        {
            return Task.Factory.StartNew(
                () =>
                {
                    var stockDataPoints = new List<StockData>();

                    try
                    {
                        using (var client = new WebClientExtended())
                        {
                            var exchangeTicker = DownloadURIBuilder.SplitExchangeTickerName(stockSymbol);
                            var result = client.DownloadString(
                                DownloadURIBuilder.GetPricesUrlForRecentData(exchangeTicker.Item1, exchangeTicker.Item2,
                                    startDate, DateTime.Now));

                            using (MemoryStream ms = new MemoryStream(Encoding.Default.GetBytes(result)))
                            {
                                string errorMessage;
                                var data = ProcessStreamMadeOfOneDayLinesToExtractHistoricalData(ms, out errorMessage);

                                if (!String.IsNullOrEmpty(errorMessage))
                                {
                                    return stockDataPoints;
                                }

                                foreach (var dayDataString in data.Split('\n').Select(x => x.Trim())
                                    .Where(x => !string.IsNullOrEmpty(x)))
                                {
                                    var fromStringToStockDataPoint = FromStringToStockData(dayDataString);
                                    if (fromStringToStockDataPoint.Close > 0 &&
                                        fromStringToStockDataPoint.Low > 0 &&
                                        fromStringToStockDataPoint.Open > 0 &&
                                        fromStringToStockDataPoint.High > 0)
                                    {
                                        stockDataPoints.Add(fromStringToStockDataPoint);
                                    }
                                }

                                // latest data
                                var latestdata = GetLatestAsync(token, stockSymbol).Result;
                                if (stockDataPoints.Last().Date < latestdata.Date)
                                {
                                    stockDataPoints.Add(latestdata);
                                }

                                stockDataPoints.Reverse();
                            }

                            return stockDataPoints;

                        }
                    }
                    catch
                    {
                        return stockDataPoints;
                    }
                }, token);
        }

        public Task<StockData> GetLatestAsync(CancellationToken token, string stockSymbol)
        {
            return Task.Factory.StartNew(
                () =>
                {
                    try
                    {
                        using (var client = new WebClientExtended())
                        {
                            var exchangeTicker = DownloadURIBuilder.SplitExchangeTickerName(stockSymbol);
                            var result = client.DownloadString(
                                DownloadURIBuilder.GetPricesUrlForLastQuote(exchangeTicker.Item1,
                                    exchangeTicker.Item2));

                            using (MemoryStream ms = new MemoryStream(Encoding.Default.GetBytes(result)))
                            {
                                string errorMessage;
                                var data =
                                    ProcessStreamMadeOfOneMinuteBarsToExtractMostRecentOHLCVForCurrentDay(ms,
                                        out errorMessage);

                                if (!String.IsNullOrEmpty(errorMessage))
                                {
                                    throw new Exception(errorMessage);
                                }

                                return FromStringToStockData(data);
                            }
                        }
                    }
                    catch
                    {
                        return new StockData();
                    }
                }, token);
        }

        public String ProcessStreamMadeOfOneMinuteBarsToExtractMostRecentOHLCVForCurrentDay(Stream stream, out string errorMessage)
        {
            CultureInfo originalCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            try
            {
                errorMessage = string.Empty;
                var processingData = false;

                var offset = int.MinValue;
                float totalVolume = 0L;
                float open = float.NaN, high = float.MinValue, low = float.MaxValue, close = float.NaN;
                var dateGoogle = DateTime.MinValue;

                var counterOfLines = 0;
                string lastLine = null;
                string[] elements = null;

                bool ExtractValue(Columns column, out float value, out string localMessage)
                {
                    localMessage = null;
                    if (!float.TryParse(elements[(int)column], out value))
                    {
                        localMessage = "Unable to retrieve' " + Enum.GetName(typeof(Columns), open) + "'. Line: " + counterOfLines;
                        return false;
                    }

                    return true;
                }

                using (StreamReader inputStream = new StreamReader(stream))
                {
                    string line;
                    while ((line = inputStream.ReadLine()) != null)
                    {
                        line = line.Trim();
                        bool isFirstLineOfData = false;
                        counterOfLines++;

                        if (String.IsNullOrEmpty(line))
                        {
                            Debug.Fail("Empty line!");
                        }

                        if (!processingData)
                        {
                            if (line.StartsWith(COLUMNS_ROW_BEGINNING))
                            {
                                if (line != COLUMNS_ROW)
                                {
                                    errorMessage = "Unexpected fields ('" + line + "')";
                                    return null;
                                }
                            }
                            else if (line.StartsWith(TIME_ZONE_OFFSET_ROW_BEGINNING))
                            {
                                if (!int.TryParse(line.Substring(TIME_ZONE_OFFSET_ROW_BEGINNING.Length), out offset))
                                {
                                    errorMessage = "Error extracting time zone offset";
                                    return null;
                                }
                            }
                            else if (line.StartsWith(BEGINNING_OF_DATE))
                            {
                                processingData = true;
                                isFirstLineOfData = true;
                            }
                        }

                        elements = null;
                        if (processingData)
                        {
                            lastLine = line;

                            if (!GetElements(line, counterOfLines, ref elements, out errorMessage))
                            {

                                return null;
                            }

                            if (isFirstLineOfData)
                            {
                                if (offset == int.MinValue)
                                {
                                    Debug.Fail("Timezone offset unitialized.");
                                }

                                if (!GetGoogleDate(elements, out dateGoogle, offset))
                                {
                                    errorMessage = "Unable to retrieve date. Line: '" + line + "'.";
                                    return null;
                                }

                                if (!ExtractValue(Columns.open, out open, out errorMessage))
                                {
                                    return null;
                                }
                            }

                            float localHigh;
                            if (!ExtractValue(Columns.high, out localHigh, out errorMessage))
                            {
                                return null;
                            }

                            if (localHigh > high)
                                high = localHigh;

                            float localLow;
                            if (!ExtractValue(Columns.low, out localLow, out errorMessage))
                            {
                                return null;
                            }

                            if (localLow < low)
                            {
                                low = localLow;
                            }

                            //Process volume
                            float localVolume;
                            if (!ExtractValue(Columns.volume, out localVolume, out errorMessage))
                            {
                                return null;
                            }
                            totalVolume += localVolume;
                        }
                    }
                }

                if (!processingData)
                {
                    errorMessage = "File with no data!";
                    return null;
                }

                if (!string.IsNullOrEmpty(lastLine))
                {
                    if (!ExtractValue(Columns.close, out close, out errorMessage))
                    {
                        return null;
                    }
                }

                float[] allValues = { open, high, low, close };

                if (allValues.Any(float.IsNaN))
                {
                    errorMessage = "Unexpected error. Some values weren't initialized.";
                    return null;
                }

                if (!(totalVolume < 0))
                {
                    return FormatData(dateGoogle, open, high, low, close, totalVolume);
                }

                errorMessage = "Incorrect volume. It can't be negative.";
                return null;
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = originalCulture;
            }
        }

        public String ProcessStreamMadeOfOneDayLinesToExtractHistoricalData(Stream str, out string errorMessage)
        {
            errorMessage = String.Empty;

            var stringResult = new StringBuilder();
            StreamReader sr = new StreamReader(str);

            string line;
            var lastInterpretedDate = DateTime.MaxValue;
            var previousDate = DateTime.MaxValue;
            var processingData = false;

            var numberOfLines = 0;
            var atLeastOneLineProcessed = false;
            while ((line = sr.ReadLine()) != null)
            {
                numberOfLines++;

                if (!processingData)
                {
                    if (line.StartsWith(BEGINNING_OF_DATE))
                    {
                        processingData = true;
                    }
                    else
                    {
                        continue;
                    }
                }

                if (line.StartsWith(TIME_ZONE_OFFSET_ROW_BEGINNING))
                {
                    continue;
                }

                string[] elements = null;
                if (!GetElements(line, numberOfLines, ref elements, out errorMessage))
                {
                    return null;
                }

                DateTime dt;
                if (elements[(int)Columns.date].StartsWith(BEGINNING_OF_DATE))
                {
                    dt = lastInterpretedDate = ConvertFromUnixTimestamp(double.Parse(elements[0].Substring(1)));
                }
                else
                {
                    int days2Add;
                    if (!int.TryParse(elements[0], out days2Add))
                    {
                        errorMessage = "Invalid line " + numberOfLines + ": '" + line + "'.";
                        return null;
                    }
                    dt = lastInterpretedDate.AddDays(int.Parse(elements[0]));
                }

                if (dt.Date == previousDate)
                {
                    continue;
                }

                string convertedLine = FormatData(dt, elements[(int)Columns.open], elements[(int)Columns.high], elements[(int)Columns.low],
                    elements[(int)Columns.close], elements[(int)Columns.volume]);

                stringResult.AppendLine(convertedLine);
                previousDate = dt.Date;

                if (!atLeastOneLineProcessed)
                {
                    atLeastOneLineProcessed = true;
                }
            }

            if (!atLeastOneLineProcessed)
            {
                errorMessage = "No data recovered.";
                return string.Empty;
            }

            return stringResult.ToString();
        }

        private DateTime ConvertFromUnixTimestamp(double timestamp)
        {
            var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return origin.AddSeconds(timestamp);
        }

        private void CheckElements(string[] elements)
        {
            if (elements == null || elements.Length != EXPECTED_NUMBER_OF_FIELDS)
            {
                throw new ArgumentException("'elements' was null or had an unexpected number of fields.");
            }
        }

        private bool GetGoogleDate(string[] elements, out DateTime googleDate, int offset)
        {
            CheckElements(elements);
            googleDate = DateTime.MinValue;

            var regexFirstDate = new Regex(@"a\d+");
            if (!regexFirstDate.IsMatch(elements[0]))
            {
                return false;
            }

            if (offset == int.MinValue)
            {
                throw new ArgumentException("Invalid value for offset.");
            }

            long value;
            if (!long.TryParse(elements[0].Substring(1), out value))
            {
                return false;
            }


            googleDate = new DateTime(ConvertFromUnixTimestamp(value).Ticks).AddMinutes(offset).Date;
            return true;
        }

        private string FormatData<T>(DateTime date, T open, T high, T low, T close, T volume)
        {
            return $"{date:yyyy-MM-dd},{open},{high},{low},{close},{volume}";
        }

        private bool GetElements(string line, int counterOfLines, ref string[] elements, out string message)
        {
            message = String.Empty;
            if (String.IsNullOrEmpty(line))
                throw new ArgumentException("'line' empty or null.");

            elements = line.Split(',');

            var returnValue = elements.Length == EXPECTED_NUMBER_OF_FIELDS;
            if (!returnValue)
            {
                message = "Unexpected number of fields at line '" + counterOfLines + "'.";
            }
            return returnValue;
        }

        private StockData FromStringToStockData(string data)
        {
            var dataSplit = data.Split(',');
            var date = Convert.ToDateTime(dataSplit[0]);
            var open = Convert.ToDouble(dataSplit[1]);
            var high = Convert.ToDouble(dataSplit[2]);
            var low = Convert.ToDouble(dataSplit[3]);
            var close = Convert.ToDouble(dataSplit[4]);

            return new StockData
            {
                Date = date,
                Open = open,
                Close = close,
                High = high,
                Low = low
            };
        }

    }
}