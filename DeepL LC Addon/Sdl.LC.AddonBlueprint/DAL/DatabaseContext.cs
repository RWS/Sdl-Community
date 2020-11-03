using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using Sdl.Community.DeeplAddon.Interfaces;

namespace Sdl.Community.DeeplAddon.DAL
{
	public class DatabaseContext : IDatabaseContext
	{
		/// <summary>
		/// The Mongo Database.
		/// </summary>
		public IMongoDatabase Mongo { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="DatabaseContext"/> class.
		/// </summary>
		/// <param name="config">The configuration.</param>
		public DatabaseContext(IConfiguration config)
		{
			var connectionString = config.GetValue<string>("Logging:MongoDb:Connection");
			var connection = new MongoClient(connectionString);
			Mongo = connection.GetDatabase(config.GetValue<string>("Logging:MongoDb:Name"));
		}

		/// <summary>
		/// Checks if the Mongo Connection is healthy.
		/// </summary>
		/// <returns>True if it's healthy.</returns>
		public bool IsConnectionHealthy()
		{
			var isMongoLive = Mongo.RunCommandAsync((Command<BsonDocument>)"{ping:1}").Wait(1000);
			return isMongoLive;
		}
	}
}
