using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kerosene.ORM.DataDB;
using Kerosene.ORM.Maps;
using Kerosene.Tools;
using System;
using System.Linq;

namespace Kerosene.ORM.Maps.Eager.Tests
{
	// ==================================================== 
	[TestClass]
	public class Test_QueryCommand
	{
		//[OnlyThisTest]
		[TestMethod]
		public void Query_All_Employees()
		{
			DB.Prepare();

			using (var repo = Repo.Create())
			{
				var cmd = repo.Query<Employee>();
				var list = cmd.ToList();

				ConsoleEx.ReadLine("\n--- Press [Enter] to print results...");
				foreach (var obj in list) ConsoleEx.WriteLine("\n> {0}", obj);

				int count = DataDB.DB.Employees.Count;
				Assert.AreEqual(count, list.Count);
			}
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Find_From_Cache_Clean()
		{
			DB.Prepare();

			using (var repo = Repo.Create())
			{
				var id = DB.Employees[0].Id;
				var obj = repo.FindNow<Employee>(x => x.Id == id);

				ConsoleEx.WriteLine("\n> Source: {0}", obj);
				Assert.IsNotNull(obj);
				Assert.IsNotNull(MetaEntity.Locate(obj).Map);
			}
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Find_Employee_From_Cache_Populated()
		{
			DB.Prepare();

			using (var repo = Repo.Create())
			{
				var id = DB.Employees[0].Id;
				var obj = repo.Where<Employee>(x => x.Id == id).First();
				ConsoleEx.WriteLine("\n> Source: {0}", obj);
				Assert.IsNotNull(obj);

				var temp = repo.FindNow<Employee>(x => x.Id == id);
				ConsoleEx.WriteLine("\n> Refreshed: {0}", obj);
				Assert.IsNotNull(temp);

				bool r = object.ReferenceEquals(obj, temp);
				ConsoleEx.WriteLine("\n> Are the same reference: {0}", r);
				Assert.IsTrue(r);
			}
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Find_Employee_No_Id_Cache_Populated()
		{
			DB.Prepare();

			using (var repo = Repo.Create())
			{
				var id = DB.Employees[0].Id;
				var obj = repo.Where<Employee>(x => x.Id == id).First();
				ConsoleEx.WriteLine("\n> Source: {0}", obj);
				Assert.IsNotNull(obj);

				var temp = repo.FindNow<Employee>(x => x.FirstName == obj.Name.First); // Using 'FirstName' as it is the column name
				ConsoleEx.WriteLine("\n> Refreshed: {0}", obj);
				Assert.IsNotNull(temp);

				bool r = object.ReferenceEquals(obj, temp);
				ConsoleEx.WriteLine("\n> Are the same reference: {0}", r);
				Assert.IsTrue(r);
			}
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Refresh_Employee_From_Cache_Clean()
		{
			DB.Prepare();

			using (var repo = Repo.Create())
			{
				var id = DB.Employees[0].Id;
				var obj = new Employee() { Id = id };
				var temp = repo.RefreshNow(obj);

				ConsoleEx.WriteLine("\n> Source: {0}", obj);
				ConsoleEx.WriteLine("\n> Refreshed: {0}", temp);
				ConsoleEx.WriteLine("\n> Are the same reference: {0}", object.ReferenceEquals(obj, temp));

				Assert.IsNotNull(temp);
				Assert.IsNotNull(MetaEntity.Locate(temp).Map);
				Assert.IsNotNull(MetaEntity.Locate(obj).Map);
			}
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Refresh_Employee_From_Cache_Populated()
		{
			DB.Prepare();

			using (var repo = Repo.Create())
			{
				var id = DB.Employees[0].Id;
				var obj = repo.Where<Employee>(x => x.Id == id).First();
				ConsoleEx.WriteLine("\n> Source: {0}", obj);
				Assert.IsNotNull(obj);

				var temp = repo.RefreshNow(obj);
				ConsoleEx.WriteLine("\n> Refreshed: {0}", obj);
				Assert.IsNotNull(temp);

				bool r = object.ReferenceEquals(obj, temp);
				ConsoleEx.WriteLine("\n> Are the same reference: {0}", r);
				Assert.IsTrue(r);
			}
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Query_Extended_Several_Froms()
		{
			DB.Prepare();

			using (var repo = Repo.Create())
			{
				var id = DB.Talents[0].Id;
				var cmd = repo
					.Where<Employee>(x => x.Emp.Id == x.Temp.EmployeeId)
					.MasterAlias(x => x.Emp)
					.From(x =>
						x(repo.Where<EmployeeTalent>(y => y.TalentId == id))
						.As(x.Temp));

				var str = cmd.ToString();
				Assert.AreEqual(
					"SELECT Emp.Id, Emp.FirstName, Emp.LastName, Emp.BirthDate, Emp.Active, Emp.ManagerId, Emp.CountryId, Emp.JoinDate, Emp.StartTime, Emp.Photo, Emp.FullName, Emp.RowVersion " +
					"FROM (SELECT EmployeeId, TalentId, RowVersion FROM EmployeeTalents WHERE (TalentId = @0)) AS Temp, " +
					"Employees AS Emp " +
					"WHERE (Emp.Id = Temp.EmployeeId) " +
					"-- [@0 = '" + id + "']",
					str);

				var list = cmd.ToList();
				ConsoleEx.ReadLine("\n--- Press [Enter] to print results...");
				foreach (var obj in list) ConsoleEx.WriteLine("\n> {0}", obj);

				var count = DataDB.DB.EmployeeTalents.Count(x => x.TalentId == id);
				Assert.AreEqual(count, list.Count);
			}
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Query_With_Year_Virtual_Method()
		{
			DB.Prepare();

			using (var repo = Repo.Create())
			{
				var cmd = repo.Where<Employee>(x => x.BirthDate.Year() <= 1968);
				ConsoleEx.WriteLine("\n> Command: {0}", cmd);

				var list = cmd.ToList(); foreach (var obj in list) ConsoleEx.WriteLine("\n- Employee: {0}", obj);
				var count = DB.Employees.Count(x => x.BirthDate != null && x.BirthDate.Year <= 1968);
				Assert.AreEqual(count, list.Count);
			}
		}
	}
}
