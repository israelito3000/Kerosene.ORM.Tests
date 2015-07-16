using Kerosene.Tools;
using System;
using System.Linq;
using System.Text;

namespace Kerosene.ORM.Maps.Table.Tests
{
	// ==================================================== 
	public class Employee
	{
		string _Id = null;
		string _FirstName = null;
		string _LastName = null;
		CalendarDate _BirthDate = null;
		bool? _Active = null;
		string _ManagerId = null;
		string _CountryId = null;
		CalendarDate _JoinDate = null;
		ClockTime _StartTime = null;
		byte[] _Photo = null;
		string _FullName = null;

		object RowVersion = null;
		public string Id { get { return _Id; } set { _Id = value; } }
		public string FirstName { get { return _FirstName; } set { _FirstName = value; } }
		public string LastName { get { return _LastName; } set { _LastName = value; } }
		public CalendarDate BirthDate { get { return _BirthDate; } set { _BirthDate = value; } }
		public bool? Active { get { return _Active; } set { _Active = value; } }
		public string ManagerId { get { return _ManagerId; } set { _ManagerId = value; } }
		public string CountryId { get { return _CountryId; } set { _CountryId = value; } }
		public CalendarDate JoinDate { get { return _JoinDate; } set { _JoinDate = value; } }
		public ClockTime StartTime { get { return _StartTime; } set { _StartTime = value; } }
		public byte[] Photo { get { return _Photo; } set { _Photo = value; } }
		public string FullName { get { return _FullName; } set { _FullName = value; } }

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder("{");
			sb.AppendFormat("Id={0}", Id ?? string.Empty);
			if (FirstName != null) sb.AppendFormat(", FirstName={0}", FirstName);
			if (LastName != null) sb.AppendFormat(", LastName={0}", LastName);
			if (BirthDate != null) sb.AppendFormat(", BirthDate={0}", BirthDate);
			if (Active != null) sb.AppendFormat(", Active={0}", Active);
			if (ManagerId != null) sb.AppendFormat(", ManagerId={0}", ManagerId);
			if (CountryId != null) sb.AppendFormat(", CountryId={0}", CountryId);
			if (JoinDate != null) sb.AppendFormat(", JoinDate={0}", JoinDate);
			if (StartTime != null) sb.AppendFormat(", StartTime={0}", StartTime);
			if (Photo != null) sb.AppendFormat(", Photo={0}", Photo.Sketch());
			if (FullName != null) sb.AppendFormat(", FullName={0}", FullName);
			if (RowVersion != null) sb.AppendFormat(", RowVersion={0}", RowVersion.Sketch());
			sb.Append("}"); return sb.ToString();
		}
	}
}
