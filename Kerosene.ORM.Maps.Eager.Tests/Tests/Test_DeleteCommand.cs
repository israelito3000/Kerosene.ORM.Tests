using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kerosene.ORM.DataDB;
using Kerosene.ORM.Maps;
using Kerosene.Tools;
using System;
using System.Linq;
using Kerosene.ORM.Core;

namespace Kerosene.ORM.Maps.Eager.Tests
{
	// ==================================================== 
	[TestClass]
	public class Test_DeleteCommand
	{
		//[OnlyThisTest]
		[TestMethod]
		public void Delete_Isolated_Item()
		{
			DB.DeleteContents();
			DB.Link.Insert(x => x.Regions).Columns(x => x.Id == "000").Execute();

			using (var repo = Repo.Create())
			{
				var obj = repo.Where<Region>(x => x.Id == "000").First(); Assert.IsNotNull(obj);
				Console.WriteLine("\n> Item: {0}", obj);

				repo.DeleteNow(obj);
				obj = repo.Where<Region>(x => x.Id == "000").First(); Assert.IsNull(obj);
			}
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Delete_Parent_Item()
		{
			DB.DeleteContents();
			DB.Link.Insert(x => x.Regions).Columns(x => x.Id == "000").Execute();
			DB.Link.Insert(x => x.Regions).Columns(x => x.Id = "001", x => x.ParentId = "000").Execute();

			using (var repo = Repo.Create())
			{
				var obj = repo.Where<Region>(x => x.Id == "000").First(); Assert.IsNotNull(obj);
				Console.WriteLine("\n> Item: {0}", obj);

				repo.DeleteNow(obj);

				ConsoleEx.WriteLine("\n> Validating results...");
				obj = repo.Where<Region>(x => x.Id == "000").First(); Assert.IsNull(obj);
				obj = repo.Where<Region>(x => x.Id == "001").First(); Assert.IsNull(obj);
			}
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Delete_Child_Item()
		{
			DB.DeleteContents();
			DB.Link.Insert(x => x.Regions).Columns(x => x.Id == "000").Execute();
			DB.Link.Insert(x => x.Regions).Columns(x => x.Id = "001", x => x.ParentId = "000").Execute();

			using (var repo = Repo.Create())
			{
				var obj = repo.Where<Region>(x => x.Id == "001").First(); Assert.IsNotNull(obj);
				Console.WriteLine("\n> Item: {0}", obj);

				repo.DeleteNow(obj);

				ConsoleEx.WriteLine("\n> Validating results...");
				obj = repo.Where<Region>(x => x.Id == "000").First(); Assert.IsTrue(obj.Childs.Count == 0);
				obj = repo.Where<Region>(x => x.Id == "001").First(); Assert.IsNull(obj);
			}
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Delete_Root_Cascade()
		{
			DB.Prepare();
			DB.IsPrepared = false;

			using (var repo = Repo.Create())
			{
				var root = repo.Where<Region>(x => x.ParentId == null).First(); Assert.IsNotNull(root);
				repo.DeleteNow(root);

				ConsoleEx.WriteLine("\n> Validating results...");
				root = repo.Where<Region>(x => x.ParentId == null).First();
				Assert.IsNull(root);
			}
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Delete_With_Child_Version_Change()
		{
			DB.DeleteContents();
			DB.Link.Insert(x => x.Regions).Columns(x => x.Id == "000").Execute();
			DB.Link.Insert(x => x.Regions).Columns(x => x.Id = "001", x => x.ParentId = "000").Execute();

			using (var repo = Repo.Create())
			{
				var root = repo.Where<Region>(x => x.Id == "000").First(); Assert.IsNotNull(root);
				ConsoleEx.WriteLine("\n> Root: {0}", root);

				// Emulating the child has been updated in the database...
				DB.Link.Update(x => x.Regions)
					.Where(x => x.Id == "001")
					.Columns(x => x.Name = "Whatever")
					.Execute();

				// Deleting the parent shall identify a child has been updated...
				try { repo.DeleteNow(root); Assert.Fail(); }
				catch (ChangedException e)
				{
					ConsoleEx.WriteLine("\n> Cannot delete because: {0}", e.ToDisplayString());
				}

				// Let's validate we still have a working repo...
				var link = repo.Link;
				Assert.IsFalse(link.IsDisposed);
			}
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Delete_With_Child_Of_Child_Version_Change()
		{
			DB.Prepare();
			DB.IsPrepared = false;

			using (var repo = Repo.Create())
			{
				var root = repo.Where<Region>(x => x.Id == "000").First(); Assert.IsNotNull(root);
				ConsoleEx.WriteLine("\n> Root: {0}", root);

				var child = root.Childs[0].Childs[0]; Assert.IsNotNull(child);
				ConsoleEx.WriteLine("\n> Child of Child: {0}", child);

				DB.Link.Update(x => x.Regions)
					.Where(x => x.Id == child.Id)
					.Columns(x => x.Name = "Whatever")
					.Execute();

				try { repo.DeleteNow(root); Assert.Fail(); } // Deleting from aggregate root...
				catch (ChangedException e)
				{
					ConsoleEx.WriteLine("\n> Cannot delete because: {0}", e.ToDisplayString());
				}

				// Let's validate we still have a working repo...
				var link = repo.Link;
				Assert.IsFalse(link.IsDisposed);
			}
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Delete_With_Child_Of_Child_Of_Child_Version_Changed()
		{
			DB.Prepare();

			using (var repo = Repo.Create())
			{
				var root = repo.Where<Region>(x => x.Id == "000").First(); Assert.IsNotNull(root);
				ConsoleEx.WriteLine("\n> Root: {0}", root);

				var child = root.Childs[0].Childs[0].Childs[0]; Assert.IsNotNull(child);
				ConsoleEx.WriteLine("\n> Child of Child: {0}", child);

				DB.Link.Update(x => x.Regions)
					.Where(x => x.Id == child.Id)
					.Columns(x => x.Name = "Whatever")
					.Execute();

				try { repo.DeleteNow(root); Assert.Fail(); } // Deleting from aggregate root...
				catch (ChangedException e)
				{
					ConsoleEx.WriteLine("\n> Cannot delete because: {0}", e.ToDisplayString());
				}

				// Let's validate we still have a working repo...
				var link = repo.Link;
				Assert.IsFalse(link.IsDisposed);
			}
			DB.IsPrepared = false;
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Delete_Child_Of_Other_Map()
		{
			DB.DeleteContents();
			DB.Link.Insert(x => x.Regions).Columns(x => x.Id = "000").Execute();
			DB.Link.Insert(x => x.Countries).Columns(x => x.Id = "aa", x => x.RegionId = "000").Execute();
			DB.Link.Insert(x => x.Countries).Columns(x => x.Id = "bb", x => x.RegionId = "000").Execute();

			using (var repo = Repo.Create())
			{
				var root = repo.Where<Region>(x => x.Id == "000").First();
				ConsoleEx.WriteLine("\n> Source: {0}", root);

				root.Countries.RemoveAt(0);
				root.Countries.Add(new Country() { Id = "zz", Region = root });
				ConsoleEx.WriteLine("\n> Source: {0}", root);

				repo.UpdateNow(root);

				root = repo.Where<Region>(x => x.ParentId == null).First();
				ConsoleEx.WriteLine("\n> Source: {0}", root);
				Assert.AreEqual(2, root.Countries.Count);
				Assert.IsNotNull(root.Countries.FirstOrDefault(x => x.Id == "bb"));
				Assert.IsNotNull(root.Countries.FirstOrDefault(x => x.Id == "zz"));
			}
			DB.IsPrepared = false;
		}
	}
}
