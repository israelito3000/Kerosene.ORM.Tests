using Kerosene.ORM.ExampleDB;
using Kerosene.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Kerosene.ORM.Direct.Tests
{
	// ====================================================
	[TestClass]
	public class Test_DataEngine
	{
		//[OnlyThisTest]
		[TestMethod]
		public void Validate_Direct_Engines_Initialization()
		{
			Direct.DataEngine.InitializeEngines();
			var list = Direct.DataEngine.Engines.ToList();
			ConsoleEx.WriteLine("\n> Engines: {0}", list.Sketch());

			Assert.AreEqual(4, list.Count);
			Assert.IsTrue(list.Find(x => x.InvariantName == "System.Data.Odbc") != null);
			Assert.IsTrue(list.Find(x => x.InvariantName == "System.Data.OleDb") != null);
			Assert.IsTrue(list.Find(x => x.InvariantName == "System.Data.OracleClient") != null);
			Assert.IsTrue(list.Find(x => x.InvariantName == "System.Data.SqlClient") != null);
		}

		//[OnlyThisTest]
		[TestMethod]
		public void LocateWithPartialName()
		{
			Direct.DataEngine.InitializeEngines();
			var engine = Direct.DataEngine.Locate("SqlClient");
			ConsoleEx.WriteLine("\n> {0}\n", engine);

			Assert.IsNotNull(engine);
		}

		//[OnlyThisTest]
		[TestMethod]
		public void LocateByDefault()
		{
			Direct.DataEngine.InitializeEngines();
			var engine = Direct.DataEngine.Locate();
			ConsoleEx.WriteLine("\n> {0}\n", engine);

			Assert.IsNotNull(engine);
		}
	}
}