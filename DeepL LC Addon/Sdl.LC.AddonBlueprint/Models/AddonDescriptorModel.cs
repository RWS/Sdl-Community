using System.Collections.Generic;

namespace Sdl.Community.DeeplAddon.Models
{
    public class AddonDescriptorModel
	{
		public const string Descriptor = "Descriptor";

		/// <summary>
		/// The name.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// The version.
		/// </summary>
		public string Version { get; set; }

		/// <summary>
		/// The description.
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// The scopes.
		/// </summary>
		public List<string> Scopes { get; set; }

		/// <summary>
		/// The extensions.
		/// </summary>
		public List<ExtensionModel> Extensions { get; set; }

		/// <summary>
		/// The base url.
		/// </summary> 
		public string BaseUrl { get; set; }

		/// <summary>
		/// The standard endpoints.
		/// </summary>
		public AddonStandardEndpointsModel StandardEndpoints { get; set; }

		/// <summary>
		/// The configurations.
		/// </summary>
		public List<AddonConfigurationModel> Configurations { get; set; }

		/// <summary>
		/// The release notes.
		/// </summary>
		public string ReleaseNotes { get; set; }

		/// <summary>
		/// The minimum version.
		/// </summary>
		public string MinimumVersion { get; set; }

		/// <summary>
		/// The vendor.
		/// </summary>
		public AddonVendorModel Vendor { get; set; }
	}
}
