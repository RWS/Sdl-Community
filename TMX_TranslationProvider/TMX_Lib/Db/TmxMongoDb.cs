using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace TMX_Lib.Db
{
    // example:
    // TmxMongoDb db = new TmxMongoDb("localhost:27017", "mydb");
    public class TmxMongoDb
    {
        // FIXME test with Atlas credentials as well
        private string _url;
        private string _databaseName;
        private bool _connected;

        private MongoClient _client;
        private IMongoDatabase _database;

        private IMongoCollection<TmxMeta> _metas;
        private IMongoCollection<TmxLanguage> _languages;
        private IMongoCollection<TmxTranslationUnit> _translationUnits;
        private IMongoCollection<TmxText> _texts;


        public TmxMongoDb(string url, string databaseName)
        {
            _url = url;
            _databaseName = databaseName;
            Connect();
        }

        private void Connect()
        {
            try
            {
                var url = new MongoUrl($"mongodb://{_url}");
                _client = new MongoClient(url);
                _database = _client.GetDatabase(_databaseName);
                _connected = true;
                Initialize();
            }
            catch (TimeoutException e)
            {
                throw new TmxException($"Timeout connecting to {_databaseName}");
            }
            catch (Exception e)
            {
                throw new TmxException($"Can't connect to {_databaseName}", e);
            }
        }


        private void Initialize()
        {
            if (!_connected)
                return;
            try
            {
                var names = _database.ListCollectionNames().ToList();
                if (!names.Any())
                {
                    _database.CreateCollection("languages");
                    _database.CreateCollection("meta");
                    _database.CreateCollection("texts");
                    _database.CreateCollection("translation_units");
                    CreateIndexes();
                }

                _metas = _database.GetCollection<TmxMeta>("meta");
                _languages = _database.GetCollection<TmxLanguage>("languages");
                _translationUnits = _database.GetCollection<TmxTranslationUnit>("translation_units");
                _texts = _database.GetCollection<TmxText>("texts");
            }
            catch (Exception e)
            {
                throw new TmxException($"Can't initialize MongoDb {_url}", e);
            }
        }

        public async Task ClearAsync()
        {
            try
            {
                if (HasAnyData())
                {
                    // remove everything, we'll restart the import from scratch
                    await _metas.DeleteManyAsync(Builders<TmxMeta>.Filter.Empty);
                    await _languages.DeleteManyAsync(Builders<TmxLanguage>.Filter.Empty);
                    await _translationUnits.DeleteManyAsync(Builders<TmxTranslationUnit>.Filter.Empty);
                    await _texts.DeleteManyAsync(Builders<TmxText>.Filter.Empty);
                }
            }
            catch (Exception e)
            {
                throw new TmxException("Can't initialize db", e);
            }
        }

        public bool HasAnyData()
        {
            try
            {
                return _metas.EstimatedDocumentCount() > 0;
            }
            catch (Exception e)
            {
                throw new TmxException("Error accessing db - HasAnyData", e);
            }
        }

        private void CreateIndexes()
        {
            try
            {
                //_metas.Indexes.CreateOne();
            }
            catch (Exception e)
            {
                throw new TmxException("Can't create indexes", e);
            }
        }

        public async Task AddMetasAsync(IEnumerable<TmxMeta> metas)
        {
            try
            {
                await _metas.InsertManyAsync(metas);
            }
            catch (Exception e)
            {
                throw new TmxException($"Can't add Metas", e);
            }
        }
        public async Task AddLanguagesAsync(IEnumerable<TmxLanguage> languages)
        {
            try
            {
                await _languages.InsertManyAsync(languages);
            }
            catch (Exception e)
            {
                throw new TmxException($"Can't add Languages", e);
            }
        }
        public async Task AddTextsAsync(IEnumerable<TmxText> texts)
        {
            try
            {
                await _texts.InsertManyAsync(texts);
            }
            catch (Exception e)
            {
                throw new TmxException($"Can't add Texts", e);
            }
        }
        public async Task AddTranslationUnitsAsync(IEnumerable<TmxTranslationUnit> translationUnits)
        {
            try
            {
                await _translationUnits.InsertManyAsync(translationUnits);
            }
            catch (Exception e)
            {
                throw new TmxException($"Can't add TranslationUnits", e);
            }
        }



        // new table -> completed_operations - just so i know where i am

    }
}
