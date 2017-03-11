using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using Prism.Mvvm;

namespace sevenA.Models
{
    public class Stock : BindableBase
    {
        private string _name;
        private string _symbol;
        private double _lastPrice;
        private long _lastVolume;
        private double _high52;
        private double _low52;
        private bool _isDownloading;
        private bool? _isDownloadSuccess;

        public Stock()
        {
            this.MarketData = new ObservableCollection<MarketPoint>();
        }

        public string Name
        {
            get { return this._name; }
            set { this.SetProperty(ref this._name, value); }
        }

        public string Symbol
        {
            get { return this._symbol; }
            set { this.SetProperty(ref this._symbol, value); }
        }

        public double LastPrice
        {
            get { return this._lastPrice; }
            set { this.SetProperty(ref this._lastPrice, value); }
        }

        public long LastVolume
        {
            get { return this._lastVolume; }
            set { this.SetProperty(ref this._lastVolume, value); }
        }

        public double High52
        {
            get { return this._high52; }
            set { this.SetProperty(ref this._high52, value); }
        }

        public double Low52
        {
            get { return this._low52; }
            set { this.SetProperty(ref this._low52, value); }
        }

        public ObservableCollection<MarketPoint> MarketData { get; set; }

        public bool IsDownloading
        {
            get { return this._isDownloading; }
            private set { this.SetProperty(ref this._isDownloading, value); }
        }

        public bool? IsDownloadSuccess
        {
            get { return this._isDownloadSuccess; }
            set { this.SetProperty(ref this._isDownloadSuccess, value); }
        }

        public void Download()
        {
            if (string.IsNullOrEmpty(this.Symbol))
                return;

            this.IsDownloading = true;

            var url = $@"http://ichart.yahoo.com/table.csv?s={this.Symbol}";
            var downloadPath = AppHelper.GetDownloadDirectory(this.Symbol);

            if (!Directory.Exists(downloadPath))
            {
                Directory.CreateDirectory(downloadPath);
            }

            using (var webclient = new WebClient())
            {
                webclient.DownloadFileCompleted += (sender, args) =>
                {
                    this.IsDownloading = false;
                    this.IsDownloadSuccess = args.Error == null;

                    if (this.IsDownloadSuccess.Value)
                    {
                        this.DownloadInfo();
                        this.LoadMarketData();
                    }
                };

                webclient.DownloadFileAsync(new Uri(url), Path.Combine(downloadPath, this.Symbol));
            }
        }

        public Task DownloadInfo()
        {
            return Task.Run(() =>
                {
                    var url = $@"http://download.finance.yahoo.com/d/quotes.csv?s={this.Symbol}&f=nl1vkj";
                    var downloadPath = AppHelper.GetDownloadDirectory(this.Symbol);

                    if (!Directory.Exists(downloadPath))
                    {
                        Directory.CreateDirectory(downloadPath);
                    }

                    using (var webclient = new WebClient())
                    {
                        webclient.DownloadFileCompleted += (sender, args) => this.LoadInfo().ContinueWith(x => this.LoadMarketData());

                        webclient.DownloadFileAsync(new Uri(url), Path.Combine(downloadPath, this.Symbol + ".info"));
                    }
                });
        }

        public Task<bool> LoadInfo()
        {
            return Task.Run(() =>
            {
                try
                {
                    var lines = File.ReadLines(Path.Combine(AppHelper.GetDownloadDirectory(this.Symbol), this.Symbol + ".info"));
                    var split = lines.First().Split(',');

                    Application.Current.Dispatcher.Invoke(() =>
                        {
                            this.Name = split[0].Replace("\"", "");
                            this.LastPrice = double.Parse(split[1]);
                            this.LastVolume = long.Parse(split[2]);
                            this.High52 = double.Parse(split[3]);
                            this.Low52 = double.Parse(split[4]);
                        });

                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            });
        }

        public Task<bool> LoadMarketData()
        {
            return Task.Run(() =>
                {
                    try
                    {
                        var lines = File.ReadLines(Path.Combine(AppHelper.GetDownloadDirectory(this.Symbol), this.Symbol)).Skip(1);
                        foreach (var line in lines)
                        {
                            this.MarketData.Add(new MarketPoint(line));
                        }
                        this.MarketData = new ObservableCollection<MarketPoint>(this.MarketData.OrderBy(x => x.Date));
                        return true;
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                });
        }

    }
}