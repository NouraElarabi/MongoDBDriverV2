namespace Tests.Serialization
{
	using System;
	using MongoDB.Bson;
	using MongoDB.Bson.Serialization.Attributes;
	using NUnit.Framework;

	public class DecimalRepresentationDemonstration : DemoTests
	{
		public class Person
		{
			public string FirstName { get; set; }
			public int Age { get; set; }

			[BsonRepresentation(BsonType.Double)]
			public decimal NetWorth { get; set; }
		}

		[Test]
		public void ChangeRepresentation()
		{
			var person = new Person
			{
				NetWorth = 100.50m
			};

			Console.WriteLine(person.ToJson());
		}
	}
}