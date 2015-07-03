using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kerosene.Tools;
using System;
using System.Linq;

namespace Kerosene.ORM.Core.Tests
{
	// ==================================================== 
	[TestClass]
	public class Test_Record
	{
		public ISchema CreateSchema()
		{
			var schema = new Concrete.Schema();
			schema.Aliases.AddCreate("Employees", "Emp");
			schema.Aliases.AddCreate("Countries", "Ctry");

			schema.Add(new Concrete.SchemaEntry() { TableName = "Emp", ColumnName = "Id", IsPrimaryKeyColumn = true });
			schema.AddCreate("Emp", "FirstName");
			schema.AddCreate("LastName");
			schema.AddCreate("Ctry", "Id");

			return schema;
		}

		public IRecord CreateInstance()
		{
			var schema = CreateSchema();
			var record = new Concrete.Record(schema);

			record["Employees", "Id"] = "007";
			record["Emp", "FirstName"] = "James";

			record[x => x.LastName] = "Bond";

			dynamic r = record; r.Ctry.Id = "uk";

			return record;
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Creation()
		{
			var obj = CreateInstance();
			Assert.AreEqual(4, obj.Count);

			var str = obj.ToString();
			Assert.AreEqual(
				"[Employees.Id = '007', Employees.FirstName = 'James', .LastName = 'Bond', Countries.Id = 'uk']",
				str);

			obj.Schema = null;
			str = obj.ToString();
			Assert.AreEqual(
				"[#0 = '007', #1 = 'James', #2 = 'Bond', #3 = 'uk']",
				str);
		}

		//[OnlyThisTest]
		[TestMethod]
		public void ReAssig_Schema()
		{
			var obj = CreateInstance();

			var schema = CreateSchema();
			try { obj.Schema = schema; Assert.Fail(); }
			catch (InvalidOperationException) { }

			obj.Schema = null;
			obj.Schema = schema;
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Dispose_Twice()
		{
			var obj = CreateInstance();
			var count = obj.Count;
			var schema = obj.Schema;

			obj.Dispose();
			Assert.IsTrue(obj.IsDisposed);
			Assert.IsTrue(obj.Count == 0);
			Assert.IsFalse(schema.IsDisposed);

			obj.Dispose();
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Dispose_RecordAndSchema()
		{
			var obj = CreateInstance();
			var count = obj.Count;
			var schema = obj.Schema;

			obj.Dispose(disposeSchema: true);
			Assert.IsTrue(obj.IsDisposed);
			Assert.IsTrue(obj.Count == 0);
			Assert.IsNull(obj.Schema);
			Assert.IsTrue(schema.IsDisposed);
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Equivalence_OnlyValues()
		{
			var schema1 = new Concrete.Schema();
			schema1.AddCreate("Alpha");
			schema1.AddCreate("Beta");

			var record1 = new Concrete.Record(schema1);
			record1[0] = "Cero";
			record1[1] = "One";

			var schema2 = new Concrete.Schema();
			schema2.AddCreate("Delta");
			schema2.AddCreate("Gamma");

			var record2 = new Concrete.Record(schema2);
			record2[0] = "Cero";
			record2[1] = "One";

			var res = record1.EquivalentTo(record2);
			Assert.IsFalse(res);

			res = record1.EquivalentTo(record2, onlyValues: true);
			Assert.IsTrue(res);
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Cloning()
		{
			Action<IRecord, bool> del = (source, withSchema) =>
			{
				ConsoleEx.WriteLine("\n- Source: {0}", source);

				var with = withSchema ? "with schema" : "with no schema";
				var temp = source.Clone(cloneSchema: withSchema);
				ConsoleEx.WriteLine("- Cloned {0}: {1}", with, temp);

				Assert.IsTrue(source.IsEquivalentTo(temp),
					"Source '{0}' and cloned '{1}' are not equivalent."
					.FormatWith(source, temp));
			};

			var obj = CreateInstance();
			del(obj, true);
			del(obj, false);
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Serialization()
		{
			Action<IRecord, bool, bool> del = (source, binary, withSchema) =>
			{
				ConsoleEx.WriteLine("\n- Source: {0}", source);

				var mode = binary ? "binary" : "text/soap";
				var with = withSchema ? "with schema" : "with no schema";
				var path = "c:\\temp\\data"; path += binary ? ".bin" : ".xml";

				if (!withSchema) source.SerializeSchema = false;
				path.PathSerialize(source, binary);

				var temp = path.PathDeserialize(binary);
				ConsoleEx.WriteLine("- Created ({0}, {1}): {2}", mode, with, temp);

				bool r;

				if (!withSchema)
				{
					r = source.EquivalentTo((IRecord)temp, onlyValues: true);
					Assert.IsTrue(r,
						"With mode '{0}' source '{1}' and deserialized '{2}' are not equivalent."
						.FormatWith(mode, source.Sketch(), temp.Sketch()));

					((IRecord)temp).Schema = source.Schema;
				}
				r = source.IsEquivalentTo(temp);
				Assert.IsTrue(r,
					"With mode '{0}' source '{1}' and deserialized '{2}' are not equivalent."
					.FormatWith(mode, source.Sketch(), temp.Sketch()));
			};

			var obj = CreateInstance();
			del(obj, true, true); del(obj, true, false);
			del(obj, false, true); del(obj, false, false);
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Sizer()
		{
			var obj = new Concrete.Record(1); obj[0] = 50;
			var size = obj.Size();
			Assert.IsTrue(size == sizeof(int));

			var str = "James Bond";
			obj = new Concrete.Record(2); obj[0] = str; obj[1] = 50;
			size = obj.Size();
			var temp = (str.Length * sizeof(char)) + sizeof(int);
			Assert.IsTrue(size == temp);
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Record_Builder()
		{
			var builder = new RecordBuilder();
			builder.Aliases.AddCreate("Employees", "Emp");
			builder.Aliases.AddCreate("Countries", "Ctry");

			dynamic b = builder;
			b.Emp.Id = "007";
			b.Emp.FirstName = "James";
			b.LastName = "Bond";
			b.Ctry.Id = "uk";

			IRecord record = builder.Create();
			var str = record.ToString();
			Assert.AreEqual(
				"[Employees.Id = '007', Employees.FirstName = 'James', .LastName = 'Bond', Countries.Id = 'uk']",
				str);
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Changes_NoChanges()
		{
			var source = CreateInstance();
			var target = CreateInstance();

			var r = source.Changes(target);
			Assert.IsNull(r);
		}

		//[OnlyThisTest]
		[TestMethod]
		public void Changes_FromSource_Only()
		{
			var source = CreateInstance();
			var target = CreateInstance();

			target["Emp", "Id"] = "008";
			var r = source.Changes(target);

			Assert.IsNotNull(r);
			Assert.AreEqual(1, r.Count);
			Assert.AreEqual("007", r[0]);
			Assert.IsTrue(source.Schema[0].EquivalentTo(r.Schema[0]));
		}
	}
}
