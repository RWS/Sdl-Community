using System;
using System.Collections.Generic;
using System.Linq;
using LanguageMappingProvider;
using LanguageMappingProvider.Extensions;
using Xunit;

namespace LanguageMappingProviderFacts.Tests;

public class LanguageMappingDatabaseUnitTests
{
	private readonly List<LanguageMapping> _mappedLanguages;
	private ILanguageMappingDatabase _database;

	public LanguageMappingDatabaseUnitTests()
	{
		_mappedLanguages = new List<LanguageMapping>
			{
				new LanguageMapping { Name = "English", LanguageCode = "en_DEFAULT" },
				new LanguageMapping { Name = "English", Region = "United States", LanguageCode = "en_US" },
			};

		_database = new LanguageMappingDatabase("unittesting2", _mappedLanguages);
	}

	[Fact]
	public void Initialization_Successful()
	{
		_database = new LanguageMappingDatabase("test", _mappedLanguages);
		Assert.NotNull(_database);
	}

	[Fact]
	public void Initialization_NullCollection_ThrowsException()
	{
		var exception = Assert.Throws<DatabaseInitializationException>(() =>
		{
			_database = new LanguageMappingDatabase("invalid", null);
		});

		var expectedErrorMessage = "The pluginSupportedLanguages collection must be provided and must contain at least one supported language in order to initialize this class and create the database";
		Assert.Contains(expectedErrorMessage, exception.Message);
	}

	[Fact]
	public void Initialization_EmptyCollection_ThrowsException()
	{
		var exception = Assert.Throws<DatabaseInitializationException>(() =>
		{
			_database = new LanguageMappingDatabase("invalid", new List<LanguageMapping>());
		});

		var expectedErrorMessage = "The pluginSupportedLanguages collection must be provided and must contain at least one supported language in order to initialize this class and create the database";
		Assert.Contains(expectedErrorMessage, exception.Message);
	}

	[Fact]
	public void InsertLanguage_CanInsertNewLanguages()
	{
		_database.InsertLanguage(new LanguageMapping()
		{
			Name = "TestingName",
			Region = "TestingRegion",
			TradosCode = "TestingTradosCode",
			LanguageCode = "TestingLanguageCode"
		});

		var mappedLanguages = _database.GetMappedLanguages();
		var mappedLanguage = mappedLanguages.FirstOrDefault(x => x.Name == "TestingName");

		Assert.NotNull(mappedLanguage);
		Assert.Equal("TestingName", mappedLanguage.Name);
		Assert.Equal("TestingRegion", mappedLanguage.Region);
		Assert.Equal("TestingTradosCode", mappedLanguage.TradosCode);
		Assert.Equal("TestingLanguageCode", mappedLanguage.LanguageCode);

		_database.ResetToDefault();
	}

	[Fact]
	public void InsertLanguage_CanInsertDuplicates()
	{
		var newMapping = new LanguageMapping()
		{
			Index = 999,
			Name = "TestingName",
			Region = "TestingRegion",
			TradosCode = "TestingTradosCode",
			LanguageCode = "TestingLanguageCode"
		};

		_database.InsertLanguage(newMapping);
		_database.InsertLanguage(newMapping);
		_database.ResetToDefault();
	}

	[Fact]
	public void InsertLanguage_NullMapping_ThrowsException()
	{
		var exception = Assert.Throws<MappedLanguageNullException>(() =>
		{
			_database.InsertLanguage(null);
		});

		Assert.Contains("Mapped language cannot be null", exception.Message);
		Assert.Equal("mappedLanguage", exception.ParamName);
	}

	[Theory]
	[InlineData(null, "eng", "Name")]
	[InlineData("", "eng", "Name")]
	[InlineData("English", null, "TradosCode")]
	[InlineData("English", "", "TradosCode")]
	public void InsertLanguage_InvalidMappedLanguage_InvalidName_ThrowsException(string mappedName, string mappedCode, string property)
	{
		var mapping = new LanguageMapping()
		{
			Name = mappedName,
			TradosCode = mappedCode
		};

		var exception = Assert.Throws<MappedLanguageValidationException>(() =>
		{
			_database.InsertLanguage(mapping);
		});

		Assert.Contains("Mapped language property must be set", exception.Message);
		Assert.Equal(property, exception.ParamName);
	}

	[Fact]
	public void UpdateAll_CanUpdateAllModifications()
	{
		var originalMappedLanguages = _database.GetMappedLanguages().ToList();
		var randomIndex = new Random().Next(originalMappedLanguages.Count);
		var originalMappedLanguage = originalMappedLanguages[randomIndex];
		Assert.NotEqual("modified", originalMappedLanguage.LanguageCode);

		originalMappedLanguage.LanguageCode = "modified";
		_database.UpdateAll(originalMappedLanguages);
		var updatedDatabase = _database.GetMappedLanguages();

		var updatedMappedLanguage = updatedDatabase.ElementAt(randomIndex);
		Assert.Equal(updatedMappedLanguage.Name, originalMappedLanguage.Name);
		Assert.Equal(updatedMappedLanguage.Region, originalMappedLanguage.Region);
		Assert.Equal(updatedMappedLanguage.TradosCode, originalMappedLanguage.TradosCode);
		Assert.Equal("modified", updatedDatabase.ElementAt(randomIndex).LanguageCode);
		_database.ResetToDefault();
	}

	[Fact]
	public void UpdateAll_NullOrEmptyCollection_DoesNotThrowsExceptions()
	{
		_database.UpdateAll(null);
		_database.UpdateAll(new List<LanguageMapping>());

		// If this test passes, no errors were thrown above
		Assert.True(true);
	}

	[Fact]
	public void UpdateAt_ModifyLanguage()
	{
		var originalMappedLanguages = _database.GetMappedLanguages().ToList();
		var randomIndex = new Random().Next(originalMappedLanguages.Count);
		var originalMappedLanguage = originalMappedLanguages[randomIndex];

		Assert.NotEqual("modified", originalMappedLanguage.LanguageCode);

		_database.UpdateAt(originalMappedLanguage.Index, nameof(originalMappedLanguage.LanguageCode), "modified");
		var updatedDatabase = _database.GetMappedLanguages();

		var updatedMappedLanguage = updatedDatabase.ElementAt(randomIndex);
		Assert.Equal(updatedMappedLanguage.Name, originalMappedLanguage.Name);
		Assert.Equal(updatedMappedLanguage.Region, originalMappedLanguage.Region);
		Assert.Equal(updatedMappedLanguage.TradosCode, originalMappedLanguage.TradosCode);
		Assert.Equal("modified", updatedDatabase.ElementAt(randomIndex).LanguageCode);
		_database.ResetToDefault();
	}

	[Theory]
	[InlineData(int.MinValue)]
	[InlineData(int.MaxValue)]
	public void UpdateAt_InvalidIndex_ThrowsException(int index)
	{
		var exception = Assert.Throws<MappedLanguageIndexOutOfRangeException>(() =>
		{
			_database.UpdateAt(index, "LanguageCode", "newValue");
		});

		Assert.Contains("The provided index is out of range for the mapped languages", exception.Message);
	}

	[Theory]
	[InlineData(null, "newValue", "property")]
	[InlineData("", "newValue", "property")]
	[InlineData("LanguageCode", null, "value")]
	[InlineData("LanguageCode", "", "value")]
	public void UpdateAt_InvalidParameters_ThrowsException(string property, string value, string exceptionLocation)
	{
		var exception = Assert.Throws<MappedLanguageValidationException>(() =>
		{
			_database.UpdateAt(1, property, value);
		});

		Assert.Equal(exceptionLocation, exception.ParamName);
		Assert.Contains($"The {exceptionLocation} must be set", exception.Message);
	}

	[Fact]
	public void HasMappedLanguagesChanged_InvalidMappedLanguages_ReturnsFalse()
	{
		Assert.False(_database.HasMappedLanguagesChanged(null));
		Assert.False(_database.HasMappedLanguagesChanged(new List<LanguageMapping>()));
	}

	[Theory]
	[InlineData("Name")]
	[InlineData("Region")]
	[InlineData("TradosCode")]
	[InlineData("LanguageCode")]
	public void HasMappedLanguagesChanged_ReturnsTrue(string property)
	{
		var mappedLanguages = _database.GetMappedLanguages().ToList();
		Assert.False(_database.HasMappedLanguagesChanged(mappedLanguages));

		var randomIndex = new Random().Next(mappedLanguages.Count);
		var language = mappedLanguages[randomIndex];
		switch (property)
		{
			case "Name":
				language.Name = "Modified";
				break;
			case "Region":
				language.Region = "Modified";
				break;
			case "TradosCode":
				language.TradosCode = "Modified";
				break;
			case "LanguageCode":
				language.LanguageCode = "Modified";
				break;
		}

		Assert.True(_database.HasMappedLanguagesChanged(mappedLanguages));
	}

	[Fact]
	public void HasMappedLanguagesChanged_IndexNotSet_ReturnsFalse()
	{
		Assert.False(_database.HasMappedLanguagesChanged(_database.GetMappedLanguages()));
	}

	[Fact]
	public void HasMappedLanguagesChanged_DuplicateIndexes_ReturnsFalse()
	{
		var exception = Assert.Throws<DuplicateIndexException>(() =>
		{
			_database.HasMappedLanguagesChanged(_mappedLanguages);
		});

		Assert.Contains("Duplicate index detected in the provided collection", exception.Message);
	}

	[Fact]
	public void GetMappedLanguages_ReturnsData()
	{
		var result = _database.GetMappedLanguages();
		Assert.NotNull(result);
		Assert.NotNull(_database.GetMappedLanguages().FirstOrDefault());
		Assert.NotEmpty(result);
	}
}