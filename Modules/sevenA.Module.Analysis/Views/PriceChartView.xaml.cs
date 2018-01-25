namespace sevenA.Module.Analysis.Views
{
    using DevExpress.Xpf.Grid;

    // ReSharper disable once StyleCop.SA1601
    public partial class PriceChartView
    {
        public PriceChartView()
        {
            this.InitializeComponent();
            DataControlBase.AllowInfiniteGridSize = true;
        }
    }
}
