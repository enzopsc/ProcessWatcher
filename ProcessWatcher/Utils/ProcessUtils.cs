using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using BECCore.AutoLog;
using MoreLinq;

namespace ProcessWatcher.Utils
{
	public static class ProcessUtils
	{
		public static void KillProcess(string fileName)
		{
			try
			{
				Process.GetProcessesByName(Path.GetFileNameWithoutExtension(fileName)).Where(e => e.MainModule?.FileName.Equals(fileName, StringComparison.InvariantCultureIgnoreCase) ?? false).ForEach(e => e.Kill());
			}
			catch(Exception ex)
			{
				Logging.Logger.Error("-> ProcessUtils -> KillProcess : ", ex);
			}
		}

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