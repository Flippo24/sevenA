﻿namespace sevenA.Module.Analysis.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;
    using System.Threading;

    using Constants;

    using Core.Elements;
    using Core.Stats;

    using DevExpress.Mvvm;
    using DevExpress.Mvvm.DataAnnotations;

    using Enums;

    using JetBrains.Annotations;

    using Models;

    using Services;

    using SevenA.Module.Analysis.Services;

    [POCOViewModel]
    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    public class DashboardViewModel : NavigationViewModelBase
    {
        private const double Delta = 1E-6;

        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        private readonly MorningStarDataService _morningStarDataService;

        private readonly ValuationService _valuationService;

        private readonly YahooFinanceDataService _yahooFinanceDataService;

        private double _averageCashFlow;

        private double _initialGrowthRate;

        private double _numberOfShares;

        private double _totalCash;

        private double _waccModified;

        public DashboardViewModel()
        {
            this._morningStarDataService = MorningStarDataService.Instance;
            this._yahooFinanceDataService = YahooFinanceDataService.Instance;
            this._valuationService = ValuationService.Instance;
            this.Favorites = new ObservableCollection<string>();
            this.ProgressLoader = new ProgressLoader();
            this.AllRatios = new ObservableCollection<FinancialRatio>();
            this.RatiosFinancials = new ObservableCollection<FinancialRatio>();
            this.RatiosCashFlow = new ObservableCollection<FinancialRatio>();
            this.RatiosEfficiency = new ObservableCollection<FinancialRatio>();
            this.RatiosHealth = new ObservableCollection<FinancialRatio>();
            this.RatiosLiquidity = new ObservableCollection<FinancialRatio>();
            this.RatiosProfitability = new ObservableCollection<FinancialRatio>();
            this.IncomeStatement = new ObservableCollection<FinancialRatio>();
            this.BalanceSheet = new ObservableCollection<FinancialRatio>();
            this.CashFlowStatement = new ObservableCollection<FinancialRatio>();
            this.StockData = new ObservableCollection<StockData>();
            this.YearsTillTerminal = 10;
        }

        [UsedImplicitly]
        public CountryEnum Country
        {
            get => this.GetProperty(() => this.Country);
            set => this.SetProperty(() => this.Country, value);
        }

        [UsedImplicitly]
        public ObservableCollection<FinancialRatio> AllRatios
        {
            get => this.GetProperty(() => this.AllRatios);
            set => this.SetProperty(() => this.AllRatios, value);
        }

        [UsedImplicitly]
        public double AverageCashFlow
        {
            get => this._averageCashFlow;

            set
            {
                this._averageCashFlow = value;
                this.RaisePropertyChanged(() => this.AverageCashFlow);

                if (this.AllRatios.Any() && Math.Abs(value - default(double)) > Delta)
                    this.CalculateValutionTask();
            }
        }

        [UsedImplicitly]
        public ObservableCollection<FinancialRatio> BalanceSheet
        {
            get => this.GetProperty(() => this.BalanceSheet);
            set => this.SetProperty(() => this.BalanceSheet, value);
        }

        [UsedImplicitly]
        public double BookValue
        {
            get => this.GetProperty(() => this.BookValue);
            set => this.SetProperty(() => this.BookValue, value);
        }

        [UsedImplicitly]
        public double MarketCap
        {
            get => this.GetProperty(() => this.MarketCap);
            set => this.SetProperty(() => this.MarketCap, value);
        }

        [UsedImplicitly]
        public ObservableCollection<FinancialRatio> CashFlowStatement
        {
            get => this.GetProperty(() => this.CashFlowStatement);
            set => this.SetProperty(() => this.CashFlowStatement, value);
        }

        [UsedImplicitly]
        public double COD
        {
            get => this.GetProperty(() => this.COD);
            set => this.SetProperty(() => this.COD, value);
        }

        [UsedImplicitly]
        public double COE
        {
            get => this.GetProperty(() => this.COE);
            set => this.SetProperty(() => this.COE, value);
        }

        [UsedImplicitly]
        public double CurrentGrowth
        {
            get => this.GetProperty(() => this.CurrentGrowth);
            set => this.SetProperty(() => this.CurrentGrowth, value);
        }

        [UsedImplicitly]
        public double DividendYield
        {
            get => this.GetProperty(() => this.DividendYield);
            set => this.SetProperty(() => this.DividendYield, value);
        }

        [UsedImplicitly]
        public double EPS
        {
            get => this.GetProperty(() => this.EPS);
            set => this.SetProperty(() => this.EPS, value);
        }

        [UsedImplicitly]
        public double FastGrowth
        {
            get => this.GetProperty(() => this.FastGrowth);
            set => this.SetProperty(() => this.FastGrowth, value);
        }

        [UsedImplicitly]
        public ObservableCollection<string> Favorites
        {
            get => this.GetProperty(() => this.Favorites);
            set => this.SetProperty(() => this.Favorites, value);
        }

        [UsedImplicitly]
        public double FreeCashFlowPS
        {
            get => this.GetProperty(() => this.FreeCashFlowPS);
            set => this.SetProperty(() => this.FreeCashFlowPS, value);
        }

        [UsedImplicitly]
        public double GrossMargin
        {
            get => this.GetProperty(() => this.GrossMargin);
            set => this.SetProperty(() => this.GrossMargin, value);
        }

        [UsedImplicitly]
        public ObservableCollection<FinancialRatio> IncomeStatement
        {
            get => this.GetProperty(() => this.IncomeStatement);
            set => this.SetProperty(() => this.IncomeStatement, value);
        }

        [UsedImplicitly]
        public double InitialGrowthRate
        {
            get => this._initialGrowthRate;

            set
            {
                this._initialGrowthRate = value;
                this.RaisePropertyChanged(() => this.InitialGrowthRate);
                if (this.AllRatios.Any() && Math.Abs(value - default(double)) > Delta)
                    this.CalculateValutionTask();
            }
        }

        [UsedImplicitly]
        public StockData LatestPrice
        {
            get => this.GetProperty(() => this.LatestPrice);
            set => this.SetProperty(() => this.LatestPrice, value);
        }

        [UsedImplicitly]
        public double MaxAverageCashFlow
        {
            get => this.GetProperty(() => this.MaxAverageCashFlow);

            private set
            {
                this.SetProperty(() => this.MaxAverageCashFlow, value);
                this.RaisePropertiesChanged(() => this.FreeCashFlowSmallStep, () => this.FreeCashFlowBigStep);
            }
        }

        [UsedImplicitly]
        public IMessageBoxService MessageBoxService => this.GetService<IMessageBoxService>();

        [UsedImplicitly]
        public double MinAverageCashFlow
        {
            get => this.GetProperty(() => this.MinAverageCashFlow);

            private set
            {
                this.SetProperty(() => this.MinAverageCashFlow, value);
                this.RaisePropertiesChanged(() => this.FreeCashFlowSmallStep, () => this.FreeCashFlowBigStep);
            }
        }

        [UsedImplicitly]
        public double FreeCashFlowSmallStep => Math.Abs(this.MaxAverageCashFlow - this.MinAverageCashFlow) / 20;

        [UsedImplicitly]
        public double FreeCashFlowBigStep => Math.Abs(this.MaxAverageCashFlow - this.MinAverageCashFlow) / 5;

        [UsedImplicitly]
        public double NetMargin
        {
            get => this.GetProperty(() => this.NetMargin);
            set => this.SetProperty(() => this.NetMargin, value);
        }

        [UsedImplicitly]
        public double OperatingMargin
        {
            get => this.GetProperty(() => this.OperatingMargin);
            set => this.SetProperty(() => this.OperatingMargin, value);
        }

        [UsedImplicitly]
        public double PE
        {
            get => this.GetProperty(() => this.PE);
            set => this.SetProperty(() => this.PE, value);
        }

        [UsedImplicitly]
        public ProgressLoader ProgressLoader { get; }

        [UsedImplicitly]
        public ObservableCollection<FinancialRatio> RatiosCashFlow
        {
            get => this.GetProperty(() => this.RatiosCashFlow);
            set => this.SetProperty(() => this.RatiosCashFlow, value);
        }

        [UsedImplicitly]
        public ObservableCollection<FinancialRatio> RatiosEfficiency
        {
            get => this.GetProperty(() => this.RatiosEfficiency);
            set => this.SetProperty(() => this.RatiosEfficiency, value);
        }

        [UsedImplicitly]
        public ObservableCollection<FinancialRatio> RatiosFinancials
        {
            get => this.GetProperty(() => this.RatiosFinancials);
            set => this.SetProperty(() => this.RatiosFinancials, value);
        }

        [UsedImplicitly]
        public ObservableCollection<FinancialRatio> RatiosHealth
        {
            get => this.GetProperty(() => this.RatiosHealth);
            set => this.SetProperty(() => this.RatiosHealth, value);
        }

        [UsedImplicitly]
        public ObservableCollection<FinancialRatio> RatiosLiquidity
        {
            get => this.GetProperty(() => this.RatiosLiquidity);
            set => this.SetProperty(() => this.RatiosLiquidity, value);
        }

        [UsedImplicitly]
        public ObservableCollection<FinancialRatio> RatiosProfitability
        {
            get => this.GetProperty(() => this.RatiosProfitability);
            set => this.SetProperty(() => this.RatiosProfitability, value);
        }

        [UsedImplicitly]
        public double ROA
        {
            get => this.GetProperty(() => this.ROA);
            set => this.SetProperty(() => this.ROA, value);
        }

        [UsedImplicitly]
        public double ROE
        {
            get => this.GetProperty(() => this.ROE);
            set => this.SetProperty(() => this.ROE, value);
        }

        [UsedImplicitly]
        public double ROIC
        {
            get => this.GetProperty(() => this.ROIC);
            set => this.SetProperty(() => this.ROIC, value);
        }

        [UsedImplicitly]
        public double SlowGrowth
        {
            get => this.GetProperty(() => this.SlowGrowth);
            set => this.SetProperty(() => this.SlowGrowth, value);
        }

        [UsedImplicitly]
        public ObservableCollection<StockData> StockData
        {
            get => this.GetProperty(() => this.StockData);
            set => this.SetProperty(() => this.StockData, value);
        }

        [UsedImplicitly]
        public string StockName
        {
            get => this.GetProperty(() => this.StockName);
            set => this.SetProperty(() => this.StockName, value);
        }

        [UsedImplicitly]
        public string Symbol
        {
            get => this.GetProperty(() => this.Symbol);
            set => this.SetProperty(() => this.Symbol, value);
        }

        [UsedImplicitly]
        public double? WACC
        {
            get => this.GetProperty(() => this.WACC);
            set => this.SetProperty(() => this.WACC, value);
        }

        [UsedImplicitly]
        public double WACCModified
        {
            get => this._waccModified;

            set
            {
                this._waccModified = value;
                this.RaisePropertyChanged(() => this.WACCModified);
                if (this.AllRatios.Any() && Math.Abs(value - default(double)) > Delta)
                    this.CalculateValutionTask();
            }
        }

        [UsedImplicitly]
        public double WorkingCapitalPS
        {
            get => this.GetProperty(() => this.WorkingCapitalPS);
            set => this.SetProperty(() => this.WorkingCapitalPS, value);
        }

        [UsedImplicitly]
        public int YearsTillTerminal
        {
            get => this.GetProperty(() => this.YearsTillTerminal);

            set
            {
                if (this.SetProperty(() => this.YearsTillTerminal, value) && this.AllRatios.Any() && value != default(int))
                    this.CalculateValutionTask();
            }
        }

        [UsedImplicitly]
        public virtual FinancialRatio SelectedFinancialRatio { get; set; }

        [Command]
        [UsedImplicitly]
        public void AddFavorite()
        {
            if (string.IsNullOrEmpty(this.Symbol)) return;

            if (!this.Favorites.Contains(this.GetSymbol(this.Symbol)))
            {
                this.Favorites.Add(this.GetSymbol(this.Symbol));
                this.MessageBoxService.ShowMessage(
                    $"{this.StockName} added to favorites",
                    "Favorites",
                    MessageButton.OK,
                    MessageIcon.Information);
            }
            else
                this.MessageBoxService.ShowMessage(
                    $"{this.StockName} is already in favorites",
                    "Favorites",
                    MessageButton.OK,
                    MessageIcon.Exclamation);
        }

        [Command]
        [UsedImplicitly]
        public async void Search()
        {
            try
            {
                this.Clear();

                this.ProgressLoader.UpdateProgress(MessageConstants.DownloadingRatios, 0);

                var results = await this._morningStarDataService.GetKeyRatiosAsync(this._cancellationTokenSource.Token, this.GetSymbol(this.Symbol));

                this.ProgressLoader.UpdateProgress(MessageConstants.DownloadingRatios, 60);
                this.AllRatios = new ObservableCollection<FinancialRatio>(results.ToList());
                this.ProgressLoader.UpdateProgress(MessageConstants.DownloadingRatios, 70);

                this.RatiosFinancials =
                    new ObservableCollection<FinancialRatio>(
                        results.Where(x => x.Section == FinancialRatioSectionEnum.Financials).ToList());
                this.RatiosProfitability =
                    new ObservableCollection<FinancialRatio>(
                        results.Where(x => x.Section == FinancialRatioSectionEnum.Profitability).ToList());
                this.RatiosCashFlow =
                    new ObservableCollection<FinancialRatio>(
                        results.Where(x => x.Section == FinancialRatioSectionEnum.CashFlow).ToList());
                this.RatiosHealth =
                    new ObservableCollection<FinancialRatio>(
                        results.Where(x => x.Section == FinancialRatioSectionEnum.Health).ToList());
                this.RatiosLiquidity =
                    new ObservableCollection<FinancialRatio>(
                        results.Where(x => x.Section == FinancialRatioSectionEnum.Liquidity).ToList());
                this.RatiosEfficiency =
                    new ObservableCollection<FinancialRatio>(
                        results.Where(x => x.Section == FinancialRatioSectionEnum.Efficiency).ToList());
                this.ProgressLoader.UpdateProgress(MessageConstants.DownloadingRatios, 100);
                this.StockName = this.RatiosFinancials.Any() ? this.RatiosFinancials.First().StockName : string.Empty;

                this.ProgressLoader.UpdateProgress(MessageConstants.DownloadingFinancials, 0);
                var financials =
                    await
                    this._morningStarDataService.GetFinancialsAsync(this._cancellationTokenSource.Token, this.GetSymbol(this.Symbol));
                this.ProgressLoader.UpdateProgress(MessageConstants.DownloadingFinancials, 70);
                this.IncomeStatement =
                    new ObservableCollection<FinancialRatio>(
                        financials.Where(x => x.Section == FinancialRatioSectionEnum.IncomeStatement).ToList());
                this.BalanceSheet =
                    new ObservableCollection<FinancialRatio>(
                        financials.Where(x => x.Section == FinancialRatioSectionEnum.BalanceSheet).ToList());
                this.CashFlowStatement =
                    new ObservableCollection<FinancialRatio>(
                        financials.Where(x => x.Section == FinancialRatioSectionEnum.CashflowStatement).ToList());
                this.AllRatios.AddRange(financials);
                this.ProgressLoader.UpdateProgress(MessageConstants.DownloadingFinancials, 100);

                this.ProgressLoader.UpdateProgress(MessageConstants.DownloadingHistorical, 0);

                DateTime.TryParseExact(this.RatiosFinancials.First().Data.First().Item1, "yyyy-MM", CultureInfo.InvariantCulture, DateTimeStyles.None, out var startDate);

                if (startDate == default(DateTime)) startDate = DateTime.Now.AddYears(-10);

                var prices = await this._yahooFinanceDataService.GetHistoricalDataAsync(
                                 this._yahooFinanceDataService.GetYahooFinanceSymbol(this.GetSymbol(this.Symbol)),
                                 startDate);
                this.ProgressLoader.UpdateProgress(MessageConstants.DownloadingHistorical, 70);
                this.StockData = new ObservableCollection<StockData>(prices);
                this.ProgressLoader.UpdateProgress(MessageConstants.DownloadingHistorical, 100);
                this.ProgressLoader.UpdateProgress(MessageConstants.DownloadingLatest, 0);
                this.LatestPrice = prices.LastOrDefault();

                this.ProgressLoader.UpdateProgress(MessageConstants.DownloadingLatest, 100);

                this.PrepareCurrentIndicators();
                this.AddComposedIndicators();

                this.CalculateValuations();
            }
            catch
            {
                // something wrong .. ignore
            }
            finally
            {
                this.ProgressLoader.IsLoading = false;
            }
        }

        private static bool StringContains(string obj, string substring)
        {
            return obj.IndexOf(substring, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private string GetSymbol(string symbol)
        {
            return CountryConverter.GetPrefix(this.Country) + symbol;
        }

        private void AddComposedIndicators()
        {
            try
            {
                var roe =
                    this.RatiosProfitability.First(x => x.Name.Contains("Return on Equity"))
                        .Data.Where(x => !x.Item1.Equals("TTM"))
                        .ToList();

                var payout =
                    this.RatiosFinancials.First(x => x.Name.Contains("Payout"))
                        .Data.Where(x => !x.Item1.Equals("TTM"))
                        .ToList();

                var shares =
                    this.RatiosFinancials.First(x => x.Name.Contains("Shares"))
                        .Data.Where(x => !x.Item1.Equals("TTM"))
                        .ToList();

                var dividends =
                    this.RatiosFinancials.First(x => x.Name.Contains("Dividends"))
                        .Data.Where(x => !x.Item1.Equals("TTM"))
                        .ToList();

                var interestExpenses =
                    this.IncomeStatement.FirstOrDefault(x => StringContains(x.Name, "Interest Expense")) != null
                        ? this.IncomeStatement.First(x => StringContains(x.Name, "Interest Expense"))
                              .Data.Where(x => !x.Item1.Equals("TTM"))
                              .ToList()
                        : this.GetDefaultData();

                var earningPerShareData =
                    this.RatiosFinancials.First(x => StringContains(x.Name, "Earnings Per Share"))
                        .Data.Where(x => !x.Item1.Equals("TTM"))
                        .ToList();
                var bookValueData =
                    this.RatiosFinancials.First(x => StringContains(x.Name, "Book Value"))
                        .Data.Where(x => !x.Item1.Equals("TTM"))
                        .ToList();

                var shortTermDebtText = this.BalanceSheet.FirstOrDefault(x => StringContains(x.Name, "Short-term debt")) != null
                    ? "Short-term debt"
                    : "Short-term borrowing";

                var shortTermDebt = this.BalanceSheet.FirstOrDefault(x => StringContains(x.Name, shortTermDebtText))
                                    != null
                                        ? this.BalanceSheet.First(x => StringContains(x.Name, shortTermDebtText))
                                              .Data.Where(x => !x.Item1.Equals("TTM"))
                                              .ToList()
                                        : this.GetDefaultData();

                var longTermDebt = this.BalanceSheet.FirstOrDefault(x => StringContains(x.Name, "Long-term debt")) != null
                        ? this.BalanceSheet.First(x => StringContains(x.Name, "Long-term debt")).Data
                            .Where(x => !x.Item1.Equals("TTM")).ToList()
                        : this.GetDefaultData();

                var taxRate =
                    this.RatiosProfitability.First(x => StringContains(x.Name, "Tax Rate"))
                        .Data.Where(x => !x.Item1.Equals("TTM"))
                        .ToList();

                FinancialRatio divYield = new FinancialRatio
                {
                    Section = FinancialRatioSectionEnum.Financials,
                    Name = "Dividend Yield",
                    StockName = this.RatiosFinancials.First().StockName,
                    Data = dividends.ToList()
                };
                FinancialRatio coe = new FinancialRatio
                {
                    Section = FinancialRatioSectionEnum.Financials,
                    Name = "Cost Of Equity",
                    StockName = this.RatiosFinancials.First().StockName,
                    Data = roe.ToList()
                };
                FinancialRatio cod = new FinancialRatio
                {
                    Section = FinancialRatioSectionEnum.Financials,
                    Name = "Cost Of Debt",
                    StockName = this.RatiosFinancials.First().StockName,
                    Data = longTermDebt.ToList()
                };
                FinancialRatio wacc = new FinancialRatio
                {
                    Section = FinancialRatioSectionEnum.Financials,
                    Name = "WACC",
                    StockName = this.RatiosFinancials.First().StockName,
                    Data = longTermDebt.ToList()
                };
                FinancialRatio pe = new FinancialRatio
                {
                    Section = FinancialRatioSectionEnum.Financials,
                    Name = "P/E",
                    StockName = this.RatiosFinancials.First().StockName,
                    Data = shares.ToList()
                };
                FinancialRatio pbv = new FinancialRatio
                {
                    Section = FinancialRatioSectionEnum.Financials,
                    Name = "P/BV",
                    StockName = this.RatiosFinancials.First().StockName,
                    Data = shares.ToList()
                };
                FinancialRatio reinvestment = new FinancialRatio
                {
                    Section = FinancialRatioSectionEnum.Financials,
                    Name = "Reinvestment",
                    StockName = this.RatiosFinancials.First().StockName,
                    Data = shares.ToList()
                };

                this.ProgressLoader.UpdateProgress(MessageConstants.Analysing, 60);

                if (!this.StockData.Any()) return;

                for (int i = 0; i < dividends.Count; i++)
                {
                    try
                    {
                        var price = this.IsValidDateTimeString(dividends[i].Item1)
                                        ? this.StockData.FirstOrDefault(
                                            x => x.Date < DateTime.Parse(dividends[i].Item1)) != null
                                              ? this.StockData.First(x => x.Date < DateTime.Parse(dividends[i].Item1))
                                                    .Close
                                              : this.StockData.Last().Close
                                        : this.LatestPrice.Close;

                        this.DividendYield = dividends[i].Item2.GetValueOrDefault() / price * 100.0;

                        var equity = shares[i].Item2.GetValueOrDefault() * price;

                        divYield.Data[i] = Tuple.Create(
                            dividends[i].Item1,
                            (double?)this.DividendYield,
                            dividends[i].Item3);

                        double? dividendGrowthRate = (1.0 - (payout[i].Item2.GetValueOrDefault() / 100.0)) * roe[i].Item2.GetValueOrDefault();

                        coe.Data[i] = Tuple.Create(roe[i].Item1, this.DividendYield + dividendGrowthRate, roe[i].Item3);
                        cod.Data[i] = Tuple.Create(
                            longTermDebt[i].Item1,
                            this.FilterInfinityNaN(i > 0 ? interestExpenses[i].Item2.GetValueOrDefault() / (longTermDebt[i].Item2.GetValueOrDefault() + shortTermDebt[i].Item2.GetValueOrDefault()) * (1.0 - (taxRate[i].Item2 / 100.0)) * 100.0 : 0.0),
                            longTermDebt[i].Item3);

                        var ratio1 = equity
                                     / (equity + longTermDebt[i].Item2.GetValueOrDefault()
                                        + shortTermDebt[i].Item2.GetValueOrDefault());
                        var ratio2 = (longTermDebt[i].Item2.GetValueOrDefault()
                                      + shortTermDebt[i].Item2.GetValueOrDefault())
                                     / (equity + longTermDebt[i].Item2.GetValueOrDefault()
                                        + shortTermDebt[i].Item2.GetValueOrDefault());
                        var waccValue = (ratio1 * coe.Data[i].Item2.GetValueOrDefault()) + (ratio2 * cod.Data[i].Item2.GetValueOrDefault() * (1.0 - (taxRate[i].Item2.GetValueOrDefault() / 100.0)));
                        wacc.Data[i] = Tuple.Create(
                            longTermDebt[i].Item1,
                            this.FilterInfinityNaN(waccValue),
                            longTermDebt[i].Item3);

                        var priceEarningValue = price / earningPerShareData[i].Item2.GetValueOrDefault();
                        pe.Data[i] = Tuple.Create(shares[i].Item1, this.FilterInfinityNaN(priceEarningValue), shares[i].Item3);
                        var pbvValue = price / bookValueData[i].Item2.GetValueOrDefault();
                        pbv.Data[i] = Tuple.Create(shares[i].Item1, this.FilterInfinityNaN(pbvValue), shares[i].Item3);
                    }
                    catch (Exception)
                    {
                        // ignore
                    }
                }

                this.AllRatios.Add(divYield);
                this.AllRatios.Add(coe);
                this.AllRatios.Add(cod);
                this.AllRatios.Add(wacc);
                this.AllRatios.Add(pe);
                this.AllRatios.Add(pbv);
                this.AllRatios.Add(reinvestment);

                this.COE = coe.Latest.GetValueOrDefault();
                this.COD = cod.Latest.GetValueOrDefault();
                this.WACC = wacc.Data.Last(x => x.Item2.HasValue && x.Item2.Value > 0)?.Item2.GetValueOrDefault();

                this.ProgressLoader.UpdateProgress(MessageConstants.Analysing, 70);
            }
            catch
            {
                // ignore
            }
        }

        private void CalculateValuations()
        {
            this.ProgressLoader.UpdateProgress(MessageConstants.Analysing, 80);

            var freeCashFlow = this.CashFlowStatement.First(x => StringContains(x.Name, "Free cash")).Data.OrderByDescending(x => x.Item1).ToArray();

            this._averageCashFlow = Stats.SimpleAverage(freeCashFlow.Select(x => x.Item2.GetValueOrDefault()).ToArray(), 3);
            this.RaisePropertyChanged(() => this.AverageCashFlow);
            this.MinAverageCashFlow = freeCashFlow.Select(x => x.Item2.GetValueOrDefault()).Min();
            if (Math.Abs(this.MinAverageCashFlow - this._averageCashFlow) < Delta)
                this.MinAverageCashFlow = this.AverageCashFlow > 0 ? this.AverageCashFlow * 0.1 : -100;

            this.MaxAverageCashFlow = freeCashFlow.Select(x => x.Item2.GetValueOrDefault()).Max();
            if (Math.Abs(this.MaxAverageCashFlow - this._averageCashFlow) < Delta)
                this.MinAverageCashFlow = this.AverageCashFlow > 0 ? this.AverageCashFlow * 3 : 100;

            this._initialGrowthRate = 0d;

            this._waccModified = this.WACC.GetValueOrDefault();
            this.RaisePropertyChanged(() => this.WACCModified);

            this._numberOfShares =
                this.RatiosFinancials.First(x => StringContains(x.Name, "Shares"))
                    .Data.Last(x => !x.Item1.Equals("TTM"))
                    .Item2.GetValueOrDefault();

            this.MarketCap = this.LatestPrice.Close * this._numberOfShares;

            var totalCashText = this.BalanceSheet.FirstOrDefault(x => StringContains(x.Name, "Total Cash")) != null
                ? "Total Cash"
                : "Cash and cash equivalents";

            this._totalCash =
                this.BalanceSheet.First(x => StringContains(x.Name, totalCashText))
                    .Data.Last(x => !x.Item1.Equals("TTM"))
                    .Item2.GetValueOrDefault();

            this.CalculateValutionTask();

            this.ProgressLoader.UpdateProgress(MessageConstants.Analysing, 100);
        }

        private void CalculateValutionTask()
        {
            this.SlowGrowth = this._valuationService.GetDcfTwoStages(
                this._numberOfShares,
                this.AverageCashFlow,
                this.InitialGrowthRate - 5.0,
                this.YearsTillTerminal,
                2,
                this.WACCModified) + (this._totalCash / this._numberOfShares);

            this.CurrentGrowth = this._valuationService.GetDcfTwoStages(
                this._numberOfShares,
                this.AverageCashFlow,
                this.InitialGrowthRate,
                this.YearsTillTerminal,
                2,
                this.WACCModified) + (this._totalCash / this._numberOfShares);

            this.FastGrowth = this._valuationService.GetDcfTwoStages(
                this._numberOfShares,
                this.AverageCashFlow,
                this.InitialGrowthRate + 5.0,
                this.YearsTillTerminal,
                2,
                this.WACCModified) + (this._totalCash / this._numberOfShares);
        }

        private void Clear()
        {
            this.AllRatios.Clear();
            this.RatiosFinancials.Clear();
            this.RatiosCashFlow.Clear();
            this.RatiosEfficiency.Clear();
            this.RatiosHealth.Clear();
            this.RatiosLiquidity.Clear();
            this.RatiosProfitability.Clear();
            this.IncomeStatement.Clear();
            this.BalanceSheet.Clear();
            this.CashFlowStatement.Clear();
            this.StockData.Clear();

            this.StockName = string.Empty;
            this.LatestPrice = null;
            this.MarketCap = 0;
            this.PE = 0;
            this.EPS = 0;
            this.DividendYield = 0;
            this.BookValue = 0;
            this.FreeCashFlowPS = 0;
            this.WorkingCapitalPS = 0;
            this.GrossMargin = 0;
            this.OperatingMargin = 0;
            this.NetMargin = 0;
            this.ROA = 0;
            this.ROE = 0;
            this.ROIC = 0;
            this.COE = 0;
            this.COD = 0;
            this.WACC = 0;
            this.WACCModified = 0;

            this.SlowGrowth = 0;
            this.CurrentGrowth = 0;
            this.FastGrowth = 0;
            this.AverageCashFlow = 0;

            this._numberOfShares = 0;
            this._totalCash = 0;
        }

        private double? FilterInfinityNaN(double? value)
        {
            return double.IsInfinity(value.GetValueOrDefault()) || double.IsNaN(value.GetValueOrDefault())
                       ? null
                       : value;
        }

        private bool IsValidDateTimeString(string dateTime)
        {
            return DateTime.TryParseExact(dateTime, "yyyy-MM", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime _);
        }

        private void PrepareCurrentIndicators()
        {
            this.ProgressLoader.UpdateProgress(MessageConstants.Analysing, 0);

            this.EPS =
                this.RatiosFinancials.First(x => x.Name.Contains("Earnings Per Share"))
                    .Data.Last(x => !x.Item1.Equals("TTM"))
                    .Item2.GetValueOrDefault();
            this.PE = this.LatestPrice.Close / this.EPS;
            this.BookValue =
                this.RatiosFinancials.First(x => x.Name.Contains("Book Value"))
                    .Data.Last(x => !x.Item1.Equals("TTM"))
                    .Item2.GetValueOrDefault();
            this.ProgressLoader.UpdateProgress(MessageConstants.Analysing, 10);
            this.FreeCashFlowPS =
                this.RatiosFinancials.First(x => x.Name.Contains("Free Cash Flow Per Share"))
                    .Data.Last(x => !x.Item1.Equals("TTM"))
                    .Item2.GetValueOrDefault();
            this.WorkingCapitalPS =
                this.RatiosFinancials.First(x => x.Name.Contains("Working Capital"))
                    .Data.Last(x => !x.Item1.Equals("TTM"))
                    .Item2.GetValueOrDefault()
                / this.RatiosFinancials.First(x => x.Name.Contains("Shares"))
                      .Data.Last(x => !x.Item1.Equals("TTM"))
                      .Item2.GetValueOrDefault();
            this.ProgressLoader.UpdateProgress(MessageConstants.Analysing, 20);
            this.GrossMargin =
                this.RatiosProfitability.First(x => x.Name.Contains("Gross Margin"))
                    .Data.Last(x => !x.Item1.Equals("TTM"))
                    .Item2.GetValueOrDefault();
            this.OperatingMargin =
                this.RatiosProfitability.First(x => x.Name.Contains("Operating Margin"))
                    .Data.Last(x => !x.Item1.Equals("TTM"))
                    .Item2.GetValueOrDefault();
            this.NetMargin =
                this.RatiosProfitability.First(x => x.Name.Contains("Net Margin"))
                    .Data.Last(x => !x.Item1.Equals("TTM"))
                    .Item2.GetValueOrDefault();
            this.ProgressLoader.UpdateProgress(MessageConstants.Analysing, 30);
            this.ROA =
                this.RatiosProfitability.First(x => x.Name.Contains("Return on Assets"))
                    .Data.Last(x => !x.Item1.Equals("TTM"))
                    .Item2.GetValueOrDefault();
            this.ROE =
                this.RatiosProfitability.First(x => x.Name.Contains("Return on Equity"))
                    .Data.Last(x => !x.Item1.Equals("TTM"))
                    .Item2.GetValueOrDefault();
            this.ROIC =
                this.RatiosProfitability.First(x => x.Name.Contains("Return on Invested Capital"))
                    .Data.Last(x => !x.Item1.Equals("TTM"))
                    .Item2.GetValueOrDefault();
            this.ProgressLoader.UpdateProgress(MessageConstants.Analysing, 50);
        }

        private List<Tuple<string, double?, double?>> GetDefaultData()
        {
            try
            {
                var dates = this.RatiosFinancials.First(x => x.Name.Contains("Shares")).Data.Select(x => x.Item1)
                    .ToArray();

                var data = Enumerable.Range(0, this.RatiosFinancials.First(x => x.Name.Contains("Shares")).Data.Count)
                    .Select(i => Tuple.Create(dates[i], (double?)0d, (double?)0d));

                return data.ToList();
            }
            catch
            {
                return new List<Tuple<string, double?, double?>>();
            }
        }
    }
}