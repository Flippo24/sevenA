namespace sevenA.Module.Analysis.Views
{
    using System;
    using System.Globalization;
    using System.Windows.Media;

    using DevExpress.Xpf.Charts;

    public partial class AnalysisView
    {
        public AnalysisView()
        {
            this.InitializeComponent();
        }

        private void OnCustomDrawRatiosPoints(object sender, CustomDrawSeriesPointEventArgs e)
        {
            DateTime date;
            if (!DateTime.TryParseExact(e.SeriesPoint.Argument, "yyyy-MM", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
            {
                e.DrawOptions.Color = Colors.DarkGoldenrod;
            }
        }
    }
}
