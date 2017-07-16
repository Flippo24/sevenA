namespace sevenA.Core.Helpers
{
    public class ApplicationHelper
    {
        private static ApplicationHelper _instance;

        private ApplicationHelper()
        {
            _instance = this;
        }

        public static ApplicationHelper Instance => _instance ?? new ApplicationHelper();

        public readonly double RiskFreeRateSG = 2.09;
        public readonly double RiskFreeRatePT = 3.15;
        public readonly double RiskFreeRate = 0.6;
    }
}