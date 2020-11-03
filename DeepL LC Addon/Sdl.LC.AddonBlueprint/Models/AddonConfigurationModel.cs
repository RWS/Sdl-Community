using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Sdl.Community.DeeplAddon.Enums;

namespace Sdl.Community.DeeplAddon.Models
{
	public class AddonConfigurationModel
    {
        /// <summary>
        /// The name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The identifier.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The optional.
        /// </summary>
        public bool Optional { get; set; }

        /// <summary>
        /// The default value.
        /// </summary>
        public object DefaultValue { get; set; }

		/// <summary>
		/// Base url which will be read from appsettings.json
		/// </summary>
		public bool BaseUrl { get; set; }

		/// <summary>
		/// The data type.
		/// </summary>
		[JsonConverter(typeof(StringEnumConverter))]
		public DataTypeEnum DataType { get; set; }
    }
}
