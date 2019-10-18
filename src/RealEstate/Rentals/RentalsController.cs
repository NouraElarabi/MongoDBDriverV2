﻿namespace RealEstate.Rentals
{
    using App_Start;
    using MongoDB.Bson;
    using MongoDB.Driver;
    using MongoDB.Driver.Builders;
    using MongoDB.Driver.GridFS;
    using MongoDB.Driver.Linq;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;

    public class RentalsController : Controller
	{
		public readonly RealEstateContext Context = new RealEstateContext();
		public readonly RealEstateContextNewApis ContextNew = new RealEstateContextNewApis();

        public async Task<ActionResult> Index(RentalsFilter filters)
		{
            //var rentals = FilterRentals(filters);

            //var rentals = await ContextNew.Rentals
            //    .Find(new BsonDocument())
            //    .ToListAsync();

            //var filterDefinition = Builders<Rental>.Filter.Where(r => r.NumberOfRooms >= filters.MinimumRooms);
            //var rentals = await ContextNew.Rentals
            //    .Find(filterDefinition)
            //    .ToListAsync();

            var rentals = await ContextNew.Rentals
                .Find(filters.ToFilterDefinition())
                .Project(r => new RentalViewModel
                {
                    Id = r.Id,
                    Description = r.Description,
                    NumberOfRooms = r.NumberOfRooms,
                    Price = r.Price,
                    Address = r.Address
                })
                //.Sort(Builders<Rental>.Sort.Ascending(r => r.Price))
                .SortBy(r => r.Price)
                .ThenByDescending(r => r.NumberOfRooms)
                .ToListAsync();

            var model = new RentalsList
			{
				Rentals = rentals,
				Filters = filters
			};
			return View(model);
		}

		private IEnumerable<Rental> FilterRentals(RentalsFilter filters)
		{
			IQueryable<Rental> rentals = Context.Rentals.AsQueryable()
				.OrderBy(r => r.Price);

			if (filters.MinimumRooms.HasValue)
			{
				rentals = rentals
					.Where(r => r.NumberOfRooms >= filters.MinimumRooms);
			}

			if (filters.PriceLimit.HasValue)
			{
				var query = Query<Rental>.LTE(r => r.Price, filters.PriceLimit);
				rentals = rentals
					.Where(r => query.Inject());
			}

			return rentals;
		}

		public ActionResult Post()
		{
			return View();
		}

		[HttpPost]
		public async Task<ActionResult> Post(PostRental postRental)
		{
			var rental = new Rental(postRental);
			//Context.Rentals.Insert(rental);
			//ContextNew.Rentals.InsertOne(rental);
			await ContextNew.Rentals.InsertOneAsync(rental);
            return RedirectToAction("Index");
		}

		public ActionResult AdjustPrice(string id)
		{
			var rental = GetRental(id);
			return View(rental);
		}

		private Rental GetRental(string id)
		{
			var rental = Context.Rentals.FindOneById(new ObjectId(id));
			return rental;
		}

		[HttpPost]
		public async Task<ActionResult> AdjustPrice(string id, AdjustPrice adjustPrice)
		{
			var rental = GetRental(id);

            //rental.AdjustPrice(adjustPrice);
            // Context.Rentals.Save(rental);
            //ContextNew.Rentals.ReplaceOne(r => r.Id == id, rental);

            //var adjustment = new PriceAdjustment(adjustPrice, rental.Price);
            //var modificationUpdate = Builders<Rental>.Update
            //    .Push(r => r.Adjustments, adjustment)
            //    .Set(r => r.Price, adjustPrice.NewPrice);
            ////ContextNew.Rentals.UpdateOne(r => r.Id == id, modificationUpdate);
            //await ContextNew.Rentals.UpdateOneAsync(r => r.Id == id, modificationUpdate);

            //await ContextNew.Rentals.ReplaceOneAsync(r => r.Id == id, rental);

            rental.AdjustPrice(adjustPrice);
            UpdateOptions options = new UpdateOptions
            {
                IsUpsert = true
            };
            await ContextNew.Rentals.ReplaceOneAsync(r => r.Id == id, rental, options);

            return RedirectToAction("Index");
		}

		[HttpPost]
		public ActionResult AdjustPriceUsingModification(string id, AdjustPrice adjustPrice)
		{
			var rental = GetRental(id);
			var adjustment = new PriceAdjustment(adjustPrice, rental.Price);
			var modificationUpdate = Update<Rental>
				.Push(r => r.Adjustments, adjustment)
				.Set(r => r.Price, adjustPrice.NewPrice);
			Context.Rentals.Update(Query.EQ("_id", new ObjectId(id)), modificationUpdate);
			return RedirectToAction("Index");
		}

		public async Task<ActionResult> Delete(string id)
		{
			//Context.Rentals.Remove(Query.EQ("_id", new ObjectId(id)));
            // all contextNew crud operations are async under the hood
			await ContextNew.Rentals.DeleteOneAsync(r => r.Id == id);
            return RedirectToAction("Index");
		}

		public string PriceDistribution()
		{
			return new QueryPriceDistribution()
				.Run(Context.Rentals)
				.ToJson();
		}

		public ActionResult AttachImage(string id)
		{
			var rental = GetRental(id);
			return View(rental);
		}

		[HttpPost]
		public ActionResult AttachImage(string id, HttpPostedFileBase file)
		{
			var rental = GetRental(id);
			if (rental.HasImage())
			{
				DeleteImage(rental);
			}
			StoreImage(file, id);
			return RedirectToAction("Index");
		}

		private void DeleteImage(Rental rental)
		{
			Context.Database.GridFS.DeleteById(new ObjectId(rental.ImageId));
			SetRentalImageId(rental.Id, null);
		}

		private void StoreImage(HttpPostedFileBase file, string rentalId)
		{
			var imageId = ObjectId.GenerateNewId();
			SetRentalImageId(rentalId, imageId.ToString());
			var options = new MongoGridFSCreateOptions
			{
				Id = imageId,
				ContentType = file.ContentType
			};
			Context.Database.GridFS.Upload(file.InputStream, file.FileName, options);
		}

		private void SetRentalImageId(string rentalId, string imageId)
		{
			var rentalByid = Query<Rental>.Where(r => r.Id == rentalId);
			var setRentalImageId = Update<Rental>.Set(r => r.ImageId, imageId);
			Context.Rentals.Update(rentalByid, setRentalImageId);
		}

		public ActionResult GetImage(string id)
		{
			var image = Context.Database.GridFS
				.FindOneById(new ObjectId(id));
			if (image == null)
			{
				return HttpNotFound();
			}
			return File(image.OpenRead(), image.ContentType);
		}
	}
}