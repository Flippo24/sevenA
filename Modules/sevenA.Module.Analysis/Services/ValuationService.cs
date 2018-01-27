namespace sevenA.Module.Analysis.Services
{
    using System;
    using System.Linq;

    using sevenA.Module.Analysis.Enums;
    using sevenA.Module.Analysis.Models;
    using sevenA.Module.Analysis.Models.DTOs;

    public static class ValuationService
    {
        public static double GetRiskFreeRate(CountryEnum country)
        {
            var allRates = PersistenceService.Instance.GetAllRiskFreeRates().Result;
            return (allRates.SingleOrDefault(r => r.Country == (int)country) ?? new RiskFreeRateDTO(country, 0d)).Rate;
        }

        public static void SaveRiskFreeRate(CountryEnum country, double rate)
        {
            PersistenceService.Instance.SaveRiskFreeRate(new RiskFreeRateDTO(country, rate)).Wait();
        }

        public static Valuation CalculateValuations(
            CountryEnum country,
            double coe,
            double dividend,
            double earnings,
            double equity,
            double netIncome,
            double shares)
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
                },
                Graham = new Range
                {
                    Min = Math.Sqrt(21 * equity * netIncome / shares) / 100d,
                    Max = Math.Sqrt(23 * equity * netIncome * factorRiskFreeRate * factorRiskFreeRate / shares) / 100d
                },
                Dfc = new Range
                {
                    Min = GetDcfTwoStages(shares, netIncome, 0, 20, 0, Math.Max(coe, riskFreeRate)),
                    Max = GetDcfTwoStages(shares, netIncome, riskFreeRate / 2d, 20, riskFreeRate, Math.Max(coe, riskFreeRate))
                },
            };

            return result;
        }

        private static double GetDcfTwoStages(
            double shares,
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
                currentCf *= 1.0 + GetLinearInterpolation(initialGrowth, terminalGrowth, 1, numberYearsTillTerminalGrowth + 1, i);
                sumCf += GetPresentValue(currentCf, discountRate, numberYearsTillTerminalGrowth);
            }

            var total = sumCf + GetPresentValue(GetTerminalValue(currentCf * (1.0 + terminalGrowth), terminalGrowth, discountRate), discountRate, numberYearsTillTerminalGrowth);

            return total / shares;
        }

        private static double GetPresentValue(double value, double rate, int year)
        {
            return value / Math.Pow(1.0 + rate, year);
        }

        private static double GetTerminalValue(double terminalCashflow, double terminalGrowth, double discoutRate)
        {
            return terminalCashflow * (1.0 + terminalGrowth) / (discoutRate - terminalGrowth);
        }

        private static double GetLinearInterpolation(double y0, double y1, double x0, double x1, double x)
        {
            var delta = (y1 - y0) / (x1 - x0);
            return y0 + (delta * (x - x0));
        }
    }
}