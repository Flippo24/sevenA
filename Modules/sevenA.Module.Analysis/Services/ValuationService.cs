namespace sevenA.Module.Analysis.Services
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using sevenA.Module.Analysis.Enums;
    using sevenA.Module.Analysis.Models;
    using sevenA.Module.Analysis.Models.DTOs;

    public class ValuationService
    {
        static ValuationService()
        {
            Instance = new ValuationService();
        }

        public static ValuationService Instance { get; }

        public double GetRiskFreeRate(CountryEnum country)
        {
            var allRates = PersistenceService.Instance.GetAllRiskFreeRates().Result;
            return (allRates.SingleOrDefault(r => r.Country == (int)country) ?? new RiskFreeRateDTO(country, 0d)).Rate;
        }

        public void SaveRiskFreeRate(CountryEnum country, double rate)
        {
            PersistenceService.Instance.SaveRiskFreeRate(new RiskFreeRateDTO(country, rate)).Wait();
        }

        public Valuation CalculateValuations(CountryEnum country, double coe, double dividend, double earnings)
        {
            var riskFreeRate = GetRiskFreeRate(country) / 100d;
            var factorRiskFreeRate = 1d + riskFreeRate;

            Valuation result = new Valuation
            {
                DD = new Range
                {
                    Min = dividend / (coe - riskFreeRate),
                    Max = dividend * factorRiskFreeRate / (coe - riskFreeRate)
                },
                SP = new Range
                {
                    Min = ((earnings * riskFreeRate) / (coe * coe)) + (dividend / coe),
                    Max = ((earnings * factorRiskFreeRate * riskFreeRate) / (coe * coe)) + (dividend * factorRiskFreeRate / coe)
                }
            };

            return result;
        }

        public double GetDcfTwoStages(
            double numShares,
            double cashflow0,
            double initialGrowth,
            int numberYearsTillTerminalGrowth,
            double terminalGrowth,
            double discountRate)
        {
            var currentCf = cashflow0;
            var sumCf = currentCf;
            for (int i = 1; i <= numberYearsTillTerminalGrowth; i++)
            {
                // ReSharper disable once StyleCop.SA1407
                currentCf *= 1.0 + this.GetLinearInterpolation(initialGrowth, terminalGrowth, 1, numberYearsTillTerminalGrowth + 1, i) /
                             100.0;
                sumCf += this.GetPresentValue(currentCf, discountRate, numberYearsTillTerminalGrowth);
            }

            var total = sumCf
                        + this.GetPresentValue(
                            this.GetTerminalValue(
                                // ReSharper disable once StyleCop.SA1407
                                currentCf * (1.0 + terminalGrowth / 100.0),
                                terminalGrowth,
                                discountRate),
                            discountRate,
                numberYearsTillTerminalGrowth);

            return total / numShares;
        }

        private double GetPresentValue(double value, double rate, int year)
        {
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