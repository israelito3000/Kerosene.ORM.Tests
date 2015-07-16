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

				repo.ClearEntities();
				root = repo.Where<Region>(x => x.ParentId == null).First(); Assert.IsNotNull(root);
				Assert.AreEqual("Whatever", root.Name);
			}
			DB.IsPrepared = false;
		}
	}
}