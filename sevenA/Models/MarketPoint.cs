namespace sevenA.Models
{
    using System;
    using System.Globalization;

    public class MarketPoint
    {
        public MarketPoint(string yahooString)
        {
            try
            {
                if (string.IsNullOrEmpty(yahooString))
                {
                    return;
                }

                var splittedData = yahooString.Split(',');
                if (splittedData.Length != 7)
                {
                    return;
                }

                this.Date = DateTime.ParseExact(splittedData[0], "yyyy-MM-dd", CultureInfo.InvariantCulture);
                this.Open = double.Parse(splittedData[1]);
                this.High = double.Parse(splittedData[2]);
                this.Low = double.Parse(splittedData[3]);
                this.Close = double.Parse(splittedData[4]);
                this.Volume = long.Parse(splittedData[5]);
                this.AdjClose = double.Parse(splittedData[6]);
            }
            catch
            {
                // ignored
            }
        }

        public DateTime Date { get; set; }

        public double Open { get; set; }

        public double High { get; set; }

        public double Low { get; set; }

        public double Close { get; set; }

        public long Volume { get; set; }

        public double AdjClose { get; set; }
    }
}