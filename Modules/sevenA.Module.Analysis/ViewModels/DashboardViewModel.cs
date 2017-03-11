namespace sevenA.Module.Analysis.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using System.Threading;

    using DevExpress.Mvvm;
    using DevExpress.Mvvm.DataAnnotations;

    using sevenA.Core.Elements;
    using sevenA.Core.Helpers;
    using sevenA.Core.Stats;
    using sevenA.Module.Analysis.Constants;
    using sevenA.Module.Analysis.Enums;
    using sevenA.Module.Analysis.Models;
    using sevenA.Module.Analysis.Services;

    using SevenA.Module.Analysis.Services;

    [POCOViewModel]
    public class DashboardViewModel : NavigationViewModelBase
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        private readonly MorningStarDataService _morningStarDataService;

        private readonly ValuationService _valuationService;

        private readonly YahooFinanceDataService _yahooFinanceDataService;

        private double _averageCashFlow;

        private double _initialGrowthRate;

        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation",
            Justification = "Reviewed. Suppression is OK here.")]
        private double _nShares;

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

        public ObservableCollection<FinancialRatio> AllRatios
        {
            get
            {
                return this.GetProperty(() => this.AllRatios);
            }

            set
            {
                this.SetProperty(() => this.AllRatios, value);
            }
        }

        public double AverageCashFlow
        {
            get
            {
                return this._averageCashFlow;
            }

            set
            {
                this._averageCashFlow = value;
                this.RaisePropertyChanged(() => this.AverageCashFlow);

                if (this.AllRatios.Any() && value != default(double))
                {
                    this.CalculateValutionTask();
                }
            }
        }

        public ObservableCollection<FinancialRatio> BalanceSheet
        {
            get
            {
                return this.GetProperty(() => this.BalanceSheet);
            }

            set
            {
                this.SetProperty(() => this.BalanceSheet, value);
            }
        }

        public double BookValue
        {
            get
            {
                return this.GetProperty(() => this.BookValue);
            }

            set
            {
                this.SetProperty(() => this.BookValue, value);
            }
        }

        public ObservableCollection<FinancialRatio> CashFlowStatement
        {
            get
            {
                return this.GetProperty(() => this.CashFlowStatement);
            }

            set
            {
                this.SetProperty(() => this.CashFlowStatement, value);
            }
        }

        public double COD
        {
            get
            {
                return this.GetProperty(() => this.COD);
            }

            set
            {
                this.SetProperty(() => this.COD, value);
            }
        }

        // ReSharper disable once InconsistentNaming
        public double COE
        {
            get
            {
                return this.GetProperty(() => this.COE);
            }

            set
            {
                this.SetProperty(() => this.COE, value);
            }
        }

        public double CurrentGrowth
        {
            get
            {
                return this.GetProperty(() => this.CurrentGrowth);
            }

            set
            {
                this.SetProperty(() => this.CurrentGrowth, value);
            }
        }

        public double DividendYield
        {
            get
            {
                return this.GetProperty(() => this.DividendYield);
            }

            set
            {
                this.SetProperty(() => this.DividendYield, value);
            }
        }

        public double EPS
        {
            get
            {
                return this.GetProperty(() => this.EPS);
            }

            set
            {
                this.SetProperty(() => this.EPS, value);
            }
        }

        public double FastGrowth
        {
            get
            {
                return this.GetProperty(() => this.FastGrowth);
            }

            set
            {
                this.SetProperty(() => this.FastGrowth, value);
            }
        }

        public ObservableCollection<string> Favorites
        {
            get
            {
                return this.GetProperty(() => this.Favorites);
            }

            set
            {
                this.SetProperty(() => this.Favorites, value);
            }
        }

        public double FreeCashFlowPS
        {
            get
            {
                return this.GetProperty(() => this.FreeCashFlowPS);
            }

            set
            {
                this.SetProperty(() => this.FreeCashFlowPS, value);
            }
        }

        public double GrossMargin
        {
            get
            {
                return this.GetProperty(() => this.GrossMargin);
            }

            set
            {
                this.SetProperty(() => this.GrossMargin, value);
            }
        }

        public ObservableCollection<FinancialRatio> IncomeStatement
        {
            get
            {
                return this.GetProperty(() => this.IncomeStatement);
            }

            set
            {
                this.SetProperty(() => this.IncomeStatement, value);
            }
        }

        public double InitialGrowthRate
        {
            get
            {
                return this._initialGrowthRate;
            }

            set
            {
                this._initialGrowthRate = value;
                this.RaisePropertyChanged(() => this.InitialGrowthRate);
                if (this.AllRatios.Any() && value != default(double))
                {
                    this.CalculateValutionTask();
                }
            }
        }

        public StockData LatestPrice
        {
            get
            {
                return this.GetProperty(() => this.LatestPrice);
            }

            set
            {
                this.SetProperty(() => this.LatestPrice, value);
            }
        }

        public double MaxAverageCashFlow
        {
            get
            {
                return this.GetProperty(() => this.MaxAverageCashFlow);
            }

            private set
            {
                this.SetProperty(() => this.MaxAverageCashFlow, value);
            }
        }

        public IMessageBoxService MessageBoxService => this.GetService<IMessageBoxService>();

        public double MinAverageCashFlow
        {
            get
            {
                return this.GetProperty(() => this.MinAverageCashFlow);
            }

            private set
            {
                this.SetProperty(() => this.MinAverageCashFlow, value);
            }
        }

        public double NetMargin
        {
            get
            {
                return this.GetProperty(() => this.NetMargin);
            }

            set
            {
                this.SetProperty(() => this.NetMargin, value);
            }
        }

        public double OperatingMargin
        {
            get
            {
                return this.GetProperty(() => this.OperatingMargin);
            }

            set
            {
                this.SetProperty(() => this.OperatingMargin, value);
            }
        }

        public double PE
        {
            get
            {
                return this.GetProperty(() => this.PE);
            }

            set
            {
                this.SetProperty(() => this.PE, value);
            }
        }

        public ProgressLoader ProgressLoader { get; }

        public ObservableCollection<FinancialRatio> RatiosCashFlow
        {
            get
            {
                return this.GetProperty(() => this.RatiosCashFlow);
            }

            set
            {
                this.SetProperty(() => this.RatiosCashFlow, value);
            }
        }

        public ObservableCollection<FinancialRatio> RatiosEfficiency
        {
            get
            {
                return this.GetProperty(() => this.RatiosEfficiency);
            }

            set
            {
                this.SetProperty(() => this.RatiosEfficiency, value);
            }
        }

        public ObservableCollection<FinancialRatio> RatiosFinancials
        {
            get
            {
                return this.GetProperty(() => this.RatiosFinancials);
            }

            set
            {
                this.SetProperty(() => this.RatiosFinancials, value);
            }
        }

        public ObservableCollection<FinancialRatio> RatiosHealth
        {
            get
            {
                return this.GetProperty(() => this.RatiosHealth);
            }

            set
            {
                this.SetProperty(() => this.RatiosHealth, value);
            }
        }

        public ObservableCollection<FinancialRatio> RatiosLiquidity
        {
            get
            {
                return this.GetProperty(() => this.RatiosLiquidity);
            }

            set
            {
                this.SetProperty(() => this.RatiosLiquidity, value);
            }
        }

        public ObservableCollection<FinancialRatio> RatiosProfitability
        {
            get
            {
                return this.GetProperty(() => this.RatiosProfitability);
            }

            set
            {
                this.SetProperty(() => this.RatiosProfitability, value);
            }
        }

        public double ROA
        {
            get
            {
                return this.GetProperty(() => this.ROA);
            }

            set
            {
                this.SetProperty(() => this.ROA, value);
            }
        }

        public double ROE
        {
            get
            {
                return this.GetProperty(() => this.ROE);
            }

            set
            {
                this.SetProperty(() => this.ROE, value);
            }
        }

        public double ROIC
        {
            get
            {
                return this.GetProperty(() => this.ROIC);
            }

            set
            {
                this.SetProperty(() => this.ROIC, value);
            }
        }

        public double SlowGrowth
        {
            get
            {
                return this.GetProperty(() => this.SlowGrowth);
            }

            set
            {
                this.SetProperty(() => this.SlowGrowth, value);
            }
        }

        public ObservableCollection<StockData> StockData
        {
            get
            {
                return this.GetProperty(() => this.StockData);
            }

            set
            {
                this.SetProperty(() => this.StockData, value);
            }
        }

        public string StockName
        {
            get
            {
                return this.GetProperty(() => this.StockName);
            }

            set
            {
                this.SetProperty(() => this.StockName, value);
            }
        }

        public string Symbol
        {
            get
            {
                return this.GetProperty(() => this.Symbol);
            }

            set
            {
                this.SetProperty(() => this.Symbol, value);
            }
        }

        public double WACC
        {
            get
            {
                return this.GetProperty(() => this.WACC);
            }

            set
            {
                this.SetProperty(() => this.WACC, value);
            }
        }

        public double WACCModified
        {
            get
            {
                return this._waccModified;
            }

            set
            {
                this._waccModified = value;
                this.RaisePropertyChanged(() => this.WACCModified);
                if (this.AllRatios.Any() && value != default(double))
                {
                    this.CalculateValutionTask();
                }
            }
        }

        public double WorkingCapitalPS
        {
            get
            {
                return this.GetProperty(() => this.WorkingCapitalPS);
            }

            set
            {
                this.SetProperty(() => this.WorkingCapitalPS, value);
            }
        }

        public int YearsTillTerminal
        {
            get
            {
                return this.GetProperty(() => this.YearsTillTerminal);
            }

            set
            {
                if (this.SetProperty(() => this.YearsTillTerminal, value) && this.AllRatios.Any()
                    && value != default(int))
                {
                    this.CalculateValutionTask();
                }
            }
        }

        [Command]
        public void AddFavorite()
        {
            if (string.IsNullOrEmpty(this.Symbol)) return;

            if (!this.Favorites.Contains(this.Symbol))
            {
                this.Favorites.Add(this.Symbol);
                this.MessageBoxService.ShowMessage(
                    $"{this.StockName} added to favorites",
                    "Favorites",
                    MessageButton.OK,
                    MessageIcon.Information);
            }
            else
            {
                this.MessageBoxService.ShowMessage(
                    $"{this.StockName} is already in favorites",
                    "Favorites",
                    MessageButton.OK,
                    MessageIcon.Exclamation);
            }
        }

        [Command]
        public async void Search()
        {
            try
            {
                this.Clear();

                this.ProgressLoader.UpdateProgress(MessageConstants.DownloadingRatios, 0);
                var results =
                    await
                    this._morningStarDataService.GetKeyRatiosAsync(this._cancellationTokenSource.Token, this.Symbol);
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
                    this._morningStarDataService.GetFinancialsAsync(this._cancellationTokenSource.Token, this.Symbol);
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

                this.ProgressLoader.UpdateProgress(MessageConstants.DownloadingYahooHistorical, 0);

                DateTime startDate;

                DateTime.TryParseExact(
                    this.RatiosFinancials.First().Data.First().Item1,
                    "yyyy-MM",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out startDate);

                if (startDate == default(DateTime))
                {
                    startDate = DateTime.Now.AddYears(-10);
                }

                var prices =
                    await
                    this._yahooFinanceDataService.GetHistoricalDataAsync(
                        this._cancellationTokenSource.Token,
                        this._yahooFinanceDataService.GetYahooSymbol(this.Symbol),
                        startDate);
                this.ProgressLoader.UpdateProgress(MessageConstants.DownloadingYahooHistorical, 70);
                this.StockData = new ObservableCollection<StockData>(prices);
                this.ProgressLoader.UpdateProgress(MessageConstants.DownloadingYahooHistorical, 100);
                this.ProgressLoader.UpdateProgress(MessageConstants.DownloadingYahooLatest, 0);
                this.LatestPrice =
                    await
                    this._yahooFinanceDataService.GetLatestAsync(
                        this._cancellationTokenSource.Token,
                        this._yahooFinanceDataService.GetYahooSymbol(this.Symbol));
                this.ProgressLoader.UpdateProgress(MessageConstants.DownloadingYahooLatest, 100);

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
                        : new List<Tuple<string, double?, double?>>();

                var peData =
                    this.RatiosFinancials.First(x => StringContains(x.Name, "Earnings Per Share"))
                        .Data.Where(x => !x.Item1.Equals("TTM"))
                        .ToList();
                var bvData =
                    this.RatiosFinancials.First(x => StringContains(x.Name, "Book Value"))
                        .Data.Where(x => !x.Item1.Equals("TTM"))
                        .ToList();

                var shortTermDebt = this.BalanceSheet.FirstOrDefault(x => StringContains(x.Name, "Short-term debt"))
                                    != null
                                        ? this.BalanceSheet.First(x => StringContains(x.Name, "Short-term debt"))
                                              .Data.Where(x => !x.Item1.Equals("TTM"))
                                              .ToList()
                                        : new List<Tuple<string, double?, double?>>();
                var longTermDebt =
                    this.BalanceSheet.First(x => StringContains(x.Name, "Long-term debt"))
                        .Data.Where(x => !x.Item1.Equals("TTM"))
                        .ToList();

                var taxRate =
                    this.RatiosProfitability.First(x => StringContains(x.Name, "Tax Rate"))
                        .Data.Where(x => !x.Item1.Equals("TTM"))
                        .ToList();

                //var capex = this.CashFlowStatement.First(x => StringContains(x.Name, "Capital Expenditure"))
                //                    .Data.Where(x => !x.Item1.Equals("TTM"))
                //                    .ToList();

                //var depreciation = this.CashFlowStatement.First(x => StringContains(x.Name, "Depreciation"))
                //                    .Data.Where(x => !x.Item1.Equals("TTM"))
                //                    .ToList();

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

                        double? dividendGrowthRate = (1.0 - payout[i].Item2.GetValueOrDefault() / 100.0)
                                                     * roe[i].Item2.GetValueOrDefault();

                        coe.Data[i] = Tuple.Create(roe[i].Item1, this.DividendYield + dividendGrowthRate, roe[i].Item3);
                        cod.Data[i] = Tuple.Create(
                            longTermDebt[i].Item1,
                            this.FilterInfinityNaN(
                                i > 0
                                    ? interestExpenses[i].Item2.GetValueOrDefault()
                                      / (longTermDebt[i].Item2.GetValueOrDefault()
                                         + shortTermDebt[i].Item2.GetValueOrDefault())
                                      * (1.0 - taxRate[i].Item2 / 100.0) * 100.0
                                    : 0.0),
                            longTermDebt[i].Item3);

                        var ratio1 = equity
                                     / (equity + longTermDebt[i].Item2.GetValueOrDefault()
                                        + shortTermDebt[i].Item2.GetValueOrDefault());
                        var ratio2 = (longTermDebt[i].Item2.GetValueOrDefault()
                                      + shortTermDebt[i].Item2.GetValueOrDefault())
                                     / (equity + longTermDebt[i].Item2.GetValueOrDefault()
                                        + shortTermDebt[i].Item2.GetValueOrDefault());
                        var waccValue = ratio1 * coe.Data[i].Item2.GetValueOrDefault()
                                        + ratio2 * cod.Data[i].Item2.GetValueOrDefault()
                                        * (1.0 - taxRate[i].Item2.GetValueOrDefault() / 100.0);
                        wacc.Data[i] = Tuple.Create(
                            longTermDebt[i].Item1,
                            this.FilterInfinityNaN(waccValue),
                            longTermDebt[i].Item3);

                        var peValue = price / peData[i].Item2.GetValueOrDefault();
                        pe.Data[i] = Tuple.Create(shares[i].Item1, this.FilterInfinityNaN(peValue), shares[i].Item3);
                        var pbvValue = price / bvData[i].Item2.GetValueOrDefault();
                        pbv.Data[i] = Tuple.Create(shares[i].Item1, this.FilterInfinityNaN(pbvValue), shares[i].Item3);

                        //reinvestment.Data[i] = Tuple.Create(
                        //    shares[i].Item1, 
                        //    this.FilterInfinityNaN(Math.Abs(capex[i].Item3.GetValueOrDefault()) - depreciation[i].Item3), shares[i].Item3);
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
                this.WACC = wacc.Data.Last(x => x.Item2.HasValue).Item2.GetValueOrDefault();

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

            var freeCashFlow =
                this.CashFlowStatement.First(x => StringContains(x.Name, "Free cash"))
                    .Data.OrderByDescending(x => x.Item1);

            this._averageCashFlow = Stats.SimpleAverage(freeCashFlow.Select(x => x.Item2.GetValueOrDefault()), 3);
            this.RaisePropertyChanged(() => this.AverageCashFlow);
            this.MinAverageCashFlow = this._averageCashFlow * 0.1;
            this.MaxAverageCashFlow = this._averageCashFlow > 0 ? this._averageCashFlow * 3.0 : 300;

            this._initialGrowthRate =
                this.CashFlowStatement.First(x => StringContains(x.Name, "Free cash")).DeltaLongTerm.GetValueOrDefault();

            this._waccModified = this.WACC;
            this.RaisePropertyChanged(() => this.WACCModified);

            this._nShares =
                this.RatiosFinancials.First(x => StringContains(x.Name, "Shares"))
                    .Data.Last(x => !x.Item1.Equals("TTM"))
                    .Item2.GetValueOrDefault();

            this._totalCash =
                this.BalanceSheet.First(x => StringContains(x.Name, "Total Cash"))
                    .Data.Last(x => !x.Item1.Equals("TTM"))
                    .Item2.GetValueOrDefault();

            this.CalculateValutionTask();

            this.ProgressLoader.UpdateProgress(MessageConstants.Analysing, 100);
        }

        private void CalculateValutionTask()
        {
            this.SlowGrowth = this._valuationService.GetDcfTwoStages(
                this._nShares,
                this.AverageCashFlow,
                this.InitialGrowthRate - 5.0,
                this.YearsTillTerminal,
                ApplicationHelper.Instance.RiskFreeRate + 2.0,
                this.WACCModified) + this._totalCash / this._nShares;

            this.CurrentGrowth = this._valuationService.GetDcfTwoStages(
                this._nShares,
                this.AverageCashFlow,
                this.InitialGrowthRate,
                this.YearsTillTerminal,
                ApplicationHelper.Instance.RiskFreeRate + 2.0,
                this.WACCModified) + this._totalCash / this._nShares;

            this.FastGrowth = this._valuationService.GetDcfTwoStages(
                this._nShares,
                this.AverageCashFlow,
                this.InitialGrowthRate + 5.0,
                this.YearsTillTerminal,
                ApplicationHelper.Instance.RiskFreeRate + 2.0,
                this.WACCModified) + this._totalCash / this._nShares;
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

            this._nShares = 0;
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
            DateTime date;
            return DateTime.TryParseExact(
                dateTime,
                "yyyy-MM",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out date);
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
    }
}