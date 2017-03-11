//using System.ComponentModel.Composition.Hosting;
//using System.IO;
//using System.Windows;
//using Prism.Mef;
//using Prism.Modularity;
//using sevenA.Module.Analysis;

//namespace sevenA
//{
//    public class UIBootstrapper : MefBootstrapper
//    {
//        protected override void ConfigureAggregateCatalog()
//        {
//            base.ConfigureAggregateCatalog();

//            this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(Shell).Assembly));

//            var dirCatalog = new DirectoryCatalog(Path.GetDirectoryName(typeof(Shell).Assembly.Location), "*.dll");
//            this.AggregateCatalog.Catalogs.Add(dirCatalog);
//        }

//        protected override void ConfigureModuleCatalog()
//        {
//            base.ConfigureModuleCatalog();
//            var moduleCatalog = (ModuleCatalog)this.ModuleCatalog;
//            moduleCatalog.AddModule(typeof(AnalysisModule));
//        }

//        protected override DependencyObject CreateShell()
//        {
//            var view = this.Container.GetExportedValue<Shell>();
//            return view;
//        }

//        protected override void InitializeShell()
//        {
//            base.InitializeShell();
//            Application.Current.MainWindow = (Window)this.Shell;
//            Application.Current.MainWindow.Show();
//        }
//    }
//}