using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace CrossGIS.Core.Triggers
{
    public class TextBoxEnterKeyTrigger : TriggerBase<UIElement>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.KeyUp += AssociatedObject_KeyUp;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.KeyUp -= AssociatedObject_KeyUp;
        }

        void AssociatedObject_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var textBox = AssociatedObject as TextBox;
                var passwordBox = AssociatedObject as PasswordBox;
                if (textBox != null)
                {
                    var bindingExpression = textBox.GetBindingExpression(TextBox.TextProperty);
                    bindingExpression.UpdateSource();
                    InvokeActions(textBox.Text);
                }
                else if (passwordBox != null)
                {
                    var bindingExpression = passwordBox.GetBindingExpression(PasswordBox.PasswordProperty);
                    bindingExpression.UpdateSource();
                    InvokeActions(passwordBox.Password);
                }

            }
        }
    }
}
