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
		private IObservable<EventPattern<DataReceivedEventArgs>> _processObservervable;

		public ProcessObserver(string fileName)
		{
			var p = new Process();
			p.StartInfo.FileName = fileName;
			p.StartInfo.WorkingDirectory = Path.GetDirectoryName(fileName);
			p.StartInfo.UseShellExecute = false;
			p.StartInfo.RedirectStandardOutput = true;
			p.StartInfo.RedirectStandardError = true;
			this._processObservervable = Observable.Defer(() =>
			{
				IObservable<EventPattern<DataReceivedEventArgs>> result = p.ObservableProcessRead();
				p.Start();
				return result;
			});
		}

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


		public IDisposable Subscribe(IObserver<EventPattern<DataReceivedEventArgs>> observer)
		{

			return _processObservervable.Subscribe(observer);
		}
	}
}