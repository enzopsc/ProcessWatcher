using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ProcessWatcher.Core;
using ProcessWatcher.Utils;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;

namespace ProcessWatcher.ViewModels
{
	public enum ProcessStatus
	{
		Error = -1,
		Created,
		Starting,
		Running,
		Stopped,
		ManuallyStopped
	}

	public interface IProcess
	{
		string Path { get; set; }
		ProcessStatus Status { get; set; }
		bool AutoRestart { get; set; }
	}
	public interface IProcessViewModel
	{
		string Path { get; set; }
		string FileName { get; }
		bool AutoRestart { get; set; }
		string GroupKey { get; set; }
		bool IsValid { get; }
		ObservableCollection<string> LogRows { get; }
		ProcessStatus Status { get; }
		bool CanStart { get; }
		bool CanStop { get; }
		ReactiveCommand<Unit, bool> StartCommand { get; }
		ReactiveCommand<Unit, bool> StopCommand { get; }

		ReactiveCommand<Unit, Unit> ConsoleCommand { get; }
		ReactiveCommand<Unit, bool> DeleteCommand { get; }
	}

	public class ProcessViewModel : ReactiveObject, IProcessViewModel
	{

		public ProcessViewModel(string fullName, IScheduler mainThreadScheduler = null)
		{
			mainThreadScheduler ??= RxApp.MainThreadScheduler;
			Path = fullName;
			this.WhenAnyValue(x => x.Path)
				.Select(e => System.IO.Path.GetFileNameWithoutExtension(Path))
				.ToPropertyEx(this, s => s.FileName);
			ConsoleCommand = ReactiveCommand.Create(() =>
			{
				var logsViewModel = Locator.Current.GetService<ILogsViewModelFactory>().GenerateLogsViewModel(this._processObserver, mainThreadScheduler);
				Locator.Current.GetService<IMainScreen>().Router.Navigate.Execute(logsViewModel).Subscribe();
			});
			StartCommand = ReactiveCommand.Create(Start, this.WhenAnyValue(e => e.CanStart).ObserveOn(mainThreadScheduler), mainThreadScheduler);
			StopCommand = ReactiveCommand.CreateFromTask(async () => await Stop(), this.WhenAnyValue(e => e.CanStop).ObserveOn(mainThreadScheduler), mainThreadScheduler);
			DeleteCommand = ReactiveCommand.CreateFromTask<Unit, bool>(async _ =>
			{
				if(this.CanStop)
					await Stop();
				this.ChangeToDeleted();
				return Statics.AppConfig.RemoveProcess(this);
			}, null, mainThreadScheduler);
			this.WhenAnyValue(e => e.AutoRestart)
				.Throttle(TimeSpan.FromSeconds(5))
				.Do(_ =>Statics.AppConfig.UpdateProcess(this))
				.Where(e => this.Status != ProcessStatus.Running && this.AutoRestart)
				.Subscribe(x =>
				{
					this.StartCommand.Execute().Subscribe();
				});
		}

		private async Task<bool> Stop()
		{
			if (!_canStop) return false;
			this.Status = ProcessStatus.ManuallyStopped;
			ChangeToCanStart();
			this._processObserver.Stop();
			lock (LogRows)
				LogRows.Add(Language.ProcessStarted);
			await _processObserver;
			return true;
		}

		private bool Start()
		{
			if (!_canStart) return false;
			this.Status = ProcessStatus.Starting;
			ChangeToCanStop();
			SetupProcessObserver();
			var started = this._processObserver.Start();
			if (started)
				this.Status = ProcessStatus.Running;
			return started;
		}

		private void ChangeToCanStop()
		{
			this.RaisePropertyChanging(nameof(CanStop));
			this.RaisePropertyChanging(nameof(CanStart));
			_canStart = false;
			_canStop = true;
			this.RaisePropertyChanged(nameof(CanStop));
			this.RaisePropertyChanged(nameof(CanStart));
		}

		private void ChangeToCanStart()
		{
			this.RaisePropertyChanging(nameof(CanStop));
			this.RaisePropertyChanging(nameof(CanStart));
			_canStop = false;
			_canStart = true;
			this.RaisePropertyChanged(nameof(CanStop));
			this.RaisePropertyChanged(nameof(CanStart));
		}
		private void ChangeToDeleted()
		{
			this.RaisePropertyChanging(nameof(CanStop));
			this.RaisePropertyChanging(nameof(CanStart));
			_canStop = false;
			_canStart = false;
			this.RaisePropertyChanged(nameof(CanStop));
			this.RaisePropertyChanged(nameof(CanStart));
		}

		private ProcessObserver _processObserver;
		private bool _canStart = true;
		private bool _canStop = false;
		public string Path { get; set; }
		public extern string FileName { [ObservableAsProperty]get; }
		public string GroupKey { get; set; }
		public bool IsValid => File.Exists(Path);
		public ObservableCollection<string> LogRows { get; } = new();

		[Reactive] public ProcessStatus Status { get; private set; }
		[Reactive] public bool AutoRestart { get; set; }


		public bool CanStart => _canStart;
		public bool CanStop => _canStop;
		public ReactiveCommand<Unit, bool> StartCommand { get; }
		public ReactiveCommand<Unit, bool> StopCommand { get; }
		public ReactiveCommand<Unit, Unit> ConsoleCommand { get; }
		public ReactiveCommand<Unit, bool> DeleteCommand { get; }

		private void SetupProcessObserver()
		{
			_processObserver?.Dispose();
			this._processObserver = new ProcessObserver(Path);
			_processObserver
				.Subscribe(x =>
				{
					lock (LogRows)
						LogRows.Add(x.Data);
				}, async () =>
				{
					this.ChangeToCanStart();
					if(this.Status == ProcessStatus.ManuallyStopped)
						return;
					this.Status = ProcessStatus.Stopped;
					if (!this.AutoRestart)
						return;
					try
					{
						do
						{
							await Task.Delay(5000);
						} while (!this.Start());
					}
					catch (Exception e)
					{
						Status = ProcessStatus.Error;
					}
				});
		}
	}
}