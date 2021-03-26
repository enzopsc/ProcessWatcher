using System.Reactive.Concurrency;
using ReactiveUI;

namespace ProcessWatcher.ViewModels
{
	public interface IMainViewModel : IRoutableViewModel
	{

	}
	public class MainViewModel : ReactiveObject, IMainViewModel
	{
		public MainViewModel(IScreen screen, IScheduler mainThreadScheduler)
		{
			HostScreen = screen;
		}
		public string? UrlPathSegment => null;
		public IScreen HostScreen { get; }
	}
}