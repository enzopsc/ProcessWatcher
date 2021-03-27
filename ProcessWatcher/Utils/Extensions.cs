using System;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using DynamicData;

namespace ProcessWatcher.Utils
{
	public static class Extensions
	{
		public static IObservable<EventPattern<EventArgs>> ObservableProcessExit(this Process p) =>
			Observable.FromEventPattern<EventHandler, EventArgs>(h => p.Exited += h, h => p.Exited -= h)
				.FirstOrDefaultAsync();

		public static IObservable<EventPattern<DataReceivedEventArgs>> ObservableProcessRead(this Process p, bool completeOnProcessExit = true) => Observable.FromEventPattern<DataReceivedEventHandler, DataReceivedEventArgs>(
			h =>
			{
				p.ErrorDataReceived += h;

			},
			h => p.ErrorDataReceived -= h).Merge(Observable.FromEventPattern<DataReceivedEventHandler, DataReceivedEventArgs>(
			h =>
			{
				p.OutputDataReceived += h;

			},
			h => p.OutputDataReceived -= h)).TakeUntil(p.ObservableProcessExit().Where(e => completeOnProcessExit));
	}
}