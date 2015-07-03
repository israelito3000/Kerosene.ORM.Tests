using Kerosene.ORM.Maps;
using Kerosene.Tools;
using System;
using System.Linq;

namespace Kerosene.ORM.Maps.Eager.Tests
{
	// ==================================================== 
	public class TalentMap : Concrete.DataMap<Talent>
	{
		public TalentMap(IDataRepository repo) : this((Concrete.DataRepository)repo) { }

		public TalentMap(Concrete.DataRepository repo)
			: base(repo, x => x.Talents)
		{
			Members.Add(x => x.Employees)
				.OnComplete((rec, obj) =>
				{
					obj.Employees.Clear();

					obj.Employees.AddRange(
						Repository
						.Where<Employee>(x => x.Emp.Id == x.Temp.EmployeeId)
						.MasterAlias(x => x.Emp)
						.From(x =>
							x(Repository.Where<EmployeeTalent>(y => y.TalentId == obj.Id))
							.As(x.Temp))
						.ToList());
				})
				.SetDependencyMode(MemberDependencyMode.Parent);

			VersionColumn.SetName(x => x.RowVersion);
		}
	}
}
