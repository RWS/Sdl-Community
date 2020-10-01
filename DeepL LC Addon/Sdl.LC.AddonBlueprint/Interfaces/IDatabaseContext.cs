using MongoDB.Driver;

namespace Sdl.LC.AddonBlueprint.Interfaces
{
    public interface IDatabaseContext
    {
        /// <summary>
        /// The Mongo Database.
        /// </summary>
        IMongoDatabase Mongo { get; }

        /// <summary>
        /// Checks if the Mongo Connection is healthy.
        /// </summary>
        /// <returns>True if it's healthy.</returns>
        bool IsConnectionHealthy();
    }
}
