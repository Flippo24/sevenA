namespace sevenA.Module.Analysis.ViewModels
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    using DevExpress.Mvvm;
    using DevExpress.Mvvm.DataAnnotations;

    using Core.Elements;
    using Constants;
    using Enums;
    using Models;

    using SevenA.Module.Analysis.Services;

    [POCOViewModel]
    public class AnalysisViewModel : NavigationViewModelBase
    {
        private readonly MorningStarDataService _morningStarDataService;

        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public AnalysisViewModel()
        {
            this._morningStarDataService = MorningStarDataService.Instance;
            this.ProgressLoader = new ProgressLoader();
        }

        public ProgressLoader ProgressLoader { get; }

        public string Symbol
        {
            get { return this.GetProperty(() => this.Symbol); }
            set { this.SetProperty(() => this.Symbol, value); }
        }

        public string StockName
        {
            get { return this.GetProperty(() => this.StockName); }
            set { this.SetProperty(() => this.StockName, value); }
        }

        public List<FinancialRatio> RatiosFinancials
        {
            get { return this.GetProperty(() => this.RatiosFinancials); }
            set { this.SetProperty(() => this.RatiosFinancials, value); }
        }

        public List<FinancialRatio> RatiosProfitability
        {
            get { return this.GetProperty(() => this.RatiosProfitability); }
            set { this.SetProperty(() => this.RatiosProfitability, value); }
        }

        public List<FinancialRatio> RatiosCashFlow
        {
            get { return this.GetProperty(() => this.RatiosCashFlow); }
            set { this.SetProperty(() => this.RatiosCashFlow, value); }
        }

        public List<FinancialRatio> RatiosHealth
        {
            get { return this.GetProperty(() => this.RatiosHealth); }
            set { this.SetProperty(() => this.RatiosHealth, value); }
        }

        public List<FinancialRatio> RatiosLiquidity
        {
            get { return this.GetProperty(() => this.RatiosLiquidity); }
            set { this.SetProperty(() => this.RatiosLiquidity, value); }
        }

        public List<FinancialRatio> RatiosEfficiency
        {
            get { return this.GetProperty(() => this.RatiosEfficiency); }
            set { this.SetProperty(() => this.RatiosEfficiency, value); }
        }

        public List<FinancialRatio> IncomeStatement
        {
            get { return this.GetProperty(() => this.IncomeStatement); }
            set { this.SetProperty(() => this.IncomeStatement, value); }
        }

        public List<FinancialRatio> BalanceSheet
        {
            get { return this.GetProperty(() => this.BalanceSheet); }
            set { this.SetProperty(() => this.BalanceSheet, value); }
        }

        public List<FinancialRatio> CashFlowStatement
        {
            get { return this.GetProperty(() => this.CashFlowStatement); }
            set { this.SetProperty(() => this.CashFlowStatement, value); }
        }

        [Command]
        public async void Search()
        {
            this.ProgressLoader.UpdateProgress(MessageConstants.DownloadingRatios, 0);
            var results = await this._morningStarDataService.GetKeyRatiosAsync(this._cancellationTokenSource.Token, this.Symbol);
            this.ProgressLoader.UpdateProgress(MessageConstants.DownloadingRatios, 70);
            this.RatiosFinancials = new List<FinancialRatio>(results.Where(x => x.Section == FinancialRatioSectionEnum.Financials).ToList());
            this.RatiosProfitability = new List<FinancialRatio>(results.Where(x => x.Section == FinancialRatioSectionEnum.Profitability).ToList());
            this.RatiosCashFlow = new List<FinancialRatio>(results.Where(x => x.Section == FinancialRatioSectionEnum.CashFlow).ToList());
            this.RatiosHealth = new List<FinancialRatio>(results.Where(x => x.Section == FinancialRatioSectionEnum.Health).ToList());
            this.RatiosLiquidity = new List<FinancialRatio>(results.Where(x => x.Section == FinancialRatioSectionEnum.Liquidity).ToList());
            this.RatiosEfficiency = new List<FinancialRatio>(results.Where(x => x.Section == FinancialRatioSectionEnum.Efficiency).ToList());
            this.ProgressLoader.UpdateProgress(MessageConstants.DownloadingRatios, 100);
            this.StockName = this.RatiosFinancials.Any() ? this.RatiosFinancials.First().StockName : string.Empty;

            this.ProgressLoader.UpdateProgress(MessageConstants.DownloadingFinancials, 0);
            var financials = await this._morningStarDataService.GetFinancialsAsync(this._cancellationTokenSource.Token, this.Symbol);
            this.ProgressLoader.UpdateProgress(MessageConstants.DownloadingFinancials, 70);
            this.IncomeStatement = new List<FinancialRatio>(financials.Where(x => x.Section == FinancialRatioSectionEnum.IncomeStatement).ToList());
            this.BalanceSheet = new List<FinancialRatio>(financials.Where(x => x.Section == FinancialRatioSectionEnum.BalanceSheet).ToList());
            this.CashFlowStatement = new List<FinancialRatio>(financials.Where(x => x.Section == FinancialRatioSectionEnum.CashflowStatement).ToList());

            this.ProgressLoader.UpdateProgress(MessageConstants.DownloadingFinancials, 100);
            this.ProgressLoader.IsLoading = false;

            // var prices = await this._yahooFinanceDataService.GetHistoricalDataAsync(this._cancellationTokenSource.Token, "B2F.SI", DateTime.Now.AddYears(-5));
        }
    }
}