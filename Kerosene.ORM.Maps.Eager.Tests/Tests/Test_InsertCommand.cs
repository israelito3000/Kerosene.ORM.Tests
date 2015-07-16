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
	public class Test_InsertCommand
	{
		//[OnlyThisTest]
		[TestMethod]
		public void Insert_One()
		{
			DB.DeleteContents();

			using (var repo = Repo.Create())
			{
				var reg = new Region() { Id = "000" };
				repo.InsertNow(reg);

				reg = repo.Where<Region>(x => x.Id == "000").First();
				Assert.IsNotNull(reg);
			}
			DB.IsPrepared = false;
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Insert_Several_From_Aggregate_Root()
		{
			DB.DeleteContents();

			using (var repo = Repo.Create())
			{
				var root = new Region() { Id = "000" };
				root.Childs.Add(new Region() { Id = "100", Parent = root });
				root.Childs.Add(new Region() { Id = "200", Parent = root });
				var child = new Region() { Id = "300", Parent = root }; root.Childs.Add(child);
				child.Childs.Add(new Region() { Id = "301", Parent = child });
				child.Childs.Add(new Region() { Id = "302", Parent = child });

				repo.InsertNow(root);

				ConsoleEx.WriteLine("\n> Clearing and presenting entities...");
				repo.ClearEntities();

				var list = repo.Query<Region>().ToList();
				foreach (var obj in list) ConsoleEx.WriteLine("\n> {0}", obj);
				Assert.AreEqual(6, list.Count);
			}
			DB.IsPrepared = false;
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Insert_Child()
		{
			DB.DeleteContents();
			DB.IsPrepared = false;

			using (var repo = Repo.Create())
			{
				var root = new Region() { Id = "000" };
				var child = new Region() { Id = "100", Parent = root }; root.Childs.Add(child);

				repo.InsertNow(child);
				ConsoleEx.WriteLine("\n> Validating results...");

				root = repo.Where<Region>(x => x.Id == "000").First();
				Assert.IsNotNull(root);
				Assert.IsNotNull(root.Childs.Find(x => x.Id == "100"));

				child = repo.Where<Region>(x => x.Id == "100").First();
				Assert.IsNotNull(child);
				Assert.IsNotNull(child.Parent);
				Assert.IsTrue(child.Parent.Id == "000");
			}
		}
	}
}
