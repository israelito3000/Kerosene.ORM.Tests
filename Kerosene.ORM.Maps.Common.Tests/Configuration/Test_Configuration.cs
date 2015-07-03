using Kerosene.ORM.Configuration;
using Kerosene.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Kerosene.ORM.Maps.Common.Tests
{
	// ==================================================== 
	[TestClass]
	public class Test_Configuration
	{
		//[OnlyThisTest]
		[TestMethod]
		public void Read_Configuration_Values()
		{
			var info = ORMConfiguration.GetInfo(); Assert.IsNotNull(info);

			Assert.IsNotNull(info.DataMap);
			Assert.AreEqual(true, info.DataMap.EnableCollector);
			Assert.AreEqual(true, info.DataMap.EnableCollectorGC);
			Assert.AreEqual(5000, info.DataMap.CollectorInterval);
			Assert.AreEqual(true, info.DataMap.EnableWeakMaps);
			Assert.AreEqual(true, info.DataMap.TrackEntities);
			Assert.AreEqual(true, info.DataMap.TrackChildEntities);
		}
	}
}
