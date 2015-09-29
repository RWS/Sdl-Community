using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using Simple.Data;

namespace Sdl.Community.RecordSourceTU
{
    public class TmDataAccess
    {
        private readonly string _tmDatabasePath;
        private dynamic _db;
        private AddSourceTmConfiguration _addSourceTmConfiguration;

        private TmDataAccess(string tmDatabasePath)
        {
            _tmDatabasePath = tmDatabasePath;
        }

        public static TmDataAccess OpenConnection(Uri providerUri)
        {
            var tmDataAccess = new TmDataAccess(providerUri.LocalPath);
            tmDataAccess.Init(providerUri);
            return tmDataAccess;
        }

        private void Init(Uri providerUri)
        {

            var builder
                = new SQLiteConnectionStringBuilder
                {
                    SyncMode = SynchronizationModes.Off,
                    DataSource = _tmDatabasePath,
                    Enlist = false,
                    Pooling = false
                };

            _db = Database.OpenConnection(builder.ToString());

            var persistance = new Persistance();
            var addSourceTmConfigurations = persistance.Load();

            _addSourceTmConfiguration = addSourceTmConfigurations.Configurations.FirstOrDefault(x => x.ProviderUri == providerUri) ??
                                           addSourceTmConfigurations.Default;

        }

        public void AddOrUpdateCustomFields(int tuId, CustomFieldValues fieldValues)
        {
            var translationUnit = _db.translation_units.Find(_db.translation_units.id == tuId);

            if (translationUnit == null) return;
            if (fieldValues == null) return;

            if (_addSourceTmConfiguration.StoreFilename)
            {
                AddOrUpdateCustomField(translationUnit.translation_memory_id, tuId,
                    _addSourceTmConfiguration.FileNameField, fieldValues.FileName);
            }
            if (_addSourceTmConfiguration.StoreFullPath)
            {
                AddOrUpdateCustomField(translationUnit.translation_memory_id, tuId,
                    _addSourceTmConfiguration.FullPathField, fieldValues.FileNameFullPath);
            }
            if (_addSourceTmConfiguration.StoreProjectName)
            {
                AddOrUpdateCustomField(translationUnit.translation_memory_id, tuId,
                    _addSourceTmConfiguration.ProjectNameField, fieldValues.ProjectName);
            }

        }

        private void AddOrUpdateCustomField(int tmId,int tuId, string fieldName, string fieldValue)
        {
            var sourceAttribute = _db.attributes.Find(_db.attributes.name == fieldName) ??
                                  AddSourceAttribute(tmId, fieldName);
            var stringAttribute = _db.string_attributes.Find(_db.string_attributes.translation_unit_id == tuId &&
                                     _db.string_attributes.attribute_id == sourceAttribute.id);
            if (stringAttribute != null)
            {
                if (IsSourceFileValueAlreadyInTu(fieldValue, stringAttribute)) return;
                stringAttribute.value = string.Format("{0}, {1}", stringAttribute.value, fieldValue);
                _db.string_attributes.UpdateByTranslation_Unit_idAndAttribute_id(stringAttribute);
            }
            else
            {
                _db.string_attributes.Insert(translation_unit_id: tuId, attribute_id: sourceAttribute.id,
                    value: fieldValue);
            }
        }

        public List<string> GetCustomFields()
        {
            var customFields = new List<string>();
            var attributes = _db.attributes.All().Where(_db.attributes.type == 2);

            foreach (var attribute in attributes)
            {
                customFields.Add(attribute.name);
            }

            return customFields;
        }

        private bool IsSourceFileValueAlreadyInTu(string sourceFile, dynamic stringAttribute)
        {
            string oldSourceAttributeValue = stringAttribute.value.ToString();
            var oldValues = oldSourceAttributeValue.Split(',');

            return oldValues.Any(oldValue => oldValue.Trim().Equals(sourceFile.Trim()));
        }

        private dynamic AddSourceAttribute(int tmId,string fieldName)
        {
            var translationMemory =
               _db.translation_memories.Find(_db.translation_memories.id == tmId);
            var attribute = _db.attributes.Insert(guid: Guid.NewGuid(), name: fieldName, type: 2, tm_id: translationMemory.id);

            return attribute;
        }

    }
}
