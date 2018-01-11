namespace sevenA.ViewModels
{
    using DevExpress.Mvvm;
    using DevExpress.Mvvm.DataAnnotations;

    [POCOViewModel]
    public class ShellViewModel : ViewModelBase
    {
        private INavigationService NavigationService => this.GetService<INavigationService>();

        [Command]
        public void OnViewLoaded()
        {
            this.NavigationService.Navigate("DashboardView", null, this);
        }
    }
}