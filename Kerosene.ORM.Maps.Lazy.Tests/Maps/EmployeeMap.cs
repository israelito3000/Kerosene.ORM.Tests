using Kerosene.ORM.Maps;
using Kerosene.Tools;
using System;
using System.Linq;

namespace Kerosene.ORM.Maps.Lazy.Tests
{
	// ==================================================== 
	public class EmployeeMap : Concrete.DataMap<Employee>
	{
		public EmployeeMap(IDataRepository repo) : this((Concrete.DataRepository)repo) { }

		public EmployeeMap(Concrete.DataRepository repo)
			: base(repo, x => x.Employees)
		{
			Members.Add(x => x.Name)
				.WithColumn(x => x.FirstName, col =>
				{
					col.SetElementName(x => x.Name.First);
				})
				.WithColumn(x => x.LastName, col =>
				{
					col.SetElementName(x => x.Name.Last);
				});

			Members.Add(x => x.Manager)
				.WithColumn(x => x.ManagerId, col =>
				{
					col.OnWriteRecord(obj => { return obj.Manager == null ? null : obj.Manager.Id; });
				})
				.OnComplete((rec, obj) =>
				{
					obj.Manager = Repository.FindNow<Employee>(x => x.Id == rec["ManagerId"]);
				})
				.SetDependencyMode(MemberDependencyMode.Parent);

			Members.Add(x => x.Country)
				.WithColumn(x => x.CountryId, col =>
				{
					col.OnWriteRecord(obj => { return obj.Country == null ? null : obj.Country.Id; });
				})
				.OnComplete((rec, obj) =>
				{
					obj.Country = Repository.FindNow<Country>(x => x.Id == rec["CountryId"]);
				})
				.SetDependencyMode(MemberDependencyMode.Parent);

			Members.Add(x => x.Employees)
				.OnComplete((rec, obj) =>
				{
					obj.Employees.Clear();
					obj.Employees.AddRange(Repository.Where<Employee>(x => x.ManagerId == obj.Id).ToList());
				})
				.SetDependencyMode(MemberDependencyMode.Child);

			Members.Add(x => x.EmployeeTalents)
				.OnComplete((rec, obj) =>
				{
					obj.EmployeeTalents.Clear();
					obj.EmployeeTalents.AddRange(Repository.Where<EmployeeTalent>(x => x.EmployeeId == obj.Id).ToList());
				})
				.SetDependencyMode(MemberDependencyMode.Child);

			VersionColumn.SetName(x => x.RowVersion);
		}
	}
}
