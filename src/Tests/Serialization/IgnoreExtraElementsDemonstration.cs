namespace Tests.Serialization
{
	using MongoDB.Bson;
	using MongoDB.Bson.Serialization;
	using MongoDB.Bson.Serialization.Attributes;
	using NUnit.Framework;

	public class IgnoreExtraElementsDemonstration : DemoTests
	{
		[BsonIgnoreExtraElements]
		public class Person
		{
			public string FirstName { get; set; }
			public int Age { get; set; }
		}

		[Test]
		public void IgnoreExtraElements()
		{
			var document = new BsonDocument
			{
				{"RemovedField", ""}
			};

			BsonSerializer.Deserialize<Person>(document);
		}
	}
}