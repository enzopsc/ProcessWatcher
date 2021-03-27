using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using DynamicData.Binding;
using ProcessWatcher.ViewModels;

namespace ProcessWatcher.Tests
{
	public class Test
	{
		public class Tests
		{
			[SetUp]
			public void Setup()
			{

			}

			[TestAttribute]
			public async Task Test1()
			{
				var testScheduler = new TestScheduler();
				// new TestScheduler().With(scheduler =>
				// {
				// 	// Initialize a new view model using the TestScheduler.
				// 	var model = new LoginViewModel(scheduler);
				//
				// 	// Play with time.
				// 	scheduler.AdvanceByMs(2 * 100);
				// });
				var processViewModel = new ProcessViewModel(@"C:\Processes\TestRunningConsole1.exe", testScheduler) {AutoRestart = true};
				var observable = processViewModel.LogRows
					.ObserveCollectionChanges()
					.Select(e => e.EventArgs)
					.Publish();
				processViewModel.LogRows
					.ObserveCollectionChanges()
					.Select(e => e.EventArgs)
					.Subscribe(x => x.NewItems.Cast<string>().ToList().ForEach(z => Trace.WriteLine(z)));
				observable.Connect();
				await Task.Delay(10000);
				await processViewModel.StopCommand.Execute();
				await Task.Delay(10000);
				await processViewModel.StartCommand.Execute();
				await observable;
				Assert.Pass();
			}
		}
	}
}