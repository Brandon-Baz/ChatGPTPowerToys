using System;
using System.Windows;

namespace ChatGPT.StandaloneApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Handles application startup logic.
        /// </summary>
        /// <param name="e">Startup event arguments.</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                // Log application startup
                Console.WriteLine("ChatGPT Standalone App is starting...");

                // Initialize the main window
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
            }
            catch (Exception ex)
            {
                // Handle any exceptions during startup
                MessageBox.Show($"An error occurred during application startup: {ex.Message}", "Startup Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
            }
        }

        /// <summary>
        /// Handles unhandled exceptions globally.
        /// </summary>
        /// <param name="sender">The source of the unhandled exception.</param>
        /// <param name="e">Details about the unhandled exception.</param>
        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            // Log the exception
            Console.WriteLine($"Unhandled exception: {e.Exception.Message}");

            // Show a user-friendly error message
            MessageBox.Show($"An unexpected error occurred: {e.Exception.Message}", "Unhandled Exception", MessageBoxButton.OK, MessageBoxImage.Error);

            // Prevent the application from crashing
            e.Handled = true;
        }
    }
}
