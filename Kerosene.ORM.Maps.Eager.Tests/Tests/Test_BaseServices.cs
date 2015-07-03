using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kerosene.ORM.Maps;
using Kerosene.Tools;
using System;
using System.Linq;
using Kerosene.ORM.ExampleDB;

namespace Kerosene.ORM.Maps.Eager.Tests
{
	// ==================================================== 
	[TestClass]
	public class Test_BaseServices
	{
		//[OnlyThisTest]
		[TestMethod]
		public void New_Repo_Creates_Its_Maps()
		{
			using (var repo = Repo.Create())
			{
				ConsoleEx.WriteLine("\n> Repo: {0}", repo.ToConsoleString());
				Assert.AreEqual(5, repo.Maps.Count());
				var reg = repo.Maps.FirstOrDefault(x => x.EntityType == typeof(Region)); Assert.IsNotNull(reg); Assert.IsFalse(reg.IsValidated);
				var ctry = repo.Maps.FirstOrDefault(x => x.EntityType == typeof(Country)); Assert.IsNotNull(ctry); Assert.IsFalse(ctry.IsValidated);
				var emp = repo.Maps.FirstOrDefault(x => x.EntityType == typeof(Employee)); Assert.IsNotNull(emp); Assert.IsFalse(emp.IsValidated);
				var tal = repo.Maps.FirstOrDefault(x => x.EntityType == typeof(Talent)); Assert.IsNotNull(tal); Assert.IsFalse(tal.IsValidated);
				var emptal = repo.Maps.FirstOrDefault(x => x.EntityType == typeof(EmployeeTalent)); Assert.IsNotNull(emptal); Assert.IsFalse(emptal.IsValidated);

				ConsoleEx.ReadLine("\n--- Press [Enter] to validate...");
				foreach (var map in repo.Maps) map.Validate();
				ConsoleEx.WriteLine("\n> Validated: {0}", repo.ToConsoleString());
			}
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Clone_Repository()
		{
			using (var repo = Repo.Create())
			{
				var reg = repo.LocateMap<Region>(); Assert.IsNotNull(reg);
				var ctry = repo.LocateMap<Country>(); Assert.IsNotNull(ctry);
				var emp = repo.LocateMap<Employee>(); Assert.IsNotNull(emp);
				var talent = repo.LocateMap<Talent>(); Assert.IsNotNull(talent);
				var emptalent = repo.LocateMap<EmployeeTalent>(); Assert.IsNotNull(emptalent);

				foreach (var map in repo.Maps) map.Validate();
				ConsoleEx.WriteLine("\n> Source: {0}", repo.ToConsoleString());

				var clone = repo.Clone(repo.Link);
				ConsoleEx.WriteLine("\n\n> Cloned: {0}", clone.ToConsoleString());
				Assert.AreEqual(repo.Maps.Count(), clone.Maps.Count());

				clone.Dispose();
				ConsoleEx.WriteLine("\n> Disposed: {0}", clone.ToConsoleString());
				Assert.IsTrue(clone.IsDisposed);
			}
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Attach_New_And_Detach()
		{
			using (var repo = Repo.Create())
			{
				var reg = new Region() { Id = "000" };
				repo.Attach(reg);

				var meta = MetaEntity.Locate(reg);
				Assert.IsNotNull(meta.Map);
				Assert.AreEqual(1, repo.Entities.Count());

				repo.Detach(reg);
				Assert.IsNull(meta.Map);
				Assert.AreEqual(0, repo.Entities.Count());
			}
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Attach_Duplicate_And_Detach()
		{
			DB.Prepare();

			using (var repo = Repo.Create())
			{
				var id = DB.Regions[0].Id;
				var reg = repo.Where<Region>(x => x.Id == id).First(); Assert.IsNotNull(reg);

				var obj = new Region() { Id = id };
				repo.Attach(obj);

				var list = repo.Entities.ToList();
				var metaReg = MetaEntity.Locate(reg); Assert.IsTrue(list.Contains(reg)); Assert.IsNotNull(metaReg.Map);
				var metaObj = MetaEntity.Locate(obj); Assert.IsTrue(list.Contains(obj)); Assert.IsNotNull(metaObj.Map);

				repo.Detach(obj);
				list = repo.Entities.ToList();
				Assert.IsFalse(list.Contains(obj)); Assert.IsNull(metaObj.Map);
			}
		}
	}
}
