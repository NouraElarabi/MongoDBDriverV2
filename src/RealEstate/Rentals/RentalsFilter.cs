using MongoDB.Driver;

namespace RealEstate.Rentals
{
	public class RentalsFilter
	{
		public decimal? PriceLimit { get; set; }
		public int? MinimumRooms { get; set; }

        public FilterDefinition<Rental> ToFilterDefinition ()
        {
            var filterDefinition = Builders<Rental>.Filter.Empty;
            if (MinimumRooms.HasValue)
            {
                filterDefinition &= Builders<Rental>.Filter.Where(r => r.NumberOfRooms >= MinimumRooms);
            }

            if (PriceLimit.HasValue)
            {
                filterDefinition &= Builders<Rental>.Filter.Where(r => r.Price <= PriceLimit);
                //filterDefinition &= Builders<Rental>.Filter.Lte(r => r.Price, PriceLimit.Value);
            }
            return filterDefinition;
        }
	}
}