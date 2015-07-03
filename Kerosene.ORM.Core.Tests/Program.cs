using Kerosene.Tools;
using System;

namespace Kerosene.ORM.Core.Tests
{
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
