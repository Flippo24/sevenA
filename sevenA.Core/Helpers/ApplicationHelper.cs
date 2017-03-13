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

        public readonly double RiskFreeRateSG = 2.31;
        public readonly double RiskFreeRatePT = 4.0;
        public readonly double RiskFreeRate = 1.219;
    }
}