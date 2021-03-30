using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using DynamicData.Binding;
using ProcessWatcher.Core;
using ProcessWatcher.Utils;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;

namespace ProcessWatcher.ViewModels
{

	public interface IMainViewModel : IRoutableViewModel
	{
		ObservableCollection<IProcessViewModel> ProcessViewModels { get; }
		ObservableCollection<IGrouping<string, IProcessViewModel>> GroupedProcessViewModels { get; }
		bool AddProcess(string path);
		bool RemoveProcess(IProcessViewModel processViewModel);



	}
	public class MainViewModel : ReactiveObject, IMainViewModel
	{
		private IScheduler _mainThreadScheduler;
		public MainViewModel(IScreen screen, IScheduler mainThreadScheduler)
		{
			HostScreen = screen;
			_mainThreadScheduler = mainThreadScheduler;
			// var processViewModelFactory = Locator.Current.GetService<IProcessFactory>();
			ProcessViewModels = Statics.AppConfig.ProcessConfigurations;
			ProcessViewModels
				.ObserveCollectionChanges()
				.ObserveOn(mainThreadScheduler)
				.Select(e => new ObservableCollection<IGrouping<string, IProcessViewModel>>(ProcessViewModels.GroupBy(c => c.Path)))
				.ToPropertyEx(this, s => s.GroupedProcessViewModels);
		}

		public ObservableCollection<IProcessViewModel> ProcessViewModels { get; }
		public extern ObservableCollection<IGrouping<string, IProcessViewModel>> GroupedProcessViewModels { [ObservableAsProperty]get; }

		public bool AddProcess(string _)
		{
			return Statics.AppConfig.AddNewProcess(Locator.Current.GetService<IProcessFactory>().BuildProcessViewModel(_, false, _mainThreadScheduler));
		}
		public bool RemoveProcess(IProcessViewModel _) => Statics.AppConfig.RemoveProcess(_);

		public string? UrlPathSegment => null;
		public IScreen HostScreen { get; }
	}
}