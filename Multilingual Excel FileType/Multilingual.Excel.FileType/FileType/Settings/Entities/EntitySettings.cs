using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json.Linq;
using Sdl.Core.Settings;
using Sdl.Core.Settings.Implementation.Json;
using Sdl.FileTypeSupport.Framework.Core.Settings;

namespace Multilingual.Excel.FileType.FileType.Settings.Entities
{
    public class EntitySettings : FileTypeSettingsBase
    {
        public ObservableDictionary<string, EntitySet> EntitySets { get; } = new ObservableDictionary<string, EntitySet>();

        protected virtual string JsonSettingName => "EntitySettings";

        protected EntitySetBuilder EntitySetBuilder;

        private const string SettingConvertEntities = "ConvertEntities";
        private const string SettingAddParserWriter = "AddParserWriter";
        private const string SettingDefaultOverridesList = "DefaultOverridesList";
        private const string SettingRemoveDefaultOverridesList = "RemoveDefaultOverridesList";

        protected bool DefaultConvertEntities;
        protected bool DefaultAddToParserAndWriter = true;
        private readonly Action<ObservableDictionary<string, EntitySet>> _modifySetsForDefaultSettings;

        public bool ConvertEntities { get; set; }
        /// <summary>
        /// When true (default) entity settings references will be added to the
        /// parser and writer if missing.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This property was introduced to be able to re-use the settings object
        /// also for HTML entity conversions, which don't reference the settings
        /// in the same way.
        /// </para>
        /// </remarks>
        public bool AddToParserAndWriter { get; set; }

        public EntitySettings()
        {
            EntitySetBuilder = new EntitySetBuilder();
            ResetToDefaults();
        }

        /// <summary>
        /// Overloaded constructor used when initializing settings for HTML usage.
        /// </summary>
        /// <param name="addToParserAndWriter"></param>
        public EntitySettings(bool addToParserAndWriter)
        {
            EntitySetBuilder = new EntitySetBuilder();
            AddToParserAndWriter = addToParserAndWriter;
        }

        [Obsolete("Use the Action<...> based constructor instead.")]
        public EntitySettings(bool defaultConvertEntities, string[] activeSetKeys)
        {
            DefaultConvertEntities = defaultConvertEntities;

            _modifySetsForDefaultSettings = (sets) =>
            {
                foreach (var key in activeSetKeys)
                    sets[key].Active = true;
            };

            EntitySetBuilder = new EntitySetBuilder();

            ResetToDefaults();
        }

        public EntitySettings(bool defaultConvertEntities, Action<ObservableDictionary<string, EntitySet>> modifySetsForDefaultSettings)
        {
            DefaultConvertEntities = defaultConvertEntities;

            _modifySetsForDefaultSettings = modifySetsForDefaultSettings;

            EntitySetBuilder = new EntitySetBuilder();

            ResetToDefaults();
        }

        public EntitySettings(bool defaultConvertEntities, bool resetToDefaults = true)
        {
            EntitySetBuilder = new EntitySetBuilder();
            DefaultConvertEntities = defaultConvertEntities;
            if (resetToDefaults)
                ResetToDefaults();
        }

        public EntitySettings(params EntitySet[] entitySets)
        {
            // NOTE: The way entity settings have been saved to older filter definitions
            // the constructor arguments contain overrides to the default entity sets.
            // This was previously handled by explicitly applying the overrides in the
            // setters for the EntitySettings properties in the XML Parser and Writer,
            // however that unfortunately means that the UI does not show the correct settings
            // if initialized from the object itself. We circumvent that issue by
            // explicitly applying the defaults and then overriding them here:

            EntitySetBuilder = new EntitySetBuilder();

            PopulateDefaultEntitySets();
            List<EntitySet> overrides = new List<EntitySet>(entitySets);
            ApplyOverrides(overrides);
        }

        public IEnumerable<EntitySet> ActiveEntitySets =>
            EntitySets.Values.Where(x => x.Active);

        /// <summary>
        /// Represents the difference between these settings and the
        /// default entity conversion settings.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This property is meant for working with filter definition files where only
        /// the differences to the default settings need to be stored.
        /// </para>
        /// </remarks>
        public virtual List<EntitySet> DefaultOverrides
        {
            get
            {
                // compare to the defaults
                EntitySettings defaults = new EntitySettings();
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

        /// <summary>
        /// Represents the  entity sets which contain mappings which have been removed (when compared to the Default sets).
        /// </summary>
        public virtual List<EntitySet> RemoveDefaultOverrides
        {
            get
            {
                EntitySettings defaults = new EntitySettings();
                defaults.PopulateDefaultEntitySets();

                return GetRemovedEntitySetDifferences(defaults.EntitySets);

            }
            set
            {
                ApplyRemoveOverrides(value);
            }
        }

        /// <summary>
        /// Returns a list of entity sets containing the differences between this set
        /// and the set passed in. Only entity sets that differ is returned, and in those
        /// only the entities that differ are set.
        /// </summary>
        /// <returns></returns>
        public List<EntitySet> GetEntitySetDifferences(ObservableDictionary<string, EntitySet> otherSets)
        {
            List<EntitySet> diffSets = new List<EntitySet>();

            foreach (var otherSet in otherSets.Values)
            {
                if (!EntitySets.ContainsKey(otherSet.Name))
                {
                    // other set is not part of our settings
                    // - add to diffs as a de-activated set
                    EntitySet diffSet = (EntitySet)otherSet.Clone();
                    diffSet.Active = false;
                    diffSets.Add(diffSet);
                }
            }

            foreach (var set in EntitySets.Values)
            {
                EntitySet otherSet;
                if (!otherSets.TryGetValue(set.Name, out otherSet))
                {
                    // not in the input sets
                    diffSets.Add(set);
                    continue;
                }

                // compare each entity in the set
                bool hasDifferences = false;
                EntitySet setDiffs = new EntitySet();
                setDiffs.Active = set.Active;
                setDiffs.IsDefault = set.IsDefault;
                setDiffs.Name = set.Name;

                if (set.IsDefault != otherSet.IsDefault)
                {
                    hasDifferences = true;
                }
                if (set.Active != otherSet.Active)
                {
                    hasDifferences = true;
                }

                foreach (EntityMapping mapping in set.Mappings)
                {
                    var otherMapping = otherSet[mapping.Name];
                    if (otherMapping == null)
                    {
                        // not in the other set
                        setDiffs.AddEntityMapping(mapping);
                        hasDifferences = true;
                        continue;
                    }

                    if (!mapping.Equals(otherMapping))
                    {
                        // mapping differs
                        setDiffs.AddEntityMapping(mapping);
                        hasDifferences = true;
                    }
                }

                if (hasDifferences)
                {
                    diffSets.Add(setDiffs);
                }
            }

            return diffSets;
        }

        /// <summary>
        /// Returns a list of entity sets containing the differences detailing where an entity mapping has been removed
        /// as compared to the input set.
        /// </summary>
        /// <param name="otherSets">Input set to compare against</param>
        /// <returns>A list of entity sets containing mapping which have been removed (when compared to the input set)</returns>
        public List<EntitySet> GetRemovedEntitySetDifferences(ObservableDictionary<string, EntitySet> otherSets)
        {
            List<EntitySet> diffSets = new List<EntitySet>();


            foreach (var set in EntitySets.Values)
            {
                EntitySet otherSet;
                if (!otherSets.TryGetValue(set.Name, out otherSet))
                {
                    continue;
                }

                // compare each entity in the set
                bool hasDifferences = false;
                EntitySet setDiffs = new EntitySet();
                setDiffs.Name = set.Name;
                setDiffs.Active = true;
                setDiffs.IsDefault = false;



                foreach (EntityMapping otherMapping in otherSet.Mappings)
                {
                    var mapping = set[otherMapping.Name];
                    if (mapping == null)
                    {
                        // not in the other set
                        setDiffs.AddEntityMapping(otherMapping);
                        hasDifferences = true;
                        continue;
                    }
                }

                if (hasDifferences)
                {
                    diffSets.Add(setDiffs);
                }
            }

            return diffSets;
        }

        public void AddEntitySet(EntitySet set)
        {
            Debug.Assert(set.Name.Length > 0);

            EntitySets.Add(set.Name, set);
        }

        public void RemoveEntitySet(string setName)
        {
            Debug.Assert(setName.Length > 0);

            if (EntitySets.ContainsKey(setName))
            {
                EntitySets.Remove(setName);
            }
        }

        /// <summary>
        /// Applies a set of entity set overrides (like what is returned
        /// from the DefaultOverrides property) to the current settings.
        /// </summary>
        /// <param name="entitySets"></param>
        public void ApplyOverrides(List<EntitySet> entitySets)
        {
            foreach (var set in entitySets)
            {
                EntitySet ourSet;
                if (EntitySets.TryGetValue(set.Name, out ourSet) && set.ApplyOverrides)// ApplyOverrides property is false for Xml Entity Sets
                {
                    ourSet.Active = set.Active;

                    if (!set.IsDefault)
                    {
                        ourSet.IsDefault = set.IsDefault;
                        foreach (var mapping in set.Mappings)
                        {
                            if (ourSet[mapping.Name] != null)
                            {
                                ourSet.RemoveEntityMapping(mapping.Name);
                            }
                            ourSet.AddEntityMapping((EntityMapping)mapping.Clone());
                        }
                    }
                    else
                    {
                        // activate/deactivate existing mappings
                        foreach (KeyValuePair<string, EntityMapping> item in set.EntityMappingsByName)
                        {
                            if (EntitySets[set.Name][item.Key] != null)
                            {
                                EntitySets[set.Name][item.Key].Active = item.Value.Active;
                                EntitySets[set.Name][item.Key].Read = item.Value.Read;
                                EntitySets[set.Name][item.Key].Write = item.Value.Write;
                            }
                        }
                    }
                }
                else
                {

                    if (EntitySets.ContainsKey(set.Name))
                    {
                        if (set.IsDefault)
                        {
                            EntitySets[set.Name].Active = set.Active;
                            //This updates the default sets Active status
                            foreach (EntityMapping mapping in set.Mappings)
                            {
                                EntitySets[set.Name][mapping.Name].Active = mapping.Active;
                                EntitySets[set.Name][mapping.Name].Read = mapping.Read;
                                EntitySets[set.Name][mapping.Name].Write = mapping.Write;
                            }
                        }
                        else
                        {
                            EntitySets.Remove(set.Name);
                            EntitySets.Add(set.Name, (EntitySet)set.Clone());
                        }
                    }
                    else
                    {
                        EntitySets.Add(set.Name, (EntitySet)set.Clone());
                    }

                }
            }
        }

        public void ApplyRemoveOverrides(List<EntitySet> entitySets)
        {
            foreach (var set in entitySets)
            {
                EntitySet ourSet;
                if (EntitySets.TryGetValue(set.Name, out ourSet))
                {
                    foreach (var mapping in set.Mappings)
                    {
                        if (ourSet[mapping.Name] != null)
                        {
                            ourSet.RemoveEntityMapping(mapping.Name);
                        }
                    }
                }

            }
        }

        public void OverrideDefaultSettings(ObservableDictionary<string, EntitySet> entitySets)
        {
            // We need to iterate over the entity sets passed in and set the default mappings' Active
            // property to be equal to the equivalent mappings' property in the passed in set(s) (if that makes sense!)
            foreach (string setName in entitySets.Keys)
            {
                if (EntitySets.ContainsKey(setName))
                {
                    EntitySets[setName].Active = entitySets[setName].Active;
                    if (!entitySets[setName].IsDefault)
                    {
                        EntitySets.Remove(setName);
                        EntitySets.Add(setName, entitySets[setName]);
                    }
                    else
                    {
                        foreach (KeyValuePair<string, EntityMapping> item in entitySets[setName].EntityMappingsByName)
                        {

                            EntitySets[setName][item.Key].Active = item.Value.Active;

                        }
                    }

                }
                else
                {
                    // Add custom set
                    EntitySets.Add(setName, entitySets[setName]);
                }
            }
        }

        public void PopulateDefaultEntitySets()
        {
            try
            {
                EntitySetBuilder.PopulateDefaultEntitySets(EntitySets);
            }
            catch (Exception e1)
            {
                // IP TODO 07112007 - throw exception?
#pragma warning disable 168
                string msg = e1.Message;
#pragma warning restore 168
            }
            finally
            {
                OnPropertyChanged("EntitySets");
            }

        }

        public void ClearAllEntitySets()
        {
            EntitySets.Clear();
        }

        public virtual EntitySet CreateXMLDEFAULTEntitySet()
        {
            var mappings = new[]
            {
                new EntityMapping("lt", '<', true),
                new EntityMapping("gt", '>', true),
                new EntityMapping("quot", '"', true),
                new EntityMapping("apos", (char)0x0027, true),
                new EntityMapping("amp", '&', true)
            };

            EntitySet eSet = new EntitySet(mappings, true)
            {
                Name = "XML Default",
                Active = true
            };
            return eSet;
        }


        public override void PopulateFromSettingsBundle(ISettingsBundle settingsBundle, string fileTypeConfigurationId)
        {
            ISettingsGroup settingsGroup = settingsBundle.GetSettingsGroup(fileTypeConfigurationId);

            if (settingsBundle is JsonSettingsBundle)
                PopulateFromJsonSettingsGroup(settingsGroup);
            else
                PopulateFromXmlSettingsGroup(settingsGroup);
        }

        private void PopulateFromXmlSettingsGroup(ISettingsGroup settingsGroup)
        {
            ConvertEntities = GetSettingFromSettingsGroup(settingsGroup, SettingConvertEntities, DefaultConvertEntities);
            AddToParserAndWriter = GetSettingFromSettingsGroup(settingsGroup, SettingAddParserWriter, DefaultAddToParserAndWriter);

            if (settingsGroup.ContainsSetting(SettingDefaultOverridesList))
            {
                ComplexObservableList<EntitySet> defaultOverrides = new ComplexObservableList<EntitySet>();
                defaultOverrides.PopulateFromSettingsGroup(settingsGroup, SettingDefaultOverridesList);
                List<EntitySet> defaults = new List<EntitySet>();
                defaults.AddRange(defaultOverrides);

                DefaultOverrides = defaults;
            }

            if (settingsGroup.ContainsSetting(SettingRemoveDefaultOverridesList))
            {
                ComplexObservableList<EntitySet> removeDefaultOverrides = new ComplexObservableList<EntitySet>();
                removeDefaultOverrides.PopulateFromSettingsGroup(settingsGroup, SettingRemoveDefaultOverridesList);
                List<EntitySet> removeDefaults = new List<EntitySet>();
                removeDefaults.AddRange(removeDefaultOverrides);

                RemoveDefaultOverrides = removeDefaults;
            }
        }

        private void PopulateFromJsonSettingsGroup(ISettingsGroup settingsGroup)
        {
            var jObject = GetSettingFromSettingsGroup(settingsGroup, JsonSettingName, new JObject());

            if (jObject.TryGetValue(SettingConvertEntities, out var convertEntities))
                ConvertEntities = convertEntities.Value<bool>();

            if (jObject.TryGetValue(SettingAddParserWriter, out var addToParserAndWriter))
                AddToParserAndWriter = addToParserAndWriter.Value<bool>();

            DefaultOverrides = ExtractEntitySetListFromJObject(jObject, SettingDefaultOverridesList);
            RemoveDefaultOverrides = ExtractEntitySetListFromJObject(jObject, SettingRemoveDefaultOverridesList);
        }

        private List<EntitySet> ExtractEntitySetListFromJObject(JObject jObject, string key)
        {
            var result = new List<EntitySet>();

            if (jObject != null
                && jObject.TryGetValue(key, out var token)
                && token is JArray array)
            {
                foreach (var item in array)
                    result.Add(EntitySet.FromJObject(item as JObject));
            }

            return result;
        }

        public override void ResetToDefaults()
        {
            ConvertEntities = DefaultConvertEntities;
            AddToParserAndWriter = DefaultAddToParserAndWriter;

            EntitySets.Clear();
            PopulateDefaultEntitySets();

            _modifySetsForDefaultSettings?.Invoke(EntitySets);
        }

        public override void SaveToSettingsBundle(ISettingsBundle settingsBundle, string fileTypeConfigurationId)
        {
            ISettingsGroup settingsGroup = settingsBundle.GetSettingsGroup(fileTypeConfigurationId);

            if (settingsBundle is JsonSettingsBundle)
                SaveToJsonSettingsGroup(settingsGroup, this);
            else
                SaveToXmlSettingsGroup(settingsGroup);
        }

        private void SaveToJsonSettingsGroup(ISettingsGroup settingsGroup, EntitySettings objectToSave)
        {
            var settingsObject = new JObject
            {
                { SettingConvertEntities, objectToSave.ConvertEntities },
                { SettingAddParserWriter, objectToSave.AddToParserAndWriter }
            };

            SaveEntitySetListToJobject(objectToSave.DefaultOverrides, settingsObject, SettingDefaultOverridesList);
            SaveEntitySetListToJobject(objectToSave.RemoveDefaultOverrides, settingsObject, SettingRemoveDefaultOverridesList);

            UpdateSettingInSettingsGroup(settingsGroup, JsonSettingName, settingsObject, new JObject());
        }

        private void SaveEntitySetListToJobject(List<EntitySet> list, JObject jObject, string key)
        {
            var jArray = new JArray(list.Select(x => x.ToJObject()));
            jObject.Add(key, jArray);
        }

        private void SaveToXmlSettingsGroup(ISettingsGroup settingsGroup)
        {
            UpdateSettingInSettingsGroup(settingsGroup, SettingConvertEntities, ConvertEntities, DefaultConvertEntities);
            UpdateSettingInSettingsGroup(settingsGroup, SettingAddParserWriter, AddToParserAndWriter, DefaultAddToParserAndWriter);

            ComplexObservableList<EntitySet> defaultOverrides = new ComplexObservableList<EntitySet>();
            defaultOverrides.AddRange(DefaultOverrides);
            defaultOverrides.ClearListItemSettings(settingsGroup, SettingDefaultOverridesList);
            defaultOverrides.SaveToSettingsGroup(settingsGroup, SettingDefaultOverridesList);

            ComplexObservableList<EntitySet> removeDefaultOverrides = new ComplexObservableList<EntitySet>();
            removeDefaultOverrides.AddRange(RemoveDefaultOverrides);
            removeDefaultOverrides.ClearListItemSettings(settingsGroup, SettingRemoveDefaultOverridesList);
            removeDefaultOverrides.SaveToSettingsGroup(settingsGroup, SettingRemoveDefaultOverridesList);
        }

        public override void SaveDefaultsToSettingsBundle(ISettingsBundle settingsBundle, string fileTypeConfigurationId)
        {
            var settingsGroup = settingsBundle.GetSettingsGroup(fileTypeConfigurationId);

            if (settingsBundle is JsonSettingsBundle)
                SaveToJsonSettingsGroup(settingsGroup, CreateInstanceWithDefaultSettings());
            else
                SaveDefaultsToXmlSettingsGroup(settingsGroup);
        }

        private void SaveDefaultsToXmlSettingsGroup(ISettingsGroup settingsGroup)
        {
            var instance = CreateInstanceWithDefaultSettings();

            SaveInGroup(settingsGroup, SettingConvertEntities, instance.ConvertEntities);
            SaveInGroup(settingsGroup, SettingAddParserWriter, instance.AddToParserAndWriter);

            var defaultOverrides = new ComplexObservableList<EntitySet>();
            defaultOverrides.AddRange(instance.DefaultOverrides);
            defaultOverrides.SaveToSettingsGroup(settingsGroup, SettingDefaultOverridesList);

            var removeDefaultOverrides = new ComplexObservableList<EntitySet>();
            removeDefaultOverrides.AddRange(instance.RemoveDefaultOverrides);
            removeDefaultOverrides.SaveToSettingsGroup(settingsGroup, SettingRemoveDefaultOverridesList);
        }

        protected virtual EntitySettings CreateInstanceWithDefaultSettings()
        {
            return new EntitySettings(DefaultConvertEntities, _modifySetsForDefaultSettings);
        }
    }
}
