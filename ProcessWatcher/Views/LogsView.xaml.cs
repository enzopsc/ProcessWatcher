using System;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Controls;
using DynamicData.Binding;
using ProcessWatcher.Utils;
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
				
				// this.ViewModel.ConsoleOutput.ObserveCollectionChanges()
				// 	.Where(x => !this.Scroller.IsFocused)
				// 	.Subscribe(x => this.Scroller.Dispatcher.Invoke(() => this.Scroller.ScrollToBottom()))
				// 	.DisposeWith(x);
				bool autoScroll = false;
				
				this.Scroller.Events().ScrollChanged.Subscribe(e =>
				{
					// User scroll event : set or unset auto-scroll mode
					if (e.ExtentHeightChange == 0)
					{
						// Content unchanged : user scroll event
						if (Math.Abs(this.Scroller.VerticalOffset - this.Scroller.ScrollableHeight) < 0.0001)
						{
							// Scroll bar is in bottom
							// Set auto-scroll mode
							autoScroll = true;
						}
						else
						{
							// Scroll bar isn't in bottom
							// Unset auto-scroll mode
							autoScroll = false;
						}
					}
				
					// Content scroll event : auto-scroll eventually
					if (autoScroll && e.ExtentHeightChange != 0)
					{
						// Content changed and auto-scroll mode set
						// Autoscroll
						this.Scroller.ScrollToVerticalOffset(this.Scroller.ExtentHeight);
					}
				}).DisposeWith(x);
				// this.ViewModel.ConsoleOutput
				// 	.ObserveCollectionChanges()
				// 	.Where(e => e.EventArgs.Action == NotifyCollectionChangedAction.Add)
				// 	.ObserveOn(RxApp.MainThreadScheduler)
				// 	.Subscribe(ea =>
				// 	{
				// 		var item = ea.EventArgs.NewItems.OfType<string>().Last();
				// 		this.ListBox.ScrollToCenterOfView(this.ListBox.SelectedItem ?? item);
				// 	});
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
			// this.Loaded += (_, _) => this.Scroller.ScrollToBottom();
		}
	}
}