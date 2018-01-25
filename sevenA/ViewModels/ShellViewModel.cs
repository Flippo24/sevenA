namespace sevenA.ViewModels
{
    using DevExpress.Mvvm;
    using DevExpress.Mvvm.DataAnnotations;

    using JetBrains.Annotations;

    [POCOViewModel]
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ShellViewModel : ViewModelBase
    {
        private INavigationService NavigationService => this.GetService<INavigationService>();

        [Command]
        [UsedImplicitly]
        public void OnViewLoaded()
        {
            this.NavigationService.Navigate("DashboardView", null, this);
        }
    }
}