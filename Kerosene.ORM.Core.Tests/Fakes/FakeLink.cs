using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kerosene.Tools;
using System;
using System.Linq;

namespace Kerosene.ORM.Core.Tests
{
	// ==================================================== 
	public class FakeLink : Concrete.DataLink
	{
		bool _IsOpen = false;

		public FakeLink(IDataEngine engine, NestableTransactionMode mode) : base(engine, mode) { }

		protected override INestableTransaction CreateTransaction() { return new FakeTransaction(this, DefaultTransactionMode); }

		public IDataLink Clone() { throw new NotSupportedException(); }

		public IDataLink Clone(IDataEngine engine) { throw new NotSupportedException(); }

		public override void Open() { _IsOpen = true; }

		public override void Close() { _IsOpen = false; }

		public override bool IsOpen { get { return _IsOpen; } }
	}
}
