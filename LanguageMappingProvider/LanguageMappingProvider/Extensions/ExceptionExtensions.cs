using System;

namespace LanguageMappingProvider.Extensions
{
	public class DatabaseInitializationException : Exception
    {
        public DatabaseInitializationException()
            : base("The pluginSupportedLanguages collection must be provided and must contain at least one supported language in order to initialize this class and create the database.") { }
    }

    public class MappedLanguageNullException : ArgumentNullException
    {
        public MappedLanguageNullException(string paramName)
            : base(paramName, "Mapped language cannot be null.") { }
    }

    public class MappedLanguageValidationException : ArgumentException
    {
        public MappedLanguageValidationException(string message, string paramName)
            : base(message, paramName) { }
    }

    public class MappedLanguageIndexOutOfRangeException : Exception
    {
        public MappedLanguageIndexOutOfRangeException()
            : base("The provided index is out of range for the mapped languages.") { }
    }

    public class DuplicateIndexException : Exception
    {
        public DuplicateIndexException()
            : base("Duplicate index detected in the provided collection.") { }
    }

	public class LanguageNotFoundException : Exception
	{
		public LanguageNotFoundException()
			: base("The specified language does not match any language in the database.") { }
	}
}