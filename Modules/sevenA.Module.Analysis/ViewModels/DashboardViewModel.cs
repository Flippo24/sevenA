namespace sevenA.Module.Analysis.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;
    using System.Threading;

    using Constants;

    using Core.Elements;

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
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        private readonly MorningStarDataService _morningStarDataService;

        private readonly YahooFinanceDataService _yahooFinanceDataService;

        private double _numberOfShares;

        private double _equity;

        private double _growth;

        private FinancialRatio _coe;

        private double _averageNetIncome;

        private double _averageEarnings;

        private double _averagePE;

        private double _dividendGrowth;

        public DashboardViewModel()
        {
            _morningStarDataService = MorningStarDataService.Instance;
            _yahooFinanceDataService = YahooFinanceDataService.Instance;
            ProgressLoader = new ProgressLoader();
            AllRatios = new ObservableCollection<FinancialRatio>();
            RatiosFinancials = new ObservableCollection<FinancialRatio>();
            RatiosCashFlow = new ObservableCollection<FinancialRatio>();
            RatiosEfficiency = new ObservableCollection<FinancialRatio>();
            RatiosHealth = new ObservableCollection<FinancialRatio>();
            RatiosLiquidity = new ObservableCollection<FinancialRatio>();
            RatiosProfitability = new ObservableCollection<FinancialRatio>();
            IncomeStatement = new ObservableCollection<FinancialRatio>();
            BalanceSheet = new ObservableCollection<FinancialRatio>();
            CashFlowStatement = new ObservableCollection<FinancialRatio>();
            StockData = new ObservableCollection<StockData>();

            Country = CountryEnum.Singapore;
        }

        [UsedImplicitly]
        public CountryEnum Country
        {
            get => this.GetProperty(() => this.Country);
            set
            {
                this.SetProperty(() => this.Country, value);
                RiskFreeRate = ValuationService.GetRiskFreeRate(Country);
            }
        }

        [UsedImplicitly]
        public ObservableCollection<FinancialRatio> AllRatios
        {
            get => this.GetProperty(() => this.AllRatios);
            set => this.SetProperty(() => this.AllRatios, value);
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
        public double DividendYield
        {
            get => this.GetProperty(() => this.DividendYield);
            set => this.SetProperty(() => this.DividendYield, value);
        }

        [UsedImplicitly]
        public double Dividend
        {
            get => GetProperty(() => Dividend);
            set => SetProperty(() => Dividend, value);
        }

        [UsedImplicitly]
        public double EPS
        {
            get => this.GetProperty(() => this.EPS);
            set => this.SetProperty(() => this.EPS, value);
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
        public StockData LatestPrice
        {
            get => this.GetProperty(() => this.LatestPrice);
            set => this.SetProperty(() => this.LatestPrice, value);
        }

        [UsedImplicitly]
        public IMessageBoxService MessageBoxService => this.GetService<IMessageBoxService>();

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
        public double WorkingCapitalPS
        {
            get => this.GetProperty(() => this.WorkingCapitalPS);
            set => this.SetProperty(() => this.WorkingCapitalPS, value);
        }

        [UsedImplicitly]
        public virtual FinancialRatio SelectedFinancialRatio { get; set; }

        [UsedImplicitly]
        public virtual double RiskFreeRate
        {
            get => GetProperty(() => RiskFreeRate);
            set => this.SetProperty(() => this.RiskFreeRate, value);
        }

        [UsedImplicitly]
        public virtual Valuation Valuation
        {
            get => GetProperty(() => Valuation);
            set => this.SetProperty(() => this.Valuation, value);
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

                RiskFreeRate = ValuationService.GetRiskFreeRate(Country);

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

        [Command]
        [UsedImplicitly]
        public void Valuate()
        {
            SaveRiskFreeRate();
            CalculateValutionTask();
        }

        [UsedImplicitly]
        public bool CanValuate()
        {
            return !ProgressLoader.IsLoading;
        }

        [UsedImplicitly]
        protected void SaveRiskFreeRate()
        {
            ValuationService.SaveRiskFreeRate(Country, RiskFreeRate);
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

                var netIncomeGrowth = this.AllRatios.First(x => StringContains(x.Name, "Net Income 10yr Growth"))
                    .Data.Where(x => !x.Item1.Equals("TTM"))
                    .ToList();

                if (netIncomeGrowth.All(x => !x.Item2.HasValue))
                {
                    netIncomeGrowth = this.AllRatios.First(x => StringContains(x.Name, "Net Income 5yr Growth"))
                        .Data.Where(x => !x.Item1.Equals("TTM"))
                        .ToList();
                }

                FinancialRatio divYield = new FinancialRatio
                {
                    Section = FinancialRatioSectionEnum.Financials,
                    Name = "Dividend Yield",
                    StockName = this.RatiosFinancials.First().StockName,
                    Data = dividends.ToList()
                };
                this._coe = new FinancialRatio
                {
                    Section = FinancialRatioSectionEnum.Financials,
                    Name = "Cost Of Equity",
                    StockName = this.RatiosFinancials.First().StockName,
                    Data = roe.ToList() // calculated properly below
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
                FinancialRatio excessReturns = new FinancialRatio
                {
                    Section = FinancialRatioSectionEnum.Financials,
                    Name = "Excess Returns",
                    StockName = this.RatiosFinancials.First().StockName,
                    Data = roe.ToList() // calculated properly below
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
                                              ? this.StockData.First(x => x.Date > DateTime.Parse(dividends[i].Item1).AddMonths(1))
                                                    .Close
                                              : this.StockData.Last().Close
                                        : this.LatestPrice.Close;

                        var priceEarningValue = price / earningPerShareData[i].Item2.GetValueOrDefault();
                        pe.Data[i] = Tuple.Create(shares[i].Item1, this.FilterInfinityNaN(priceEarningValue), shares[i].Item3);
                        var pbvValue = price / bookValueData[i].Item2.GetValueOrDefault();
                        pbv.Data[i] = Tuple.Create(shares[i].Item1, this.FilterInfinityNaN(pbvValue), shares[i].Item3);

                        this.DividendYield = dividends[i].Item2.GetValueOrDefault() / price * 100.0;
                        this.Dividend = dividends[i].Item2.GetValueOrDefault();

                        _equity = shares[i].Item2.GetValueOrDefault() * price;

                        divYield.Data[i] = Tuple.Create(
                            dividends[i].Item1,
                            (double?)this.DividendYield,
                            dividends[i].Item3);

                        var growth = netIncomeGrowth[i].Item2 / 100d;
                        var coeImplicit = ((1d + growth) / pe.Data[i].Item2) + growth;

                        this._coe.Data[i] = Tuple.Create(roe[i].Item1, coeImplicit * 100d, roe[i].Item3);

                        cod.Data[i] = Tuple.Create(
                            longTermDebt[i].Item1,
                            this.FilterInfinityNaN(i > 0 ? interestExpenses[i].Item2.GetValueOrDefault() / (longTermDebt[i].Item2.GetValueOrDefault() + shortTermDebt[i].Item2.GetValueOrDefault()) * (1.0 - (taxRate[i].Item2 / 100.0)) * 100.0 : 0.0),
                            longTermDebt[i].Item3);

                        excessReturns.Data[i] = Tuple.Create(roe[i].Item1, roe[i].Item2 - this._coe.Data[i].Item2, roe[i].Item3);

                        var ratio1 = _equity
                                     / (_equity + longTermDebt[i].Item2.GetValueOrDefault()
                                        + shortTermDebt[i].Item2.GetValueOrDefault());
                        var ratio2 = (longTermDebt[i].Item2.GetValueOrDefault()
                                      + shortTermDebt[i].Item2.GetValueOrDefault())
                                     / (_equity + longTermDebt[i].Item2.GetValueOrDefault()
                                        + shortTermDebt[i].Item2.GetValueOrDefault());
                        var waccValue = (ratio1 * this._coe.Data[i].Item2.GetValueOrDefault()) + (ratio2 * cod.Data[i].Item2.GetValueOrDefault() * (1.0 - (taxRate[i].Item2.GetValueOrDefault() / 100.0)));
                        wacc.Data[i] = Tuple.Create(
                            longTermDebt[i].Item1,
                            this.FilterInfinityNaN(waccValue),
                            longTermDebt[i].Item3);
                    }
                    catch (Exception)
                    {
                        // ignore
                    }
                }

                this.AllRatios.Add(divYield);
                this.AllRatios.Add(this._coe);
                this.AllRatios.Add(cod);
                this.AllRatios.Add(wacc);
                this.AllRatios.Add(excessReturns);
                this.AllRatios.Add(pe);
                this.AllRatios.Add(pbv);
                this.AllRatios.Add(reinvestment);

                this.COE = this._coe.Latest.GetValueOrDefault();
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

            this._numberOfShares =
                this.RatiosFinancials.First(x => StringContains(x.Name, "Shares"))
                    .Data.Last(x => !x.Item1.Equals("TTM"))
                    .Item2.GetValueOrDefault();

            this.MarketCap = this.LatestPrice.Close * this._numberOfShares;

            this._growth = this.AllRatios.First(x => StringContains(x.Name, "EPS 5yr Growth")).Data.Last(x => !x.Item1.Equals("TTM")).Item2.GetValueOrDefault();
            var cleanNetIncome = this.AllRatios.First(x => x.Name.Contains("Net income")).Data.Where(x => !x.Item1.Equals("TTM") && x.Item2.HasValue).ToList();
            this._averageNetIncome =
                (0.6d * cleanNetIncome[cleanNetIncome.Count - 1].Item2
                 + 0.3d * cleanNetIncome[cleanNetIncome.Count - 2].Item2
                 + 0.1d * cleanNetIncome[cleanNetIncome.Count - 3].Item2).GetValueOrDefault();

            var cleanEarnings = this.AllRatios.First(x => x.Name.Contains("Earnings Per Share")).Data.Where(x => !x.Item1.Equals("TTM")).ToList();
            this._averageEarnings = cleanEarnings.Where(x => x.Item2.HasValue).Skip(cleanEarnings.Count - 3).Take(3).Select(x => x.Item2).Average().GetValueOrDefault();
            var cleanPE = this.AllRatios.First(x => x.Name.Contains("P/E")).Data.Where(x => !x.Item1.Equals("TTM")).ToList();
            this._averagePE = cleanPE.Where(x => x.Item2.HasValue).Select(x => x.Item2).Average().GetValueOrDefault();

            var cleanDividend = this.RatiosFinancials.First(x => x.Name.Contains("Dividends")).Data.Where(x => !x.Item1.Equals("TTM") && x.Item2.HasValue).ToList();
            var firstDividend = cleanDividend.FirstOrDefault(x => x.Item2 > 0);
            var lastDividend = cleanDividend.LastOrDefault(x => x.Item2 > 0);
            var years = (DateTime.Parse(lastDividend?.Item1) - DateTime.Parse(firstDividend?.Item1)).TotalDays / 365d;
            this._dividendGrowth = firstDividend == null ? 0 : ((lastDividend?.Item2 - firstDividend.Item2) / firstDividend.Item2).GetValueOrDefault() / years;

            this.CalculateValutionTask();

            this.ProgressLoader.UpdateProgress(MessageConstants.Analysing, 100);
        }

        private void CalculateValutionTask()
        {
            var cleanCOE = this._coe.Data.Where(x => !x.Item1.Equals("TTM") && x.Item2 > 0).ToList();

            double coe = 0;
            if (cleanCOE.Count >= 3)
            {
                coe = (0.6d * cleanCOE[cleanCOE.Count - 1].Item2 + 0.3d * cleanCOE[cleanCOE.Count - 2].Item2 + 0.1d * cleanCOE[cleanCOE.Count - 3].Item2).GetValueOrDefault();
            }
            else
            {
                coe = cleanCOE.Last().Item2.GetValueOrDefault();
            }

            this.Valuation = ValuationService.CalculateValuations(
                    Country,
                    coe / 100d,
                    Dividend,
                    this._dividendGrowth,
                    EPS,
                    this.BookValue,
                    this._averageEarnings,
                    this._averagePE,
                    this._averageNetIncome,
                    this._growth,
                    this._numberOfShares);
        }

        private void Clear()
        {
            AllRatios.Clear();
            RatiosFinancials.Clear();
            RatiosCashFlow.Clear();
            RatiosEfficiency.Clear();
            RatiosHealth.Clear();
            RatiosLiquidity.Clear();
            RatiosProfitability.Clear();
            IncomeStatement.Clear();
            BalanceSheet.Clear();
            CashFlowStatement.Clear();
            StockData.Clear();

            StockName = string.Empty;
            LatestPrice = null;
            MarketCap = 0;
            PE = 0;
            EPS = 0;
            DividendYield = 0;
            BookValue = 0;
            FreeCashFlowPS = 0;
            WorkingCapitalPS = 0;
            GrossMargin = 0;
            OperatingMargin = 0;
            NetMargin = 0;
            ROA = 0;
            ROE = 0;
            ROIC = 0;
            COE = 0;
            COD = 0;
            WACC = 0;

            _numberOfShares = 0;
            _equity = 0;
            this._averageNetIncome = 0;

            this.Valuation = null;
            this.SelectedFinancialRatio = null;

            this.RaisePropertyChanged(() => SelectedFinancialRatio);
            this.RaisePropertyChanged(() => Valuation);
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