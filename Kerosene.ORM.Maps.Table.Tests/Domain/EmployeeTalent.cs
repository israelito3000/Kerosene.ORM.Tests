using Kerosene.Tools;
using System;
using System.Linq;
using System.Text;

namespace Kerosene.ORM.Maps.Table.Tests
{
	// ==================================================== 
	public class EmployeeTalent
	{
		string _EmployeeId = null;
		string _TalentId = null;

		object RowVersion = null;
		public string EmployeeId { get { return _EmployeeId; } set { _EmployeeId = value; } }
		public string TalentId { get { return _TalentId; } set { _TalentId = value; } }

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
