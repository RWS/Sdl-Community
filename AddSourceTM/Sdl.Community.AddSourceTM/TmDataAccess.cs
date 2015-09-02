using System;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using Sdl.Community.AddSourceTM.Source_Configurtion;
using Simple.Data;

namespace Sdl.Community.AddSourceTM
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

        public void Init(Uri providerUri)
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

        public void AddOrUpdateSourceFile(int tuId,string sourceFile)
        {
            var translationUnit = _db.translation_units.Find(_db.translation_units.id == tuId);

            if (translationUnit == null) return;

            if (!_addSourceTmConfiguration.StoreFullPath)
                sourceFile = Path.GetFileName(sourceFile);

            var sourceAttribute = _db.attributes.Find(_db.attributes.name == _addSourceTmConfiguration.TmSourceFieldName) ??
                                  AddSourceAttribute(translationUnit.translation_memory_id);
            var stringAttribute = _db.string_attributes.Find(_db.string_attributes.translation_unit_id == tuId &&
                                     _db.string_attributes.attribute_id == sourceAttribute.id);
            if (stringAttribute != null)
            {
                stringAttribute.value = sourceFile;
                _db.string_attributes.UpdateById(stringAttribute);
            }
            else
            {
                _db.string_attributes.Insert(translation_unit_id: translationUnit.id, attribute_id: sourceAttribute.id,
                    value: sourceFile);
            }

        }

        private dynamic AddSourceAttribute(int tmId)
        {
            var translationMemory =
               _db.translation_memories.Find(_db.translation_memories.id == tmId);
            var attribute = _db.attributes.Insert(guid: Guid.NewGuid(), name: _addSourceTmConfiguration.TmSourceFieldName, type: 2, tm_id: translationMemory.id);

            return attribute;
        }

    }
}
