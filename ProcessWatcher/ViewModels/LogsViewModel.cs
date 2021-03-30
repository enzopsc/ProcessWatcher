using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using ReactiveUI;
using Splat;

namespace ProcessWatcher.ViewModels
{
	public interface ILogsViewModelFactory
	{
		ILogsViewModel GenerateLogsViewModel(IObservable<DataReceivedEventArgs> processObserver, IScheduler mainScreenScheduler);
	}

	public class LogsViewModelFactory : ILogsViewModelFactory
	{
		public ILogsViewModel GenerateLogsViewModel(IObservable<DataReceivedEventArgs> processObserver, IScheduler mainScreenScheduler)
		{
			return new LogsViewModel(processObserver, mainScreenScheduler);
		}
	}
	public interface ILogsViewModel : IRoutableViewModel
	{
		ObservableCollection<string> ConsoleOutput { get; }
		ReactiveCommand<Unit,Unit> GoBackCommand { get; }
	}

	public class LogsViewModel : ReactiveObject, ILogsViewModel
	{
		public ObservableCollection<string> ConsoleOutput { get; } = new();
		public ReactiveCommand<Unit, Unit> GoBackCommand { get; }

		public LogsViewModel(IObservable<DataReceivedEventArgs> processObserver, IScheduler mainScreenScheduler)
		{
			HostScreen = Locator.Current.GetService<IMainScreen>();
			GoBackCommand = ReactiveCommand.Create(() =>
			{
				this.HostScreen.Router.NavigateBack.Execute().Subscribe();
			});
			processObserver?.ObserveOn(mainScreenScheduler).Subscribe(x => ConsoleOutput.Add(x.Data));
		}

		public string? UrlPathSegment => null;
		public IScreen HostScreen { get; }
	}
}