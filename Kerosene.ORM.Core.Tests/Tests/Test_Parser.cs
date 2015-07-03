using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kerosene.Tools;
using System;
using System.Linq;

namespace Kerosene.ORM.Core.Tests
{
	// ==================================================== 
	[TestClass]
	public class Test_Parser
	{
		//[OnlyThisTest]
		[TestMethod]
		public void Dispose_Twice()
		{
			var engine = new Concrete.DataEngine();
			var parser = new Concrete.Parser(engine);

			parser.Dispose();
			parser.Dispose();

			Assert.IsTrue(parser.IsDisposed);
			Assert.IsNull(parser.Engine);
		}

		//[OnlyThisTest]
		[TestMethod]
		public void NullValues()
		{
			var engine = new Concrete.DataEngine();
			var parser = new Concrete.Parser(engine);
			var pars = engine.CreateParameterCollection();

			var result = parser.Parse(null, pars);
			Assert.AreEqual("NULL", result);
			Assert.AreEqual(0, pars.Count);
		}

		//[OnlyThisTest]
		[TestMethod]
		public void ShortTag()
		{
			var engine = new Concrete.DataEngine();
			var parser = new Concrete.Parser(engine);
			var pars = engine.CreateParameterCollection();

			var result = parser.Parse((Func<dynamic, object>)(x => x.Name), pars);
			Assert.AreEqual("Name", result);
			Assert.AreEqual(0, pars.Count);
		}

		//[OnlyThisTest]
		[TestMethod]
		public void LongTag()
		{
			var engine = new Concrete.DataEngine();
			var parser = new Concrete.Parser(engine);
			var pars = engine.CreateParameterCollection();

			Parser.ComplexTags = true;
			var result = parser.Parse((Func<dynamic, object>)(emp => emp.Name), pars);
			Assert.AreEqual("emp.Name", result);
			Assert.AreEqual(0, pars.Count);
		}

		//[OnlyThisTest]
		[TestMethod]
		public void GetMember()
		{
			var engine = new Concrete.DataEngine();
			var parser = new Concrete.Parser(engine);
			var pars = engine.CreateParameterCollection();

			var result = parser.Parse((Func<dynamic, object>)(x => x.Alpha.Beta), pars);
			Assert.AreEqual("Alpha.Beta", result);
			Assert.AreEqual(0, pars.Count);
		}

		//[OnlyThisTest]
		[TestMethod]
		public void SetMember()
		{
			var engine = new Concrete.DataEngine();
			var parser = new Concrete.Parser(engine);
			var pars = engine.CreateParameterCollection();

			var result = parser.Parse((Func<dynamic, object>)(x => x.Alpha.Beta = null), pars);
			Assert.AreEqual("Alpha.Beta = (NULL)", result);
			Assert.AreEqual(0, pars.Count);

			result = parser.Parse((Func<dynamic, object>)(x => x.Alpha.Beta = "Bond"), pars);
			Assert.AreEqual("Alpha.Beta = (#0)", result);
			Assert.AreEqual(1, pars.Count);
			Assert.AreEqual("Bond", pars[0].Value);
		}

		//[OnlyThisTest]
		[TestMethod]
		public void GetIndexed_SquareSyntax()
		{
			var engine = new Concrete.DataEngine();
			var parser = new Concrete.Parser(engine);
			var pars = engine.CreateParameterCollection();

			var result = parser.Parse((Func<dynamic, object>)(x => x.Name["James", "Bond"]), pars);
			Assert.AreEqual("NameJamesBond", result);
			Assert.AreEqual(0, pars.Count);
		}

		//[OnlyThisTest]
		[TestMethod]
		public void GetIndexed_RoundedSyntax()
		{
			var engine = new Concrete.DataEngine();
			var parser = new Concrete.Parser(engine);
			var pars = engine.CreateParameterCollection();

			var result = parser.Parse((Func<dynamic, object>)(x => x("James", "Bond")), pars);
			Assert.AreEqual("JamesBond", result);
			Assert.AreEqual(0, pars.Count);
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Is_Null()
		{
			var engine = new Concrete.DataEngine();
			var parser = new Concrete.Parser(engine);
			var pars = engine.CreateParameterCollection();

			var result = parser.Parse((Func<dynamic, object>)(x => x.Name == null), pars);
			Assert.AreEqual("(Name IS NULL)", result);
			Assert.AreEqual(0, pars.Count);
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Is_NotNull()
		{
			var engine = new Concrete.DataEngine();
			var parser = new Concrete.Parser(engine);
			var pars = engine.CreateParameterCollection();

			var result = parser.Parse((Func<dynamic, object>)(x => x.Name != null), pars);
			Assert.AreEqual("(Name IS NOT NULL)", result);
			Assert.AreEqual(0, pars.Count);
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Unary_Not()
		{
			var engine = new Concrete.DataEngine();
			var parser = new Concrete.Parser(engine);
			var pars = engine.CreateParameterCollection();

			var result = parser.Parse((Func<dynamic, object>)(x => !x.Name), pars);
			Assert.AreEqual("(NOT Name)", result);
			Assert.AreEqual(0, pars.Count);
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Unary_Negate()
		{
			var engine = new Concrete.DataEngine();
			var parser = new Concrete.Parser(engine);
			var pars = engine.CreateParameterCollection();

			var result = parser.Parse((Func<dynamic, object>)(x => -x.Name), pars);
			Assert.AreEqual("-Name", result);
			Assert.AreEqual(0, pars.Count);
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Binary_WithNegate()
		{
			var engine = new Concrete.DataEngine();
			var parser = new Concrete.Parser(engine);
			var pars = engine.CreateParameterCollection();

			var result = parser.Parse((Func<dynamic, object>)(x => -x.Name + 5), pars);
			Assert.AreEqual("(-Name + #0)", result);
			Assert.AreEqual(1, pars.Count);
			Assert.AreEqual(5, pars[0].Value);
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Method_Not()
		{
			var engine = new Concrete.DataEngine();
			var parser = new Concrete.Parser(engine);
			var pars = engine.CreateParameterCollection();

			var result = parser.Parse((Func<dynamic, object>)(x => x.Not(x.Alpha)), pars);
			Assert.AreEqual("(NOT Alpha)", result);
			Assert.AreEqual(0, pars.Count);
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Method_Distinct()
		{
			var engine = new Concrete.DataEngine();
			var parser = new Concrete.Parser(engine);
			var pars = engine.CreateParameterCollection();

			var result = parser.Parse((Func<dynamic, object>)(x => x.Distinct(x.Alpha)), pars);
			Assert.AreEqual("DISTINCT Alpha", result);
			Assert.AreEqual(0, pars.Count);
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Method_As()
		{
			var engine = new Concrete.DataEngine();
			var parser = new Concrete.Parser(engine);
			var pars = engine.CreateParameterCollection();

			var result = parser.Parse((Func<dynamic, object>)(x => x.Alpha.As(x.Beta)), pars);
			Assert.AreEqual("Alpha AS Beta", result);
			Assert.AreEqual(0, pars.Count);
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Method_In()
		{
			var engine = new Concrete.DataEngine();
			var parser = new Concrete.Parser(engine);
			var pars = engine.CreateParameterCollection();

			var result = parser.Parse((Func<dynamic, object>)(x => x.Alpha.In(x.Beta, "37", "40")), pars);
			Assert.AreEqual("Alpha IN (Beta, #0, #1)", result);
			Assert.AreEqual(2, pars.Count);
			Assert.AreEqual("37", pars[0].Value);
			Assert.AreEqual("40", pars[1].Value);
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Method_NotIn()
		{
			var engine = new Concrete.DataEngine();
			var parser = new Concrete.Parser(engine);
			var pars = engine.CreateParameterCollection();

			var result = parser.Parse((Func<dynamic, object>)(x => x.Alpha.NotIn(x.Beta, "37", "40")), pars);
			Assert.AreEqual("NOT Alpha IN (Beta, #0, #1)", result);
			Assert.AreEqual(2, pars.Count);
			Assert.AreEqual("37", pars[0].Value);
			Assert.AreEqual("40", pars[1].Value);
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Conversion()
		{
			var engine = new Concrete.DataEngine();
			var parser = new Concrete.Parser(engine);
			var pars = engine.CreateParameterCollection();

			var result = parser.Parse((Func<dynamic, object>)(x => (string)x.Name), pars);
			Assert.AreEqual("Name", result);
			Assert.AreEqual(0, pars.Count);
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Embedded_Commands()
		{
			var engine = new Concrete.DataEngine();
			var link = new FakeLink(engine, NestableTransactionMode.Database);
			var inner = link.Raw("SELECT * From WhoKnows WHERE Name == {0}", "James Bond");

			var parser = new Concrete.Parser(engine);
			var pars = engine.CreateParameterCollection();
			pars.AddCreate("PlaceHolderForPositionCero");
			Assert.AreEqual(1, pars.Count);

			var result = parser.Parse(inner, pars);
			Assert.AreEqual("SELECT * From WhoKnows WHERE Name == #1", result);
			Assert.AreEqual(2, pars.Count);
			Assert.AreEqual("James Bond", pars[1].Value);
		}

		//[OnlyThisTest]
		[TestMethod]
		public void CoreCommand_Provider()
		{
			var engine = new Concrete.DataEngine();
			var link = new FakeLink(engine, NestableTransactionMode.Database);

			var other = new FakeCommandProvider(link);
			other.TheCommand.From(x => x.Employees);

			var provider = new FakeCommandProvider(link);
			provider.TheCommand.From(x => x(other).As(x.Emp));
			provider.TheCommand.Select(x => x.Number.As(x.Id));
			provider.TheCommand.Where(x => x.Name == "James Bond");

			var parser = new Concrete.Parser(engine);
			var pars = engine.CreateParameterCollection();

			var result = parser.Parse(provider, pars);
			Assert.AreEqual(
				"SELECT Number AS Id FROM (SELECT * FROM Employees) AS Emp WHERE (Name = #0)"
				, result);
			Assert.AreEqual(1, pars.Count);
			Assert.AreEqual("James Bond", pars[0].Value);
		}
	}
}
