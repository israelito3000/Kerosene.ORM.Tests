using Kerosene.ORM.ExampleDB;
using Kerosene.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Transactions;

namespace Kerosene.ORM.SqlServer.Tests
{
	// ====================================================
	[TestClass]
	public class Test_Connections
	{
		//[OnlyThisTest]
		[TestMethod]
		public void Validate_Versions_Are_Registered()
		{
			Direct.DataEngine.InitializeEngines();

			var v2008 = Direct.DataEngine.Locate(name: "SqlClient", minVersion: "10", maxVersion: "10.999"); Assert.IsNotNull(v2008);
			var v2012 = Direct.DataEngine.Locate(name: "SqlClient", minVersion: "11", maxVersion: "11.999"); Assert.IsNotNull(v2012);
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Create_With_Engine_And_Without_ConnectionString()
		{
			Direct.DataEngine.InitializeEngines();

			var engine = Direct.DataEngine.Locate();
			var link = Direct.DataLink.Create(engine);
			Assert.IsNotNull(link);
			Assert.IsNull(link.ConnectionString);
			ConsoleEx.WriteLine("\n> Link: {0}", link);

			try { link.Open(); Assert.Fail(); } // Because null string
			catch (InvalidOperationException) { }
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Create_From_Partial_Engine_Name()
		{
			Direct.DataEngine.InitializeEngines();

			var link = Direct.DataLink.Create("SqlClient");
			Assert.IsNotNull(link);
			Assert.IsNull(link.ConnectionString);
			ConsoleEx.WriteLine("\n> Link: {0}", link);

			try { link.Open(); Assert.Fail(); }
			catch (InvalidOperationException) { }
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Create_With_Engine_Name_And_Connection_String()
		{
			Direct.DataEngine.InitializeEngines();

			var link = Direct.DataLink.Create("SqlClient",
				"Server=localhost;Database=KeroseneDB;Integrated Security=true");

			Assert.IsNotNull(link);
			Assert.IsNotNull(link.ConnectionString);
			ConsoleEx.WriteLine("\n> Link: {0}", link);
			ConsoleEx.WriteLine("- Connection String: {0}", link.ConnectionString);

			link.Open();
			link.Close();
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Create_From_Default_Entry()
		{
			Direct.DataEngine.InitializeEngines();

			var link = Direct.DataLink.Create();

			Assert.IsNotNull(link);
			Assert.IsNotNull(link.ConnectionString);
			ConsoleEx.WriteLine("\n> Link: {0}", link);
			ConsoleEx.WriteLine("- Connection String: {0}", link.ConnectionString);

			link.Open();
			link.Dispose();
		}
	}
}
