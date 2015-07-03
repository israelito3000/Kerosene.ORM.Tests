using Kerosene.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kerosene.ORM.Maps.Lazy.Tests
{
	// ==================================================== 
	public class Region
	{
		string _Id = null;
		string _Name = null;
		Region _Parent = null;
		List<Region> _Childs = new List<Region>();
		List<Country> _Countries = new List<Country>();

		object RowVersion = null;
		public string Id { get { return _Id; } set { _Id = value; } }
		public string Name { get { return _Name; } set { _Name = value; } }
		virtual public Region Parent { get { return _Parent; } set { _Parent = value; } }
		virtual public List<Region> Childs { get { return _Childs; } }
		virtual public List<Country> Countries { get { return _Countries; } }

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder("{");
			sb.AppendFormat("Id={0}", Id ?? string.Empty);
			if (Name != null) sb.AppendFormat(", Name={0}", Name);
			if (Parent != null) sb.AppendFormat(", Parent={0}", Parent.Id);
			if (Childs.Count != 0) sb.AppendFormat(", Childs={0}", Childs.Select(x => x.Id).Sketch());
			if (Countries.Count != 0) sb.AppendFormat(", Countries={0}", Countries.Select(x => x.Id).Sketch());
			if (RowVersion != null) sb.AppendFormat(", RowVersion={0}", RowVersion.Sketch());
			sb.Append("}"); return sb.ToString();
		}
	}
}
