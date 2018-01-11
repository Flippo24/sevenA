namespace sevenA.Models
{
    using System;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using System.Windows;

    using DevExpress.Mvvm;

    public class Stock : BindableBase
    {
        public Stock()
        {
            this.MarketData = new ObservableCollection<MarketPoint>();
        }

        public string Name
        {
            get => this.GetProperty(() => this.Name);
            set => this.SetProperty(() => this.Name, value);
        }

        public string Symbol
        {
            get => this.GetProperty(() => this.Symbol);
            set => this.SetProperty(() => this.Symbol, value);
        }

        public double LastPrice
        {
            get => this.GetProperty(() => this.LastPrice);
            set => this.SetProperty(() => this.LastPrice, value);
        }

        public long LastVolume
        {
            get => this.GetProperty(() => this.LastVolume);
            set => this.SetProperty(() => this.LastVolume, value);
        }

        public double High52
        {
            get => this.GetProperty(() => this.High52);
            set => this.SetProperty(() => this.High52, value);
        }

        public double Low52
        {
            get => this.GetProperty(() => this.Low52);
            set => this.SetProperty(() => this.Low52, value);
        }

        public ObservableCollection<MarketPoint> MarketData { get; set; }

        public bool IsDownloading
        {
            get => this.GetProperty(() => this.IsDownloading);
            set => this.SetProperty(() => this.IsDownloading, value);
        }

        public bool? IsDownloadSuccess
        {
            get => this.GetProperty(() => this.IsDownloadSuccess);
            set => this.SetProperty(() => this.IsDownloadSuccess, value);
        }

        public void Download()
        {
            if (string.IsNullOrEmpty(this.Symbol))
            {
                return;
            }

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
                            this.Name = split[0].Replace("\"", string.Empty);
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