using System;
using System.Diagnostics;
using System.IO;
using System.Reactive.Linq;
using NUnit.Framework;
using ProcessWatcher.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using DynamicData.Binding;
using Microsoft.Reactive.Testing;
using ReactiveUI;
using ReactiveUI.Testing;

namespace UnitTester
{
	public class ViewModelsTest
	{
		[SetUp]
		public void Setup()
		{
		}

		[Test]
		public async Task ProcssViewModelTest()
		{
			var testScheduler = new TestScheduler();
			var processPath = @"..\..\..\..\TestRunningConsole\bin\Debug\TestRunningConsole.exe";
			var processName = Path.GetFileNameWithoutExtension(processPath);
			var processViewModel = new ProcessViewModel(processPath, testScheduler) {AutoRestart = true};
			processViewModel
				.WhenAnyValue(x => x.Status)
				.Subscribe(x => Trace.WriteLine(x));
			// var observable = processViewModel.LogRows
			// 	.ObserveCollectionChanges()
			// 	.Select(e => e.EventArgs)
			// 	.Publish();
			// processViewModel.LogRows
			// 	.ObserveCollectionChanges()
			// 	.Select(e => e.EventArgs)
			// 	.Subscribe(x => x.NewItems.Cast<string>().ToList().ForEach(z => Trace.WriteLine(z)));
			// observable.Connect();
			await Task.Delay(5000);
			processViewModel.StopCommand.Execute().Subscribe();
			await Task.Delay(2000);
			processViewModel.StartCommand.Execute().Subscribe();
			// await observable.Take(20);
			var stopped = processViewModel
				.WhenAnyValue(x => x.Status)
				.Where(e => e == ProcessStatus.Stopped)
				.FirstAsync();
			var running = processViewModel
				.WhenAnyValue(x => x.Status)
				.Where(e => e == ProcessStatus.Running)
				.FirstAsync();
			foreach (var process in Process.GetProcessesByName(processName))
				process.Kill();
			await stopped;
			await running;
			Assert.Pass();
		}
	}
}