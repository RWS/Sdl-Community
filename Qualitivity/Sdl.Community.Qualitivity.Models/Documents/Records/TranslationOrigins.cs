using System;

namespace Sdl.Community.Structures.Documents.Records
{
   
    [Serializable]
    public class TranslationOrigins : ICloneable
    {
       
        public TranslationOrigin Original { get; set; }        
        public TranslationOrigin Updated { get; set; }      
        public TranslationOrigin UpdatedPrevious { get; set; }
        
               
        public TranslationOrigins()
        {
            Original = new TranslationOrigin( TranslationOrigin.LanguageType.Original);            
            Updated = new TranslationOrigin(TranslationOrigin.LanguageType.Updated);            
            UpdatedPrevious = new TranslationOrigin(TranslationOrigin.LanguageType.UpdatedPrevious);
        }

        public object Clone()
        {
            var translationOrigins = new TranslationOrigins
            {
                Original = Original,
                Updated = Updated,
                UpdatedPrevious = UpdatedPrevious
            };

            return translationOrigins;
        }
    }
}
