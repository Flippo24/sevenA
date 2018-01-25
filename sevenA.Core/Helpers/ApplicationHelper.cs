namespace sevenA.Core.Helpers
{
    using System;
    using System.IO;

    public class ApplicationHelper
    {
        static ApplicationHelper()
        {
            Instance = new ApplicationHelper();

            CompanyName = "twentySix";
            Title = "sevenA";
        }

        public static ApplicationHelper Instance { get; }

        public static string Title { get; }

        public static string CompanyName { get; }

        public static string GetAppDataFolder()
        {
            var appFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), CompanyName, Title);

            if (!Directory.Exists(appFolder))
            {
                Directory.CreateDirectory(appFolder);
            }

            return appFolder;
        }
    }
}