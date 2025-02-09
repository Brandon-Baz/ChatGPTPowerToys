using System;
using System.Windows;
using System.Windows.Controls;
using ChatGPT.StandaloneApp.Services;

namespace ChatGPT.StandaloneApp
{
    public partial class MainWindow : Window
    {
        private readonly ChatGPTService _chatGPTService;
        private Controls.FloatingBubble _floatingBubble;

        public MainWindow()
        {
            InitializeComponent();
            _chatGPTService = new ChatGPTService();
            _floatingBubble = new Controls.FloatingBubble();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            if (_floatingBubble != null)
            {
                _floatingBubble.Visibility = Visibility.Collapsed;
            }
        }

        private async void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            string userInput = QueryInput.Text;

            if (string.IsNullOrWhiteSpace(userInput))
            {
                MessageBox.Show("Please enter a query.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Display a loading message
                ResponseBrowser.NavigateToString("<html><body><p>Opening ChatGPT...</p></body></html>");

                // Open ChatGPT in the browser with the query
                await Task.Run(() => _chatGPTService.OpenChatGPT(userInput));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while opening the browser: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ToggleFloatingBubbleMode()
        {
            if (_floatingBubble.Visibility == Visibility.Visible)
            {
                _floatingBubble.Visibility = Visibility.Collapsed;
            }
            else
            {
                _floatingBubble.Visibility = Visibility.Visible;
            }
        }
    }
}