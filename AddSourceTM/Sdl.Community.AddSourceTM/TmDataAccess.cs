using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simple.Data;

namespace Sdl.Community.AddSourceTM
{
    public class TmDataAccess
    {
        private const string AtttrName = "Source File";
        private readonly string _tmDatabasePath;
        private dynamic _db;
        private TmDataAccess(string tmDatabasePath)
        {
            _tmDatabasePath = tmDatabasePath;
        }

        public static TmDataAccess OpenConnection(string tmDatabasePath)
        {
            var tmDataAccess = new TmDataAccess(tmDatabasePath);
            tmDataAccess.Init();
            return tmDataAccess;
        }

        public void Init()
        {
            _db = Database.OpenConnection(string.Format(@"Data Source={0};Version=3;", _tmDatabasePath));
        }

        public void AddOrUpdateSourceFile(int tuId,string sourceFile)
        {
            var translationUnit = _db.translation_units.Find(_db.translation_units.id == tuId);

            if (translationUnit == null) return;
           

            var sourceAttribute = _db.attributes.Find(_db.attributes.name == AtttrName) ??
                                  AddSourceAttribute(tuId);
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

        private dynamic AddSourceAttribute(int tuId)
        {
            var translationMemory =
               _db.translation_memories.Find(_db.translation_memories.id == tuId);
            var attribute = _db.attributes.Insert(guid: Guid.NewGuid(), name: AtttrName, type: 2, tm_id: translationMemory.id);

            return attribute;
        }

    }
}
