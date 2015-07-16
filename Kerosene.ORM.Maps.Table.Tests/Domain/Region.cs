using Kerosene.Tools;
using System;
using System.Linq;
using System.Text;

namespace Kerosene.ORM.Maps.Table.Tests
{
	// ==================================================== 
	public class Region
	{
		string _Id = null;
		string _Name = null;
		string _ParentId = null;

		object RowVersion = null;
		public string Id { get { return _Id; } set { _Id = value; } }
		public string Name { get { return _Name; } set { _Name = value; } }
		public string ParentId { get { return _ParentId; } set { _ParentId = value; } }

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder("{");
			sb.AppendFormat("Id={0}", Id ?? string.Empty);
			if (Name != null) sb.AppendFormat(", Name={0}", Name);
			if (ParentId != null) sb.AppendFormat(", ParentId={0}", ParentId);
			if (RowVersion != null) sb.AppendFormat(", RowVersion={0}", RowVersion.Sketch());
			sb.Append("}"); return sb.ToString();
		}
	}
}
