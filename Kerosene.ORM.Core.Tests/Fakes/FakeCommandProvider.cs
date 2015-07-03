using Kerosene.Tools;
using System;
using System.Linq;

namespace Kerosene.ORM.Core.Tests
{
	// ==================================================== 
	public class FakeCommandProvider : ICoreCommandProvider
	{
		public IQueryCommand TheCommand { get; private set; }

		public FakeCommandProvider(IDataLink link) { TheCommand = link.Query(); }

		public ICommand GenerateCoreCommand() { return TheCommand.Clone(); }
	}
}
