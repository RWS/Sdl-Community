using System;
using System.Globalization;
using Newtonsoft.Json.Linq;
using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework.Core.Settings;

namespace Multilingual.XML.FileType.FileType.Settings.Entities
{
    public class EntityMapping : ISerializableListItem, ICloneable
    {
        private const string SettingName = "Name";
        private const string SettingChar = "Char"; //legacy setting, kept for compatibility
        private const string SettingCharAsInt = "CharAsInt";
        private const string SettingActive = "Active";
        private const string SettingRead = "Read";
        private const string SettingWrite = "Write";

        private const string DefaultName = "";
        private const char DefaultChar = (char)0x0000;
        private const bool DefaultActive = true;
        private const bool DefaultRead = true;
        private const bool DefaultWrite = true;

        public string Name { get; set; }
        public char Char { get; set; }
        public bool Active { get; set; }
        public bool Read { get; set; }
        public bool Write { get; set; }

        public int CharAsInt
        {
            get => Char;
            set => Char = (char)value;
        }

        public string CharAsString
        {
            get => Char == '\0' ? string.Empty : Char.ToString(CultureInfo.InvariantCulture);
            set => Char = value == string.Empty ? '\0' : value[0];
        }

        public bool IsNumericEntity => Name.StartsWith("#");

        public EntityMapping()
        {
        }

        protected EntityMapping(EntityMapping other)
        {
            Name = other.Name;
            Char = other.Char;
            Active = other.Active;
            Read = other.Read;
            Write = other.Write;
        }

        /// <summary>
        /// Initializes a new instance of the EntityMapping class.
        /// </summary>
        /// <param name="name">Mapping name.</param>
        /// <param name="ch">Mapped character.</param>
        /// <param name="active">A value indicating whether the mapping is active.</param>
        public EntityMapping(string name, char ch, bool active)
        {
            Name = name;
            Char = ch;
            Active = active;
            Read = active;
            Write = false;
        }

        /// <summary>
        /// Initializes a new instance of the EntityMapping class.
        /// </summary>
        /// <param name="name">Mapping name.</param>
        /// <param name="ch">Mapped character.</param>
        /// <param name="read">A value indicating whether the mapping is used for reading process</param>
        /// <param name="write">A value indicating whether the mapping is used for writing process</param>
        public EntityMapping(string name, char ch, bool read, bool write)
        {
            Name = name;
            Char = ch;
            Active = read;
            Read = read;
            Write = write;
        }

        public override bool Equals(object obj)
        {
            return obj is EntityMapping otherMapping
                   && Name == otherMapping.Name
                   && Char == otherMapping.Char
                   && Active == otherMapping.Active
                   && Read == otherMapping.Read
                   && Write == otherMapping.Write;
        }

        public override int GetHashCode()
        {
            return (Name?.GetHashCode() ?? 0)
                ^ Char.GetHashCode()
                ^ Active.GetHashCode()
                ^ Read.GetHashCode()
                ^ Write.GetHashCode();
        }

        public object Clone()
        {
            return new EntityMapping(this);
        }

        #region ISerializableListItem Members

        public void ClearListItemSettings(ISettingsGroup settingsGroup, string listItemSetting)
        {
            settingsGroup.RemoveSetting(listItemSetting + SettingName);
            settingsGroup.RemoveSetting(listItemSetting + SettingChar);
            settingsGroup.RemoveSetting(listItemSetting + SettingActive);
            settingsGroup.RemoveSetting(listItemSetting + SettingRead);
            settingsGroup.RemoveSetting(listItemSetting + SettingWrite);
        }

        public void PopulateFromSettingsGroup(ISettingsGroup settingsGroup, string listItemSetting)
        {
            Name = GetSettingFromSettingsGroup(settingsGroup, listItemSetting + SettingName, DefaultName);

            //prefer to load new setting over old
            if (settingsGroup.ContainsSetting(listItemSetting + SettingCharAsInt))
            {
                Char = (char)GetSettingFromSettingsGroup(settingsGroup, listItemSetting + SettingCharAsInt,
                    (int)DefaultChar);
            }
            else
            {
                //don't have a new setting, so try to load old one. We need to keep this for backwards compatibility
                Char = GetSettingFromSettingsGroup(settingsGroup, listItemSetting + SettingChar, DefaultChar);
            }

            Active = GetSettingFromSettingsGroup(settingsGroup, listItemSetting + SettingActive, DefaultActive);
            Read = GetSettingFromSettingsGroup(settingsGroup, listItemSetting + SettingRead, DefaultRead);
            Write = GetSettingFromSettingsGroup(settingsGroup, listItemSetting + SettingWrite, DefaultWrite);
        }

        public void SaveToSettingsGroup(ISettingsGroup settingsGroup, string listItemSetting)
        {
            UpdateSettingInSettingsGroup(settingsGroup, listItemSetting + SettingName, Name, DefaultName);
            UpdateSettingInSettingsGroup(settingsGroup, listItemSetting + SettingCharAsInt, CharAsInt, DefaultChar);
            UpdateSettingInSettingsGroup(settingsGroup, listItemSetting + SettingActive, Active, DefaultActive);
            UpdateSettingInSettingsGroup(settingsGroup, listItemSetting + SettingRead, Read, DefaultRead);
            UpdateSettingInSettingsGroup(settingsGroup, listItemSetting + SettingWrite, Write, DefaultWrite);
        }

        #endregion

        protected T GetSettingFromSettingsGroup<T>(ISettingsGroup settingsGroup, string settingName, T defaultValue)
        {
            if (settingsGroup.ContainsSetting(settingName))
            {
                return settingsGroup.GetSetting<T>(settingName).Value;
            }

            return defaultValue;
        }

        protected void UpdateSettingInSettingsGroup<T>(ISettingsGroup settingsGroup, string settingName, T settingValue,
            T defaultValue)
        {
            if (settingsGroup.ContainsSetting(settingName))
            {
                SaveInGroup(settingsGroup, settingName, settingValue);
            }
            else
            {
                if (!settingValue.Equals(defaultValue))
                {
                    SaveInGroup(settingsGroup, settingName, settingValue);
                }
            }
        }

        protected void SaveInGroup<T>(ISettingsGroup settingsGroup, string settingName, T settingValue)
        {
            settingsGroup.GetSetting<T>(settingName).Value = settingValue;
        }

        #region json serialization
        internal JObject ToJObject()
        {
            return new JObject
            {
                {SettingName, Name},
                {SettingCharAsInt, CharAsInt},
                {SettingActive, Active},
                {SettingRead, Read},
                {SettingWrite, Write}
            };
        }

        internal static EntityMapping FromJObject(JObject jObject)
        {
            var instance = new EntityMapping();
            instance.ResetToDefaults();

            if (jObject == null)
                return instance;

            if (jObject.TryGetValue(SettingName, out var name))
                instance.Name = name.Value<string>();

            if (jObject.TryGetValue(SettingCharAsInt, out var charAsInt))
                instance.CharAsInt = charAsInt.Value<int>();

            if (jObject.TryGetValue(SettingActive, out var active))
                instance.Active = active.Value<bool>();

            if (jObject.TryGetValue(SettingRead, out var read))
                instance.Read = read.Value<bool>();

            if (jObject.TryGetValue(SettingWrite, out var write))
                instance.Write = write.Value<bool>();

            return instance;
        }

        private void ResetToDefaults()
        {
            Name = DefaultName;
            Char = DefaultChar;
            Active = DefaultActive;
            Read = DefaultRead;
            Write = DefaultWrite;
        }
        #endregion
    }
}
