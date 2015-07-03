using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kerosene.Tools;
using System;
using System.Linq;

namespace Kerosene.ORM.Core.Tests
{
	// ==================================================== 
	[TestClass]
	public class Test_SchemaEntry
	{
		//[OnlyThisTest]
		[TestMethod]
		public void Validation_Rules()
		{
			SchemaEntry.ValidateTable(null);

			// Relaxed: empty strings are translated into nulls...
			//try { SchemaEntry.ValidateTable(string.Empty); Assert.Fail(); }
			//catch (EmptyException) { }

			// Relaxed: empty strings are translated into nulls...
			//try { SchemaEntry.ValidateTable("  "); Assert.Fail(); }
			//catch (EmptyException) { }

			try { SchemaEntry.ValidateColumn(null); Assert.Fail(); }
			catch (ArgumentNullException) { }

			try { SchemaEntry.ValidateColumn(string.Empty); Assert.Fail(); }
			catch (EmptyException) { }

			try { SchemaEntry.ValidateColumn("  "); Assert.Fail(); }
			catch (EmptyException) { }
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Normalized_Name()
		{
			var str = SchemaEntry.NormalizedName(null, null);
			Assert.AreEqual(".", str);

			str = SchemaEntry.NormalizedName("Table", null);
			Assert.AreEqual("Table.", str);

			str = SchemaEntry.NormalizedName(null, "Column");
			Assert.AreEqual(".Column", str);
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

			var obj = new Concrete.SchemaEntry() { TableName = "MyTable", ColumnName = "MyColumn" };
			obj.Metadata["OtherMetadata"] = "whatever";
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

			var obj = new Concrete.SchemaEntry() { TableName = "MyTable", ColumnName = "MyColumn" };
			obj.Metadata["OtherMetadata"] = "whatever";

			del(obj, true);
			del(obj, false);
		}
	}

	// ==================================================== 
	[TestClass]
	public class Test_Schema
	{
		public ISchema CreateInstance()
		{
			var obj = new Concrete.Schema();

			obj.Aliases.AddCreate("Employees", "Emp");
			obj.Aliases.AddCreate("Countries", "Ctry");

			obj.Add(new Concrete.SchemaEntry() { TableName = "Emp", ColumnName = "Id", IsPrimaryKeyColumn = true });
			obj.AddCreate("Emp", "FirstName");
			obj.AddCreate("LastName");
			obj.AddCreate("Ctry", "Id");

			return obj;
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Creation()
		{
			var obj = CreateInstance();
			Assert.AreEqual(4, obj.Count);
			Assert.AreEqual("Employees", obj[0].TableName); Assert.AreEqual(obj[0].ColumnName, "Id"); Assert.IsTrue(obj[0].IsPrimaryKeyColumn);
			Assert.IsNull(obj[2].TableName);
		}

		//[OnlyThisTest]
		[TestMethod]
		public void NotOrphan_Invariants()
		{
			var obj = CreateInstance();

			try { obj[0].TableName = "whatever"; Assert.Fail(); }
			catch (NotOrphanException) { }

			try { obj[0].ColumnName = "whatever"; Assert.Fail(); }
			catch (NotOrphanException) { }

			try { obj[0].Metadata[obj[0].Tags.TableNameTag] = "AnotherTable"; Assert.Fail(); }
			catch (NotOrphanException) { }

			try { obj[0].Metadata[obj[0].Tags.ColumnNameTag] = "AnotherColumn"; Assert.Fail(); }
			catch (NotOrphanException) { }

			// All others are not invariant...
			obj[0].IsPrimaryKeyColumn = false;
			obj[0].Metadata["whatever"] = "other"; // assuming it is not Table or Column names...
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Add_EmptyMember()
		{
			var obj = CreateInstance();
			var member = ((Concrete.Schema)obj).CreateOrphanMember();

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
		public void Add_Invalid_Tables()
		{
			var obj = CreateInstance();

			var member = obj.AddCreate(null, "whatever");

			// Relaxed: empty strings are translated into nulls...
			//try { obj.AddCreate(string.Empty, "other"); Assert.Fail(); }
			//catch (EmptyException) { }
			member = obj.AddCreate(string.Empty, "other");
			Assert.IsNull(member.TableName);

			// Relaxed: empty strings are translated into nulls...
			//try { obj.AddCreate("   ", "other"); Assert.Fail(); }
			//catch (EmptyException) { }
			obj.AddCreate("   ", "another");
			Assert.IsNull(member.TableName);
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Add_Invalid_Columns()
		{
			var obj = CreateInstance();

			try { obj.AddCreate("whatever", null); Assert.Fail(); }
			catch (ArgumentNullException) { }

			try { obj.AddCreate("whatever", string.Empty); Assert.Fail(); }
			catch (EmptyException) { }

			try { obj.AddCreate("whatever", "   "); Assert.Fail(); }
			catch (EmptyException) { }
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Add_Duplicate_FullName()
		{
			var obj = CreateInstance();

			try { obj.AddCreate("Employees", "Id"); Assert.Fail(); }
			catch (DuplicateException) { }

			try { obj.AddCreate("EMPLOYEES", "ID"); Assert.Fail(); }
			catch (DuplicateException) { }

			try { obj.AddCreate("Emp", "Id"); Assert.Fail(); }
			catch (DuplicateException) { }

			try { obj.AddCreate("EMP", "ID"); Assert.Fail(); }
			catch (DuplicateException) { }
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Add_DuplicateColumn_In_DefaultTable()
		{
			var obj = CreateInstance();

			try { obj.AddCreate("other", "LastName"); Assert.Fail(); }
			catch (DuplicateException) { }

			try { obj.AddCreate("OTHER", "LASTNAME"); Assert.Fail(); }
			catch (DuplicateException) { }
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Add_DuplicateColumn_In_NonDefault_Table()
		{
			var obj = CreateInstance();

			try { obj.AddCreate(null, "Id"); Assert.Fail(); }
			catch (DuplicateException) { }

			try { obj.AddCreate(null, "ID"); Assert.Fail(); }
			catch (DuplicateException) { }
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Set_Owner()
		{
			var obj = CreateInstance();
			var count = obj.Count;

			var member = ((Concrete.Schema)obj).CreateOrphanMember(); member.TableName = "MyTable"; member.ColumnName = "MyColumn";
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
		public void Remove_NotOwne_dMember()
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
				ConsoleEx.WriteLine("- Cloned: {0}".FormatWith(temp));

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
