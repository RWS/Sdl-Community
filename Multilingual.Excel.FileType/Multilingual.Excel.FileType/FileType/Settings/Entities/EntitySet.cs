using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework;
using Sdl.FileTypeSupport.Framework.Core.Settings;

namespace Multilingual.Excel.FileType.FileType.Settings.Entities
{
    public class EntitySet : ISerializableListItem, ICloneable
    {
        private readonly ObservableDictionary<string, EntityMapping> _nameEntityMap = new ObservableDictionary<string, EntityMapping>();
        private readonly ObservableDictionary<char, List<EntityMapping>> _charEntityMap = new ObservableDictionary<char, List<EntityMapping>>();

        private const string SettingName = "Name";
        private const string SettingActive = "Active";
        private const string SettingIsDefault = "IsDefault";
        private const string SettingApplyOverrides = "ApplyOverrides";
        private const string SettingNameEntityMap = "NameEntityMap";
        private const string SettingNameEntityKey = "NameEntityKey";

        private const string DefaultName = "";
        private const bool DefaultActive = true;
        private const bool DefaultIsDefault = true;
        private const bool DefaultApplyOverrides = true;

        public string Name { get; set; }
        public bool Active { get; set; }
        public bool IsDefault { get; set; }
        public bool ApplyOverrides { get; set; } = true;

        public EntitySet()
        {
        }

        /// <summary>
        /// Copy constructor used in cloning. Creates a deep clone of the other object.
        /// </summary>
        /// <param name="other"></param>
        protected EntitySet(EntitySet other)
        {
            Name = other.Name;
            Active = other.Active;
            IsDefault = other.IsDefault;

            foreach (var mapping in other._nameEntityMap.Values)
            {
                EntityMapping m = (EntityMapping)mapping.Clone();
                AddEntityMapping(m);
            }
        }

        public EntitySet(params EntityMapping[] mappings)
        {
            foreach (EntityMapping map in mappings)
            {
                if (_nameEntityMap.ContainsKey(map.Name))
                {
                    // TODO: uncomment the line when the problem is resolved.
                    throw new FileTypeSupportException(String.Format("Duplicate entity name: {0}", map.Name));
                }
                if (_charEntityMap.ContainsKey(map.Char))
                {
                    _charEntityMap[map.Char].Add(map);
                }
                else
                {
                    List<EntityMapping> list = new List<EntityMapping>();
                    list.Add(map);
                    _charEntityMap.Add(map.Char, list);
                }
                _nameEntityMap.Add(map.Name, map);


            }
        }

        public EntitySet(EntityMapping[] mappings, bool isDefault)
            : this(mappings)
        {
            IsDefault = isDefault;
        }

        public object Clone()
        {
            return new EntitySet(this);
        }

        public EntityMapping this[string key] =>
            _nameEntityMap.TryGetValue(key, out var mapping) ? mapping : null;

        public EntityMapping this[char key] =>
            _charEntityMap.TryGetValue(key, out var mappingList) ? mappingList.FirstOrDefault() : null;

        public int Count => _nameEntityMap.Count;

        public IEnumerable<KeyValuePair<string, EntityMapping>> EntityMappingsByName => _nameEntityMap;

        public bool HasMappings => _nameEntityMap.Any();

        /// <summary>
        /// Gets or sets the mappings via a list. Used with Spring object initialization.
        /// </summary>
        public List<EntityMapping> Mappings
        {
            get => new List<EntityMapping>(_nameEntityMap.Values);
            set
            {
                ClearEntityMappings();

                foreach (var mapping in value)
                    AddEntityMapping(mapping);
            }
        }

        public IEnumerable<EntityMapping> ActiveMappings =>
            _nameEntityMap.Values.Where(x => x.Active);

        public IEnumerable<KeyValuePair<char, List<EntityMapping>>> EntityMappingListByChar => _charEntityMap;

        public void AddEntityMapping(EntityMapping mapping)
        {
            if (_nameEntityMap.ContainsKey(mapping.Name))
            {
                // TODO: uncomment the line when the problem is resolved.
                throw new FileTypeSupportException("Entity already exists in the entity set");
            }
            _nameEntityMap.Add(mapping.Name, mapping);
            if (_charEntityMap.ContainsKey(mapping.Char))
            {
                _charEntityMap[mapping.Char].Add(mapping);
            }
            else
            {
                _charEntityMap.Add(
                    mapping.Char,
                    new List<EntityMapping> { mapping });
            }
        }

        public void OverrideEntitySet(EntitySet other)
        {
            foreach (var mapping in other.Mappings)
            {
                _nameEntityMap[mapping.Name] = mapping;

                var list = new List<EntityMapping> { mapping };
                _charEntityMap[mapping.Char] = list;
            }
        }

        public void RemoveEntityMapping(string name)
        {
            if (_nameEntityMap.ContainsKey(name))
            {
                List<EntityMapping> list = _charEntityMap[_nameEntityMap[name].Char];
                for (int ii = 0; ii < list.Count; ii++)
                {
                    if (list[ii].Name == name)
                    {
                        list.RemoveAt(ii);
                        if (list.Count == 0)
                        {
                            _charEntityMap.Remove(_nameEntityMap[name].Char);
                        }
                        break;
                    }
                }
                _nameEntityMap.Remove(name);
            }
        }

        public void ClearEntityMappings()
        {
            _nameEntityMap.Clear();
            _charEntityMap.Clear();
        }

        #region ISerializableListItem Members

        public void ClearListItemSettings(ISettingsGroup settingsGroup, string listItemSetting)
        {
            settingsGroup.RemoveSetting(listItemSetting + SettingName);
            settingsGroup.RemoveSetting(listItemSetting + SettingActive);
            settingsGroup.RemoveSetting(listItemSetting + SettingIsDefault);
            settingsGroup.RemoveSetting(listItemSetting + SettingApplyOverrides);

            EntityMapping mapping = new EntityMapping();
            int nameEntityMapIndex = 0;
            bool hasSetting;
            do
            {
                hasSetting = false;
                string rootName = listItemSetting + SettingNameEntityMap + nameEntityMapIndex;
                nameEntityMapIndex++;

                if (settingsGroup.ContainsSetting(rootName + SettingNameEntityKey))
                {
                    hasSetting = true;
                    settingsGroup.RemoveSetting(rootName + SettingNameEntityKey);
                    mapping.ClearListItemSettings(settingsGroup, rootName);
                }
            }
            while (hasSetting);
        }

        public void PopulateFromSettingsGroup(ISettingsGroup settingsGroup, string listItemSetting)
        {
            Name = GetSettingFromSettingsGroup(settingsGroup, listItemSetting + SettingName, DefaultName);
            Active = GetSettingFromSettingsGroup(settingsGroup, listItemSetting + SettingActive, DefaultActive);
            IsDefault = GetSettingFromSettingsGroup(settingsGroup, listItemSetting + SettingIsDefault, DefaultIsDefault);
            ApplyOverrides = GetSettingFromSettingsGroup(settingsGroup, listItemSetting + SettingApplyOverrides, DefaultApplyOverrides);

            _nameEntityMap.Clear();
            _charEntityMap.Clear();

            int nameEntityMapIndex = 0;
            bool hasSetting;
            do
            {
                hasSetting = false;
                string rootName = listItemSetting + SettingNameEntityMap + nameEntityMapIndex;
                nameEntityMapIndex++;

                if (settingsGroup.ContainsSetting(rootName + SettingNameEntityKey))
                {
                    hasSetting = true;
                    EntityMapping mapping = new EntityMapping();
                    mapping.PopulateFromSettingsGroup(settingsGroup, rootName);
                    AddEntityMapping(mapping);
                }
            }
            while (hasSetting);

        }

        public void SaveToSettingsGroup(ISettingsGroup settingsGroup, string listItemSetting)
        {
            UpdateSettingInSettingsGroup(settingsGroup, listItemSetting + SettingName, Name, DefaultName);
            UpdateSettingInSettingsGroup(settingsGroup, listItemSetting + SettingActive, Active, DefaultActive);
            UpdateSettingInSettingsGroup(settingsGroup, listItemSetting + SettingIsDefault, IsDefault, DefaultIsDefault);
            UpdateSettingInSettingsGroup(settingsGroup, listItemSetting + SettingApplyOverrides, ApplyOverrides, DefaultApplyOverrides);

            int nameEntityMapIndex = 0;
            foreach (var nameEntity in _nameEntityMap)
            {
                string rootName = listItemSetting + SettingNameEntityMap + nameEntityMapIndex;
                settingsGroup.GetSetting<string>(rootName + SettingNameEntityKey).Value = nameEntity.Key;
                nameEntity.Value.SaveToSettingsGroup(settingsGroup, rootName);
                nameEntityMapIndex++;
            }

            // Note: No need to save the charEntity map content as this can be constructed from the contents of
            // the NameEntityMap
        }

        #endregion

        protected T GetSettingFromSettingsGroup<T>(ISettingsGroup settingsGroup, string settingName, T defaultValue)
        {
            if (settingsGroup.ContainsSetting(settingName))
                return settingsGroup.GetSetting<T>(settingName).Value;

            return defaultValue;
        }


        protected void UpdateSettingInSettingsGroup<T>(ISettingsGroup settingsGroup, string settingName, T settingValue, T defaultValue)
        {
            if (settingsGroup.ContainsSetting(settingName))
            {
                SaveInGroup(settingsGroup, settingName, settingValue);
            }
            else
            {
                if (!settingValue.Equals(defaultValue))
                    SaveInGroup(settingsGroup, settingName, settingValue);
            }
        }

        protected void SaveInGroup<T>(ISettingsGroup settingsGroup, string settingName, T settingValue)
        {
            settingsGroup.GetSetting<T>(settingName).Value = settingValue;
        }

        #region json serialization
        private const string MappingsArrayJsonKey = "Mappings";

        internal JObject ToJObject()
        {
            var result = new JObject
            {
                {SettingName, Name},
                {SettingActive, Active},
                {SettingIsDefault, IsDefault},
                {SettingApplyOverrides, ApplyOverrides}
            };

            var mappingsArray = new JArray();
            foreach (var mapping in _nameEntityMap)
                mappingsArray.Add(mapping.Value.ToJObject());

            result.Add(MappingsArrayJsonKey, mappingsArray);

            return result;
        }

        internal static EntitySet FromJObject(JObject jObject)
        {
            var instance = new EntitySet();
            instance.ResetToDefaults();

            if (jObject == null)
                return instance;

            if (jObject.TryGetValue(SettingName, out var name))
                instance.Name = name.Value<string>();


            if (jObject.TryGetValue(SettingActive, out var active))
                instance.Active = active.Value<bool>();

            if (jObject.TryGetValue(SettingIsDefault, out var isDefault))
                instance.IsDefault = isDefault.Value<bool>();

            if (jObject.TryGetValue(SettingApplyOverrides, out var applyOverrides))
                instance.ApplyOverrides = applyOverrides.Value<bool>();

            if (!jObject.TryGetValue(MappingsArrayJsonKey, out var mappingsToken) ||
                !(mappingsToken is JArray mappings)) return instance;

            // Populate from settings
            foreach (var token in mappings)
            {
                var mapping = EntityMapping.FromJObject(token as JObject);

                instance._nameEntityMap[mapping.Name] = mapping;

                var list = new List<EntityMapping> { mapping };

                instance._charEntityMap[mapping.Char] = list;
            }

            return instance;
        }

        private void ResetToDefaults()
        {
            Name = DefaultName;
            Active = DefaultActive;
            IsDefault = DefaultIsDefault;
            ApplyOverrides = DefaultApplyOverrides;

            _nameEntityMap.Clear();
            _charEntityMap.Clear();
        }
        #endregion
    }
}
