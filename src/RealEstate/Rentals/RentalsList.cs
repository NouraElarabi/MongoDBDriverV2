namespace RealEstate.Rentals
{
	using System.Collections.Generic;

	public class RentalsList
	{
		public IEnumerable<RentalViewModel> Rentals { get; set; }
		public RentalsFilter Filters { get; set; }
	}

    public class RentalViewModel
    {
        public string Id { set; get; }
        public string Description { set; get; }
        public int NumberOfRooms { set; get; }
        public decimal Price { set; get; }
        public List<string> Address { set; get; }
    }
}