using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kerosene.Tools;
using System;
using System.Linq;

namespace Kerosene.ORM.Core.Tests
{
	// ==================================================== 
	public class FakeTransaction : Concrete.NestableTransaction
	{
		public FakeTransaction(IDataLink link, NestableTransactionMode mode) : base(link, mode) { }

		public int TheLevel { get; set; }

		public override int Level { get { return TheLevel; } }

		public override void Start() { TheLevel++; }

		public override void Commit() { if ((--TheLevel) < 0) TheLevel = 0; }

		public override void Abort() { TheLevel = 0; }
	}
}
