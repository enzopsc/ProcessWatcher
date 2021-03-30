using System;
using System.Diagnostics;
using System.Windows.Input;
using BECCore.AutoLog;
using Notifications.Wpf;
using ProcessWatcher.ViewModels;
using ReactiveUI;
using Splat;

namespace ProcessWatcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : IActivatableView
    {
        public MainWindow()
        {
            Statics.NotificationEvent += GlobalOnNotificationEvent;
            var host = Locator.Current.GetService<IMainScreen>();
            DataContext = host;
            this.Closed += OnClosed;
            InitializeComponent();
            host.Router.NavigateAndReset
                .Execute(Locator.Current.GetService<IMainViewModel>())
                .Subscribe();
        }

        private void OnClosed(object sender, EventArgs e)
        {
            foreach (var appConfigProcessConfiguration in Statics.AppConfig.ProcessConfigurations)
                foreach (var process in Process.GetProcessesByName(appConfigProcessConfiguration.FileName))
                    try { process.Kill(); }catch(Exception ex) { Logging.Logger.Error("-> MainWindow -> OnClosed : ", ex); }
        }

        private void GlobalOnNotificationEvent(object sender, NotificationEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                var notificationManager = new NotificationManager();
                var content = new NotificationContent
                {
                    Title = e.Title,
                    Message = e.Message,
                    Type = e.NotificationType
                };
                notificationManager.Show(
                    content,
                    expirationTime: TimeSpan.FromSeconds(5),
                    onClick: () =>
                    {

                    },
                    areaName: nameof(WindowArea));
            });
        }
    }
}