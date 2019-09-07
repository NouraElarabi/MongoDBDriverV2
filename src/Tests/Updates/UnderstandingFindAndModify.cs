namespace Tests.Updates
{
	using System;
	using MongoDB.Driver;
	using MongoDB.Driver.Builders;
	using NUnit.Framework;
	using RealEstate.App_Start;
	using Serialization;

	[TestFixture]
	public class UnderstandingFindAndModify : DemoTests
	{
		[Test]
		public void CreatingCounters()
		{
			var context = new RealEstateContext();

			var counters = context.Database.GetCollection("counters");
			counters.Drop();

			var arguments = new FindAndModifyArgs
			{
				Query = Query.EQ("_id", "apples"),
				Update = Update.Inc("counter", 1),
				SortBy = SortBy.Null,
				VersionReturned = FindAndModifyDocumentVersion.Modified,
				Upsert = true
			};
			var apples = counters.FindAndModify(arguments);
			Console.WriteLine("FindAndModify Initialize: " + apples.ModifiedDocument);

			arguments.VersionReturned = FindAndModifyDocumentVersion.Original;
			apples = counters.FindAndModify(arguments);
			Console.WriteLine("FindAndModify (before): " + apples.ModifiedDocument);

			arguments.VersionReturned = FindAndModifyDocumentVersion.Modified;
			apples = counters.FindAndModify(arguments);
			Console.WriteLine("FindAndModify (after): " + apples.ModifiedDocument);
		}
	}
}