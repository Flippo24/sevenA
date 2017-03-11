using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using sevenA.Module.Analysis.Views;

namespace sevenA.Views
{
    [POCOViewModel]
    public class ShellViewModel : ViewModelBase
    {
        private INavigationService NavigationService => this.GetService<INavigationService>();

        [Command]
        public void OnViewLoaded()
        {
            NavigationService.Navigate("DashboardView", null, this);
        }
    }
}