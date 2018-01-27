namespace sevenA.Module.Analysis.Enums
{
    public enum CountryEnum
    {
        Singapore = 1,

        Portugal = 2,

        Other = 4,
    }

    internal static class CountryConverter
    {
        public static string GetPrefix(CountryEnum countryEnum)
        {
            switch (countryEnum)
            {
                case CountryEnum.Singapore:
                    return "XSES:";
                case CountryEnum.Portugal:
                    return "XLIS:";
                default:
                    return string.Empty;
            }
        }
    }
}