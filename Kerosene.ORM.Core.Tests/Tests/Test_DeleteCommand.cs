using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kerosene.Tools;
using System;
using System.Linq;

namespace Kerosene.ORM.Core.Tests
{
	// ==================================================== 
	[TestClass]
	public class Test_DeleteCommand
	{
		//[OnlyThisTest]
		[TestMethod]
		public void Empty_DeleteCommand_IsValid()
		{
			var link = new FakeLink(new Concrete.DataEngine(), NestableTransactionMode.Database);
			var cmd = link.Delete(x => x.Employees);
			var str = cmd.TraceString().NullIfTrimmedIsEmpty() ?? "<empty>";

			ConsoleEx.WriteLine("\n> {0}", str);
			Assert.IsTrue(cmd.CanBeExecuted); // In this case is valid
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Examples()
		{
			var link = new FakeLink(new Concrete.DataEngine(), NestableTransactionMode.Database);
			var cmd = link.Delete(x => x.Emp);
			Assert.AreEqual("DELETE FROM Emp", cmd.GetCommandText(false));

			try { cmd.Where(null); Assert.Fail(); }
			catch (ArgumentNullException) { }

			try { cmd.Where(x => null); Assert.Fail(); }
			catch (ArgumentException) { }

			try { cmd.Where(x => "   "); Assert.Fail(); }
			catch (EmptyException) { }

			cmd = link
				.Delete(x => x.Emp)
				.Where(x => x.Name > 7);
			Assert.AreEqual("DELETE FROM Emp WHERE (Name > #0)", cmd.GetCommandText(false));
		}
	}
}
