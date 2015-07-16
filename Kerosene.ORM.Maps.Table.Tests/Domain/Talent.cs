using Kerosene.Tools;
using System;
using System.Linq;
using System.Text;

namespace Kerosene.ORM.Maps.Table.Tests
{
	// ==================================================== 
	public class Talent
	{
		string _Id = null;
		string _Description = null;

		object RowVersion = null;
		public string Id { get { return _Id; } set { _Id = value; } }
		public string Description { get { return _Description; } set { _Description = value; } }

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
