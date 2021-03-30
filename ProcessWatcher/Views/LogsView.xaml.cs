using System;
using System.Reactive.Disposables;
using System.Windows.Controls;
using DynamicData.Binding;
using ReactiveUI;

namespace ProcessWatcher.Views
{
	public partial class LogsView
	{
		public LogsView()
		{
			InitializeComponent();
			this.WhenActivated(x =>
			{
				if (this.ViewModel == null) return;
				this.ViewModel.ConsoleOutput.ObserveCollectionChanges()
					.Subscribe(x => this.Scroller.Dispatcher.Invoke(() => this.Scroller.ScrollToBottom()))
					.DisposeWith(x);
			});
			this.Loaded += (_, _) => this.Scroller.ScrollToBottom();
		}
	}
}