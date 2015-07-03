using Kerosene.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kerosene.ORM.Maps.Eager.Tests
{
	// ==================================================== 
	public class Country
	{
		string _Id = null;
		string _Name = null;
		Region _Region = null;
		List<Employee> _Employees = new List<Employee>();

		object RowVersion = null;
		public string Id { get { return _Id; } set { _Id = value; } }
		public string Name { get { return _Name; } set { _Name = value; } }
		public Region Region { get { return _Region; } set { _Region = value; } }
		public List<Employee> Employees { get { return _Employees; } }

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder("{");
			sb.AppendFormat("Id={0}", Id ?? string.Empty);
			if (Name != null) sb.AppendFormat(", Name={0}", Name);
			if (Region != null) sb.AppendFormat(", Region={0}", Region.Id);
			if (Employees.Count != 0) sb.AppendFormat(", Employees={0}", Employees.Select(x => x.Id).Sketch());
			if (RowVersion != null) sb.AppendFormat(", RowVersion={0}", RowVersion.Sketch());
			sb.Append("}"); return sb.ToString();
		}
	}
}
