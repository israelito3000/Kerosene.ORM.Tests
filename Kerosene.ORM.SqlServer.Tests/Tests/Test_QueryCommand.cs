using Kerosene.ORM.ExampleDB;
using Kerosene.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Transactions;

namespace Kerosene.ORM.SqlServer.Tests
{
	// ====================================================
	[TestClass]
	public class Test_QueryCommand
	{
		//[OnlyThisTest]
		[TestMethod]
		public void Query_WithSimulatedSkipTake()
		{
			DB.Prepare();
			Assert.AreEqual(true, DB.Link.Engine.SupportsNativeSkipTake);

			var cmd = DB.Link
				.From(x => x.Employees.As(x.Emp))
				.Skip(6)
				.Take(2);

			ConsoleEx.WriteLine("\n> Command: {0}", cmd);
			Assert.IsFalse(cmd.IsValidForNativeSkipTake());

			foreach (var obj in cmd) ConsoleEx.WriteLine("\n- {0}", obj);
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Query_WithValidSkipTake()
		{
			DB.Prepare();
			Assert.AreEqual(true, DB.Link.Engine.SupportsNativeSkipTake);

			var cmd = DB.Link
				.From(x => x.Employees.As(x.Emp))
				.Skip(6)
				.Take(2)
				.OrderBy(x => x.Id);

			ConsoleEx.WriteLine("\n> Command: {0}", cmd);
			Assert.AreEqual(DB.Link.Engine.SupportsNativeSkipTake, cmd.IsValidForNativeSkipTake());
			Assert.AreEqual(
				"SELECT * FROM Employees AS Emp ORDER BY Id OFFSET 6 ROWS FETCH NEXT 2 ROWS ONLY",
				cmd.GetCommandText(iterable: false));

			foreach (var obj in cmd) ConsoleEx.WriteLine("\n- {0}", obj);
		}
	}
}