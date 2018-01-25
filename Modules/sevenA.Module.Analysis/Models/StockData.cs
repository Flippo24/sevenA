namespace sevenA.Module.Analysis.Models
{
    using System;

    using DevExpress.Mvvm;

    using JetBrains.Annotations;

    public class StockData : BindableBase
    {
        public DateTime Date
        {
            get { return this.GetProperty(() => this.Date); }
            set { this.SetProperty(() => this.Date, value); }
        }

        [UsedImplicitly]
        public double Open
        {
            get { return this.GetProperty(() => this.Open); }
            set { this.SetProperty(() => this.Open, value); }
        }

        [UsedImplicitly]
        public double High
        {
            get { return this.GetProperty(() => this.High); }
            set { this.SetProperty(() => this.High, value); }
        }

        [UsedImplicitly]
        public double Low
        {
            get { return this.GetProperty(() => this.Low); }
            set { this.SetProperty(() => this.Low, value); }
        }

        public double Close
        {
            get { return this.GetProperty(() => this.Close); }
            set { this.SetProperty(() => this.Close, value); }
        }
    }
}