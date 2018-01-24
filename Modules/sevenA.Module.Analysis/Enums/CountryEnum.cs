namespace sevenA.Module.Analysis.Enums
{
    public enum CountryEnum
    {
        Other,

        Singapore,

        Portugal
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