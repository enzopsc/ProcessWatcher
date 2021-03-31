using System;
using System.IO;
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
							var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file);
							var groupKey = await metroWindow.ShowInputAsync(ProcessWatcher.Language.Resources.SetGroupKeyTitle, ProcessWatcher.Language.Resources.SetGroupKeyMessage.Replace("$CONTENT$", fileNameWithoutExtension));
							if (this.ViewModel.AddProcess(file, groupKey.ToUpper()))
								Statics.Notify(this, new NotificationEventArgs(ProcessWatcher.Language.Resources.ProcessAddedSuccess, ProcessWatcher.Language.Resources.ProcessAddedMessage.Replace("$CONTENT$", fileNameWithoutExtension), NotificationType.Success));
							else
								Statics.Notify(this, new NotificationEventArgs(ProcessWatcher.Language.Resources.ProcessAddedFail, ProcessWatcher.Language.Resources.ProcessAddedFailMessage.Replace("$CONTENT$", fileNameWithoutExtension), NotificationType.Error));
						}
					})
					.DisposeWith(x);
			});

		}
	}
}