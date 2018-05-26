using System;

namespace ConsoleApp
{
	internal static class ScreenOutput
	{
		internal static void Write(string message)
		{
			Console.Write(message);
		}

		internal static void WriteLine(string message, ConsoleColor color = ConsoleColor.Gray)
		{
			Console.ForegroundColor = color;
			Console.WriteLine(message);
			Console.ForegroundColor = ConsoleColor.Gray;
		}
	}
}
