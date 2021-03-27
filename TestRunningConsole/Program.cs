using System;
using System.Threading;

namespace TestRunningConsole
{
	internal class Program
	{
		public static void Main(string[] args)
		{
			for (int i = 0; i < 5000; i++)
			{
				Console.WriteLine("Test Log");
				Thread.Sleep(1000);
			}
		}
	}
}