using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kerosene.Tools;
using System;
using System.Linq;

namespace Kerosene.ORM.Core.Tests
{
	// ==================================================== 
	[TestClass]
	public class Test_InsertCommand
	{
		//[OnlyThisTest]
		[TestMethod]
		public void EmptyCommand()
		{
			var link = new FakeLink(new Concrete.DataEngine(), NestableTransactionMode.Database);
			var cmd = link.Insert(x => x.Employees);
			var str = cmd.TraceString().NullIfTrimmedIsEmpty() ?? "<empty>";

			ConsoleEx.WriteLine("\n> {0}", str);
			Assert.IsFalse(cmd.CanBeExecuted);
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Examples()
		{
			var link = new FakeLink(new Concrete.DataEngine(), NestableTransactionMode.Database);

			var cmd = link
				.Insert(x => x.Emp)
				.Columns(
					x => x.Id = "007",
					x => x.Name = "James Bond");

			Assert.AreEqual("INSERT INTO Emp (Id, Name) VALUES (#0, #1)", cmd.GetCommandText(false));
		}
	}
}
