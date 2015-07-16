using Kerosene.Tools;
using System;
using System.Linq;
using System.Text;

namespace Kerosene.ORM.DataDB
{
	// ==================================================== 
	public class Talent
	{
		public string Id { get; set; }
		public string Description { get; set; }
		public object RowVersion { get; set; }

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder("{");
			sb.AppendFormat("Id={0}", Id ?? string.Empty);
			if (Description != null) sb.AppendFormat(", Description={0}", Description);
			if (RowVersion != null) sb.AppendFormat(", RowVersion={0}", RowVersion.Sketch());
			sb.Append("}"); return sb.ToString();
		}
	}
}
