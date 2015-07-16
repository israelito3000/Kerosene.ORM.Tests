using Kerosene.Tools;
using System;
using System.Linq;
using System.Text;

namespace Kerosene.ORM.DataDB
{
	// ==================================================== 
	public class Country
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public string RegionId { get; set; }
		public object RowVersion { get; set; }

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder("{");
			sb.AppendFormat("Id={0}", Id ?? string.Empty);
			if (Name != null) sb.AppendFormat(", Name={0}", Name);
			if (RegionId != null) sb.AppendFormat(", RegionId={0}", RegionId);
			if (RowVersion != null) sb.AppendFormat(", RowVersion={0}", RowVersion.Sketch());
			sb.Append("}"); return sb.ToString();
		}
	}
}
