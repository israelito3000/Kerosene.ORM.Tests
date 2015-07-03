using Kerosene.Tools;
using System;
using System.Linq;
using System.Text;

namespace Kerosene.ORM.ExampleDB
{
	// ==================================================== 
	public class EmployeeTalent
	{
		public string EmployeeId { get; set; }
		public string TalentId { get; set; }
		public object RowVersion { get; set; }

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder("{");
			sb.AppendFormat("Employee={0}, Talent={1}",
				EmployeeId ?? string.Empty,
				TalentId ?? string.Empty);
			if (RowVersion != null) sb.AppendFormat(", RowVersion={0}", RowVersion.Sketch());
			sb.Append("}"); return sb.ToString();
		}
	}
}
