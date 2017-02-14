using System;
using System.Collections.Generic;

namespace Sdl.Community.Structures.Documents
{
    
    [Serializable]
    public class DocumentStateCounters : ICloneable
    {
    
        public List<StateCountItem> TranslationMatchTypes { get; set; }
        public List<StateCountItem> ConfirmationStatuses { get; set; }        

        public DocumentStateCounters()
        {
            
            TranslationMatchTypes = new List<StateCountItem>();
            ConfirmationStatuses = new List<StateCountItem>();        
        }

        public object Clone()
        {
            var documentStateCounters = new DocumentStateCounters {TranslationMatchTypes = new List<StateCountItem>()};


            foreach (var item in TranslationMatchTypes)
                documentStateCounters.TranslationMatchTypes.Add((StateCountItem)item.Clone());

            documentStateCounters.ConfirmationStatuses = new List<StateCountItem>();
            foreach (var item in ConfirmationStatuses)
                documentStateCounters.ConfirmationStatuses.Add((StateCountItem)item.Clone());


            return documentStateCounters;
        }
    }
}
