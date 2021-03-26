using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using ProcessWatcher.Utils;

namespace ProcessWatcher.Core
{
	public class ProcessObserver : IObservable<EventPattern<DataReceivedEventArgs>>
	{
		private class Unsubscriber : IDisposable
		{
			private List<IObserver<EventPattern<DataReceivedEventArgs>>> _observers;
			private IObserver<EventPattern<DataReceivedEventArgs>> _observer;
			public Unsubscriber(IObserver<EventPattern<DataReceivedEventArgs>> observer, List<IObserver<EventPattern<DataReceivedEventArgs>>> observers)
			{
				_observers = observers;
				_observer = observer;
			}
			public void Dispose()
			{
				if(_observers.Contains(_observer))
					_observers.Remove(_observer);
			}
		}
		private IObservable<EventPattern<DataReceivedEventArgs>> _processObservervable;
		private List<IObserver<EventPattern<DataReceivedEventArgs>>> _observers = new();
		private Process _p;
		public ProcessObserver(string fileName)
		{
			_p = new Process();
			_p.EnableRaisingEvents = true;
			_p.StartInfo.FileName = fileName;
			_p.StartInfo.WorkingDirectory = Path.GetDirectoryName(fileName);
			_p.StartInfo.UseShellExecute = false;
			_p.StartInfo.RedirectStandardOutput = true;
			_p.StartInfo.RedirectStandardError = true;
			this._processObservervable = _p.ObservableProcessRead();
			//this._processObservervable = Observable.Defer(() =>
			//{
			//	IObservable<EventPattern<DataReceivedEventArgs>> result = p.ObservableProcessRead();
			//	p.Start();
			//	return result;
			//});
		}

		public bool Start()
		{
			var result = _p.Start();
			if (result)
			{
				_p.BeginOutputReadLine();
				_p.BeginErrorReadLine();
			}
			return result;
		}

		public bool Stop()
		{
			try
			{
				_p.Kill();
			}
			catch
			{

			}
			return true;
		}
		public IDisposable Subscribe(IObserver<EventPattern<DataReceivedEventArgs>> observer)
		{
			_observers.Add(observer);
			_processObservervable.Subscribe(observer);
			return new Unsubscriber(observer, _observers);
		}
	}
}