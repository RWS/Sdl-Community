using System;
using System.Collections.Generic;
using System.Linq;
using Multilingual.Excel.FileType.FileType.Settings.Entities;

namespace Multilingual.Excel.FileType.Services.Entities
{
    public class EntityContext
    {
        public EntityContext()
        {
            LocalEntityMappings = new Dictionary<string, string>();
            DtdEntityMappings = new Dictionary<string, string>();
            EntityMappingSettings = new List<EntityMapping>();
        }

        public Dictionary<string, string> LocalEntityMappings { get; }

        public Dictionary<string, string> DtdEntityMappings { get; }

        public bool EnableEntityMappingsSetting { get; set; }

        public List<EntityMapping> EntityMappingSettings { get; }
        public bool ConvertNumericEntitiesToPlaceholder { get; set; }
        public bool SkipConversionInsideLockedContent { get; set; }

        public void AddLocalEntity(string entityName, string entityUnicodeValue)
        {
            LocalEntityMappings.Add(entityName, entityUnicodeValue);
        }

        public void AddDtdEntity(string entityName, string entityUnicodeValue)
        {
            DtdEntityMappings.Add(entityName, entityUnicodeValue);
        }

        public void AddSettingsEntity(EntityMapping entityMapping)
        {
            EntityMappingSettings.Add(entityMapping);
        }

        public string GetSymbolCharacter(string entityName, EntityRule? ruleType)
        {
            if (ruleType == null)
                return null;

            var character = GetKnownCharacter(LocalEntityMappings, entityName);
            if (character != null) return character;

            character = GetKnownCharacter(DtdEntityMappings, entityName);
            if (character != null) return character;

            if (!EnableEntityMappingsSetting) return null;

            var entityMapping = EntityMappingSettings.FirstOrDefault(sem => sem.Name == entityName);
            if (entityMapping == null) return null;

            if (ruleType == EntityRule.Parser && entityMapping.Read)
                return entityMapping.Char.ToString();

            if (ruleType == EntityRule.Writer && !entityMapping.Write)
                return entityMapping.Char.ToString();

            return null;
        }

        private string GetKnownCharacter(IReadOnlyDictionary<string, string> entityMappings, string value)
        {
            if (entityMappings == null || entityMappings.Count == 0) return null;
            if (!entityMappings.ContainsKey(value)) return null;

            var mappingValue = entityMappings[value];

            return mappingValue.Any(c => !char.IsDigit(c))
                ? mappingValue
                : GetCharacterByIndex(mappingValue);
        }

        public string GetCharacterByIndex(string indexInUnicodeTable)
        {
            var character = System.Net.WebUtility.HtmlDecode($"&#{indexInUnicodeTable};");
            return character;
        }

        public string ConvertCharacterToEntity(char character, bool useReaderMapping)
        {
            var encodedCharacter = ConvertDeclaredCharacterToEntity(LocalEntityMappings, character);
            if (encodedCharacter != null) return $"&{encodedCharacter};";

            encodedCharacter = ConvertDeclaredCharacterToEntity(DtdEntityMappings, character);
            if (encodedCharacter != null) return $"&{encodedCharacter};";

            if (!EnableEntityMappingsSetting) return null;

            var entityMapping = EntityMappingSettings.FirstOrDefault(sem => sem.Char == character);

            if (entityMapping != null && entityMapping.Read && useReaderMapping)
                return $"&{entityMapping.Name};";
            if (entityMapping != null && entityMapping.Write)
                return $"&{entityMapping.Name};";

            return null;
        }

        private string ConvertDeclaredCharacterToEntity(Dictionary<string, string> entityMappings, char character)
        {
            if (entityMappings == null || entityMappings.Count == 0) return null;

            var numericValue = Convert.ToInt32(character).ToString();
            if (!entityMappings.ContainsValue(numericValue)) return null;

            var entityName = entityMappings.FirstOrDefault(x => x.Value == numericValue).Key;
            return entityName;
        }
    }
}
