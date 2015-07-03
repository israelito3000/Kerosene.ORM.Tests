using Kerosene.ORM.Configuration;
using Kerosene.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Kerosene.ORM.Core.Tests
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

			Assert.IsNotNull(info.Parser);
			Assert.AreEqual(true, info.Parser.ComplexTags);

			Assert.IsNotNull(info.DataEngine);
			Assert.AreEqual(true, info.DataEngine.RelaxTransformers);

			Assert.IsNotNull(info.DataLink);
			Assert.AreEqual("KeroseneDB", info.DataLink.ConnectionString);
			Assert.AreEqual(5, info.DataLink.Retries);
			Assert.AreEqual(30, info.DataLink.RetryInterval);

			Assert.IsNotNull(info.CustomEngines);
			Assert.AreEqual(1, info.CustomEngines.Count);

			var engine = info.CustomEngines.Items.FirstOrDefault();
			Assert.IsNotNull(engine);
			Assert.AreEqual("MyUniqueId", engine.Id);
			Assert.AreEqual("Kerosene.ORM.Core.Concrete.DataEngine", engine.TypeName);
			Assert.AreEqual("Kerosene.ORM.dll", engine.AssemblyName);
			Assert.AreEqual("AnInvariantName", engine.InvariantName);
			Assert.AreEqual("1.2.3", engine.ServerVersion);
			Assert.AreEqual(true, engine.CaseSensitiveNames);
			Assert.AreEqual("p", engine.ParameterPrefix);
			Assert.AreEqual(true, engine.PositionalParameters);
			Assert.AreEqual(true, engine.SupportsNativeSkipTake);
		}
	}
}
