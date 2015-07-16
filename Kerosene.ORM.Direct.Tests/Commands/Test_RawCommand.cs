using Kerosene.ORM.DataDB;
using Kerosene.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Transactions;

namespace Kerosene.ORM.Direct.Tests
{
	// ====================================================
	[TestClass]
	public class Test_RawCommand
	{
		//[OnlyThisTest]
		[TestMethod]
		public void Raw_Basic_Query()
		{
			DB.Prepare();

			var cmd = DB.Link.Raw(
				"SELECT * FROM Employees WHERE BirthDate >= {0}",
				new CalendarDate(1969, 1, 1));

			ConsoleEx.WriteLine("\n> Command: {0}", cmd);

			var list = cmd.ToList();
			foreach (var obj in list) ConsoleEx.WriteLine("\n- {0}", obj);

			var target = DB.Employees.Where(x => x.BirthDate >= new CalendarDate(1969, 1, 1)).ToList();
			Assert.AreEqual(target.Count, list.Count);
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Query_MultipleTables()
		{
			DB.Prepare();

			var cmd = DB.Link.Raw(
				"SELECT * FROM Employees AS Emp, Countries AS Ctry " +
				"WHERE Emp.BirthDate >= {0} AND Ctry.Id = Emp.CountryId",
				new CalendarDate(1969, 1, 1));

			ConsoleEx.WriteLine("\n> Command: {0}", cmd);

			var list = cmd.ToList();
			foreach (var obj in list) ConsoleEx.WriteLine("\n- {0}", obj);

			var target = DB.Employees.Where(x => x.BirthDate >= new CalendarDate(1969, 1, 1)).ToList();
			Assert.AreEqual(target.Count, list.Count);
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Insert_JamesBond()
		{
			DB.Prepare();

			var cmd = DB.Link.Raw(
				"INSERT INTO Employees (Id, FirstName, LastName, CountryId, BirthDate, JoinDate, Photo)"
				+ " OUTPUT INSERTED.*"
				+ " VALUES ( {0}, {1}, {2}, {3}, {4}, {5}, {6} )",
				"007", "James", "Bond", "uk", new CalendarDate(1969, 1, 1), null, new byte[] { 0, 0, 7 });

			ConsoleEx.WriteLine("\n> Command: {0}", cmd);

			var list = cmd.ToList();
			foreach (var obj in list) ConsoleEx.WriteLine("\n- {0}", obj);
			Assert.AreEqual(1, list.Count);

			DB.IsPrepared = false;
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Delete_JamesBond()
		{
			DB.Prepare();

			var cmd = DB.Link.Raw(
				"DELETE FROM Employees OUTPUT DELETED.* WHERE Id = {0}",
				"007");

			ConsoleEx.WriteLine("\n> Command: {0}", cmd);
			var num = cmd.Execute();
			Assert.AreEqual(0, num);

			DB.IsPrepared = false;
			Insert_JamesBond();
			num = cmd.Execute();
			Assert.AreEqual(1, num);

			DB.IsPrepared = false;
		}

		//[OnlyThisTest]
		[TestMethod]
		public void StoredProcedure()
		{
			DB.Prepare();

			var cmd = DB.Link.Raw(
				"EXEC employee_insert @FirstName = {0}, @LastName = {1}",
				"James",
				"Bond");

			ConsoleEx.WriteLine("\n> Command: {0}", cmd);

			var list = cmd.ToList();
			foreach (var obj in list) ConsoleEx.WriteLine("\n- {0}", obj);
			Assert.AreEqual(1, list.Count);

			DB.IsPrepared = false;
		}

		//[OnlyThisTest]
		[TestMethod]
		public void With_TransactionScope()
		{
			DB.Prepare();

			using (TransactionScope scope = new TransactionScope())
			{
				var cmd = DB.Link.Raw(
					"INSERT INTO Employees (Id, FirstName, LastName, CountryId, BirthDate, JoinDate, Photo)"
					+ " OUTPUT INSERTED.*"
					+ " VALUES ( {0}, {1}, {2}, {3}, {4}, {5}, {6} )",
					"007", "James", "Bond", "uk", new CalendarDate(1969, 1, 1), null, new byte[] { 0, 0, 7 });

				ConsoleEx.WriteLine("\n> Command: {0}", cmd);

				var list = cmd.ToList();
				foreach (var obj in list) ConsoleEx.WriteLine("\n> Record: {0}", obj);
				Assert.AreEqual(1, list.Count);

				cmd.Set("DELETE FROM Employees WHERE Id = {0}", "007");
				var num = cmd.Execute();
				Assert.AreEqual(1, num);

				scope.Complete();
			}

			DB.IsPrepared = false;
		}

		//[OnlyThisTest]
		[TestMethod]
		public void With_TransactionScope_Failed()
		{
			DB.Prepare();

			using (TransactionScope scope = new TransactionScope())
			{
				var cmd = DB.Link.Raw(
					"INSERT INTO Employees (Id, FirstName, LastName, CountryId, BirthDate, JoinDate, Photo)"
					+ " OUTPUT INSERTED.*"
					+ " VALUES ( {0}, {1}, {2}, {3}, {4}, {5}, {6} )",
					"007", "James", "Bond", "uk", new CalendarDate(1969, 1, 1), null, new byte[] { 0, 0, 7 });

				ConsoleEx.WriteLine("\n> Command: {0}", cmd);

				var list = cmd.ToList();
				foreach (var obj in list) ConsoleEx.WriteLine("\n> Record: {0}", obj);
				Assert.AreEqual(1, list.Count);
			}

			var other = DB.Link.From(x => x.Employees).Where(x => x.Id == "007");
			var item = other.First();
			Assert.IsNull(item);

			DB.IsPrepared = false;
		}

		//[OnlyThisTest]
		[TestMethod]
		public void With_StandardDb_Transaction()
		{
			DB.Prepare();

			IDbTransaction tran = null;
			try
			{
				DB.Link.Open();
				tran = DB.Link.DbConnection.BeginTransaction();

				var cmd = DB.Link.Raw(
					"INSERT INTO Employees (Id, FirstName, LastName, CountryId, BirthDate, JoinDate, Photo)"
					+ " OUTPUT INSERTED.*"
					+ " VALUES ( {0}, {1}, {2}, {3}, {4}, {5}, {6} )",
					"007", "James", "Bond", "uk", new CalendarDate(1969, 1, 1), null, new byte[] { 0, 0, 7 });

				ConsoleEx.WriteLine("\n> Command: {0}", cmd);

				var list = cmd.ToList();
				foreach (var obj in list) ConsoleEx.WriteLine("\n> Record: {0}", obj);
				Assert.AreEqual(1, list.Count);

				cmd.Set("DELETE FROM Employees WHERE Id = {0}", "007");
				var num = cmd.Execute();
				Assert.AreEqual(1, num);

				tran.Commit();
			}
			catch (Exception e)
			{
				tran.Rollback();
				Assert.Fail(e.ToDisplayString());
			}
			finally
			{
				tran.Dispose();
				DB.Link.Close();
			}

			DB.IsPrepared = false;
		}

		//[OnlyThisTest]
		[TestMethod]
		public void With_StandardDb_Transaction_Failed()
		{
			DB.Prepare();

			IDbTransaction tran = null;
			try
			{
				DB.Link.Open();
				tran = DB.Link.DbConnection.BeginTransaction();

				var cmd = DB.Link.Raw(
					"INSERT INTO Employees (Id, FirstName, LastName, CountryId, BirthDate, JoinDate, Photo)"
					+ " OUTPUT INSERTED.*"
					+ " VALUES ( {0}, {1}, {2}, {3}, {4}, {5}, {6} )",
					"007", "James", "Bond", "uk", new CalendarDate(1969, 1, 1), null, new byte[] { 0, 0, 7 });

				ConsoleEx.WriteLine("\n> Command: {0}", cmd);

				var list = cmd.ToList();
				foreach (var obj in list) ConsoleEx.WriteLine("\n> Record: {0}", obj);
				Assert.AreEqual(1, list.Count);

				tran.Rollback();
			}
			catch (Exception e)
			{
				tran.Rollback();
				Assert.Fail(e.ToDisplayString());
			}
			finally
			{
				tran.Dispose();
				DB.Link.Close();
			}

			var other = DB.Link.From(x => x.Employees).Where(x => x.Id == "007");
			var item = other.First();
			Assert.IsNull(item);

			DB.IsPrepared = false;
		}

		//[OnlyThisTest]
		[TestMethod]
		public void With_DataLink_Transaction()
		{
			DB.Prepare();

			try
			{
				DB.Link.Transaction.Start();

				var cmd = DB.Link.Raw(
					"INSERT INTO Employees (Id, FirstName, LastName, CountryId, BirthDate, JoinDate, Photo)"
					+ " OUTPUT INSERTED.*"
					+ " VALUES ( {0}, {1}, {2}, {3}, {4}, {5}, {6} )",
					"007", "James", "Bond", "uk", new CalendarDate(1969, 1, 1), null, new byte[] { 0, 0, 7 });

				ConsoleEx.WriteLine("\n> Command: {0}", cmd);

				var list = cmd.ToList();
				foreach (var obj in list) ConsoleEx.WriteLine("\n> Record: {0}", obj);
				Assert.AreEqual(1, list.Count);

				cmd.Set("DELETE FROM Employees WHERE Id = {0}", "007");
				var num = cmd.Execute();
				Assert.AreEqual(1, num);

				DB.Link.Transaction.Commit();
			}
			catch (Exception e)
			{
				DB.Link.Transaction.Abort();
				Assert.Fail(e.ToDisplayString());
			}

			Assert.IsFalse(DB.Link.IsOpen);

			DB.IsPrepared = false;
		}

		//[OnlyThisTest]
		[TestMethod]
		public void With_DataLink_Transaction_Failed()
		{
			DB.Prepare();

			try
			{
				DB.Link.Transaction.Start();

				var cmd = DB.Link.Raw(
					"INSERT INTO Employees (Id, FirstName, LastName, CountryId, BirthDate, JoinDate, Photo)"
					+ " OUTPUT INSERTED.*"
					+ " VALUES ( {0}, {1}, {2}, {3}, {4}, {5}, {6} )",
					"007", "James", "Bond", "uk", new CalendarDate(1969, 1, 1), null, new byte[] { 0, 0, 7 });

				ConsoleEx.WriteLine("\n> Command: {0}", cmd);

				var list = cmd.ToList();
				foreach (var obj in list) ConsoleEx.WriteLine("\n> Record: {0}", obj);
				Assert.AreEqual(1, list.Count);

				DB.Link.Transaction.Abort();
			}
			catch (Exception e)
			{
				DB.Link.Transaction.Abort();
				Assert.Fail(e.ToDisplayString());
			}

			Assert.IsFalse(DB.Link.IsOpen);

			var other = DB.Link.From(x => x.Employees).Where(x => x.Id == "007");
			var item = other.First();
			Assert.IsNull(item);

			DB.IsPrepared = false;
		}
	}
}
