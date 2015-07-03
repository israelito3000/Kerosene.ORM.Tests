using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kerosene.Tools;
using System;
using System.Linq;

namespace Kerosene.ORM.Core.Tests
{
	// ==================================================== 
	[TestClass]
	public class Test_ElementAlias
	{
		//[OnlyThisTest]
		[TestMethod]
		public void Validation_Rules()
		{
			ElementAlias.ValidateElement(null);

			// Relaxed: empty strings are translated into nulls...
			//try { ElementAlias.ValidateElement(string.Empty); Assert.Fail(); }
			//catch (EmptyException) { }
			ElementAlias.ValidateElement(string.Empty);

			// Relaxed: empty strings are translated into nulls...
			//try { ElementAlias.ValidateElement("  "); Assert.Fail(); }
			//catch (EmptyException) { }
			ElementAlias.ValidateElement("  ");

			try { ElementAlias.ValidateAlias(null); Assert.Fail(); }
			catch (ArgumentNullException) { }

			try { ElementAlias.ValidateAlias(string.Empty); Assert.Fail(); }
			catch (EmptyException) { }

			try { ElementAlias.ValidateAlias("  "); Assert.Fail(); }
			catch (EmptyException) { }
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Cloning()
		{
			Action<object> del = (source) =>
			{
				ConsoleEx.WriteLine("\n- Source: {0}", source);

				var temp = ((ICloneable)source).Clone();
				ConsoleEx.WriteLine("- Cloned: {0}", temp);

				Assert.IsTrue(source.IsEquivalentTo(temp),
					"Source '{0}' and cloned '{1}' are not equivalent."
					.FormatWith(source, temp));
			};

			var obj = new Concrete.ElementAlias() { Element = "Employees", Alias = "Emp" };
			del(obj);
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Serialization()
		{
			Action<object, bool> del = (source, binary) =>
			{
				var mode = binary ? "binary" : "text/soap";
				ConsoleEx.WriteLine("\n- Source ({0}): {1}", mode, source);

				var path = "c:\\temp\\data"; path += binary ? ".bin" : ".xml";
				path.PathSerialize(source, binary);

				var temp = path.PathDeserialize(binary);
				ConsoleEx.WriteLine("- Created ({0}): {1}", mode, temp);

				var result = source.IsEquivalentTo(temp);
				Assert.IsTrue(result,
					"With mode '{0}' source '{1}' and deserialized '{2}' are not equivalent."
					.FormatWith(mode, source.Sketch(), temp.Sketch()));
			};

			var obj = new Concrete.ElementAlias() { Element = "Employees", Alias = "Emp" };
			del(obj, true);
			del(obj, false);
		}
	}

	// ==================================================== 
	[TestClass]
	public class Test_ElementAliasCollection
	{
		public IElementAliasCollection CreateInstance()
		{
			var obj = new Concrete.ElementAliasCollection();

			obj.Add(new Concrete.ElementAlias() { Element = "Employees", Alias = "Emp" });
			obj.AddCreate("Countries", "Ctry");
			obj.AddCreate("Default");

			return obj;
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Creation()
		{
			var obj = CreateInstance();
			Assert.AreEqual(3, obj.Count);
			Assert.AreEqual("Employees", obj[0].Element); Assert.AreEqual("Emp", obj[0].Alias);
			Assert.AreEqual("Countries", obj[1].Element); Assert.AreEqual("Ctry", obj[1].Alias);
			Assert.AreEqual(null, obj[2].Element); Assert.AreEqual("Default", obj[2].Alias);
		}

		//[OnlyThisTest]
		[TestMethod]
		public void NotOrphan_Invariants()
		{
			var obj = CreateInstance();

			try { obj[0].Element = "whatever"; Assert.Fail(); }
			catch (NotOrphanException) { }

			try { obj[0].Alias = "whatever"; Assert.Fail(); }
			catch (NotOrphanException) { }
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Add_Empty_Member()
		{
			var obj = CreateInstance();
			var member = ((Concrete.ElementAliasCollection)obj).CreateOrphanMember();

			try { obj.Add(member); Assert.Fail(); }
			catch (ArgumentNullException) { }
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Add_NotOrphan_Member()
		{
			var obj = CreateInstance();
			var member = obj[0];

			try { var other = CreateInstance(); other.Add(member); Assert.Fail(); }
			catch (NotOrphanException) { }
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Add_Duplicate_Element()
		{
			var obj = CreateInstance();
			var member = ((Concrete.ElementAliasCollection)obj).CreateOrphanMember();
			member.Element = obj[0].Element;
			member.Alias = "whatever";

			obj.Add(member);
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Add_Duplicate_Alias()
		{
			var obj = CreateInstance();
			try { obj.AddCreate(obj[0].Alias); Assert.Fail(); }
			catch (DuplicateException) { }
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Set_Owner()
		{
			var obj = CreateInstance();
			var count = obj.Count;

			var member = ((Concrete.ElementAliasCollection)obj).CreateOrphanMember(); member.Alias = "whatever";
			member.Owner = obj;
			Assert.AreEqual(count + 1, obj.Count);
			Assert.IsTrue(object.ReferenceEquals(obj, member.Owner));
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Remove_Member()
		{
			var obj = CreateInstance();
			var count = obj.Count;
			var member = obj[0];

			var r = obj.Remove(member);
			Assert.IsTrue(r);
			Assert.AreEqual(count - 1, obj.Count);
			Assert.IsNull(member.Owner);
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Remove_NotOwned_Member()
		{
			var obj = CreateInstance();
			var count = obj.Count;
			var member = obj[0];

			var other = CreateInstance();
			var r = other.Remove(member);
			Assert.IsFalse(r);
			Assert.AreEqual(count, obj.Count);
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Erase_Owner()
		{
			var obj = CreateInstance();
			var count = obj.Count;
			var member = obj[0];

			member.Owner = null;
			Assert.AreEqual(null, member.Owner);
			Assert.AreEqual(count - 1, obj.Count);
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Dispose_Twice()
		{
			var obj = CreateInstance();
			var count = obj.Count;
			var member = obj[0];

			obj.Dispose();
			Assert.IsTrue(obj.IsDisposed);
			Assert.IsTrue(obj.Count == 0);
			Assert.IsTrue(member.IsDisposed);

			obj.Dispose();
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Dispose_Child()
		{
			var obj = CreateInstance();
			var count = obj.Count;
			var member = obj[0];

			member.Dispose();
			Assert.IsTrue(member.IsDisposed);
			Assert.IsNull(member.Owner);
			Assert.AreEqual(count - 1, obj.Count);
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Cloning()
		{
			Action<object> del = (source) =>
			{
				ConsoleEx.WriteLine("\n- Source: {0}", source);

				var temp = ((ICloneable)source).Clone();
				ConsoleEx.WriteLine("- Cloned: {0}", temp);

				Assert.IsTrue(source.IsEquivalentTo(temp),
					"Source '{0}' and cloned '{1}' are not equivalent."
					.FormatWith(source, temp));
			};

			var obj = CreateInstance();
			del(obj);
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Serialization()
		{
			Action<object, bool> del = (source, binary) =>
			{
				var mode = binary ? "binary" : "text/soap";
				ConsoleEx.WriteLine("\n- Source ({0}): {1}", mode, source);

				var path = "c:\\temp\\data"; path += binary ? ".bin" : ".xml";
				path.PathSerialize(source, binary);

				var temp = path.PathDeserialize(binary);
				ConsoleEx.WriteLine("- Created ({0}): {1}", mode, temp);

				var result = source.IsEquivalentTo(temp);
				Assert.IsTrue(result,
					"With mode '{0}' source '{1}' and deserialized '{2}' are not equivalent."
					.FormatWith(mode, source.Sketch(), temp.Sketch()));
			};

			var obj = CreateInstance();
			del(obj, true);
			del(obj, false);
		}
	}
}
