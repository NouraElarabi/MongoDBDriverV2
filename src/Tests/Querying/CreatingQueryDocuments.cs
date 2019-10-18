namespace Tests.Querying
{
	using System;
	using MongoDB.Bson;
    using MongoDB.Bson.Serialization;
    using MongoDB.Driver;
	using MongoDB.Driver.Builders;
	using MongoDB.Driver.Wrappers;
	using NUnit.Framework;
	using Serialization;

	[TestFixture]
	public class CreatingQueryDocuments : DemoTests
	{
		[Test]
		public void UntypedQuery()
		{
			var query = Query.NE("name", "anne");

			Console.WriteLine(query);
		}

		public class Person
		{
			public string name { get; set; }
		}

		[Test]
		public void TypedQuery()
		{
			//var query = Query<Person>.NE(p => p.name, "anne");
			var query = Builders<Person>.Filter.Ne(p => p.name, "anne");

            //Console.WriteLine(query);
            Console.WriteLine(query.Render(BsonSerializer.SerializerRegistry.GetSerializer<Person>(),BsonSerializer.SerializerRegistry));
        }

		[Test]
		public void QueryExpressions()
		{
			var query = Query<Person>.Where(p => p.name == "anne");

			Console.WriteLine(query);
		}

		[Test]
		public void QueryDocument()
		{
			var query = new QueryDocument
			{
				{
					"name", new BsonDocument
					{
						{"$ne", "anne"}
					}
				}
			};

			Console.WriteLine(query);
		}


		[Test]
		public void QueryWrapper()
		{
			var document = new BsonDocument
			{
				{
					"name", new BsonDocument
					{
						{"$ne", "anne"}
					}
				}
			};
			var query = new QueryWrapper(document);


			Console.WriteLine(query.ToBsonDocument());
		}
	}
}