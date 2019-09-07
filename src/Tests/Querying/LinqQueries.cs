namespace Tests.Querying
{
	using System;
	using System.Linq;
	using MongoDB.Driver.Builders;
	using MongoDB.Driver.Linq;
	using NUnit.Framework;
	using RealEstate.App_Start;
	using RealEstate.Rentals;
	using Serialization;

	public class LinqQueries : DemoTests
	{
		[Test]
		public void Inject()
		{
			var query = Query<Rental>.LTE(r => r.Price, 500);

			var queryable = new RealEstateContext()
				.Rentals.AsQueryable()
				.Where(r => query.Inject());

			var translated = MongoQueryTranslator.Translate(queryable)
				as SelectQuery;
			Console.WriteLine(translated.BuildQuery());
		}
	}
}