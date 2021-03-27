using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ProcessWatcher.Core;
using ProcessWatcher.Utils;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ProcessWatcher.ViewModels
{
	public enum ProcessStatus
	{
		Error = -1,
		Created,
		Running,
		Stopped,
		ManuallyStopped
	}

	public interface IProcessViewModel
	{
		string Path { get; }
		ObservableCollection<string> LogRows { get; }
		ProcessStatus Status { get; }
		bool AutoRestart { get; set; }
		bool CanStart { get; }
		bool CanStop { get; }
		ReactiveCommand<Unit, bool> StartCommand { get; }
		ReactiveCommand<Unit, bool> StopCommand { get; }
	}

	public class ProcessViewModel : ReactiveObject, IProcessViewModel
	{

		public ProcessViewModel(string fullName, IScheduler mainThreadScheduler = null)
		{
			mainThreadScheduler ??= RxApp.MainThreadScheduler;
			Path = fullName;
			StartCommand = ReactiveCommand.Create(() =>
			{
				if (!_canStart) return false;
				ChangeToCanStop();
				SetupProcessObserver();
				return this._processObserver.Start();
			}, this.WhenAnyValue(e => e.CanStart), mainThreadScheduler);
			StopCommand = ReactiveCommand.CreateFromTask(async () =>
			{
				if (!_canStop) return false;
				this.Status = ProcessStatus.ManuallyStopped;
				ChangeToCanStart();
				this._processObserver.Stop();
				lock (LogRows)
					LogRows.Add(Language.Resources.Language.ProcessStarted);
				await _processObserver;
				return true;
			}, this.WhenAnyValue(e => e.CanStop), mainThreadScheduler);
			this.WhenAnyValue(e => e.AutoRestart)
				.Where(e => this.Status != ProcessStatus.Running)
				.Subscribe(x => this.StartCommand.Execute().Subscribe());
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

		private ProcessObserver _processObserver;
		private bool _canStart = true;
		private bool _canStop = false;

		public string Path { get; }
		public ObservableCollection<string> LogRows { get; } = new();

		[Reactive] public ProcessStatus Status { get; private set; }
		[Reactive] public bool AutoRestart { get; set; }

		public bool CanStart => _canStart;
		public bool CanStop => _canStop;
		public ReactiveCommand<Unit, bool> StartCommand { get; }
		public ReactiveCommand<Unit, bool> StopCommand { get; }

		private void SetupProcessObserver()
		{
			this._processObserver = new ProcessObserver(Path);
			_processObserver
				.Subscribe(x =>
				{
					lock (LogRows)
						LogRows.Add(x.EventArgs.Data);
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
						} while (await this.StartCommand.Execute());
						Status = ProcessStatus.Running;
					}
					catch (Exception e)
					{
						Status = ProcessStatus.Error;
					}
				});
		}
	}
}