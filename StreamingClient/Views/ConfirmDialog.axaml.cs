using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace StreamingClient.Views
{
    public partial class ConfirmDialog : Window
    {
        public bool Confirmed { get; private set; }

        public ConfirmDialog(string message)
        {
            InitializeComponent();
            this.FindControl<TextBlock>("MessageText").Text = message;
        }

        private void OnCancel(object? sender, RoutedEventArgs e)
        {
            Confirmed = false;
            Close(false);
        }

        private void OnConfirm(object? sender, RoutedEventArgs e)
        {
            Confirmed = true;
            Close(true);
        }
    }
}

