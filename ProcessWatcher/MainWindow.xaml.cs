using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;
using BECCore.AutoLog;
using MahApps.Metro.Controls.Dialogs;
using MoreLinq;
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
            this.Closing += OnClosing;
            this.Closed += OnClosed;
            InitializeComponent();
            host.Router.NavigateAndReset
                .Execute(Locator.Current.GetService<IMainViewModel>())
                .Subscribe();
        }

        private bool _isClosing;

        private async void OnClosing(object sender, CancelEventArgs e)
        {
            if (!_isClosing)
            {
                e.Cancel = true;
                if(await this.ShowMessageAsync("Are you sure?", "Closing this app will close all attached process", MessageDialogStyle.AffirmativeAndNegative) == MessageDialogResult.Affirmative)
                {
                    this._isClosing = true;
                    this.Close();
                }
            }

        }

        private void OnClosed(object sender, EventArgs e)
        {
            Statics.AppConfig.ProcessConfigurations.ForEach(c => Utils.ProcessUtils.KillProcess(c.Path));
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