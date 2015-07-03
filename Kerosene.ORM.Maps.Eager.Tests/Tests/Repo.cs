using Kerosene.ORM.ExampleDB;
using Kerosene.ORM.Maps;
using Kerosene.Tools;
using System;
using System.Linq;
using System.Text;

namespace Kerosene.ORM.Maps.Eager.Tests
{
	// ==================================================== 
	/// <summary>
	/// Management of repositories for testing purposes.
	/// </summary>
	public static class Repo
	{
		/// <summary>
		/// Creates a new repository associated with the test database connection.
		/// </summary>
		public static IDataRepository Create()
		{
			var repo = DataRepository.Create(DB.Link);

			new RegionMap(repo);
			new CountryMap(repo);
			new EmployeeMap(repo);
			new TalentMap(repo);
			new EmployeeTalentMap(repo);

			return repo;
		}

		/// <summary>
		/// Gets a shared repository instance fort testing purposes.
		/// </summary>
		public static IDataRepository Instance
		{
			get
			{
				if (_Instance == null || _Instance.IsDisposed) _Instance = Create();
				return _Instance;
			}
		}
		static IDataRepository _Instance = null;

		/// <summary>
		/// Generates a string valid to be printed in the console.
		/// </summary>
		public static string ToConsoleString(this IDataMap map, int spaces = 0)
		{
			var header = spaces <= 0 ? string.Empty : new string(' ', spaces);
			var sb = new StringBuilder();

			sb.AppendFormat("{0}", map == null ? "<null>" : map.ToString());
			if (map != null && !map.IsDisposed)
			{
				if (map.IsValidated) sb.AppendFormat("\n{0}- Validated: true", header);
				if (map.VersionColumn.Name != null) sb.AppendFormat("\n{0}- Version Column: {1}", header, map.VersionColumn);
				if (map.Columns.Count != 0) sb.AppendFormat("\n{0}- Columns: {1}", header, map.Columns);
				foreach (var member in map.Members) sb.AppendFormat("\n{0}- Member: {1}", header, member);
			}
			return sb.ToString();
		}

		/// <summary>
		/// Generates a string valid to be printed in the console.
		/// </summary>
		public static string ToConsoleString(this IDataRepository repo)
		{
			var sb = new StringBuilder();
			var num = DebugEx.IndentSize; if (num < 2) num = 2;
			var header = new string(' ', num);

			sb.AppendFormat("{0}", repo == null ? "<null>" : repo.ToString());
			if (repo != null && !repo.IsDisposed)
			{
				foreach (var map in repo.Maps)
					sb.AppendFormat("\n\n{0}- Map: {1}", header, map.ToConsoleString(num * 2));
			}
			return sb.ToString();
		}
	}
}
