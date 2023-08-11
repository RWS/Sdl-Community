using Newtonsoft.Json;

namespace Sdl.Community.DeepLMTProvider.Model
{
	public class GlossaryInfo
	{
		[JsonProperty("glossary_id")]
		public string Id { get; set; }
		public string Name { get; set; }

		[JsonProperty("source_lang")]
		public string SourceLanguage { get; set; }
		[JsonProperty("target_lang")]
		public string TargetLanguage { get; set; }

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((GlossaryInfo)obj);
		}

		protected bool Equals(GlossaryInfo other)
		{
			return Id == other.Id && Name == other.Name && SourceLanguage == other.SourceLanguage && TargetLanguage == other.TargetLanguage;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = (Id != null ? Id.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (SourceLanguage != null ? SourceLanguage.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (TargetLanguage != null ? TargetLanguage.GetHashCode() : 0);
				return hashCode;
			}
		}

		[JsonIgnore]
		public static GlossaryInfo NoGlossary { get; } = new() { Name = PluginResources.NoGlossary };
	}
}