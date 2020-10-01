namespace Sdl.LC.AddonBlueprint.Services
{
    /// <summary>
    /// Used to check whether the service is healthy.
    /// </summary>
    public interface IHealthReporter
    {
        /// <summary>
        /// Verifies if the database connection is healthy.
        /// </summary>
        /// <returns>True if the connection is healthy.</returns>
        bool IsServiceHealthy();
    }
}