using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace Sdl.LC.AddonBlueprint.Models
{
    public class AccountInfoEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        /// <summary>
        /// The public key.
        /// </summary>
        public string PublicKey { get; set; }

        /// <summary>
        /// The tenant id.
        /// </summary>
        public string TenantId { get; set; }

        /// <summary>
        /// The client credentials.
        /// </summary>
        public ClientCredentials ClientCredentials { get; set; }

        /// <summary>
        /// The configuration values.
        /// </summary>
        public List<ConfigurationValueModel> ConfigurationValues { get; set; }
    }
}

