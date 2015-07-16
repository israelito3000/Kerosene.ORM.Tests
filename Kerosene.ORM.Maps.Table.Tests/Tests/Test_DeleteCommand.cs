using Kerosene.ORM.Core;
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
		public void Delete_Leave_Item()
		{
			DB.DeleteContents();
			DB.Link.Insert(x => x.Regions).Columns(x => x.Id == "000").Execute();
			DB.Link.Insert(x => x.Regions).Columns(x => x.Id = "001", x => x.ParentId = "000").Execute();

			using (var repo = Repo.Create())
			{
				var obj = repo.Where<Region>(x => x.Id == "001").First(); Assert.IsNotNull(obj);
				Console.WriteLine("\n> Item: {0}", obj);

				repo.DeleteNow(obj);
				obj = repo.Where<Region>(x => x.Id == "001").First(); Assert.IsNull(obj);
			}
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Delete_Version_Changed()
		{
			DB.DeleteContents();
			DB.Link.Insert(x => x.Regions).Columns(x => x.Id = "000").Execute();

			using (var repo = Repo.Create())
			{
				// Preparing a test record...
				var root = repo.Where<Region>(x => x.Id == "000").First(); Assert.IsNotNull(root);
				ConsoleEx.WriteLine("\n> Root: {0}", root);

				// Emulating a change in the database...
				DB.Link.Update(x => x.Regions)
					.Where(x => x.Id == "000")
					.Columns(x => x.Name = "Whatever")
					.Execute();

				// In table mode we don't keep record of row version, so it won't fail...
				repo.DeleteNow(root);
				root = repo.Where<Region>(x => x.Id == "000").First(); Assert.IsNull(root);
			}
			DB.IsPrepared = false;
		}
	}
}