using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using DynamicData.Binding;
using ReactiveUI;

namespace ProcessWatcher.ViewModels
{
	public interface IMainViewModel : IRoutableViewModel
	{

	}
	public class MainViewModel : ReactiveObject, IMainViewModel
	{
		public MainViewModel(IScreen screen, IScheduler mainThreadScheduler)
		{
			HostScreen = screen;
			var processViewModel = new ProcessViewModel(@"C:\Processes\TestRunningConsole1.exe") {AutoRestart = true};
			processViewModel.LogRows
				.ObserveCollectionChanges()
				.Select(e => e.EventArgs)
				.Subscribe(x => x.NewItems.Cast<string>().ToList().ForEach(z => Trace.WriteLine(z)));
		}
		public string? UrlPathSegment => null;
		public IScreen HostScreen { get; }
	}
}