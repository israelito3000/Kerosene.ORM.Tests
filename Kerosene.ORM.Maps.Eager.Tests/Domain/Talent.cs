using Kerosene.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kerosene.ORM.Maps.Eager.Tests
{
	// ==================================================== 
	public class Talent
	{
		string _Id = null;
		string _Description = null;
		List<Employee> _Employees = new List<Employee>();

		object RowVersion = null;
		public string Id { get { return _Id; } set { _Id = value; } }
		public string Description { get { return _Description; } set { _Description = value; } }
		public List<Employee> Employees { get { return _Employees; } }

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder("{");
			sb.AppendFormat("Id={0}", Id ?? string.Empty);
			if (Description != null) sb.AppendFormat(", Description={0}", Description);
			if (Employees.Count != 0) sb.AppendFormat(", Employees={0}", Employees.Select(x => x.Id).Sketch());
			if (RowVersion != null) sb.AppendFormat(", RowVersion={0}", RowVersion.Sketch());
			sb.Append("}"); return sb.ToString();
		}
	}
}
