using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using ProcessWatcher.Utils;

namespace ProcessWatcher.Core
{
	public class ProcessObserver : IObservable<DataReceivedEventArgs>, IDisposable
	{
		private class Unsubscriber : IDisposable
		{
			private List<IObserver<DataReceivedEventArgs>> _observers;
			private IObserver<DataReceivedEventArgs> _observer;
			public Unsubscriber(IObserver<DataReceivedEventArgs> observer, List<IObserver<DataReceivedEventArgs>> observers)
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
		private IConnectableObservable<DataReceivedEventArgs> _processObservervable;
		private List<IObserver<DataReceivedEventArgs>> _observers = new();
		private CircularBuffer<DataReceivedEventArgs> _buffer = new(Statics.AppConfig.LogsBufferSize);
		private Process _p;
		public ProcessObserver(string fileName)
		{
			if (!File.Exists(fileName))
				throw new FileNotFoundException("File not found", fileName);
			ProcessUtils.KillProcess(fileName);
			_p = new Process();
			_p.EnableRaisingEvents = true;
			_p.StartInfo.FileName = fileName;
			_p.StartInfo.WorkingDirectory = Path.GetDirectoryName(fileName);
			_p.StartInfo.UseShellExecute = false;
			_p.StartInfo.CreateNoWindow = true;
			_p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
			_p.StartInfo.RedirectStandardOutput = true;
			_p.StartInfo.RedirectStandardError = true;
			//Fix Duplicate logs
			// this._processObservervable = process.Window(() => process.Throttle(TimeSpan.FromMilliseconds(500)))
			// 	.SelectMany(e => e.Select(z => z.EventArgs).ToList())
			// 	.SelectMany(e => e.DistinctBy(z => z.Data.ToLower()).ToObservable())
			// 	.Do(x =>
			// 	{
			// 		try
			// 		{
			// 			foreach (var observer in _observers)
			// 				observer.OnNext(x);
			// 		}
			// 		catch
			// 		{
			// 		}
			// 		_buffer.Add(x);
			// 	})
			// 	.Publish();
			this._processObservervable = _p.ObservableProcessRead()
				.Select(e => e.EventArgs)
				.Do(x =>
				{
					lock(_buffer)
						_buffer.Add(x);
				})
				.Publish();
		}

		~ProcessObserver()
		{
			this.Dispose();
		}

		public bool Start()
		{
			var result = _p.Start();
			if (result)
			{
				_p.BeginOutputReadLine();
				_p.BeginErrorReadLine();
			}
			this._processObservervable.Connect();
			return result;
		}

		public bool Stop()
		{
			try
			{
				_p.Kill();
				// _observers.ForEach(e => e.OnCompleted());
			}
			catch
			{

			}
			return true;
		}

		public IDisposable Subscribe(IObserver<DataReceivedEventArgs> observer)
		{
			_observers.Add(observer);
			lock (_buffer)
			{
				foreach (var dataReceivedEventArgs in _buffer.Latest())
					observer.OnNext(dataReceivedEventArgs);
			}
			_processObservervable.Subscribe(observer);
			return new Unsubscriber(observer, _observers);
		}

		public void Dispose()
		{
			_p?.Dispose();
		}
	}
}