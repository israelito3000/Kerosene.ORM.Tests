using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kerosene.Tools;
using System;
using System.Linq;

namespace Kerosene.ORM.Core.Tests
{
	// ==================================================== 
	[TestClass]
	public class Test_UpdateCommand
	{
		//[OnlyThisTest]
		[TestMethod]
		public void EmptyCommand()
		{
			var link = new FakeLink(new Concrete.DataEngine(), NestableTransactionMode.Database);
			var cmd = link.Update(x => x.Employees);
			var str = cmd.TraceString().NullIfTrimmedIsEmpty() ?? "<empty>";

			ConsoleEx.WriteLine("\n> {0}", str);
			Assert.IsFalse(cmd.CanBeExecuted);
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Examples()
		{
			var link = new FakeLink(new Concrete.DataEngine(), NestableTransactionMode.Database);
			var cmd = link.Update(x => x.Emp);

			try { cmd.Where(null); Assert.Fail(); }
			catch (ArgumentNullException) { }

			try { cmd.Where(x => null); Assert.Fail(); }
			catch (ArgumentException) { }

			try { cmd.Where(x => "   "); Assert.Fail(); }
			catch (EmptyException) { }

			cmd = link
				.Update(x => x.Emp)
				.Where(x => x.Name > 7)
				.Columns(x => x.Id = "007");

			Assert.AreEqual("UPDATE Emp SET Id = #1 WHERE (Name > #0)", cmd.GetCommandText(false));
		}
	}
}
