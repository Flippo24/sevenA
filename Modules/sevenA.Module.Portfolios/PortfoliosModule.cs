namespace sevenA.Module.Portfolios
{
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;

    using Prism.Modularity;
    using Prism.Regions;

    public class PortfoliosModule : IModule
    {
        private readonly CompositionContainer _compositionContainer;
        private readonly IRegionManager _regionManager;

        [ImportingConstructor]
        public PortfoliosModule(CompositionContainer container, IRegionManager regionManager)
        {
            this._compositionContainer = container;
            this._regionManager = regionManager;
        }

        public void Initialize()
        {
            throw new System.NotImplementedException();
        }
    }
}