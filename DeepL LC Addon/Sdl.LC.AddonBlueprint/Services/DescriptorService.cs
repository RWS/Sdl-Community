using Microsoft.Extensions.Options;
using Sdl.LC.AddonBlueprint.Enums;
using Sdl.LC.AddonBlueprint.Interfaces;
using Sdl.LC.AddonBlueprint.Models;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.LC.AddonBlueprint.Services
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

        /// <summary>
        /// The secret data type.
        /// </summary>
        private const DataTypeEnum SecretDataType = DataTypeEnum.secret;

        /// <summary>
        /// The secret mask const string.
        /// </summary>
        private const string SecretMask = "*****";

        /// <summary>
        /// Initializes a new instance of the <see cref="DescriptorService"/> class.
        /// </summary>
        /// <param name="addonDescriptor">The addon descriptor.</param>
        public DescriptorService(IOptions<AddonDescriptorModel> addonDescriptor)
        {
            _addonDescriptor = addonDescriptor.Value;
        }

        /// <summary>
        /// Gets the descriptor.
        /// </summary>
        /// <returns></returns>
        public AddonDescriptorModel GetDescriptor()
        {
            foreach (var configuration in _addonDescriptor.Configurations.Where(c => c.DataType == SecretDataType))
            {
                configuration.DefaultValue = SecretMask;
            }

            return _addonDescriptor;
        }

        /// <summary>
        /// Gets the secret configurations ids.
        /// </summary>
        /// <returns></returns>
        public List<string> GetSecretConfigurations()
        {
            return _addonDescriptor.Configurations.Where(c => c.DataType == SecretDataType).Select(s => s.Id).ToList();
        }
    }
}
