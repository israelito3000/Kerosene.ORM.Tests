using Kerosene.Tools;
using System;
using System.Linq;
using System.Text;

namespace Kerosene.ORM.Maps.Table.Tests
{
	// ==================================================== 
	public class Country
	{
		string _Id = null;
		string _Name = null;
		string _RegionId = null;

		object RowVersion = null;
		public string Id { get { return _Id; } set { _Id = value; } }
		public string Name { get { return _Name; } set { _Name = value; } }
		public string RegionId { get { return _RegionId; } set { _RegionId = value; } }

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
