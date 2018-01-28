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
            double dividendGrowth,
            double earnings,
            double bookValue,
            double averageEarnings,
            double averagePE,
            double netIncome,
            double netIncomeGrowth,
            double shares)
        {
            var riskFreeRate = GetRiskFreeRate(country) / 100d;
            var factorRiskFreeRate = 1d + riskFreeRate;
            netIncomeGrowth /= 100d;
            var terminalGrowth = netIncomeGrowth;

            if (terminalGrowth < 0)
            {
                terminalGrowth = 0d;
            }
            else
            {
                terminalGrowth /= 2d;
            }

            if (terminalGrowth < riskFreeRate)
            {
                terminalGrowth = riskFreeRate;
            }

            Valuation result = new Valuation
            {
                EarningsPower = new Range
                {
                    Min = earnings / Math.Max(coe, riskFreeRate),
                    Max = 1.1d * earnings / Math.Max(coe, riskFreeRate)
                },
                DD = new Range
                {
                    Min = dividend / (coe - dividendGrowth > 0 ? coe - dividendGrowth : riskFreeRate),
                    Max = dividend * factorRiskFreeRate / (coe - dividendGrowth > 0 ? coe - dividendGrowth : riskFreeRate),
                },
                SP = new Range
                {
                    Min = ((earnings * riskFreeRate) / (coe * coe)) + (dividend / coe),
                    Max = ((earnings * factorRiskFreeRate * (1.1d * riskFreeRate)) / (coe * coe)) + (dividend * factorRiskFreeRate / coe)
                },
                Graham = new Range
                {
                    Min = Math.Sqrt(22.5d * earnings * bookValue),
                    Max = Math.Sqrt(22.5d * 1.1d * earnings * bookValue),
                },
                PEBased = new Range
                {
                    Min = averageEarnings * averagePE,
                    Max = 1.1d * averageEarnings * averagePE,
                },
                Dfc = new Range
                {
                    Min = GetDcfTwoStages(shares, netIncome, netIncomeGrowth, 10, terminalGrowth, Math.Max(coe, riskFreeRate)),
                    Max = GetDcfTwoStages(shares, netIncome * factorRiskFreeRate, netIncomeGrowth, 10, terminalGrowth, Math.Max(coe, riskFreeRate)),
                }
            };

            result.Weighted = new Range
            {
                Min = new[] { result.Dfc.Min, result.EarningsPower.Min, result.DD.Min, result.SP.Min, result.Graham.Min, result.PEBased.Min }.Where(x => x > 0).Average(),
                Max = new[] { result.Dfc.Max, result.EarningsPower.Max, result.DD.Max, result.SP.Max, result.Graham.Max, result.PEBased.Max }.Where(x => x > 0).Average()
            };

            result.MarginSafety = new Range { Min = result.Weighted.Min * 0.80d, Max = result.Weighted.Max * 0.80d };

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