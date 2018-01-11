namespace sevenA.Module.Analysis.Views
{
    using System;
    using System.Globalization;
    using System.Windows.Media;

    using DevExpress.Xpf.Charts;

    // ReSharper disable once StyleCop.SA1601
    public partial class AnalysisView
    {
        public AnalysisView()
        {
            this.InitializeComponent();
        }

        private void OnCustomDrawRatiosPoints(object sender, CustomDrawSeriesPointEventArgs e)
        {
            if (!DateTime.TryParseExact(e.SeriesPoint.Argument, "yyyy-MM", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
            {
                e.DrawOptions.Color = Colors.DarkGoldenrod;
            }
        }
    }
}
