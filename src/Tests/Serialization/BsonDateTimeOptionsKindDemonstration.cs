namespace Tests.Serialization
{
	using System;
	using MongoDB.Bson;
	using MongoDB.Bson.Serialization;
	using MongoDB.Bson.Serialization.Attributes;
	using NUnit.Framework;

	public class BsonDateTimeOptionsKindDemonstration : DemoTests
	{
		public class Person
		{
			public string FirstName { get; set; }
			public int Age { get; set; }
			[BsonDateTimeOptions(Kind = DateTimeKind.Local)]
			public DateTime BirthTime { get; set; }
		}

		[Test]
		public void SpecifyDateTimeKind()
		{
			var document = new BsonDocument
			{
				{"BirthTime", new DateTime(2014, 1, 2, 11, 30, 0)}
			};

			Console.WriteLine(document);

			var person = BsonSerializer.Deserialize<Person>(document);

			Console.WriteLine(new
			{
				person.BirthTime.Kind,
				Value = person.BirthTime.ToString()
			});
		}
	}
}