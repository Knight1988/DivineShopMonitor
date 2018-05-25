using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using DivineShopMonitor.Annotations;
using HtmlAgilityPack;

namespace DivineShopMonitor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private readonly DispatcherTimer _dispatcherTimer = new DispatcherTimer();
        private readonly StringBuilder _logBuilder = new StringBuilder();
        private string _logText = string.Empty;
        private bool _isDialogOpened;

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

        private void AppendText(string value)
        {
            if (LogText.Count(p => p == '\n') > 100) _logBuilder.Remove(0, LogText.IndexOf('\n') + 1);
            _logBuilder.AppendLine(value);
            LogText = _logBuilder.ToString();
            Dispatcher.Invoke(() =>
            {
                LogTextBox.ScrollToEnd();
            });
        }

        public MainWindow()
        {
            InitializeComponent();
            MainGrid.DataContext = this;
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            _dispatcherTimer.Tick += dispatcherTimer_Tick;
            _dispatcherTimer.Interval = new TimeSpan(0,1,0);
            _dispatcherTimer.Start();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                CheckForStock("Garena 500", "http://divineshop.vn/garena-500?tag=garena");
                CheckForStock("Garena 200", "http://divineshop.vn/garena-200?tag=garena");
            });
        }

        private void CheckForStock(string productName, string productUrl)
        {
            try
            {
                // From Web
                var web = new HtmlWeb();
                var doc = web.Load(productUrl);

                var statusLabel = doc.DocumentNode
                    .Descendants("li")
                    .SingleOrDefault(x => x.InnerText.Contains("Tình trạng"));

                AppendText($"{DateTime.Now:hh:mm:ss} - {productName} {statusLabel?.InnerText}");
                
                if (statusLabel != null && statusLabel.InnerText.Contains("Còn hàng") && !_isDialogOpened)
                {
                    _isDialogOpened = true;
                    MessageBox.Show($"{productName} {statusLabel.InnerText}");
                    _isDialogOpened = false;
                }
            }
            catch (Exception e)
            {
                AppendText($"{DateTime.Now:hh:mm:ss} - Error: {e.Message}");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
