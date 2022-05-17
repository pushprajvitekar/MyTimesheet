using NLog;

using System.Windows;

namespace MyTimesheet
{
    /// <summary>
    /// Interaction logic for App.xamlb
    /// </summary>
    public partial class App : Application
    {
        readonly Logger logger = LogManager.GetCurrentClassLogger();
        public App()
        {
            this.Dispatcher.UnhandledException += OnDispatcherUnhandledException;
        }

        void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            string errorMessage = string.Format("An unhandled exception occurred: {0}", e.Exception.Message);
            logger.Error(e.Exception, errorMessage);
            MessageBox.Show("Oh no!!! Something went wrong!! Totally our fault. Kindly try again, if error persists contact support.", "Bad dog", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }
    }
}
