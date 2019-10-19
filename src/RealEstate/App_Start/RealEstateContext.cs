namespace RealEstate.App_Start
{
    using System;
    using log4net;
    using MongoDB.Driver;
    using MongoDB.Driver.Core.Events;
    using Properties;
    using Rentals;

    public class RealEstateContext
	{
		public MongoDatabase Database;

		public RealEstateContext()
		{
			var client = new MongoClient(Settings.Default.RealEstateConnectionString);
			var server = client.GetServer();
			Database = server.GetDatabase(Settings.Default.RealEstateDatabaseName);
		}

		public MongoCollection<Rental> Rentals => Database.GetCollection<Rental>("rentals");
	}

	public class RealEstateContextNewApis
	{
		public IMongoDatabase Database;

		public RealEstateContextNewApis()
		{
            var connectionString = Settings.Default.RealEstateConnectionString;
            var settings = MongoClientSettings.FromUrl(new MongoUrl(connectionString));
            //settings.ClusterConfigurator = builder => builder.Subscribe<CommandStartedEvent>(started =>
            //{
            //    //do something
            //});
            settings.ClusterConfigurator = builder => builder.Subscribe(new logMongoEvents());
            var client = new MongoClient(settings);

            //var client = new MongoClient(Settings.Default.RealEstateConnectionString);
            Database = client.GetDatabase(Settings.Default.RealEstateDatabaseName);
		}

		public IMongoCollection<Rental> Rentals => Database.GetCollection<Rental>("rentals");
	}

    public class logMongoEvents: IEventSubscriber
    {
        private static ILog CommandStartedLog = LogManager.GetLogger("CommandStarted");
        private ReflectionEventSubscriber _Subscriber;
        public logMongoEvents()
        {
            _Subscriber = new ReflectionEventSubscriber(this);
        }

        public bool TryGetEventHandler<TEvent>(out Action<TEvent> handler)
        {
            //if (typeof(TEvent) != typeof(CommandStartedEvent))
            //{
            //    handler = null;
            //    return false;
            //}

            //handler = e =>
            //{

            //};
            //return true;

            return _Subscriber.TryGetEventHandler(out handler); // looks for a function called Handle that takes the type of event and returns void
        }

        public void Handle(CommandStartedEvent started){
            CommandStartedLog.Info(new
            {
                started.Command,
                started.CommandName
            });
        }

        public void Handle(CommandSucceededEvent succeeded)
        {

        }
    }
}







