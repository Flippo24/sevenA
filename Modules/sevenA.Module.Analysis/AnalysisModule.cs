namespace sevenA.Module.Analysis
{
    using System.ComponentModel.Composition;

    using Prism.Mef.Modularity;
    using Prism.Modularity;
    using Prism.Regions;

    using sevenA.Core.Elements;
    using Views;

    [ModuleExport(typeof(AnalysisModule))]
    public class AnalysisModule : IModule
    {
        private readonly IRegionManager _regionManager;

        [ImportingConstructor]
        public AnalysisModule(IRegionManager regionManager)
        {
            this._regionManager = regionManager;
        }

        public void Initialize()
        {
            this._regionManager.RegisterViewWithRegion(Regions.Main, typeof(DashboardView));
        }
    }
}
