namespace Tests.Updates
{
	using System;
	using MongoDB.Bson;
	using MongoDB.Driver;
	using MongoDB.Driver.Builders;
	using NUnit.Framework;
	using RealEstate.App_Start;
	using RealEstate.Rentals;
	using Serialization;

	[TestFixture]
	public class UnderstandingSave : DemoTests
	{
		[Test]
		public void Save_Using_Update()
		{
			var rentals = new RealEstateContext().Database.GetCollection<Rental>("rentals");

			var rental = new Rental();
			rental.Id = ObjectId.GenerateNewId().ToString();
			rentals.Save(rental);

			var query = Query.EQ("_id", ObjectId.Parse(rental.Id));
			var update = Update.Replace(rental);
			rentals.Update(query, update, UpdateFlags.Upsert);

			Console.WriteLine("Query Document:");
			Console.WriteLine(query);
			Console.WriteLine("Update Document:");
			Console.WriteLine(update.ToJson());
		}
	}
}