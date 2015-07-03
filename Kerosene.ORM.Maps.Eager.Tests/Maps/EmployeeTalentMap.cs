using Kerosene.ORM.Maps;
using Kerosene.Tools;
using System;
using System.Linq;

namespace Kerosene.ORM.Maps.Eager.Tests
{
	// ==================================================== 
	public class EmployeeTalentMap : Concrete.DataMap<EmployeeTalent>
	{
		public EmployeeTalentMap(IDataRepository repo) : this((Concrete.DataRepository)repo) { }

		public EmployeeTalentMap(Concrete.DataRepository repo)
			: base(repo, x => x.EmployeeTalents)
		{
			Members.Add(x => x.Employee)
				.WithColumn(x => x.EmployeeId, col =>
				{
					col.OnWriteRecord(obj => { return obj.Employee == null ? null : obj.Employee.Id; });
				})
				.OnComplete((rec, obj) =>
				{
					obj.Employee = Repository.FindNow<Employee>(x => x.Id == rec["EmployeeId"]);
				})
				.SetDependencyMode(MemberDependencyMode.Parent);

			Members.Add(x => x.Talent)
				.WithColumn(x => x.TalentId, col =>
				{
					col.OnWriteRecord(obj => { return obj.Talent == null ? null : obj.Talent.Id; });
				})
				.OnComplete((rec, obj) =>
				{
					obj.Talent = Repository.FindNow<Talent>(x => x.Id == rec["TalentId"]);
				})
				.SetDependencyMode(MemberDependencyMode.Parent);

			VersionColumn.SetName(x => x.RowVersion);
		}
	}
}
