using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kerosene.Tools;
using System;
using System.Linq;

namespace Kerosene.ORM.Core.Tests
{
	// ==================================================== 
	[TestClass]
	public class Test_RawCommand
	{
		//[OnlyThisTest]
		[TestMethod]
		public void EmptyCommand()
		{
			var link = new FakeLink(new Concrete.DataEngine(), NestableTransactionMode.Database);
			var cmd = link.Raw();
			var str = cmd.TraceString().NullIfTrimmedIsEmpty() ?? "<empty>";

			ConsoleEx.WriteLine("\n> {0}", str);
			Assert.IsFalse(cmd.CanBeExecuted);
		}

		//[OnlyThisTest]

		[TestMethod]
		public void Set_Text_And_Arguments()
		{
			var link = new FakeLink(new Concrete.DataEngine(), NestableTransactionMode.Database);

			var str = "SELECT * FROM Employees WHERE LastName == {0}";
			var cmd = link.Raw(str, "Bond");
			ConsoleEx.WriteLine("\n> {0}", cmd.TraceString());

			str = "SELECT * FROM Employees WHERE LastName == #0";
			var temp = cmd.GetCommandText(false);
			Assert.AreEqual(str, temp);
			Assert.AreEqual("Bond", cmd.Parameters[0].Value);
		}
	}
}
