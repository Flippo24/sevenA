namespace sevenA.Core.Helpers
{
    public class ApplicationHelper
    {
        public readonly double RiskFreeRateSg = 2.09d;

        public readonly double RiskFreeRatePt = 3.15d;

        public readonly double RiskFreeRate = 0.6d;

        static ApplicationHelper()
        {
            Instance = new ApplicationHelper();
        }

        public static ApplicationHelper Instance { get; }
    }
}