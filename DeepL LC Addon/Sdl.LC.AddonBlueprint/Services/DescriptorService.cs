using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Sdl.Community.DeeplAddon.Enums;
using Sdl.Community.DeeplAddon.Interfaces;
using Sdl.Community.DeeplAddon.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sdl.Community.DeeplAddon.Services
{
	/// <summary>
	/// Used to return the addon descriptor
	/// </summary>
	public class DescriptorService : IDescriptorService
	{
		/// <summary>
		/// The addon descriptor.
		/// </summary>
		private readonly AddonDescriptorModel _addonDescriptor;
		private readonly IConfiguration _configuration;
		private const string BaseUrl = "AddonBaseUrl";
		/// <summary>
		/// The secret data type.
		/// </summary>
		private const DataTypeEnum SecretDataType = DataTypeEnum.secret;

		/// <summary>
		/// The secret mask const string. This is used to show *** on the LC UI when there is an secret saved in db
		/// </summary>
		private const string SecretMask = "*****";

		/// <summary>
		/// Initializes a new instance of the <see cref="DescriptorService"/> class.
		/// </summary>
		public DescriptorService(IConfiguration configuration)
		{
			// Reading from the descriptor.json file, the descriptor for this Add-On. 
			// Customize it to represent your Add-On behavior.
			var descriptorText = File.ReadAllText("descriptor.json");
			_addonDescriptor = JsonConvert.DeserializeObject<AddonDescriptorModel>(descriptorText);
			_configuration = configuration;
		}

		/// <summary>
		/// Gets the descriptor.
		/// </summary>
		public AddonDescriptorModel GetDescriptor()
		{
			_addonDescriptor.BaseUrl = _configuration.GetValue<string>(BaseUrl);

			foreach (var configuration in _addonDescriptor.Configurations.Where(c => c.DataType == SecretDataType))
			{
				configuration.DefaultValue = SecretMask;
			}

			return _addonDescriptor;
		}

		/// <summary>
		/// Gets the secret configurations ids.
		/// </summary>
		public List<string> GetSecretConfigurations()
		{
			return _addonDescriptor.Configurations.Where(c => c.DataType == SecretDataType).Select(s => s.Id).ToList();
		}
	}
}
