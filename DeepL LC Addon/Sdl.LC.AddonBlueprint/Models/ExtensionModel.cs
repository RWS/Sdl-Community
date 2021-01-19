namespace Sdl.Community.DeeplAddon.Models
{
    public class ExtensionModel
    {
        /// <summary>
        /// The extension point identifier.
        /// </summary>
        public string ExtensionPointId { get; set; }

        /// <summary>
        /// The identifier.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The extension point version.
        /// </summary>
        public string ExtensionPointVersion { get; set; }

        /// <summary>
        /// The configuration.
        /// </summary>
        public dynamic Configuration { get; set; }
    }
}
