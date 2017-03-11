using DevExpress.Mvvm;

namespace sevenA.Core.Elements
{
    public class ProgressLoader : BindableBase
    {
        public bool IsLoading
        {
            get { return GetProperty(() => IsLoading); }
            set { SetProperty(() => IsLoading, value); }
        }

        public string Text
        {
            get { return GetProperty(() => Text); }
            set { SetProperty(() => Text, value); }
        }

        public int Progress
        {
            get { return GetProperty(() => Progress); }
            set { SetProperty(() => Progress, value); }
        }

        public void UpdateProgress(string text, int progress)
        {
            if (IsLoading != true) IsLoading = true;

            Text = text;
            Progress = progress;
        }
    }
}