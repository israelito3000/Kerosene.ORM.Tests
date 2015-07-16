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
	public class Test_QueryCommand
	{
		//[OnlyThisTest]
		[TestMethod]
		public void Query_Basic()
		{
			DB.Prepare();

			var cmd = DB.Link
				.From(x => x.Employees.As(x.Emp))
				.Where(x => x.Emp.LastName >= "P");

			var str = cmd.ToString();
			ConsoleEx.WriteLine("\n> Command: {0}", cmd);
			Assert.AreEqual(
				"SELECT * FROM Employees AS Emp WHERE (Emp.LastName >= @0) -- [@0 = 'P']",
				str);

			var list = cmd.ToList();
			foreach (var obj in list) ConsoleEx.WriteLine("\n- {0}", obj);

			var target = DB.Employees.Where(x => string.Compare(x.LastName, "P") >= 0).ToList();
			Assert.AreEqual(target.Count, list.Count);
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Query_With_Dynamic_And_Indexed_Getters()
		{
			DB.Prepare();

			var cmd = DB.Link
				.From(x => x.Employees.As(x.Emp))
				.Where(x => x.Emp.LastName >= "C");

			ConsoleEx.WriteLine("\n> Command: {0}", cmd);

			var list = cmd.ToList();
			var target = DB.Employees.Where(x => string.Compare(x.LastName, "C") >= 0).ToList();
			Assert.AreEqual(target.Count, list.Count);

			foreach (Core.IRecord obj in list)
			{
				dynamic rec = obj;
				string str = string.Format("\n> Result = Id:{0}, First:{1}, Last:{2}, Date:{3}, Time:{4}, Country:{5}",
					rec.Id,
					rec.Emp.FirstName,
					rec.Employees.LastName,
					rec["BirthDate"],
					obj["Emp", "StartTime"],
					obj["Employees", "CountryId"]
					);

				ConsoleEx.WriteLine(str);
			}
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Query_Using_Complex_Tags()
		{
			DB.Prepare();

			bool keep = Core.Parser.ComplexTags = true;

			var cmd = DB.Link
				.From(x => x.Employees.As(x.Emp))
				.Where(x => x.Emp.LastName >= "P");

			Core.Parser.ComplexTags = keep;

			ConsoleEx.WriteLine("\n> Command: {0}", cmd);
			Assert.AreEqual(
				"SELECT * FROM Employees AS Emp WHERE (Emp.LastName >= @0) -- [@0 = 'P']",
				cmd.ToString());

			foreach (var obj in cmd) ConsoleEx.WriteLine("\n- {0}", obj);
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Query_From_Multiples_Tables()
		{
			DB.Prepare();

			var cmd = DB.Link
				.From(x => x.Employees.As(x.Emp)).Where(Emp => Emp.JoinDate >= new CalendarDate(2000, 1, 1))
				.From(x => x.Countries.As(x.Ctry)).Where(x => x.Ctry.Id == x.Emp.CountryId)
				.Select(x => x.Ctry.All())
				.Select(x => x.Emp.Id, x => x.Emp.BirthDate, x => x.Emp.LastName);

			ConsoleEx.WriteLine("\n> Command: {0}", cmd);
			Assert.AreEqual(
				"SELECT Ctry.*, Emp.Id, Emp.BirthDate, Emp.LastName " +
				"FROM Employees AS Emp, Countries AS Ctry " +
				"WHERE (Emp.JoinDate >= @0) AND (Ctry.Id = Emp.CountryId) -- [@0 = '2000-1-1']",
				cmd.ToString());

			foreach (var obj in cmd) ConsoleEx.WriteLine("\n- {0}", obj);
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Query_Using_In_Operator()
		{
			DB.Prepare();

			var cmd = DB.Link
				.From(x => x.Employees)
				.Where(x => x.Id.In("2001", "2002", "2003", "This does not exist"));

			ConsoleEx.WriteLine("\n> Command: {0}", cmd);
			Assert.AreEqual(
				"SELECT * FROM Employees WHERE Id IN (@0, @1, @2, @3) " +
				"-- [@0 = '2001', @1 = '2002', @2 = '2003', @3 = 'This does not exist']",
				cmd.ToString());

			foreach (var obj in cmd) ConsoleEx.WriteLine("\n- {0}", obj);
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Query_Using_NotIn_Operator()
		{
			DB.Prepare();

			var cmd = DB.Link
				.From(x => x.Employees)
				.Where(x => x.Id.NotIn("2001", "2002", "2003", "This does not exist"));

			ConsoleEx.WriteLine("\n> Command: {0}", cmd);
			Assert.AreEqual(
				"SELECT * FROM Employees WHERE NOT Id IN (@0, @1, @2, @3) " +
				"-- [@0 = '2001', @1 = '2002', @2 = '2003', @3 = 'This does not exist']",
				cmd.ToString());

			foreach (var obj in cmd) ConsoleEx.WriteLine("\n- {0}", obj);
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Query_Using_InList_Operator()
		{
			DB.Prepare();

			var list = new string[] { "2001", "2002", "2003" };
			var cmd = DB.Link
				.From(x => x.Employees)
				.Where(x => x.Id.InList(list));

			ConsoleEx.WriteLine("\n> Command: {0}", cmd);
			Assert.AreEqual(
				"SELECT * FROM Employees WHERE Id IN (@0, @1, @2) " +
				"-- [@0 = '2001', @1 = '2002', @2 = '2003']",
				cmd.ToString());

			foreach (var obj in cmd) ConsoleEx.WriteLine("\n- {0}", obj);
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Query_Using_NotInList_Operator()
		{
			DB.Prepare();

			var list = new List<string>() { "2001", "2002", "2003" };
			var cmd = DB.Link
				.From(x => x.Employees)
				.Where(x => x.Id.NotInList(list));

			ConsoleEx.WriteLine("\n> Command: {0}", cmd);
			Assert.AreEqual(
				"SELECT * FROM Employees WHERE NOT Id IN (@0, @1, @2) " +
				"-- [@0 = '2001', @1 = '2002', @2 = '2003']",
				cmd.ToString());

			foreach (var obj in cmd) ConsoleEx.WriteLine("\n- {0}", obj);
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Query_With_Chained_Froms()
		{
			DB.Prepare();

			var cmd = DB.Link
				.Select(x => x.Emp.All())
				.From(x => x.Employees.As(x.Emp))
				.From(x => x.Countries.As(x.Ctry)).Where(x => x.Ctry.RegionId == x.Reg.Id)
				.From(x => x.Regions.As(x.Reg)).Where(x => x.Reg.ParentId == x.Super.Id)
				.From(x => x.Regions.As(x.Super)).Where(x => x.Super.Name == "Americas");

			ConsoleEx.WriteLine("\n> Command: {0}", cmd);
			Assert.AreEqual(
				"SELECT Emp.* FROM Employees AS Emp, Countries AS Ctry, Regions AS Reg, Regions AS Super " +
				"WHERE (Ctry.RegionId = Reg.Id) AND (Reg.ParentId = Super.Id) AND (Super.Name = @0) " +
				"-- [@0 = 'Americas']",
				cmd.ToString());

			foreach (var obj in cmd) ConsoleEx.WriteLine("\n- {0}", obj);
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Query_With_Joins()
		{
			DB.Prepare();

			var cmd = DB.Link
				.From(x => x.Employees.As(x.Emp))
				.From(x => x.Countries.As(x.Ctry))
				.Join(x => x.Regions.As(x.Reg).On(x.Reg.Id == x.Ctry.RegionId))
				.Join(x => x("LEFT JOIN").Regions.As(x.Super).On(x.Super.Id == x.Reg.ParentId))
				.Where(x => x.Super.Name == "Americas")
				.Select(x => x.Emp.All());

			ConsoleEx.WriteLine("\n> Command: {0}", cmd);
			Assert.AreEqual(
				"SELECT Emp.* FROM Employees AS Emp, Countries AS Ctry " +
				"JOIN Regions AS Reg ON (Reg.Id = Ctry.RegionId) " +
				"LEFT JOIN Regions AS Super ON (Super.Id = Reg.ParentId) " +
				"WHERE (Super.Name = @0) -- [@0 = 'Americas']",
				cmd.ToString());

			foreach (var obj in cmd) ConsoleEx.WriteLine("\n- {0}", obj);
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Query_With_Distinct()
		{
			DB.Prepare();

			var cmd = DB.Link
				.From(x => x.Employees.As(x.Emp))
				.Join(x => x.Countries.As(x.Ctry).On(x.Ctry.Id == x.Emp.CountryId))
				.Where(x => x.Emp.LastName >= "K")
				.Distinct()
				.Select(x => x.Emp.All());

			ConsoleEx.WriteLine("\n> Command: {0}", cmd);
			Assert.AreEqual(
				"SELECT DISTINCT Emp.* " +
				"FROM Employees AS Emp " +
				"JOIN Countries AS Ctry ON (Ctry.Id = Emp.CountryId) " +
				"WHERE (Emp.LastName >= @0) -- [@0 = 'K']",
				cmd.ToString());

			foreach (var obj in cmd) ConsoleEx.WriteLine("\n- {0}", obj);
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Query_With_Command_As_Source()
		{
			DB.Prepare();

			var other = DB.Link.From(x => x.Countries.As(x.Ctry)).Where(x => x.Ctry.Id == "us");
			var cmd = DB.Link
				.From(x => x(other).As(x.Location))
				.From(x => x.Employees.As(x.Emp))
				.Where(x => x.Emp.CountryId == x.Location.Id)
				.Select(x => x.Emp.All());

			ConsoleEx.WriteLine("\n> Command: {0}", cmd);
			Assert.AreEqual(
				"SELECT Emp.* FROM " +
				"(SELECT * FROM Countries AS Ctry WHERE (Ctry.Id = @0)) AS Location, " +
				"Employees AS Emp " +
				"WHERE (Emp.CountryId = Location.Id) " +
				"-- [@0 = 'us']",
				cmd.ToString());

			foreach (var obj in cmd) ConsoleEx.WriteLine("\n- {0}", obj);
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Query_With_Simulated_SkipTake()
		{
			DB.Prepare();

			var cmd = DB.Link
				.From(x => x.Employees.As(x.Emp))
				.Skip(6)
				.Take(2);

			ConsoleEx.WriteLine("\n> Command: {0}", cmd);

			bool valid = cmd.IsValidForNativeSkipTake();
			Assert.IsFalse(valid);
			ConsoleEx.WriteLine("- Emulated skip/take: {0}", !valid);

			foreach (var obj in cmd) ConsoleEx.WriteLine("\n- {0}", obj);
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Query_With_Valid_SkipTake()
		{
			DB.Prepare();

			var cmd = DB.Link
				.From(x => x.Employees.As(x.Emp))
				.Skip(6)
				.Take(2)
				.OrderBy(x => x.Id);

			ConsoleEx.WriteLine("\n> Command: {0}", cmd);

			bool valid = cmd.IsValidForNativeSkipTake();
			bool native = DB.Link.Engine.SupportsNativeSkipTake;
			Assert.IsTrue(valid == native);
			ConsoleEx.WriteLine("- Emulated skip/take: {0}", !valid);

			foreach (var obj in cmd) ConsoleEx.WriteLine("\n- {0}", obj);
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Query_To_Scalar_Manual_Way()
		{
			DB.Prepare();

			var cmd = DB.Link
				.From(x => x.Employees)
				.Select(x => x.Count(x.Id).As(x.SumOfEmployees));

			ConsoleEx.WriteLine("\n> Command: {0}", cmd);
			Assert.AreEqual(
				"SELECT Count(Id) AS SumOfEmployees FROM Employees",
				cmd.ToString());

			dynamic result = cmd.First();
			string str = result.ToString(); ConsoleEx.WriteLine(str);
			str = result.SumOfEmployees.ToString(); ConsoleEx.WriteLine(str);

			Assert.AreEqual(DB.Employees.Count, result.SumOfEmployees);
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Query_To_Scalar_AutoConversion()
		{
			DB.Prepare();

			var cmd = DB.Link
				.From(x => x.Employees)
				.Select(x => x.Count(x.Id).As(x.SumOfEmployees));

			ConsoleEx.WriteLine("\n> Command: {0}", cmd);
			Assert.AreEqual(
				"SELECT Count(Id) AS SumOfEmployees FROM Employees",
				cmd.ToString());

			int n = (int)cmd.ToScalar();
			Assert.AreEqual(DB.Employees.Count, n);
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Query_With_Rounded_Scape_Syntax()
		{
			DB.Prepare();

			var cmd = DB.Link
				.From(x => x.Employees.As(x.Emp))
				.Select(x => x.Count(x("*")).As(x.Result));

			ConsoleEx.WriteLine("\n> Command: {0}", cmd);
			Assert.AreEqual(
				"SELECT Count(*) AS Result FROM Employees AS Emp",
				cmd.ToString());

			dynamic result = cmd.First();
			var str = result.ToString(); ConsoleEx.WriteLine(str);
			str = result.Result.ToString(); ConsoleEx.WriteLine(str);

			Assert.AreEqual(DB.Employees.Count, result.Result);
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Query_With_Squared_Scape_Syntax()
		{
			DB.Prepare();

			var cmd = DB.Link
				.From(x => x.Employees.As(x.Emp))
				.Select(x => x.Count(x["*"]).As(x.Result));

			ConsoleEx.WriteLine("\n> Command: {0}", cmd);
			Assert.AreEqual(
				"SELECT Count(*) AS Result FROM Employees AS Emp",
				cmd.ToString());

			dynamic result = cmd.First();
			var str = result.ToString(); ConsoleEx.WriteLine(str);
			str = result.Result.ToString(); ConsoleEx.WriteLine(str);

			Assert.AreEqual(DB.Employees.Count, result.Result);
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Query_With_Conversion_To_Anonymous()
		{
			DB.Prepare();

			var cmd = DB.Link
				.From(x => x.Employees.As(x.Emp))
				.Where(x => x.LastName >= "C");

			ConsoleEx.WriteLine("\n> Command: {0}", cmd);

			Func<Core.IRecord, object> converter = rec =>
			{
				dynamic obj = rec; return new
				{
					obj.Id,
					Name = string.Format("{0}, {1}", obj.LastName, obj["Emp", "FirstName"])
				};
			};

			foreach (var obj in cmd.ConvertBy(converter)) ConsoleEx.WriteLine("\n- {0}", obj);
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Query_With_ConvertBy()
		{
			DB.Prepare();

			var cmd = DB.Link.From(x => x.Regions.As(x.Reg));
			ConsoleEx.WriteLine("\n> Command: {0}", cmd);

			Func<Core.IRecord, object> converter = rec =>
			{
				dynamic row = rec;
				DataDB.Region reg = new DataDB.Region();

				reg.Id = row.Id;
				reg.Name = (string)rec["Name"];
				reg.ParentId = row.Reg.ParentId; // Using the alias
				return reg;
			};

			foreach (var obj in cmd.ConvertBy(converter)) ConsoleEx.WriteLine("\n- {0}", obj);
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Query_With_Complex_And_Nested_Conversion()
		{
			DB.Prepare();

			var cmdCtry = DB.Link.From(x => x.Countries.As(x.Ctry));
			ConsoleEx.WriteLine("\n> Command: {0}", cmdCtry);

			foreach (DataDB.Country ctry in cmdCtry.ConvertBy(recCtry =>
			{
				ConsoleEx.WriteLine("\n- Country: {0}", recCtry);

				dynamic dinCtry = recCtry;
				DataDB.Country objCtry = new DataDB.Country();
				objCtry.Id = dinCtry.Id;
				objCtry.Name = dinCtry.Name;
				objCtry.RegionId = dinCtry.RegionId;

				var cmdEmp = DB.Link.From(x => x.Employees).Where(x => x.CountryId == objCtry.Id);
				ConsoleEx.WriteLine("\n\t> SubCommand: {0}", cmdEmp);

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
		}
	}
}
