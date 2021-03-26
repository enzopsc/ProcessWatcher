using System.Threading.Tasks;
using ProcessWatcher.Utils;

namespace ProcessWatcher.ViewModels
{
	public enum ProcessStatus
	{
		Running,
		Crashed,
		Stopped,
	}
	public interface IProcessViewModel
	{
		string Path { get; }
		ProcessStatus Status { get; }
		Task<bool> Start();
		Task<bool> Stop();
	}
	public class ProcessViewModel : IProcessViewModel
	{
		public ProcessViewModel(string fullName)
		{
			Path = fullName;
			var process = ProcessUtils.ProcessObservable(Path);
		}
		public string Path { get; }
		public ProcessStatus Status { get; }
		public Task<bool> Start()
		{
			return null;
		}

		public Task<bool> Stop()
		{
			return null;
		}
	}
}