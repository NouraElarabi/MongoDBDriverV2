namespace Tests.Updates
{
	using System;
	using MongoDB.Bson;
    using MongoDB.Bson.Serialization;
    using MongoDB.Driver;
	using MongoDB.Driver.Builders;
	using MongoDB.Driver.Wrappers;
	using NUnit.Framework;
	using RealEstate.Rentals;
	using Serialization;

	[TestFixture]
	public class UnderstandingUpdateDocuments : DemoTests
	{
		[Test]
		public void UpdateStatic()
		{
			var update = Update.Set("Age", 54);
			Console.WriteLine(update);
		}

		[Test]
		public void UpdateBuilder()
		{
			var update = new UpdateBuilder().Set("Age", 54);
			Console.WriteLine(update);
		}

		public class Person
		{
			public int Age { get; set; }
		}

		[Test]
		public void TypedUpdateStatic()
		{
			var update = Update<Person>.Set(p => p.Age, 54);
			Console.WriteLine(update);
		}

		[Test]
		public void TypedUpdateBuilder()
		{
            //var update = new UpdateBuilder<Person>().Set(p => p.Age, 54);
            var update = Builders<Person>.Update.Set(p => p.Age, 54);
            Console.WriteLine(update.Render(BsonSerializer.SerializerRegistry.GetSerializer<Person>(), BsonSerializer.SerializerRegistry));
		}

		[Test]
		public void UpdateDocumentType()
		{
			var update = new UpdateDocument
			{
				{
					"$set", new BsonDocument
					{
						{"Age", 54}
					}
				}
			};
			Console.WriteLine(update);
		}

		[Test]
		public void UpdateWrapperType()
		{
			var document = new BsonDocument
			{
				{
					"$set", new BsonDocument
					{
						{"Age", 54}
					}
				}
			};
			var update = UpdateWrapper.Create(document);
			Console.WriteLine(update.ToBsonDocument());
		}

		[Test]
		public void ModificationUpdateOfRentalAdjustPrice()
		{
			var rental = new Rental {Price = 100};
			var adjustPrice = new AdjustPrice {NewPrice = 200, Reason = "Charge more!!!"};
			var adjustment = new PriceAdjustment(adjustPrice, rental.Price);

			var modificationUpdate = Update<Rental>
				.Push(r => r.Adjustments, adjustment)
				.Set(r => r.Price, adjustPrice.NewPrice);

			Console.WriteLine(modificationUpdate);
		}
	}
}