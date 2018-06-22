using System;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using DivineShopMonitor.Annotations;
using HtmlAgilityPack;

namespace DivineShopMonitor
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private readonly DispatcherTimer _dispatcherTimer = new DispatcherTimer();
        private readonly StringBuilder _logBuilder = new StringBuilder();
        private bool _isDialogOpened;
        private string _logText = string.Empty;
        private DateTime _pauseTime = DateTime.Today;

        public MainWindow()
        {
            InitializeComponent();
            MainGrid.DataContext = this;
        }

        public string LogText
        {
            get => _logText;
            set
            {
                if (value == _logText) return;
                _logText = value;
                OnPropertyChanged(nameof(LogText));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void AppendText(string value)
        {
            if (LogText.Count(p => p == '\n') > 100) _logBuilder.Remove(0, LogText.IndexOf('\n') + 1);
            _logBuilder.AppendLine(value);
            LogText = _logBuilder.ToString();
            Dispatcher.Invoke(() => { LogTextBox.ScrollToEnd(); });
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            _dispatcherTimer.Tick += dispatcherTimer_Tick;
            _dispatcherTimer.Interval = new TimeSpan(0, 1, 0);
            _dispatcherTimer.Start();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                CheckForStock("Garena 1000", "http://divineshop.vn/garena-2500?tag=garena");
                CheckForStock("Garena 500", "http://divineshop.vn/garena-500?tag=garena");
                CheckForStock("Garena 200", "http://divineshop.vn/garena-200?tag=garena");
                CheckForStock("Garena 100", "http://divineshop.vn/garena-100?tag=garena");
            });
        }

        private void CheckForStock(string productName, string productUrl)
        {
            try
            {
                // Check product stock
                var web = new HtmlWeb();
                var doc = web.Load(productUrl);

                var statusLabel = doc.DocumentNode
                    .Descendants("li")
                    .SingleOrDefault(x => x.InnerText.Contains("Tình trạng"));

                AppendText($"{DateTime.Now:hh:mm:ss} - {productName} {statusLabel?.InnerText}");

                if (statusLabel != null && statusLabel.InnerText.Contains("Còn hàng") && !_isDialogOpened && DateTime.Now > _pauseTime)
                {
                    // prevent many dialog
                    _isDialogOpened = true;
                    MessageBox.Show($"{productName} {statusLabel.InnerText}", "Prompt", MessageBoxButton.OK, MessageBoxImage.None, MessageBoxResult.None, MessageBoxOptions.DefaultDesktopOnly);
                    _isDialogOpened = false;
                    // No dialog for next 5 min
                    _pauseTime = DateTime.Now.AddMinutes(5);
                }
            }
            catch (Exception e)
            {
                AppendText($"{DateTime.Now:hh:mm:ss} - Error: {e.Message}");
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}