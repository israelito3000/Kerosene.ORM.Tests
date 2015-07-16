using Kerosene.ORM.DataDB;
using Kerosene.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Kerosene.ORM.Direct.Tests
{
	// ====================================================
	[TestClass]
	public class Test_ChangeCommands
	{
		//[OnlyThisTest]
		[TestMethod]
		public void Insert_JamesBond()
		{
			DB.Prepare();

			var cmd = DB.Link
				.Insert(x => x.Employees)
				.Columns(
					x => x.Id = "007",
					x => x.FirstName = "James",
					x => x.LastName = "Bond",
					x => x.CountryId = "uk",
					x => x.JoinDate = null,
					x => x.Photo = new byte[] { 0, 0, 7 });

			ConsoleEx.WriteLine("\n> Command: {0}", cmd);

			var list = cmd.ToList();
			Assert.AreEqual(1, list.Count);
			foreach (var obj in list) ConsoleEx.WriteLine("\n- {0}", obj);

			DB.IsPrepared = false;
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Delete_JamesBond()
		{
			DB.Prepare();

			var cmd = DB.Link
				.Delete(x => x.Employees)
				.Where(x => x.Id == "007");

			ConsoleEx.WriteLine("\n> Command: {0}", cmd);

			var num = cmd.Execute();
			Assert.AreEqual(0, num);

			Insert_JamesBond();

			ConsoleEx.WriteLine("\n> Command: {0}", cmd);
			var list = cmd.ToList();
			Assert.AreEqual(1, list.Count);
			foreach (var obj in list) ConsoleEx.WriteLine("\n- {0}", obj);

			DB.IsPrepared = false;
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Update_Several_Rows_At_Once()
		{
			DB.Prepare();

			var cmd = DB.Link
				.Update(x => x.Employees)
				.Where(x => x.FirstName >= "E")
				.Columns(
					x => x.ManagerId = null,
					x => x.LastName = x.LastName + "_1", // An example of using the previous contents of the column :-)
					x => x.Photo = new byte[] { 99, 98, 97, 96 });

			ConsoleEx.WriteLine("\n> Command: {0}", cmd);

			var list = cmd.ToList();
			foreach (var obj in list) ConsoleEx.WriteLine("\n- {0}", obj);

			var target = DB.Employees.Where(x => string.Compare(x.FirstName, "E") >= 0).ToList();
			Assert.AreEqual(target.Count, list.Count);

			DB.IsPrepared = false;
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Delete_Several_Rows_Enumerate()
		{
			DB.Prepare();
			DB.Link.Raw("DELETE FROM EmployeeTalents").Execute();
			DB.Link.Raw("DELETE FROM Talents").Execute();

			var cmd = DB.Link
				.Delete(x => x.Employees)
				.Where(x => x.ManagerId != null);

			ConsoleEx.WriteLine("\n> Command: {0}", cmd);

			var list = cmd.ToList();
			foreach (var obj in list) ConsoleEx.WriteLine("\n- {0}", obj);

			var target = DB.Employees.Where(x => x.ManagerId != null).ToList();
			Assert.AreEqual(target.Count, list.Count);

			DB.IsPrepared = false;
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Delete_Several_Rows_Scalar()
		{
			DB.Prepare();
			DB.Link.Raw("DELETE FROM EmployeeTalents").Execute();
			DB.Link.Raw("DELETE FROM Talents").Execute();

			var cmd = DB.Link
				.Delete(x => x.Employees)
				.Where(x => x.ManagerId != null);

			ConsoleEx.WriteLine("\n> Command: {0}", cmd);

			var num = cmd.Execute();

			var target = DB.Employees.Where(x => x.ManagerId != null).ToList();
			Assert.AreEqual(target.Count, num);

			DB.IsPrepared = false;
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Nested_Updates_And_Suspending_Constrains()
		{
			DB.Prepare();

			DB.Link.Raw("ALTER TABLE Countries NOCHECK CONSTRAINT ALL").Execute();
			DB.Link.Raw("ALTER TABLE Employees NOCHECK CONSTRAINT ALL").Execute();

			var cmdCtry = DB.Link
				.Update(x => x.Countries)
				.Where(x => x.Id == "es")
				.Columns(
					x => x.Id = "es#");

			ConsoleEx.WriteLine("\n> Command: {0}", cmdCtry);

			foreach (DataDB.Country ctry in cmdCtry.ConvertBy(recCtry =>
			{
				ConsoleEx.WriteLine("\n\t- Country: {0}", recCtry);

				dynamic dinCtry = recCtry;
				DataDB.Country objCtry = new DataDB.Country();
				objCtry.Id = dinCtry.Id;
				objCtry.Name = dinCtry.Name;
				objCtry.RegionId = dinCtry.RegionId;

				var cmdEmp = DB.Link
					.Update(x => x.Employees)
					.Where(x => x.CountryId == "es")
					.Columns(
						x => x.CountryId = "es#");

				ConsoleEx.WriteLine("\n\t> Command: {0}", cmdEmp);

				foreach (DataDB.Employee emp in cmdEmp.ConvertBy(recEmp =>
				{
					ConsoleEx.WriteLine("\n\t\t- Employee: {0}", recEmp);

					dynamic dinEmp = recEmp;
					DataDB.Employee objEmp = new DataDB.Employee();
					objEmp.Id = dinEmp.Id; objEmp.BirthDate = dinEmp.BirthDate;
					objEmp.FirstName = dinEmp.FirstName; objEmp.LastName = dinEmp.LastName;
					objEmp.ManagerId = dinEmp.ManagerId; objEmp.CountryId = dinEmp.CountryId;

					return objEmp;
				})) ;

				return objCtry;
			})) ;

			DB.Link.Raw("ALTER TABLE Countries CHECK CONSTRAINT ALL").Execute();
			DB.Link.Raw("ALTER TABLE Employees CHECK CONSTRAINT ALL").Execute();

			DB.IsPrepared = false;
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Update_With_Row_Version_Check()
		{
			DB.Prepare();

			DB.Link
				.Insert(x => x.Regions)
				.Columns(
					x => x.Id = "ZZZ").Execute();

			var reg = DB.Link
				.From(x => x.Regions)
				.Where(x => x.Id == "ZZZ").First();

			ConsoleEx.WriteLine("\n> Region = {0}".FormatWith(reg));

			var rv = ((Core.IRecord)reg)["RowVersion"]; // Capturing before updating
			var cmd = DB.Link
				.Update(x => x.Regions)
				.Where(x => x.Id == "ZZZ")
				.Columns(x => x.Name = x.Name + "_2");

			ConsoleEx.WriteLine("\n> Command: {0}", cmd);
			cmd.Execute();

			var other = DB.Link
				.From(x => x.Regions)
				.Where(x => x.RowVersion == rv); // Using the former one

			ConsoleEx.WriteLine("\n> Command: {0}", other);
			reg = other.First();

			Assert.IsNull(reg);

			DB.IsPrepared = false;
		}
	}
}
