namespace sevenA.Module.Analysis.Views
{
    using DevExpress.Xpf.Grid;

    // ReSharper disable once StyleCop.SA1601
    public partial class FinancialsGridView
    {
        public FinancialsGridView()
        {
            this.InitializeComponent();
            DataControlBase.AllowInfiniteGridSize = true;
        }
    }
}
