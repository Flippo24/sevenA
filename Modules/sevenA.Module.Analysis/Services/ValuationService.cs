namespace sevenA.Module.Analysis.Services
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using sevenA.Module.Analysis.Enums;
    using sevenA.Module.Analysis.Models.DTOs;

    public class ValuationService
    {
        static ValuationService()
        {
            Instance = new ValuationService();
        }

        public static ValuationService Instance { get; }

        public static double GetRiskFreeRate(CountryEnum country)
        {
            var allRates = PersistenceService.Instance.GetAllRiskFreeRates().Result;

            return (allRates.SingleOrDefault(r => r.Country == (int)country) ?? new RiskFreeRateDTO { Rate = 0d }).Rate;
        }

        // ReSharper disable once StyleCop.SA1305
        public double GetStableGrowthValuation(double nShares, double cashFlow0, double terminalGrowth, double wacc)
        {
            try
            {
                return cashFlow0 / ((wacc - terminalGrowth) / 100.0) / nShares;
            }
            catch
            {
                return 0d;
            }
        }

        public double GetDcfTwoStages(
            // ReSharper disable once StyleCop.SA1305
            double nShares,
            double cashflow0,
            double initialGrowth,
            // ReSharper disable once StyleCop.SA1305
            int nYearsTillTerminalGrowth,
            double terminalGrowth,
            double discountRate)
        {
            var currentCf = cashflow0;
            var sumCf = currentCf;
            for (int i = 1; i <= nYearsTillTerminalGrowth; i++)
            {
                // ReSharper disable once StyleCop.SA1407
                currentCf *= 1.0 + this.GetLinearInterpolation(initialGrowth, terminalGrowth, 1, nYearsTillTerminalGrowth + 1, i) /
                             100.0;
                sumCf += this.GetPresentValue(currentCf, discountRate, nYearsTillTerminalGrowth);
            }

            var total = sumCf
                        + this.GetPresentValue(
                            this.GetTerminalValue(
                                // ReSharper disable once StyleCop.SA1407
                                currentCf * (1.0 + terminalGrowth / 100.0),
                                terminalGrowth,
                                discountRate),
                            discountRate,
                nYearsTillTerminalGrowth);

            return total / nShares;
        }

        public double GetPresentValue(double value, double rate, int year)
        {
            // ReSharper disable once StyleCop.SA1407
            return value / Math.Pow(1.0 + rate / 100.0, year);
        }

        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1407:ArithmeticExpressionsMustDeclarePrecedence", Justification = "Reviewed. Suppression is OK here.")]
        private double GetTerminalValue(double terminalCashflow, double terminalGrowth, double discoutRate)
        {
            return terminalCashflow * (1.0 + terminalGrowth / 100.0) / ((discoutRate - terminalGrowth) / 100.0);
        }

        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1407:ArithmeticExpressionsMustDeclarePrecedence", Justification = "Reviewed. Suppression is OK here.")]
        private double GetLinearInterpolation(double y0, double y1, double x0, double x1, double x)
        {
            var delta = (y1 - y0) / (x1 - x0);
            return y0 + delta * (x - x0);
        }
    }
}