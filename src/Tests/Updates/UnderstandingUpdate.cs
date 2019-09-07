namespace Tests.Updates
{
	using System;
	using System.Linq;
	using MongoDB.Bson;
	using MongoDB.Driver;
	using MongoDB.Driver.Builders;
	using NUnit.Framework;
	using RealEstate.App_Start;
	using Serialization;

	[TestFixture]
	public class UnderstandingUpdate : DemoTests
	{
		[Test]
		public void UsingSetToModifyADocument()
		{
			var documents = new RealEstateContext().Database.GetCollection<BsonDocument>("documents");

			var person = new BsonDocument {{"name", "bob"}};
			documents.Insert(person);

			Console.WriteLine("\nBefore:");
			Console.WriteLine(documents.FindOneById(person["_id"]));

			var query = Query.EQ("_id", person["_id"]);
			var update = Update.Set("age", 54);
			documents.Update(query, update);

			Console.WriteLine("\nQuery Document:");
			Console.WriteLine(query);
			Console.WriteLine("\nUpdate Document:");
			Console.WriteLine(update.ToJson());

			Console.WriteLine("\nAfter:");
			Console.WriteLine(documents.FindOneById(person["_id"]));
		}

		[Test]
		public void UpdatingMultipleDocuments()
		{
			var documents = new RealEstateContext().Database.GetCollection<BsonDocument>("documents");
			documents.Drop();

			documents.Insert(new BsonDocument {{"name", "bob"}});
			documents.Insert(new BsonDocument {{"name", "jane"}});
			documents.Insert(new BsonDocument {{"name", "anne"}});

			Console.WriteLine("\nBefore:");
			documents.FindAll().ToList().ForEach(Console.WriteLine);

			var query = Query.NE("name", "anne");
			var update = Update.Set("age", 54);
			documents.Update(query, update, UpdateFlags.Multi);

			Console.WriteLine("\nQuery Document:");
			Console.WriteLine(query);
			Console.WriteLine("\nUpdate Document:");
			Console.WriteLine(update.ToJson());

			Console.WriteLine("\nAfter:");
			documents.FindAll().ToList().ForEach(Console.WriteLine);
		}
	}
}