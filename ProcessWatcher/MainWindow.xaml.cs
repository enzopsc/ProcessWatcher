using System;
using System.Windows.Input;
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
            var host = Locator.Current.GetService<IMainScreen>();
            DataContext = host;
            InitializeComponent();
            host.Router.NavigateAndReset.Execute(Locator.Current.GetService<IMainViewModel>()).Subscribe();
            // mainViewModel.Global.NotificationEvent += GlobalOnNotificationEvent;

            // this.WhenActivated(d =>
            // {
            //
            // });
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
                //new CustomNotification()
                notificationManager.Show(
                    content,
                    expirationTime: TimeSpan.MaxValue,
                    onClick: () =>
                    {

                    },
                    areaName: nameof(WindowArea));
            });

        }
    }
}