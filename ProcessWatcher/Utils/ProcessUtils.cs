using System;
using System.Diagnostics;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;

namespace ProcessWatcher.Utils
{
	public static class ProcessUtils
	{

		public static IObservable<EventPattern<DataReceivedEventArgs>> ProcessObservable(string fileName)
		{
			var p = new Process();
			p.StartInfo.FileName = fileName;
			p.StartInfo.WorkingDirectory = Path.GetDirectoryName(fileName);
			p.StartInfo.UseShellExecute = false;
			p.StartInfo.RedirectStandardOutput = true;
			p.StartInfo.RedirectStandardError = true;
			return Observable.Defer(() =>
			{
				IObservable<EventPattern<DataReceivedEventArgs>> result = p.ObservableProcessRead();
				p.Start();
				return result;
			});
		}
	}
}