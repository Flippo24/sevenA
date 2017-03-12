using System.IO;
using System.Reflection;

namespace sevenA
{
    public static class AppHelper
    {
        public static string GetDownloadDirectory(string symbol)
        {
            return Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Stocks", symbol, "Downloads");
        }
    }
}