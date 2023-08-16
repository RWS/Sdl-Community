using Newtonsoft.Json;

namespace Sdl.Community.DeepLMTProvider.Model
{
    public class GlossaryInfo : ViewModel.ViewModel
    {
        private bool _checked;

        [JsonIgnore]
        public static GlossaryInfo NoGlossary { get; } = new() { Name = PluginResources.NoGlossary };

        public bool IsChecked
        {
            get => _checked;
            set => SetField(ref _checked, value);
        }

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
            if (obj.GetType() != GetType()) return false;
            return Equals((GlossaryInfo)obj);
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

        protected bool Equals(GlossaryInfo other)
        {
            return Id == other.Id && Name == other.Name && SourceLanguage == other.SourceLanguage && TargetLanguage == other.TargetLanguage;
        }
    }
}