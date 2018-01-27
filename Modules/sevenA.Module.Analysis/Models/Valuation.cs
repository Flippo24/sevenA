namespace sevenA.Module.Analysis.Models
{
    using JetBrains.Annotations;

    public class Range
    {
        [UsedImplicitly]
        public double Min { get; set; }

        [UsedImplicitly]
        public double Max { get; set; }
    }

    // ReSharper disable once StyleCop.SA1402
    public class Valuation
    {
        [UsedImplicitly]
        public Range DD { get; set; }

        [UsedImplicitly]
        public Range SP { get; set; }

        [UsedImplicitly]
        public Range Graham { get; set; }

        [UsedImplicitly]
        public Range Dfc { get; set; }
    }
}