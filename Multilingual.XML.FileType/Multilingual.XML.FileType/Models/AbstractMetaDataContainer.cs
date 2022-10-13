using System.Collections.Generic;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Multilingual.XML.FileType.Models
{
	public abstract class AbstractMetaDataContainer : IMetaDataContainer
    {
        protected Dictionary<string, string> _MetaData = new Dictionary<string, string>();

        public AbstractMetaDataContainer()
        {

        }

        protected AbstractMetaDataContainer(AbstractMetaDataContainer other) => ReplaceMetaDataWithCloneOf(other._MetaData);

        public void ReplaceMetaDataWithCloneOf(IEnumerable<KeyValuePair<string, string>> toClone)
        {
            _MetaData.Clear();
            foreach (var keyValuePair in toClone)
            {
	            _MetaData.Add(keyValuePair.Key, keyValuePair.Value);
            }
        }

        public override bool Equals(object obj)
        {
	        if (obj == null || GetType() != obj.GetType())
	        {
		        return false;
	        }

            var metaDataContainer = (AbstractMetaDataContainer)obj;
            if (_MetaData.Count != metaDataContainer._MetaData.Count)
            {
	            return false;
            }

            foreach (var keyValuePair in _MetaData)
            {
	            if (!metaDataContainer._MetaData.TryGetValue(keyValuePair.Key, out var str) || 
	                str == null != (keyValuePair.Value == null) || 
	                str != null && !str.Equals(keyValuePair.Value))
                    return false;
            }
            return true;
        }

        public override int GetHashCode()
        {
            var num = 0;
            foreach (var keyValuePair in _MetaData)
            {
	            num ^= keyValuePair.Key.GetHashCode() << 16 ^ (keyValuePair.Value ?? "").GetHashCode();
            }
            return num;
        }

        public override string ToString() => _MetaData.ToString();

        public void ClearMetaData() => _MetaData.Clear();

        public string GetMetaData(string key)
        {
			return _MetaData.TryGetValue(key, out var str) ? str : null;
		}

        public bool HasMetaData => _MetaData.Count > 0;

        public IEnumerable<KeyValuePair<string, string>> MetaData => _MetaData;

        public bool MetaDataContainsKey(string key) => _MetaData.ContainsKey(key);

        public int MetaDataCount => _MetaData.Count;

        public bool RemoveMetaData(string key) => _MetaData.Remove(key);

        public void SetMetaData(string key, string value) => _MetaData[key] = value;
    }
}
