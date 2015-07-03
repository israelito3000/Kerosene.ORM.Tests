using Kerosene.Tools;
using System;
using System.Linq;
using System.Text;

namespace Kerosene.ORM.Maps.Lazy.Tests
{
	// ==================================================== 
	public class EmployeeTalent
	{
		Employee _Employee = null;
		Talent _Talent = null;

		object RowVersion = null;
		virtual public Employee Employee { get { return _Employee; } set { _Employee = value; } }
		virtual public Talent Talent { get { return _Talent; } set { _Talent = value; } }

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder("{");
			sb.AppendFormat("Employee={0}, Talent={1}",
				Employee == null ? string.Empty : (Employee.Id ?? string.Empty),
				Talent == null ? string.Empty : (Talent.Id ?? string.Empty));
			if (RowVersion != null) sb.AppendFormat(", RowVersion={0}", RowVersion.Sketch());
			sb.Append("}"); return sb.ToString();
		}
	}
}
