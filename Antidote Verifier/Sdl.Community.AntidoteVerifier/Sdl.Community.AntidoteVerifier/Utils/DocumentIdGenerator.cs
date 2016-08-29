using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.AntidoteVerifier.Utils
{
    public sealed class DocumentIdGenerator
    {
        private readonly Dictionary<Guid, int> _ids;
        private static readonly Lazy<DocumentIdGenerator> lazy = 
            new Lazy<DocumentIdGenerator>(()=> new DocumentIdGenerator());
        private DocumentIdGenerator()
        {
            _ids = new Dictionary<Guid, int>();
        }

        public static DocumentIdGenerator Instance { get { return lazy.Value; } }

        public int GetDocumentId(Guid id)
        {
            if (_ids.ContainsKey(id))
            {
                return _ids[id];
            }
            else
            {
                var newId = 1;
                if (_ids.Count > 0)
                {
                    var maxValue = _ids.Max(x => x.Value);
                    newId = maxValue+1;
                }
                _ids.Add(id, newId);
                return newId;
            }
        }
    }
}
