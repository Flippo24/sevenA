namespace sevenA.Core.Elements
{
    using DevExpress.Mvvm;

    public class ProgressLoader : BindableBase
    {
        public bool IsLoading
        {
            get { return this.GetProperty(() => this.IsLoading); }
            set { this.SetProperty(() => this.IsLoading, value); }
        }

        public string Text
        {
            get { return this.GetProperty(() => this.Text); }
            set { this.SetProperty(() => this.Text, value); }
        }

        public int Progress
        {
            get { return this.GetProperty(() => this.Progress); }
            set { this.SetProperty(() => this.Progress, value); }
        }

        public void UpdateProgress(string text, int progress)
        {
            if (this.IsLoading != true)
            {
                this.IsLoading = true;
            }

            this.Text = text;
            this.Progress = progress;
        }
    }
}