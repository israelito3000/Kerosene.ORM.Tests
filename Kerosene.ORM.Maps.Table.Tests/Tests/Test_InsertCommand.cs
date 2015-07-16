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
	}
}