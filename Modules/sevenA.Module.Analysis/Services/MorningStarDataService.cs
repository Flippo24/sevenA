namespace SevenA.Module.Analysis.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.VisualBasic.FileIO;

    using sevenA.Core.Elements;
    using sevenA.Module.Analysis.Enums;
    using sevenA.Module.Analysis.Models;

    public class MorningStarDataService
    {
        private static MorningStarDataService instance;

        protected MorningStarDataService()
        {
            instance = this;
        }

        public static MorningStarDataService Instance => instance ?? new MorningStarDataService();

        public Task<List<FinancialRatio>> GetFinancialsAsync(CancellationToken token, string symbol)
        {
            return Task.Factory.StartNew(
                () =>
                    {
                        var result = new List<FinancialRatio>();
                        try
                        {
                            // income statement
                            var url =
                                $"http://financials.morningstar.com/ajax/ReportProcess4CSV.html?&t={symbol}&cur=&reportType=is&period=12&dataType=A&order=asc&columnYear=10&curYearPart=1st5year&rounding=3&view=raw&r=912080&denominatorView=raw&number=3";
                            string data;
                            using (var webclient = new WebClientExtended())
                            {
                                webclient.Headers.Add(
                                    HttpRequestHeader.Cookie,
                                    "mstar=V6314P49NN4O621LKMM0PMK8NP113LL0M06K2N2M0O388MNK649NO6O29LK97K4O7288P2230NMLN8N0M10P7NLOLP1540M32O8K77MO4N00P5NN0M225O354OK7N9NPOM2PLNM238PP47MN84875M7N1M6051L21302F30B1FB0A5B44129C288A0068D6743;");
                                data = webclient.DownloadString(url);
                            }

                            if (!string.IsNullOrEmpty(data))
                            {
                                result.AddRange(this.ProcessFinancials(data, FinancialRatioSectionEnum.IncomeStatement));
                            }

                            // balance sheet
                            url =
                                $"http://financials.morningstar.com/ajax/ReportProcess4CSV.html?&t={symbol}&cur=&reportType=bs&period=12&dataType=A&order=asc&columnYear=10&curYearPart=1st5year&rounding=3&view=raw&r=912080&denominatorView=raw&number=3";
                            using (var webclient = new WebClientExtended())
                            {
                                webclient.Headers.Add(
                                    HttpRequestHeader.Cookie,
                                    "mstar=V6314P49NN4O621LKMM0PMK8NP113LL0M06K2N2M0O388MNK649NO6O29LK97K4O7288P2230NMLN8N0M10P7NLOLP1540M32O8K77MO4N00P5NN0M225O354OK7N9NPOM2PLNM238PP47MN84875M7N1M6051L21302F30B1FB0A5B44129C288A0068D6743;");
                                data = webclient.DownloadString(url);
                            }

                            if (!string.IsNullOrEmpty(data))
                            {
                                result.AddRange(this.ProcessFinancials(data, FinancialRatioSectionEnum.BalanceSheet));
                            }

                            // cashflow statement
                            url =
                                $"http://financials.morningstar.com/ajax/ReportProcess4CSV.html?&t={symbol}&cur=&reportType=cf&period=12&dataType=A&order=asc&columnYear=10&curYearPart=1st5year&rounding=3&view=raw&r=912080&denominatorView=raw&number=3";
                            using (var webclient = new WebClientExtended())
                            {
                                webclient.Headers.Add(
                                    HttpRequestHeader.Cookie,
                                    "mstar=V6314P49NN4O621LKMM0PMK8NP113LL0M06K2N2M0O388MNK649NO6O29LK97K4O7288P2230NMLN8N0M10P7NLOLP1540M32O8K77MO4N00P5NN0M225O354OK7N9NPOM2PLNM238PP47MN84875M7N1M6051L21302F30B1FB0A5B44129C288A0068D6743;");
                                data = webclient.DownloadString(url);
                            }

                            if (!string.IsNullOrEmpty(data))
                            {
                                result.AddRange(
                                    this.ProcessFinancials(data, FinancialRatioSectionEnum.CashflowStatement));
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

        public Task<List<FinancialRatio>> GetKeyRatiosAsync(CancellationToken token, string symbol)
        {
            return Task.Factory.StartNew(
                () =>
                    {
                        var url = $"http://financials.morningstar.com/ajax/exportKR2CSV.html?t={symbol}";

                        var result = new List<FinancialRatio>();
                        try
                        {
                            string data;
                            using (var webclient = new WebClientExtended())
                            {
                                data = webclient.DownloadString(url);
                                if (string.IsNullOrEmpty(data))
                                {
                                    data = webclient.DownloadString(url);
                                }
                            }

                            if (!string.IsNullOrEmpty(data))
                            {
                                result.AddRange(this.ProcessKeyRatios(data));
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

        private IEnumerable<FinancialRatio> ExtractFinancials(
            string stockName,
            IEnumerable<string> allLines,
            IEnumerable<string> datesString,
            FinancialRatioSectionEnum section)
        {
            foreach (var line in allLines)
            {
                using (var csvParser = new TextFieldParser(new StringReader(line)) { Delimiters = new[] { "," } })
                {
                    csvParser.Delimiters = new[] { "," };
                    var fields = csvParser.ReadFields();
                    if (fields == null || fields.Length < 2)
                    {
                        continue;
                    }

                    var quantityName = fields[0];
                    var quantityData =
                        fields.Skip(1).Select(x => !string.IsNullOrEmpty(x) ? double.Parse(x) : (double?)null).ToList();
                    // ReSharper disable once StyleCop.SA1305
                    var pChange = Enumerable.Range(0, quantityData.Count).Select(
                        i =>
                            {
                                if (i == 0)
                                {
                                    return null;
                                }

                                if (quantityData.ElementAt(i - 1) == null)
                                {
                                    return null;
                                }

                                return (quantityData.ElementAt(i) - quantityData.ElementAt(i - 1))
                                       / quantityData.ElementAt(i - 1) * 100.0;
                            });
                    var tup =
                        Enumerable.Range(0, quantityData.Count)
                            .Select(
                                i =>
                                Tuple.Create(datesString.ElementAt(i), quantityData.ElementAt(i), pChange.ElementAt(i)))
                            .ToList();

                    yield return
                        new FinancialRatio { StockName = stockName, Section = section, Name = quantityName, Data = tup };
                }
            }
        }

        private IEnumerable<FinancialRatio> ExtractRatios(
            string stockName,
            IEnumerable<string> allLines,
            IEnumerable<string> datesString,
            FinancialRatioSectionEnum section,
            int fromLine,
            int toLine)
        {
            for (int lineNumber = fromLine; lineNumber <= toLine; lineNumber++)
            {
                var ratio = allLines.ElementAt(lineNumber);

                using (var csvParser = new TextFieldParser(new StringReader(ratio)) { Delimiters = new[] { "," } })
                {
                    csvParser.Delimiters = new[] { "," };
                    var fields = csvParser.ReadFields();
                    if (fields != null)
                    {
                        var ratioName = fields[0];
                        var ratioData =
                            fields.Skip(1).Select(x => !string.IsNullOrEmpty(x) ? double.Parse(x) : (double?)null).ToList();
                        // ReSharper disable once StyleCop.SA1305
                        var pChange = Enumerable.Range(0, ratioData.Count).Select(
                            i =>
                                {
                                    if (i == 0)
                                    {
                                        return null;
                                    }

                                    if (ratioData.ElementAt(i - 1) == null)
                                    {
                                        return null;
                                    }

                                    return (ratioData.ElementAt(i) - ratioData.ElementAt(i - 1))
                                           / ratioData.ElementAt(i - 1) * 100.0;
                                });
                        var tup =
                            Enumerable.Range(0, ratioData.Count)
                                .Select(
                                    i =>
                                    Tuple.Create(datesString.ElementAt(i), ratioData.ElementAt(i), pChange.ElementAt(i)))
                                .ToList();

                        yield return
                            new FinancialRatio { StockName = stockName, Section = section, Name = ratioName, Data = tup };
                    }
                }
            }
        }

        private IEnumerable<FinancialRatio> ProcessFinancials(string data, FinancialRatioSectionEnum type)
        {
            var result = new List<FinancialRatio>();

            try
            {
                var allLines = data.Split(new[] { "\n" }, StringSplitOptions.None).ToList();
                var stockName =
                    allLines.ElementAt(0)
                        .Split(new[] { "(" }, StringSplitOptions.RemoveEmptyEntries)
                        .ElementAt(0)
                        .TrimStart();
                allLines.RemoveAt(0);

                var datesString = allLines.ElementAt(0).Split(new[] { "," }, StringSplitOptions.None).Skip(1);
                result.AddRange(this.ExtractFinancials(stockName, allLines.Skip(1), datesString, type));
            }
            catch (Exception)
            {
                // ignored
            }

            return result;
        }

        private IEnumerable<FinancialRatio> ProcessKeyRatios(string data)
        {
            var result = new List<FinancialRatio>();

            try
            {
                var allLines = data.Split(new[] { "\n" }, StringSplitOptions.None).ToList();
                var stockName =
                    allLines.ElementAt(0)
                        .Split(new[] { "Financial Ratios for" }, StringSplitOptions.RemoveEmptyEntries)
                        .ElementAt(1)
                        .TrimStart();
                allLines.RemoveAt(0);

                // financials section
                var section = FinancialRatioSectionEnum.Financials;
                var datesString = allLines.ElementAt(1).Split(new[] { "," }, StringSplitOptions.None).Skip(1);
                result.AddRange(this.ExtractRatios(stockName, allLines, datesString, section, 2, 16));

                // Profitability
                section = FinancialRatioSectionEnum.Profitability;
                datesString = allLines.ElementAt(19).Split(new[] { "," }, StringSplitOptions.None).Skip(1);
                result.AddRange(this.ExtractRatios(stockName, allLines, datesString, section, 20, 28));

                // Profitability II
                section = FinancialRatioSectionEnum.Profitability;
                datesString = allLines.ElementAt(30).Split(new[] { "," }, StringSplitOptions.None).Skip(1);
                result.AddRange(this.ExtractRatios(stockName, allLines, datesString, section, 31, 38));

                // CashFlow
                section = FinancialRatioSectionEnum.CashFlow;
                datesString = allLines.ElementAt(64).Split(new[] { "," }, StringSplitOptions.None).Skip(1);
                result.AddRange(this.ExtractRatios(stockName, allLines, datesString, section, 65, 69));

                // Financial Health
                section = FinancialRatioSectionEnum.Health;
                datesString = allLines.ElementAt(72).Split(new[] { "," }, StringSplitOptions.None).Skip(1);
                result.AddRange(this.ExtractRatios(stockName, allLines, datesString, section, 73, 92));

                // Liquidity
                section = FinancialRatioSectionEnum.Liquidity;
                datesString = allLines.ElementAt(94).Split(new[] { "," }, StringSplitOptions.None).Skip(1);
                result.AddRange(this.ExtractRatios(stockName, allLines, datesString, section, 95, 98));

                // Efficiency
                section = FinancialRatioSectionEnum.Efficiency;
                datesString = allLines.ElementAt(101).Split(new[] { "," }, StringSplitOptions.None).Skip(1);
                result.AddRange(this.ExtractRatios(stockName, allLines, datesString, section, 102, 109));
            }
            catch (Exception)
            {
                // ignored
            }

            return result;
        }
    }
}