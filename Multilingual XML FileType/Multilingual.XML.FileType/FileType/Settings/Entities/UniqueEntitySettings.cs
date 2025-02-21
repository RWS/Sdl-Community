using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Sdl.Core.Settings;
using Sdl.Core.Settings.Implementation.Json;

namespace Multilingual.XML.FileType.FileType.Settings.Entities
{
    public class UniqueEntitySettings : EntitySettings
    {
        protected override string JsonSettingName => "UniqueEntitySettings";

        private const string SettingNumericEntitiesToPlaceholder = "NumericEntitiesToPlaceholders";
        private const string SettingSkipConversionInsideLockedContent = "SkipConversionInsideLockedContent";

        protected bool DefaultNumericEntitiesToPlaceholder = false;
        protected bool DefaultSkipConversionInsideLockedContent = false;

        public UniqueEntitySettings()
        {
            EntitySetBuilder = new UniqueEntitySetBuilder();
            DefaultConvertEntities = true;
            ResetToDefaults();
        }

        /// <summary>
        /// Overloaded constructor used when initializing settings for HTML usage.
        /// </summary>
        /// <param name="addToParserAndWriter"></param>
        public UniqueEntitySettings(bool addToParserAndWriter) : base(addToParserAndWriter)
        {
            EntitySetBuilder = new UniqueEntitySetBuilder();
            DefaultConvertEntities = true;
        }

        public UniqueEntitySettings(bool defaultConvertEntities, bool resetToDefaults = true)
            : base(defaultConvertEntities, resetToDefaults)
        {
            EntitySetBuilder = new UniqueEntitySetBuilder();
            DefaultConvertEntities = true;
        }

        public UniqueEntitySettings(params EntitySet[] entitySets) : base(entitySets)
        {
            EntitySetBuilder = new UniqueEntitySetBuilder();
            DefaultConvertEntities = true;
        }


        public bool ConvertNumericEntityToPlaceholder { get; set; }
        public bool SkipConversionInsideLockedContent { get; set; }

        /// <summary>
        /// Represents the  entity sets which contain mappings which have been removed (when compared to the Default sets).
        /// </summary>
        public override List<EntitySet> RemoveDefaultOverrides
        {
            get
            {
                UniqueEntitySettings defaults = new UniqueEntitySettings();
                defaults.PopulateDefaultEntitySets();

                return GetRemovedEntitySetDifferences(defaults.EntitySets);

            }
            set
            {
                ApplyRemoveOverrides(value);
            }
        }


        /// <summary>
        /// Represents the difference between these settings and the
        /// default entity conversion settings.
        /// </summary>
        public override List<EntitySet> DefaultOverrides
        {
            get
            {
                // compare to the defaults
                UniqueEntitySettings defaults = new UniqueEntitySettings();
                defaults.PopulateDefaultEntitySets();

                return GetEntitySetDifferences(defaults.EntitySets);
            }
            set
            {
                // reset to defaults, then apply the list of differences as overrides
                PopulateDefaultEntitySets();
                ApplyOverrides(value);
            }
        }

        public override EntitySet CreateXMLDEFAULTEntitySet()
        {

            EntitySet eSet = new EntitySet(new EntityMapping[0], true);
            eSet.Name = "XML Default";
            eSet.Active = true;
            return eSet;
        }

        public override void PopulateFromSettingsBundle(ISettingsBundle settingsBundle, string fileTypeConfigurationId)
        {
            base.PopulateFromSettingsBundle(settingsBundle, fileTypeConfigurationId);
            ISettingsGroup settingsGroup = settingsBundle.GetSettingsGroup(fileTypeConfigurationId);

            if (settingsBundle is JsonSettingsBundle)
                PopulateFromJsonSettingsGroup(settingsGroup);
            else
                PopulateFromXmlSettingsGroup(settingsGroup);

        }

        private void PopulateFromJsonSettingsGroup(ISettingsGroup settingsGroup)
        {
            var jObject = GetSettingFromSettingsGroup(settingsGroup, JsonSettingName, new JObject());

            ConvertNumericEntityToPlaceholder = jObject.TryGetValue(SettingNumericEntitiesToPlaceholder, out var numericToPlaceHolder)
                ? numericToPlaceHolder.Value<bool>()
                : DefaultNumericEntitiesToPlaceholder;

            SkipConversionInsideLockedContent = jObject.TryGetValue(SettingSkipConversionInsideLockedContent, out var skipInLockedContent)
                ? skipInLockedContent.Value<bool>()
                : DefaultSkipConversionInsideLockedContent;
        }

        private void PopulateFromXmlSettingsGroup(ISettingsGroup settingsGroup)
        {
            ConvertNumericEntityToPlaceholder = GetSettingFromSettingsGroup(settingsGroup,
                SettingNumericEntitiesToPlaceholder, DefaultNumericEntitiesToPlaceholder);

            SkipConversionInsideLockedContent = GetSettingFromSettingsGroup(settingsGroup,
                SettingSkipConversionInsideLockedContent, DefaultSkipConversionInsideLockedContent);
        }

        public override void ResetToDefaults()
        {
            base.ResetToDefaults();
            ConvertNumericEntityToPlaceholder = DefaultNumericEntitiesToPlaceholder;
            SkipConversionInsideLockedContent = DefaultSkipConversionInsideLockedContent;
        }


        public override void SaveToSettingsBundle(ISettingsBundle settingsBundle, string fileTypeConfigurationId)
        {
            base.SaveToSettingsBundle(settingsBundle, fileTypeConfigurationId);
            ISettingsGroup settingsGroup = settingsBundle.GetSettingsGroup(fileTypeConfigurationId);

            if (settingsBundle is JsonSettingsBundle)
                SaveToJsonSettingsGroup(settingsGroup, false);
            else
                SaveToXmlSettingsGroup(settingsGroup);
        }

        private void SaveToJsonSettingsGroup(ISettingsGroup settingsGroup, bool useDefaultValues)
        {
            var jObject = GetSettingFromSettingsGroup(settingsGroup, JsonSettingName, new JObject());

            if (jObject.ContainsKey(SettingNumericEntitiesToPlaceholder))
                jObject.Remove(SettingNumericEntitiesToPlaceholder);

            var value = useDefaultValues ? DefaultNumericEntitiesToPlaceholder : ConvertNumericEntityToPlaceholder;
            jObject.Add(SettingNumericEntitiesToPlaceholder, value);

            if (jObject.ContainsKey(SettingSkipConversionInsideLockedContent))
                jObject.Remove(SettingSkipConversionInsideLockedContent);

            value = useDefaultValues ? DefaultSkipConversionInsideLockedContent : SkipConversionInsideLockedContent;
            jObject.Add(SettingSkipConversionInsideLockedContent, value);

            UpdateSettingInSettingsGroup(settingsGroup, JsonSettingName, jObject, new JObject());
        }

        private void SaveToXmlSettingsGroup(ISettingsGroup settingsGroup)
        {
            UpdateSettingInSettingsGroup(settingsGroup, SettingNumericEntitiesToPlaceholder, ConvertNumericEntityToPlaceholder, DefaultNumericEntitiesToPlaceholder);
            UpdateSettingInSettingsGroup(settingsGroup, SettingSkipConversionInsideLockedContent, SkipConversionInsideLockedContent, DefaultSkipConversionInsideLockedContent);
        }

        public override void SaveDefaultsToSettingsBundle(ISettingsBundle settingsBundle, string fileTypeConfigurationId)
        {
            base.SaveDefaultsToSettingsBundle(settingsBundle, fileTypeConfigurationId);
            var settingsGroup = settingsBundle.GetSettingsGroup(fileTypeConfigurationId);

            if (settingsBundle is JsonSettingsBundle)
                SaveToJsonSettingsGroup(settingsGroup, true);
            else
                SaveDefaultsToXmlSettingsGroup(settingsGroup);
        }

        private void SaveDefaultsToXmlSettingsGroup(ISettingsGroup settingsGroup)
        {
            SaveInGroup(settingsGroup, SettingNumericEntitiesToPlaceholder, DefaultNumericEntitiesToPlaceholder);
            SaveInGroup(settingsGroup, SettingSkipConversionInsideLockedContent, DefaultSkipConversionInsideLockedContent);
        }

        protected override EntitySettings CreateInstanceWithDefaultSettings()
        {
            return new UniqueEntitySettings();
        }
    }
}
