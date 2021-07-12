using System;
using System.Reflection;
using System.Threading;

namespace TestRunningConsole
{
	internal class Program
	{
		public static void Main(string[] args)
		{
			// for (int i = 1; i <= 5000; i++)
			int i = 0;
			while (true)
			{
				i++;
				Thread.Sleep(10);
				Console.WriteLine($"{Assembly.GetExecutingAssembly().GetName().Name} - Log {i}");
			}
		}
	}
}