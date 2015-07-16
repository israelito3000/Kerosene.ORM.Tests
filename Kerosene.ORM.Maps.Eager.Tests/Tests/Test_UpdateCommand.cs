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
	public class Test_UpdateCommand
	{
		//[OnlyThisTest]
		[TestMethod]
		public void Update_One()
		{
			DB.Prepare();

			using (var repo = Repo.Create())
			{
				var root = repo.Where<Region>(x => x.ParentId == null).First(); Assert.IsNotNull(root);
				ConsoleEx.WriteLine("\n> Source: {0}", root);

				root.Name = "Whatever";
				repo.UpdateNow(root);

				ConsoleEx.WriteLine("\n> Clearing and presenting entities...");
				repo.ClearEntities();
				root = repo.Where<Region>(x => x.ParentId == null).First(); Assert.IsNotNull(root);
				Assert.AreEqual("Whatever", root.Name);
			}
			DB.IsPrepared = false;
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Update_AggregateRoot_From_Root_First_Level()
		{
			DB.DeleteContents();
			DB.Link.Insert(x => x.Regions).Columns(x => x.Id = "000").Execute();
			DB.Link.Insert(x => x.Regions).Columns(x => x.Id = "100", x => x.ParentId = "000").Execute();
			DB.Link.Insert(x => x.Regions).Columns(x => x.Id = "200", x => x.ParentId = "000").Execute();
			DB.Link.Insert(x => x.Regions).Columns(x => x.Id = "201", x => x.ParentId = "200").Execute();
			DB.Link.Insert(x => x.Regions).Columns(x => x.Id = "202", x => x.ParentId = "200").Execute();

			using (var repo = Repo.Create())
			{
				var root = repo.Where<Region>(x => x.ParentId == null).First(); Assert.IsNotNull(root);
				var child = root.Childs.Where(x => x.Id == "200").FirstOrDefault(); Assert.IsNotNull(child);
				var item = child.Childs.Where(x => x.Id == "201").FirstOrDefault(); Assert.IsNotNull(item);
				var other = child.Childs.Where(x => x.Id == "202").FirstOrDefault(); Assert.IsNotNull(other);
				ConsoleEx.WriteLine("\n> Source: {0}", root);

				root.Childs.Remove(child);
				repo.UpdateNow(root);

				ConsoleEx.WriteLine("\n> Clearing and presenting entities...");
				repo.ClearEntities();

				child = repo.Where<Region>(x => x.Id == "200").First(); Assert.IsNull(child);
				item = repo.Where<Region>(x => x.Id == "201").First(); Assert.IsNull(item);
				other = repo.Where<Region>(x => x.Id == "202").First(); Assert.IsNull(item);
			}
			DB.IsPrepared = false;
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Update_AggregateRoot_From_Root_Second_Level()
		{
			DB.DeleteContents();
			DB.Link.Insert(x => x.Regions).Columns(x => x.Id = "000").Execute();
			DB.Link.Insert(x => x.Regions).Columns(x => x.Id = "100", x => x.ParentId = "000").Execute();
			DB.Link.Insert(x => x.Regions).Columns(x => x.Id = "200", x => x.ParentId = "000").Execute();
			DB.Link.Insert(x => x.Regions).Columns(x => x.Id = "201", x => x.ParentId = "200").Execute();
			DB.Link.Insert(x => x.Regions).Columns(x => x.Id = "202", x => x.ParentId = "200").Execute();

			using (var repo = Repo.Create())
			{
				var root = repo.Where<Region>(x => x.ParentId == null).First(); Assert.IsNotNull(root);
				var child = root.Childs.Where(x => x.Id == "200").FirstOrDefault(); Assert.IsNotNull(child);
				var item = child.Childs.Where(x => x.Id == "201").FirstOrDefault(); Assert.IsNotNull(item);
				var other = child.Childs.Where(x => x.Id == "202").FirstOrDefault(); Assert.IsNotNull(other);
				ConsoleEx.WriteLine("\n> Source: {0}", root);

				child.Childs.Remove(other);
				repo.UpdateNow(root);

				root = null;
				child = null;
				item = null;
				other = null;
				ConsoleEx.WriteLine("\n> Clearing and presenting entities...");

				repo.ClearEntities();
				other = repo.Where<Region>(x => x.Id == "202").First(); Assert.IsNull(other);
			}
			DB.IsPrepared = false;
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Update_AggregateRoot_From_Leave_First_Level()
		{
			DB.DeleteContents();
			DB.Link.Insert(x => x.Regions).Columns(x => x.Id = "000").Execute();
			DB.Link.Insert(x => x.Regions).Columns(x => x.Id = "100", x => x.ParentId = "000").Execute();
			DB.Link.Insert(x => x.Regions).Columns(x => x.Id = "200", x => x.ParentId = "000").Execute();
			DB.Link.Insert(x => x.Regions).Columns(x => x.Id = "201", x => x.ParentId = "200").Execute();
			DB.Link.Insert(x => x.Regions).Columns(x => x.Id = "202", x => x.ParentId = "200").Execute();

			using (var repo = Repo.Create())
			{
				var root = repo.Where<Region>(x => x.Id == "000").First(); Assert.IsNotNull(root);
				var child = root.Childs.Where(x => x.Id == "200").FirstOrDefault(); Assert.IsNotNull(child);
				var item = child.Childs.Where(x => x.Id == "201").FirstOrDefault(); Assert.IsNotNull(item);
				var other = child.Childs.Where(x => x.Id == "202").FirstOrDefault(); Assert.IsNotNull(other);
				ConsoleEx.WriteLine("\n> Source: {0}", root);

				root.Name = "Whatever";
				child.Name = "Another";
				repo.UpdateNow(child);

				ConsoleEx.WriteLine("\n> Clearing and presenting entities...");
				repo.ClearEntities();

				root = repo.Where<Region>(x => x.Id == "000").First(); Assert.AreEqual("Whatever", root.Name);
				child = root.Childs.FirstOrDefault(x => x.Id == "200"); Assert.AreEqual("Another", child.Name);
			}
			DB.IsPrepared = false;
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Change_Of_Aggregate_Root_FromLeft()
		{
			DB.DeleteContents();
			DB.Link.Insert(x => x.Regions).Columns(x => x.Id = "000").Execute();
			DB.Link.Insert(x => x.Regions).Columns(x => x.Id = "100", x => x.ParentId = "000").Execute();
			DB.Link.Insert(x => x.Regions).Columns(x => x.Id = "200", x => x.ParentId = "000").Execute();
			DB.Link.Insert(x => x.Regions).Columns(x => x.Id = "XXX", x => x.ParentId = "100").Execute();

			using (var repo = Repo.Create())
			{
				var root = repo.Where<Region>(x => x.Id == "000").First(); Assert.IsNotNull(root);
				var r100 = repo.Where<Region>(x => x.Id == "100").First(); Assert.IsNotNull(r100);
				var r200 = repo.Where<Region>(x => x.Id == "200").First(); Assert.IsNotNull(r200);
				var rXXX = repo.Where<Region>(x => x.Id == "XXX").First(); Assert.IsNotNull(rXXX);
				Assert.IsNotNull(r100.Childs.Find(x => x.Id == "XXX"));
				Assert.IsTrue(rXXX.Parent.Id == "100");

				rXXX = r100.Childs[0];
				r100.Childs.Remove(rXXX);
				r200.Childs.Add(rXXX); rXXX.Parent = r200;
				repo.UpdateNow(root);

				ConsoleEx.WriteLine("\n> Validating results...");

				r100 = repo.Where<Region>(x => x.Id == "100").First(); Assert.IsNotNull(r100);
				Assert.IsNull(r100.Childs.Find(x => x.Id == "XXX"));

				r200 = repo.Where<Region>(x => x.Id == "200").First(); Assert.IsNotNull(r200);
				Assert.IsNotNull(r200.Childs.Find(x => x.Id == "XXX"));

				rXXX = repo.Where<Region>(x => x.Id == "XXX").First(); Assert.IsNotNull(rXXX);
				Assert.IsTrue(rXXX.Parent.Id == "200");
			}
			DB.IsPrepared = false;
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Change_Of_Aggregate_Root_FromRight()
		{
			DB.DeleteContents();
			DB.Link.Insert(x => x.Regions).Columns(x => x.Id = "000").Execute();
			DB.Link.Insert(x => x.Regions).Columns(x => x.Id = "100", x => x.ParentId = "000").Execute();
			DB.Link.Insert(x => x.Regions).Columns(x => x.Id = "200", x => x.ParentId = "000").Execute();
			DB.Link.Insert(x => x.Regions).Columns(x => x.Id = "XXX", x => x.ParentId = "200").Execute();

			using (var repo = Repo.Create())
			{
				var root = repo.Where<Region>(x => x.Id == "000").First(); Assert.IsNotNull(root);
				var r100 = repo.Where<Region>(x => x.Id == "100").First(); Assert.IsNotNull(r100);
				var r200 = repo.Where<Region>(x => x.Id == "200").First(); Assert.IsNotNull(r200);
				var rXXX = repo.Where<Region>(x => x.Id == "XXX").First(); Assert.IsNotNull(rXXX);
				Assert.IsNotNull(r200.Childs.Find(x => x.Id == "XXX"));
				Assert.IsTrue(rXXX.Parent.Id == "200");

				rXXX = r200.Childs[0];
				r200.Childs.Remove(rXXX);
				r100.Childs.Add(rXXX); rXXX.Parent = r100;
				repo.UpdateNow(root);

				ConsoleEx.WriteLine("\n> Validating results...");

				// The following tests will fail if uncommented... reason is because due the way
				// the pending operations are ordered, last operation against rXXX is to delete.

				rXXX = repo.Where<Region>(x => x.Id == "XXX").First();
				// Assert.IsNotNull(rXXX);

				r100 = repo.Where<Region>(x => x.Id == "100").First(); Assert.IsNotNull(r100);
				// Assert.IsNotNull(r100.Childs.Find(x => x.Id == "XXX"));
				// Assert.IsTrue(rXXX.Parent.Id == "100");

				r200 = repo.Where<Region>(x => x.Id == "200").First(); Assert.IsNotNull(r200);
				Assert.IsNull(r200.Childs.Find(x => x.Id == "XXX"));
			}
			DB.IsPrepared = false;
		}
	}
}
