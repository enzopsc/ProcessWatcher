using System;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using DynamicData;

namespace ProcessWatcher.Utils
{
	public static class Extensions
	{
		public static IObservable<EventPattern<EventArgs>> ObservableProcessExit(this Process p) => p.HasExited ? Observable.Return(new EventPattern<EventArgs>(p, EventArgs.Empty)) : Observable.FromEventPattern<EventHandler, EventArgs>(h => p.Exited += h, h => p.Exited -= h).Take(1);

		public static IObservable<EventPattern<DataReceivedEventArgs>> ObservableProcessRead(this Process p, bool completeOnProcessExit = true) => Observable.FromEventPattern<DataReceivedEventHandler, DataReceivedEventArgs>(
			h =>
			{
				p.ErrorDataReceived += h;
				p.BeginOutputReadLine();
			},
			h => p.ErrorDataReceived -= h).Merge(Observable.FromEventPattern<DataReceivedEventHandler, DataReceivedEventArgs>(
			h =>
			{
				p.OutputDataReceived += h;
				p.BeginErrorReadLine();
			},
			h => p.OutputDataReceived -= h)).TakeUntil(p.ObservableProcessExit().Where(e => completeOnProcessExit));
	}
}