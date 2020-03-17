namespace sevenA.Module.Analysis.Models.DTOs
{
    using System.ComponentModel.DataAnnotations;

    using JetBrains.Annotations;

    using sevenA.Module.Analysis.Enums;

    public class RiskFreeRateDTO
    {
        public RiskFreeRateDTO(CountryEnum country, double rate)
        {
            this.Id = (int)country;
            this.Rate = rate;
            this.Country = (int)country;
        }

        [UsedImplicitly]
        public RiskFreeRateDTO()
        {
        }

        [Key]
        [UsedImplicitly]
        public int Id { get; set; }

        [UsedImplicitly]
        public int Country { get; set; }

        [UsedImplicitly]
        public double Rate { get; set; }
    }
}