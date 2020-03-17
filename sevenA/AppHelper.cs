namespace sevenA
{
    using System;
    using System.IO;
    using System.Reflection;

    public static class AppHelper
    {
        public static string GetDownloadDirectory(string symbol)
        {
            return Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location) ?? throw new InvalidOperationException(), "Stocks", symbol, "Downloads");
        }
    }
}