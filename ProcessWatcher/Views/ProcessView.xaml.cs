
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using ReactiveUI;

namespace ProcessWatcher.Views
{
	public partial class ProcessView
	{
		public ProcessView()
		{
			InitializeComponent();
			this.WhenActivated(x =>
			{
				if (this.ViewModel == null) return;
				this.ViewModel.RequestYesNo.RegisterHandler(async _ =>
				{
					if (Window.GetWindow(this) is MetroWindow metroWindow)
						_.SetOutput(await metroWindow.ShowMessageAsync(ProcessWatcher.Language.ConfirmDeleteTitle, ProcessWatcher.Language.ConfirmDeleteMessage.Replace("$CONTENT$", this.ViewModel.FileName), MessageDialogStyle.AffirmativeAndNegative) == MessageDialogResult.Affirmative);
					else
						_.SetOutput(true);
				}).DisposeWith(x);
			});
		}
	}
}