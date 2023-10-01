using System.Collections.Generic;
using SQLiteNetExtensions.Attributes;

namespace SQLiteNetExtensions.UnitTests.Models
{
	public class DummyClassD
	{
		[ForeignKey(typeof(DummyClassC))]
		public int ClassCKey { get; set; }

		[ManyToMany(typeof(IntermediateDummyADummyD))]
		public List<DummyClassA> ManyA { get; set; }
	}
}