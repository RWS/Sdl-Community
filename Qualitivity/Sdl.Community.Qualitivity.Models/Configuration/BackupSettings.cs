using System;
using System.Collections.Generic;
using Sdl.Community.Structures.iProperties;

namespace Sdl.Community.Structures.Configuration
{
    [Serializable]
    public class BackupSettings : ICloneable
    {
        public List<GeneralProperty> BackupProperties { get; set; }
        public BackupSettings()
        {
            BackupProperties = new List<GeneralProperty>();
        }

        public object Clone()
        {
            var backupSettings = new BackupSettings();


            backupSettings.BackupProperties = new List<GeneralProperty>();
            foreach (var property in BackupProperties)
                backupSettings.BackupProperties.Add((GeneralProperty)property.Clone());


            return backupSettings;
        }

   
    }
}
