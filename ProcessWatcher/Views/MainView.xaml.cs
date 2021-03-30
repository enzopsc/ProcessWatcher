using System;
using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Notifications.Wpf;
using ProcessWatcher.Core;
using ProcessWatcher.ViewModels;
using ReactiveUI;
using Splat;

namespace ProcessWatcher.Views
{
	public partial class MainView
	{
		public MainView()
		{


			// CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(lvUsers.ItemsSource);
			// PropertyGroupDescription groupDescription = new PropertyGroupDescription("Sex");
			// view.GroupDescriptions.Add(groupDescription);
			InitializeComponent();
			this.WhenActivated(x =>
			{
				if (this.ViewModel == null)
					return;
				// CollectionViewSource src = new CollectionViewSource();
				// src.IsLiveGroupingRequested = true;
				// src.Source = this.ViewModel.ProcessViewModels;
				// src.GroupDescriptions.Add(new PropertyGroupDescription(nameof(ProcessViewModel.GroupKey)));

				this.Events().Drop
					.Subscribe(async dragDropEvent =>
					{
						string[] files = (string[])dragDropEvent.Data.GetData(DataFormats.FileDrop);
						if (files == null)
							return;
						foreach (var file in files)
						{
							if(!file.EndsWith(".exe", StringComparison.InvariantCultureIgnoreCase))
								continue;
							var metroWindow = Window.GetWindow(this) as MetroWindow;
							var groupKey = await metroWindow.ShowInputAsync("Set Group Key", "Set Group Key");
							if (this.ViewModel.AddProcess(file, groupKey))
								Statics.Notify(this, new NotificationEventArgs(ProcessWatcher.Language.ProcessAddedSuccess, ProcessWatcher.Language.ProcessAddedMessage.Replace("$CONTENT$", file), NotificationType.Success));
							else
								Statics.Notify(this, new NotificationEventArgs(ProcessWatcher.Language.ProcessAddedFail, ProcessWatcher.Language.ProcessAddedFailMessage.Replace("$CONTENT$", file), NotificationType.Error));
						}
					})
					.DisposeWith(x);
			});

		}
	}
}