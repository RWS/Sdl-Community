using Sdl.Community.DeeplAddon.Interfaces;
using System;

namespace Sdl.Community.DeeplAddon.Services
{
    /// <summary>
    /// Used to check whether the service is healthy.
    /// </summary>
    public class HealthReporter : IHealthReporter
    {
        /// <summary>
        /// The database context.
        /// </summary>
        private readonly IDatabaseContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="HealthReporter"/> class.
        /// </summary>
        /// <param name="databaseContext">The database context.</param>
        public HealthReporter(IDatabaseContext databaseContext)
        {
            _dbContext = databaseContext;
        }

        /// <summary>
        /// Verifies if the database connection is healthy.
        /// </summary>
        /// <returns>True if the connection is healthy.</returns>
        public bool IsServiceHealthy()
        {
            try
            {
                if (_dbContext.IsConnectionHealthy())
                {
                    return true;
                }

                return false;
            }
            catch(Exception)
            {
                return false;
            }
        }
    }
}
