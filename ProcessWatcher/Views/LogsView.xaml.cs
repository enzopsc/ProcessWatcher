using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
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
				
				this.Events().MouseMove.Select(e => Unit.Default)
					.Merge(Observable.Range(0, 1)
						.Select(_ => Unit.Default))
#if DEBUG
					.Throttle(TimeSpan.FromSeconds(5))
#else
					.Throttle(TimeSpan.FromMinutes(5))
#endif
					.ObserveOn(RxApp.MainThreadScheduler)
					.Subscribe(_ => this.ViewModel!.GoBackCommand.Execute().Subscribe())
					.DisposeWith(x);
			});
			this.Loaded += (_, _) => this.Scroller.ScrollToBottom();
		}
	}
}