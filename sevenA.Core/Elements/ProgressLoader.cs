namespace sevenA.Core.Elements
{
    using DevExpress.Mvvm;

    using JetBrains.Annotations;

    public class ProgressLoader : BindableBase
    {
        [UsedImplicitly]
        public bool IsLoading
        {
            get { return this.GetProperty(() => this.IsLoading); }
            set { this.SetProperty(() => this.IsLoading, value); }
        }

        [UsedImplicitly]
        public string Text
        {
            get { return this.GetProperty(() => this.Text); }
            set { this.SetProperty(() => this.Text, value); }
        }

        [UsedImplicitly]
        public int Progress
        {
            get { return this.GetProperty(() => this.Progress); }
            set { this.SetProperty(() => this.Progress, value); }
        }

        public void UpdateProgress(string text, int progress)
        {
            if (this.IsLoading != true) this.IsLoading = true;

            this.Text = text;
            this.Progress = progress;
        }
    }
}