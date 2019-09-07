namespace Tests.Serialization
{
	using System;
	using MongoDB.Bson;
	using MongoDB.Bson.Serialization.Attributes;
	using NUnit.Framework;

	public class BsonIgnoreIfNullDemonstration : DemoTests
	{
		public class Contact
		{
			public string Email { get; set; }
			public string Phone { get; set; }
		}

		public class Person
		{
			public string FirstName { get; set; }
			public int Age { get; set; }

			[BsonIgnoreIfNull]
			public Contact Contact = new Contact();
		}

		[Test]
		public void IgnoreIfNull()
		{
			var person = new Person
			{
				Contact = null
			};

			Console.WriteLine(person.ToJson());
		}
	}
}