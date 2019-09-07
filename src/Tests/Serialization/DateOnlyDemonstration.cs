namespace Tests.Serialization
{
	using System;
	using MongoDB.Bson;
	using MongoDB.Bson.Serialization.Attributes;
	using NUnit.Framework;

	public class DateOnlyDemonstration : DemoTests
	{
		public class Person
		{
			public string FirstName { get; set; }
			public int Age { get; set; }

			[BsonDateTimeOptions(DateOnly = true)]
			public DateTime BirthDate { get; set; }
		}

		[Test]
		public void DateOnly()
		{
			var person = new Person
			{
				BirthDate = new DateTime(2014, 1, 2)
			};

			Console.WriteLine(person.ToJson());
		}
	}
}