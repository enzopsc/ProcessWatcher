using System.Reactive.Concurrency;
using ProcessWatcher.ViewModels;
using Splat;

namespace ProcessWatcher.Core
{
	public interface IProcessFactory
	{
		IProcessViewModel BuildProcessViewModel(string fullName, bool autoRestart, string groupKey, IScheduler mainThreadScheduler = null);
		// IProcessConfiguration BuildProcessConfiguration(string fullName, bool autoRestart);
	}
	public class ProcessFactory : IProcessFactory
	{
		public IProcessViewModel BuildProcessViewModel(string fullName, bool autoRestart, string groupKey, IScheduler mainThreadScheduler = null)
		{
			return new ProcessViewModel(fullName, mainThreadScheduler){AutoRestart = autoRestart, GroupKey = groupKey};
		}

		// public IProcessConfiguration BuildProcessConfiguration(string fullName, bool autoRestart)
		// {
		// 	return new ProcessConfiguration(fullName, autoRestart);
		// }
	}
}