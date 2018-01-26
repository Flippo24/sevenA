namespace sevenA.Module.Analysis.Models.DTOs
{
    using sevenA.Module.Analysis.Enums;

    public class RiskFreeRateDTO
    {
        public RiskFreeRateDTO(CountryEnum country, double rate)
        {
            this.Rate = rate;
            this.Country = (int)country;
        }

        public int Country { get; }

        public double Rate { get; }
    }
}