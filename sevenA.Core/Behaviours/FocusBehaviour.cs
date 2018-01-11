namespace sevenA.Core.Behaviours
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Interactivity;

    public class FocusBehaviour : Behavior<TextBox>
    {
        public static readonly DependencyProperty IsFocusedProperty =
            DependencyProperty.Register(
                "IsFocused",
                typeof(bool),
                typeof(FocusBehaviour),
                new PropertyMetadata(false));

        public bool IsFocused
        {
            get => (bool)this.GetValue(IsFocusedProperty);
            set => this.SetValue(IsFocusedProperty, value);
        }

        protected override void OnAttached()
        {
            this.AssociatedObject.GotFocus += (sender, args) =>
                {
                    this.IsFocused = true;
                    this.AssociatedObject.SelectAll();
                };
            this.AssociatedObject.LostFocus += (sender, a) => this.IsFocused = false;
            this.AssociatedObject.Loaded += (sender, a) =>
                {
                    this.AssociatedObject.Focus();
                };
            base.OnAttached();
        }
    }
}