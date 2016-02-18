using Sdl.Core.Globalization;
using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.IO;

namespace Sdl.Community.TMOptimizer
{
    /// <summary>
    /// Create an empty Studio TM
    /// </summary>
    class CreateStudioTmStep : ProcessingStep
    {
        private TranslationMemoryReference _tm;
        private Language _sourceLanguage;
        private Language _targetLanguage;
        private TranslationMemoryReference _templateTranslationMemory;

        public CreateStudioTmStep(
            TranslationMemoryReference tm, 
            Language sourceLanguage, 
            Language targetLanguage, 
            TranslationMemoryReference templateTranslationMemory) : base(String.Format("Create output TM {0}", Path.GetFileName(tm.FilePath)))
        {
            _tm = tm;
            _sourceLanguage = sourceLanguage;
            _targetLanguage = targetLanguage;
            _templateTranslationMemory = templateTranslationMemory;
        }

        protected override void ExecuteImpl()
        {
            CultureInfo sourceCulture;
            CultureInfo targetCulture;
            FuzzyIndexes fuzzyIndexes;
            BuiltinRecognizers recognizers;
            if (_templateTranslationMemory != null && _templateTranslationMemory.FilePath != null)
            {
                sourceCulture = _templateTranslationMemory.TranslationMemory.LanguageDirection.SourceLanguage;
                targetCulture = _templateTranslationMemory.TranslationMemory.LanguageDirection.TargetLanguage;
                fuzzyIndexes = _templateTranslationMemory.TranslationMemory.FuzzyIndexes;
                recognizers = _templateTranslationMemory.TranslationMemory.Recognizers;
            }
            else
            {
                sourceCulture = _sourceLanguage.CultureInfo;
                targetCulture = _targetLanguage.CultureInfo;
                fuzzyIndexes = FuzzyIndexes.SourceWordBased | FuzzyIndexes.TargetWordBased;
                recognizers = BuiltinRecognizers.RecognizeAll;
            }

            FileBasedTranslationMemory tm = new FileBasedTranslationMemory(
                    _tm.FilePath,
                    String.Empty,
                    sourceCulture,
                    targetCulture,
                    fuzzyIndexes,
                    recognizers,
                    TokenizerFlags.DefaultFlags,
                    WordCountFlags.DefaultFlags );

            ReportProgress(25);
            tm.Save();
            ReportProgress(50);

            if (_templateTranslationMemory != null && _templateTranslationMemory.FilePath != null)
            {
                CopyTmLanguageResources(_templateTranslationMemory.TranslationMemory, tm);
                CopyTmFieldDefinitions(_templateTranslationMemory.TranslationMemory, tm);
            }

            ReportProgress(75);

            tm.Save();

            ReportProgress(100);
        }

        private void CopyTmLanguageResources(FileBasedTranslationMemory templateTm, FileBasedTranslationMemory tm)
        {
            ITranslationMemoryLanguageDirection languageDirection = tm.LanguageDirection;

            // Copy any sourcelanguage resources from the template tm to the new tm
            LanguageResourceBundle sourceLanguageBundle = templateTm.LanguageResourceBundles[languageDirection.SourceLanguage];
            if (sourceLanguageBundle != null)
            {
                tm.LanguageResourceBundles.Add(sourceLanguageBundle.Clone());
            }

            // Copy any target language resources from the template tm to the new tm
            LanguageResourceBundle targetLanguageBundle = templateTm.LanguageResourceBundles[languageDirection.TargetLanguage];
            if (targetLanguageBundle != null)
            {
                tm.LanguageResourceBundles.Add(targetLanguageBundle.Clone());
            }

        }

        private void CopyTmFieldDefinitions(FileBasedTranslationMemory templateTm, FileBasedTranslationMemory tm)
        {
            foreach (FieldDefinition field in templateTm.FieldDefinitions)
            {
                tm.FieldDefinitions.Add(field.Clone());
            }
        }
    }
}
