namespace sevenA.Core.Behaviours
{
    using System.Windows.Controls;
    using System.Windows.Interactivity;

    public class FocusBehaviour : Behavior<TextBox>
    {
        protected override void OnAttached()
        {
            this.AssociatedObject.GotFocus += (sender, args) => { this.AssociatedObject.SelectAll(); };

            this.AssociatedObject.Loaded += (sender, a) => { this.AssociatedObject.Focus(); };
            base.OnAttached();
        }
    }
}