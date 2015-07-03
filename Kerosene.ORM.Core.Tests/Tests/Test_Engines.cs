using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kerosene.Tools;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Kerosene.ORM.Core.Tests
{
	// ==================================================== 
	[TestClass]
	public class Test_Engines
	{
		//[OnlyThisTest]
		[TestMethod]
		public void Validate_Standard_Transformers()
		{
			var obj = new Concrete.DataEngine();
			Assert.IsTrue(obj.IsTransformerRegistered<CalendarDate>());
			Assert.IsTrue(obj.IsTransformerRegistered<ClockTime>());

			var rd = obj.TryTransform(new CalendarDate(2000, 1, 1));
			Assert.AreEqual(new DateTime(2000, 1, 1), (DateTime)rd);

			var rc = obj.TryTransform(new ClockTime(1, 1, 1));
			Assert.AreEqual("1:1:1", (string)rc);
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Validate_CoreEngines()
		{
			var engines = DataEngine.Engines.ToList();
			Assert.IsNotNull(engines.Find(x => x.InvariantName == "Generic"));
			Assert.IsNotNull(engines.Find(x => x.InvariantName == "System.Data.Odbc"));
			Assert.IsNotNull(engines.Find(x => x.InvariantName == "System.Data.OleDb"));
			Assert.IsNotNull(engines.Find(x => x.InvariantName == "System.Data.OracleClient"));
			Assert.IsNotNull(engines.Find(x => x.InvariantName == "System.Data.SqlClient"));
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Disposing()
		{
			DataEngine.ClearEngines();
			var template = new Concrete.DataEngine();
			var cloned = template.Clone(new Dictionary<string, object>()
			{
				{ "InvariantName", "WhateverName"},
				{ "ServerVersion", "8"}
			});

			DataEngine.RegisterEngine(cloned);
			Assert.AreEqual(1, DataEngine.Engines.Count());
			Assert.AreEqual(cloned, DataEngine.Engines.First());

			cloned.Dispose();
			Assert.AreEqual(0, DataEngine.Engines.Count());
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Locate_With_MinVersion()
		{
			DataEngine.ClearEngines();
			var template = new Concrete.DataEngine();
			DataEngine.RegisterEngine(template.Clone(new Dictionary<string, object>()
			{
				{ "InvariantName", "WhateverName"},
				{ "ServerVersion", "8"}
			}));
			DataEngine.RegisterEngine(template.Clone(new Dictionary<string, object>()
			{
				{ "InvariantName", "WhateverName"},
				{ "ServerVersion", "10"}
			}));

			var engine = DataEngine.Locate("WhateverName", minVersion: "9");
			Assert.IsNotNull(engine);
			Assert.AreEqual("10", engine.ServerVersion);
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Locate_With_MaxVersion()
		{
			DataEngine.ClearEngines();
			var template = new Concrete.DataEngine();
			DataEngine.RegisterEngine(template.Clone(new Dictionary<string, object>()
			{
				{ "InvariantName", "WhateverName"},
				{ "ServerVersion", "8"}
			}));
			DataEngine.RegisterEngine(template.Clone(new Dictionary<string, object>()
			{
				{ "InvariantName", "WhateverName"},
				{ "ServerVersion", "10"}
			}));

			var engine = DataEngine.Locate("WhateverName", maxVersion: "9");
			Assert.IsNotNull(engine);
			Assert.AreEqual("8", engine.ServerVersion);
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Locate_Without_Version()
		{
			DataEngine.ClearEngines();
			var template = new Concrete.DataEngine();
			DataEngine.RegisterEngine(template.Clone(new Dictionary<string, object>()
			{
				{ "InvariantName", "WhateverName"},
				{ "ServerVersion", "8"}
			}));
			DataEngine.RegisterEngine(template.Clone(new Dictionary<string, object>()
			{
				{ "InvariantName", "WhateverName"},
				{ "ServerVersion", "10"}
			}));

			var engine = DataEngine.Locate("WhateverName");
			Assert.IsNotNull(engine);
			Assert.AreEqual("10", engine.ServerVersion);
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Locate_Engine_ForTest()
		{
			DataEngine.InitializeEngines();
			var engine = DataEngine.Locate("AnInvariantName");
			Assert.IsNotNull(engine);
			ConsoleEx.WriteLine("\n- {0}\n", engine);
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Locate_By_Default()
		{
			//DataEngine.InitializeEngines();
			//var engine = DataEngine.Locate();
			//Assert.IsNotNull(engine);
			//ConsoleEx.WriteLine("\n- {0}\n", engine);
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Locate_Non_Existent()
		{
			DataEngine.InitializeEngines();
			var engine = DataEngine.Locate("ThisNameSurelyDon'tExist");
			Assert.IsNull(engine);
		}
	}
}
