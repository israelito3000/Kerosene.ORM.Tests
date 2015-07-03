using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kerosene.Tools;
using System;
using System.Linq;

namespace Kerosene.ORM.Core.Tests
{
	// ==================================================== 
	[TestClass]
	public class Test_QueryCommand
	{
		//[OnlyThisTest]
		[TestMethod]
		public void EmptyCommand()
		{
			var link = new FakeLink(new Concrete.DataEngine(), NestableTransactionMode.Database);
			var cmd = link.Query();
			var str = cmd.TraceString().NullIfTrimmedIsEmpty() ?? "<empty>";

			ConsoleEx.WriteLine("\n> {0}", str);
			Assert.IsFalse(cmd.CanBeExecuted);
		}

		//[OnlyThisTest]
		[TestMethod]
		public void From_Failures()
		{
			var link = new FakeLink(new Concrete.DataEngine(), NestableTransactionMode.Database);
			var cmd = link.Query();

			try { cmd = link.From(null); Assert.Fail(); }
			catch (ArgumentNullException) { }

			try { cmd = link.From(x => null); Assert.Fail(); }
			catch (ArgumentException) { }

			try { cmd = link.From(x => "   "); Assert.Fail(); }
			catch (EmptyException) { }

			try { cmd = link.From(x => x.Employees.As()); Assert.Fail(); }
			catch (ArgumentException) { }

			try { cmd = link.From(x => x.Employees.As(null)); Assert.Fail(); }
			catch (ArgumentException) { }

			try { cmd = link.From(x => x.Employees.As("   ")); Assert.Fail(); }
			catch (EmptyException) { }
		}

		//[OnlyThisTest]
		[TestMethod]
		public void From_Valids()
		{
			var link = new FakeLink(new Concrete.DataEngine(), NestableTransactionMode.Database);
			var cmd = link.Query();

			cmd = link.From(x => "Employees");
			Assert.AreEqual("SELECT * FROM Employees", cmd.GetCommandText(false));

			cmd = link.From(x => "Employees AS Emp");
			Assert.AreEqual("SELECT * FROM Employees AS Emp", cmd.GetCommandText(false));

			cmd = link.From(x => x.Employees);
			Assert.AreEqual("SELECT * FROM Employees", cmd.GetCommandText(false));

			cmd = link.From(x => x.Employees.As(x.Emp));
			Assert.AreEqual("SELECT * FROM Employees AS Emp", cmd.GetCommandText(false));

			cmd = link.From(x => x.Employees.As("Emp"));
			Assert.AreEqual("SELECT * FROM Employees AS Emp", cmd.GetCommandText(false));

			cmd = link.From(
				x => x.Employees.As(x.Emp),
				x => x.Countries.As(x.Ctry))
				.Where(x => x.Emp.CountryId == x.Ctry.Id);
			Assert.AreEqual(
				"SELECT * FROM Employees AS Emp, Countries AS Ctry WHERE (Emp.CountryId = Ctry.Id)"
				, cmd.GetCommandText(false));
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Select_Failures()
		{
			var link = new FakeLink(new Concrete.DataEngine(), NestableTransactionMode.Database);
			var cmd = link.Query();

			try { cmd = link.From(x => x.Emp).Select(null); Assert.Fail(); }
			catch (ArgumentNullException) { }

			try { cmd = link.From(x => x.Emp).Select(x => null); Assert.Fail(); }
			catch (ArgumentException) { }

			try { cmd = link.From(x => x.Emp).Select(x => "   "); Assert.Fail(); }
			catch (EmptyException) { }

			try { cmd = link.From(x => x.Emp).Select(x => x.Id.As()); Assert.Fail(); }
			catch (ArgumentException) { }

			try { cmd = link.From(x => x.Emp).Select(x => x.Id.As(null)); Assert.Fail(); }
			catch (ArgumentException) { }

			try { cmd = link.From(x => x.Emp).Select(x => x.Id.As("   ")); Assert.Fail(); }
			catch (EmptyException) { }
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Select_Valids()
		{
			var link = new FakeLink(new Concrete.DataEngine(), NestableTransactionMode.Database);
			var cmd = link.Query();

			cmd = link.From(x => x.Emp).Select(x => x.Id);
			Assert.AreEqual("SELECT Id FROM Emp", cmd.GetCommandText(false));

			cmd = link.From(x => x.Emp).Select(x => "Id");
			Assert.AreEqual("SELECT Id FROM Emp", cmd.GetCommandText(false));

			cmd = link.From(x => x.Emp).Select(x => "Id AS Other");
			Assert.AreEqual("SELECT Id AS Other FROM Emp", cmd.GetCommandText(false));
			Assert.AreEqual(1, cmd.Aliases.Count);
			Assert.AreEqual("Other", cmd.Aliases[0].Alias);

			cmd = link.From(x => x.Emp).Select(x => x.Id.As(x.Other));
			Assert.AreEqual("SELECT Id AS Other FROM Emp", cmd.GetCommandText(false));

			cmd = link.From(x => x.Emp).Select(x => x.Id.As("Other"));
			Assert.AreEqual("SELECT Id AS Other FROM Emp", cmd.GetCommandText(false));

			cmd = link.From(x => x.Emp).Select(x => x.Emp.All());
			Assert.AreEqual("SELECT Emp.* FROM Emp", cmd.GetCommandText(false));

			var other = link.From(x => x.Countries.As(x.Ctry)).Select(x => x.Name);
			cmd = link.From(x => x.Emp).Select(x => x(other).As(x.Temp));
			Assert.AreEqual(
				"SELECT (SELECT Name FROM Countries AS Ctry) AS Temp FROM Emp",
				cmd.GetCommandText(false));
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Where_Failures()
		{
			var link = new FakeLink(new Concrete.DataEngine(), NestableTransactionMode.Database);
			var cmd = link.Query();

			try { cmd = link.From(x => x.Emp).Where(null); Assert.Fail(); }
			catch (ArgumentNullException) { }

			try { cmd = link.From(x => x.Emp).Where(x => null); Assert.Fail(); }
			catch (ArgumentException) { }

			try { cmd = link.From(x => x.Emp).Where(x => "   "); Assert.Fail(); }
			catch (EmptyException) { }
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Where_Valids()
		{
			var link = new FakeLink(new Concrete.DataEngine(), NestableTransactionMode.Database);
			var cmd = link.Query();

			cmd = link.From(x => x.Emp).Where(x => x.Name > 7);
			Assert.AreEqual("SELECT * FROM Emp WHERE (Name > #0)", cmd.GetCommandText(false));
			Assert.AreEqual(1, cmd.Parameters.Count);
			Assert.AreEqual(7, cmd.Parameters[0].Value);

			cmd.Where(x => x.Or(x.Id == "007"));
			Assert.AreEqual("SELECT * FROM Emp WHERE (Name > #0) OR (Id = #1)", cmd.GetCommandText(false));
			Assert.AreEqual(2, cmd.Parameters.Count);
			Assert.AreEqual("007", cmd.Parameters[1].Value);
		}

		//[OnlyThisTest]
		[TestMethod]
		public void OrderBy_Failures()
		{
			var link = new FakeLink(new Concrete.DataEngine(), NestableTransactionMode.Database);
			var cmd = link.Query();

			try { cmd = link.From(x => x.Emp).OrderBy(null); Assert.Fail(); }
			catch (ArgumentNullException) { }

			try { cmd = link.From(x => x.Emp).OrderBy(x => null); Assert.Fail(); }
			catch (ArgumentException) { }

			try { cmd = link.From(x => x.Emp).OrderBy(x => "   "); Assert.Fail(); }
			catch (EmptyException) { }

			try { cmd = link.From(x => x.Emp).OrderBy(x => x.Id.Asc(null)); Assert.Fail(); }
			catch (ArgumentException) { }

			try { cmd = link.From(x => x.Emp).OrderBy(x => x.Id.Asc(1)); Assert.Fail(); }
			catch (ArgumentException) { }
		}

		//[OnlyThisTest]
		[TestMethod]
		public void OrderBy_Valids()
		{
			var link = new FakeLink(new Concrete.DataEngine(), NestableTransactionMode.Database);
			var cmd = link.Query();

			cmd = link.From(x => x.Emp).OrderBy(x => x.Id);
			Assert.AreEqual("SELECT * FROM Emp ORDER BY Id", cmd.GetCommandText(false));

			cmd = link.From(x => x.Emp).OrderBy(x => x.Id.Asc());
			Assert.AreEqual("SELECT * FROM Emp ORDER BY Id ASC", cmd.GetCommandText(false));
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Join_Failures()
		{
			var link = new FakeLink(new Concrete.DataEngine(), NestableTransactionMode.Database);
			var cmd = link.Query();

			try { cmd = link.From(x => x.Emp).Join(null); Assert.Fail(); }
			catch (ArgumentNullException) { }

			try { cmd = link.From(x => x.Emp).Join(x => null); Assert.Fail(); }
			catch (ArgumentException) { }

			try { cmd = link.From(x => x.Emp).Join(x => "   "); Assert.Fail(); }
			catch (EmptyException) { }
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Join_Valids()
		{
			var link = new FakeLink(new Concrete.DataEngine(), NestableTransactionMode.Database);
			var cmd = link.Query();

			cmd = link
				.From(x => x.Emp)
				.Join(x => x("LEFT JOIN").Countries.As("Ctry").On(x.Ctry.Id == x.Emp.Id));
			Assert.AreEqual(
				"SELECT * FROM Emp LEFT JOIN Countries AS Ctry ON (Ctry.Id = Emp.Id)",
				cmd.GetCommandText(false));
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Join_With_Embedded_Scape_Syntax()
		{
			var link = new FakeLink(new Concrete.DataEngine(), NestableTransactionMode.Database);
			var cmd = link.Query();

			cmd = link
				.From(x => x.Emp)
				.Join(x => x("LEFT JOIN").x("Countries").As(x.Ctry).On(x.Ctry.Id == x.Emp.Id));
			Assert.AreEqual(
				"SELECT * FROM Emp LEFT JOIN Countries AS Ctry ON (Ctry.Id = Emp.Id)",
				cmd.GetCommandText(false));
		}
	}
}
