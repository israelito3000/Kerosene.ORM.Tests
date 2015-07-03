using Kerosene.Tools;
using System;
using System.Linq;
using System.Text;

namespace Kerosene.ORM.ExampleDB
{
	// ==================================================== 
	public class Region
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public string ParentId { get; set; }
		public object RowVersion { get; set; }

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
