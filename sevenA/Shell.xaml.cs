using System.IO;
using System.Linq;
using System.Reflection;
using DevExpress.Mvvm.UI;

namespace sevenA
{
    public partial class Shell
    {
        public Shell()
        {
            var viewLocator = new ViewLocator(
                Directory.EnumerateFiles(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "sevenA*.dll")
                    .Select(Assembly.LoadFile));
            ViewLocator.Default = viewLocator;

            this.InitializeComponent();
        }
    }
}
