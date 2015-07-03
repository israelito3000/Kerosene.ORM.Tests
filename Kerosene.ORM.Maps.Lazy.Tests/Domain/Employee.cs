using Kerosene.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kerosene.ORM.Maps.Lazy.Tests
{
	// ==================================================== 
	public class EmployeeName // This is just to test multipart properties...
	{
		string _First = null;
		string _Last = null;

		public string First { get { return _First; } set { _First = value; } }
		public string Last { get { return _Last; } set { _Last = value; } }
	}

	// ==================================================== 
	public class Employee
	{
		string _Id = null;
		EmployeeName _Name = new EmployeeName();
		CalendarDate _BirthDate = null;
		bool? _Active = null;
		Employee _Manager = null;
		List<Employee> _Employees = new List<Employee>();
		Country _Country = null;
		CalendarDate _JoinDate = null;
		ClockTime _StartTime = null;
		byte[] _Photo = null;
		string _FullName = null;
		List<EmployeeTalent> _EmployeeTalents = new List<EmployeeTalent>();

		object RowVersion = null;
		public string Id { get { return _Id; } set { _Id = value; } }
		public EmployeeName Name { get { return _Name; } }
		public CalendarDate BirthDate { get { return _BirthDate; } set { _BirthDate = value; } }
		public bool? Active { get { return _Active; } set { _Active = value; } }
		virtual public Employee Manager { get { return _Manager; } set { _Manager = value; } }
		virtual public List<Employee> Employees { get { return _Employees; } }
		virtual public Country Country { get { return _Country; } set { _Country = value; } }
		public CalendarDate JoinDate { get { return _JoinDate; } set { _JoinDate = value; } }
		public ClockTime StartTime { get { return _StartTime; } set { _StartTime = value; } }
		public byte[] Photo { get { return _Photo; } set { _Photo = value; } }
		public string FullName { get { return _FullName; } set { _FullName = value; } }
		virtual public List<EmployeeTalent> EmployeeTalents { get { return _EmployeeTalents; } }

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder("{");
			sb.AppendFormat("Id={0}", Id ?? string.Empty);
			if (Name.First != null) sb.AppendFormat(", Name.First={0}", Name.First);
			if (Name.Last != null) sb.AppendFormat(", Name.Last={0}", Name.Last);
			if (BirthDate != null) sb.AppendFormat(", BirthDate={0}", BirthDate);
			if (Active != null) sb.AppendFormat(", Active={0}", Active);
			if (Manager != null) sb.AppendFormat(", Manager={0}", Manager.Id);
			if (Employees.Count != 0) sb.AppendFormat(", Employees={0}", Employees.Select(x => x.Id).Sketch());
			if (Country != null) sb.AppendFormat(", Country={0}", Country.Id);
			if (JoinDate != null) sb.AppendFormat(", JoinDate={0}", JoinDate);
			if (StartTime != null) sb.AppendFormat(", StartTime={0}", StartTime);
			if (Photo != null) sb.AppendFormat(", Photo={0}", Photo.Sketch());
			if (FullName != null) sb.AppendFormat(", FullName={0}", FullName);
			if (EmployeeTalents.Count != 0) sb.AppendFormat(", Talents={0}", EmployeeTalents.Select(x => x.Talent.Id).Sketch());
			if (RowVersion != null) sb.AppendFormat(", RowVersion={0}", RowVersion.Sketch());
			sb.Append("}"); return sb.ToString();
		}
	}
}
