using Kerosene.ORM.DataDB;
using Kerosene.ORM.Maps;
using Kerosene.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Kerosene.ORM.Maps.Table.Tests
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
		public void Find_Root_Employee_From_Cache_Clean()
		{
			DB.Prepare();

			using (var repo = Repo.Create())
			{
				var obj = repo.FindNow<Employee>(x => x.ManagerId == null);
				ConsoleEx.WriteLine("\n> {0}", obj);
				Assert.IsNotNull(obj);
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

				ConsoleEx.WriteLine("\n\n- Populating cache...");
				var list = repo.Query<Employee>().ToList(); Assert.AreEqual(DB.Employees.Count, list.Count);
				var root = list.FirstOrDefault(x => x.Id == id); Assert.IsNotNull(root);

				ConsoleEx.WriteLine("\n\n- Finding from cache... {0}", id);
				var obj = repo.FindNow<Employee>(x => x.Id == id);
				ConsoleEx.WriteLine("\n> {0}", obj);
				Assert.IsNotNull(obj);
				Assert.IsTrue(object.ReferenceEquals(root, obj));
			}
		}

		[OnlyThisTest]
		//[TestMethod]
		public void Find_Employee_No_Id_Cache_Populated()
		{
			DB.Prepare();

			using (var repo = Repo.Create())
			{
				ConsoleEx.WriteLine("\n\n- Populating cache...");
				var list = repo.Query<Employee>().ToList(); Assert.AreEqual(DB.Employees.Count, list.Count);

				var lastName = DB.Employees[DB.Employees.Count - 1].LastName;
				var obj = repo.FindNow<Employee>(x => x.LastName == lastName);
				ConsoleEx.WriteLine("\n> {0}", obj);
				Assert.IsNotNull(obj);
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

				var metaObj = MetaEntity.Locate(obj); ConsoleEx.WriteLine("\n> Implicit: {0}", metaObj);
				var metaTemp = MetaEntity.Locate(temp); ConsoleEx.WriteLine("\n> Returned: {0}", metaTemp);

				Assert.IsNotNull(temp);
				Assert.IsNotNull(metaTemp.Map);
				Assert.IsNotNull(metaObj.Map);
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

				ConsoleEx.WriteLine("\n\n- Populating cache...");
				var list = repo.Query<Employee>().ToList(); Assert.AreEqual(DB.Employees.Count, list.Count);
				var root = list.FirstOrDefault(x => x.Id == id); Assert.IsNotNull(root);

				ConsoleEx.WriteLine("\n\n- Refreshing a new instance...");
				var obj = new Employee() { Id = id };
				var temp = repo.RefreshNow(obj);

				var metaObj = MetaEntity.Locate(obj); ConsoleEx.WriteLine("\n> Implicit: {0}", metaObj);
				var metaTemp = MetaEntity.Locate(temp); ConsoleEx.WriteLine("\n> Returned: {0}", metaTemp);

				Assert.IsNotNull(temp);
				Assert.IsNotNull(metaTemp.Map);
				Assert.IsNotNull(metaObj.Map);

				// Validanting the new one is also refreshed...
				Assert.AreEqual(root.FirstName, obj.FirstName);
				Assert.AreEqual(root.LastName, obj.LastName);
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