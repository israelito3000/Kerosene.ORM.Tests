namespace Kerosene.ORM.Maps.Eager.Tests
{
	using Kerosene.Tools;
	using System;

	// ==================================================== 
	class Program
	{
		static void Main(string[] args)
		{
			DebugEx.IndentSize = 2;
			DebugEx.AutoFlush = true;
			DebugEx.AddConsoleListener();
			ConsoleEx.AskInteractive();

			TestsLauncher.Execute();
			ConsoleEx.ReadLine("\n=== Press [Enter] to finish... ");
		}
	}
}
