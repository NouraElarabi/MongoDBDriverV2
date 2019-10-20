namespace RealEstate.Rentals
{
	using System.Collections;
	using System.Collections.Generic;
    using System.Linq;
    using MongoDB.Bson;
	using MongoDB.Driver;

	public class QueryPriceDistribution
	{
		public IEnumerable<BsonDocument> Run(MongoCollection<Rental> rentals)
		{
			var priceRange = new BsonDocument(
				"$subtract",
				new BsonArray
				{
					"$Price",
					new BsonDocument(
						"$mod",
						new BsonArray{"$Price", 500})
				});
			var grouping = new BsonDocument(
				"$group", 
				new BsonDocument
				{
					{"_id", priceRange},
					{"count", new BsonDocument("$sum", 1)}
				});
			var sort = new BsonDocument(
				"$sort", 
				new BsonDocument("_id", 1)
				);
			var args = new AggregateArgs
			{
				Pipeline = new[] {grouping, sort}
			};
			return rentals.Aggregate(args);
		}

        public IEnumerable RunAggregationFluent(IMongoCollection<Rental> rentals)
        {
            //var distributions = rentals.Aggregate()
            //    .Project(r => new { r.Price, PriceRange = (double)r.Price - ((double)r.Price % 500) })
            //    .Group(r=> r.PriceRange , g => new { GroupRangePrice = g.Key , Count = g.Count() })
            //    .SortBy(r => r.GroupRangePrice)
            //    .ToList();

            var distributions = rentals.AsQueryable()
               .Select(r => new { PriceRange = (double)r.Price - ((double)r.Price % 500) })
               .GroupBy(r => r.PriceRange)
               .Select(g => new { GroupRangePrice = g.Key, Count = g.Count() })
               .OrderBy(r => r.GroupRangePrice)
               .ToList();

            return distributions;
        }

    }
}