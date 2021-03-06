﻿using System;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using DivineShopMonitor.Annotations;
using DivineShopMonitor.Model;
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
            AppendText($"{DateTime.Now:hh:mm:ss} - Starting...");
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
            _dispatcherTimer.Interval = new TimeSpan(0, 0, 10);
            _dispatcherTimer.Start();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            AppendText($"{DateTime.Now:hh:mm:ss} - Checking...");
            try
            {
                if (chkGarena2500.IsChecked == true) CheckForStockAsync("Garena 1M-25%", "http://divineshop.vn/garena-2500?tag=garena");
                if (chkGarena2400.IsChecked == true) CheckForStock("Garena 1M-20%", "http://divineshop.vn/garena-2400?tag=garena");
                if (chkGarena520.IsChecked == true) CheckForStock("Garena 520", "http://divineshop.vn/garena-520?tag=garena");
                if (chkGarena500.IsChecked == true) CheckForStock("Garena 500", "http://divineshop.vn/garena-500?tag=garena");
                if (chkGarena200.IsChecked == true) CheckForStock("Garena 200", "http://divineshop.vn/garena-200?tag=garena");
                if (chkGarena100.IsChecked == true) CheckForStock("Garena 100", "http://divineshop.vn/garena-100?tag=garena");
                if (chkGarena87.IsChecked == true) CheckForStock("Garena 87", "http://divineshop.vn/garena-87?tag=garena");
            }
            catch (Exception ex)
            {
                AppendText(ex.Message);
            }
        }

        private void CheckForStockAsync(string productName, string productUrl)
        {
            Task.Run(() =>
            {
                try
                {
                    CheckForStock(productName, productUrl);
                }
                catch (Exception ex)
                {
                    AppendText(ex.Message);
                }
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