namespace Tests.Serialization
{
	using System;
	using MongoDB.Bson;
	using MongoDB.Bson.Serialization.Attributes;
	using NUnit.Framework;

	public class BsonIdDemonstration : DemoTests
	{
		public class Person
		{
			[BsonId]
			public int PersonId { get; set; }

			public string FirstName { get; set; }
			public int Age { get; set; }
		}

		[Test]
		public void UsePersonId()
		{
			var person = new Person {PersonId = 12};

			Console.WriteLine(person.ToJson());
		}
	}
}