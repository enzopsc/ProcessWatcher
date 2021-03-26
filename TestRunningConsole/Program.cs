using System;
using System.Threading;

namespace TestRunningConsole
{
	internal class Program
	{
		public static void Main(string[] args)
		{
			int counter = 0;
			while (true)
			{
				Console.WriteLine("Test Log");
				counter++;
				Thread.Sleep(1000);
				if (counter > 5000)
					break;
			}
		}
	}
}