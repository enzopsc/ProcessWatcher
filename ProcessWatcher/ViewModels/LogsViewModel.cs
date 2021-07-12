using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData.Binding;
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
	public interface ILogsViewModel : IRoutableViewModel, IActivatableViewModel
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
			Activator = new ViewModelActivator();
			HostScreen = Locator.Current.GetService<IMainScreen>();
			GoBackCommand = ReactiveCommand.Create(() =>
			{
				this.HostScreen.Router.NavigateBack.Execute().Subscribe();
			});

			this.WhenActivated(_ =>
			{
				GoBackCommand.DisposeWith(_);
				processObserver?.ObserveOn(mainScreenScheduler)
					.Subscribe(x =>
						{
							lock(ConsoleOutput)
								ConsoleOutput.Add(x.Data);
						})
					.DisposeWith(_);
				ConsoleOutput
					.ObserveCollectionChanges()
					.Select(e =>
					{
						lock (ConsoleOutput) return ConsoleOutput.Count;
					})
					.Where(e => e > Statics.AppConfig.LogsBufferSize)
					.ObserveOn(RxApp.MainThreadScheduler)
					.Subscribe(_ =>
						{
							lock (ConsoleOutput)
								for (int i = ConsoleOutput.Count; i > Statics.AppConfig.LogsBufferSize; i--)
									ConsoleOutput.RemoveAt(0);
						})
					.DisposeWith(_);
			});
			
		}

		public string? UrlPathSegment => null;
		public IScreen HostScreen { get; }

		
		public ViewModelActivator Activator { get; }
	}
}