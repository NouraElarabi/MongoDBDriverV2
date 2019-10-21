namespace RealEstate.Rentals
{
    using App_Start;
    using MongoDB.Bson;
    using MongoDB.Bson.IO;
    using MongoDB.Driver;
    using MongoDB.Driver.Builders;
    using MongoDB.Driver.GridFS;
    using MongoDB.Driver.Linq;
    using System;
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

            //var rentals = await ContextNew.Rentals
            //    .Find(filters.ToFilterDefinition())
            //    .Project(r => new RentalViewModel
            //    {
            //        Id = r.Id,
            //        Description = r.Description,
            //        NumberOfRooms = r.NumberOfRooms,
            //        Price = r.Price,
            //        Address = r.Address
            //    })
            //    //.Sort(Builders<Rental>.Sort.Ascending(r => r.Price))
            //    .SortBy(r => r.Price)
            //    .ThenByDescending(r => r.NumberOfRooms)
            //    .ToListAsync();

            var rentals = await FilterRentals(filters)
                .Select(r => new RentalViewModel
                {
                    Id = r.Id,
                    Description = r.Description,
                    NumberOfRooms = r.NumberOfRooms,
                    Price = r.Price,
                    Address = r.Address
                })
                .OrderBy(r => r.Price)
                .ThenByDescending(r => r.NumberOfRooms)
                .ToListAsync();

            var model = new RentalsList
			{
				Rentals = rentals,
				Filters = filters
			};
			return View(model);
		}

        private IMongoQueryable<Rental> FilterRentals(RentalsFilter filters)
        {
            IMongoQueryable<Rental> rentals = ContextNew.Rentals.AsQueryable();

            if (filters.MinimumRooms.HasValue)
            {
                rentals = rentals
                    .Where(r => r.NumberOfRooms >= filters.MinimumRooms);
            }

            if (filters.PriceLimit.HasValue)
            {
                rentals = rentals
                    .Where(r => r.Price<= filters.PriceLimit);
            }

            return rentals;
        }

        //private IEnumerable<Rental> FilterRentals(RentalsFilter filters)
        //{
        //	IQueryable<Rental> rentals = Context.Rentals.AsQueryable()
        //		.OrderBy(r => r.Price);

        //	if (filters.MinimumRooms.HasValue)
        //	{
        //		rentals = rentals
        //			.Where(r => r.NumberOfRooms >= filters.MinimumRooms);
        //	}

        //	if (filters.PriceLimit.HasValue)
        //	{
        //		var query = Query<Rental>.LTE(r => r.Price, filters.PriceLimit);
        //		rentals = rentals
        //			.Where(r => query.Inject());
        //	}

        //	return rentals;
        //}

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
				//.Run(Context.Rentals)
				.RunAggregationFluent(ContextNew.Rentals)
                .ToJson();
		}

		public ActionResult AttachImage(string id)
		{
			var rental = GetRental(id);
			return View(rental);
		}

		[HttpPost]
		public async Task<ActionResult> AttachImage(string id, HttpPostedFileBase file)
		{
			var rental = GetRental(id);
			if (rental.HasImage())
			{
				await DeleteImage(rental);
			}
			await StoreImage(file, id);
			return RedirectToAction("Index");
		}

		private async Task DeleteImage(Rental rental)
		{
            //Context.Database.GridFS.DeleteById(new ObjectId(rental.ImageId));
            await ContextNew.ImagesBucket.DeleteAsync(new ObjectId(rental.ImageId));
			SetRentalImageId(rental.Id, null);
		}

		private async Task StoreImage(HttpPostedFileBase file, string rentalId)
		{
            //var imageId = ObjectId.GenerateNewId();
            //SetRentalImageId(rentalId, imageId.ToString());
            //var options = new MongoGridFSCreateOptions
            //{
            //	Id = imageId,
            //	ContentType = file.ContentType
            //};
            //Context.Database.GridFS.Upload(file.InputStream, file.FileName, options);

            //var bucket = new GridFSBucket(ContextNew.Database, new GridFSBucketOptions { 
            //    BucketName = "images"
            //});

            //var bucket = new GridFSBucket(ContextNew.Database);
            var bucket = ContextNew.ImagesBucket;

            GridFSUploadOptions options = new GridFSUploadOptions
            {
                Metadata = new BsonDocument("content", file.ContentType)
            };
            var imageId = await bucket.UploadFromStreamAsync(file.FileName, file.InputStream, options);
            await SetRentalImageId(rentalId, imageId.ToString());
        }

		private async Task SetRentalImageId(string rentalId, string imageId)
		{
			//var rentalByid = Query<Rental>.Where(r => r.Id == rentalId);
            //var setRentalImageId = Update<Rental>.Set(r => r.ImageId, imageId);
            //Context.Rentals.Update(rentalByid, setRentalImageId);
            var setRentalImageId = Builders<Rental>.Update.Set(r => r.ImageId, imageId);
            await ContextNew.Rentals.UpdateOneAsync(r => r.Id == rentalId, setRentalImageId);
        }

		public ActionResult GetImage(string id)
		{
            //var image = Context.Database.GridFS
            //	.FindOneById(new ObjectId(id));
            //if (image == null)
            //{
            //	return HttpNotFound();
            //}
            //         //return File(image.OpenRead(), image.ContentType);
            //         return File(image.OpenRead(), image.ContentType ?? image.Metadata["contentType"].AsString);

            try
            {

                var bucket = ContextNew.ImagesBucket;
                var stream = bucket.OpenDownloadStream(new ObjectId(id));
                var contentType = stream.FileInfo.Metadata["contentType"].AsString;
                return File(stream, contentType);

            } catch (GridFSFileNotFoundException e)
            {
                return HttpNotFound();
            }

        }

        public ActionResult JoinPreLookup()
        {
            var rentals = ContextNew.Rentals.Find(new BsonDocument { }).ToList();
            var rentalZips = rentals.Select(r => r.ZipCode).Distinct().ToArray();

            var zipsById = ContextNew.Database.GetCollection<ZipCode>("zips")
                .Find(z => rentalZips.Contains(z.Id))
                .ToList()
                .ToDictionary(d => d.Id);

            var Report = rentals.Select(r => new
            {
                Rental = r,
                ZipCode = r.ZipCode != null && zipsById.ContainsKey(r.ZipCode) ? zipsById[r.ZipCode] : null
            });

            return Content(Report.ToJson(new JsonWriterSettings { OutputMode = JsonOutputMode.Strict }), "application/json");
        }

        public ActionResult JoinWithLookup()
        {
            //var Report = ContextNew.Rentals.Aggregate()
            //    .Lookup<Rental, ZipCode, BsonDocument>(ContextNew.Database.GetCollection<ZipCode>("zips"),
            //    r => r.ZipCode,
            //    z => z.Id,
            //    d => d["zips"]
            //    );

            var Report = ContextNew.Rentals.Aggregate()
                .Lookup<Rental, ZipCode, RentalWithZipCodes>(ContextNew.Database.GetCollection<ZipCode>("zips"),
                r => r.ZipCode,
                z => z.Id,
                w => w.ZipCodes
                );

            return Content(Report.ToJson(new JsonWriterSettings { OutputMode = JsonOutputMode.Strict }), "application/json");
        }
    }
    public class RentalWithZipCodes: Rental
    {
        public ZipCode[] ZipCodes { get; set; }
    }
}