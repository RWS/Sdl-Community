# Language Mapping Provider

Language Mapping Provider is a library that provides functionality for managing language mappings in a database. It allows you to insert, update, and retrieve language mappings for various purposes, such as language code conversions or language-specific operations.

## Features

- Insert a new language mapping into the database
- Update existing language mappings
- Retrieve language mappings from the database
- Check if language mappings have changed
- Reset language mappings to their default values

## Installation

Language Mapping Provider is available as a [NuGet package](https://www.nuget.org/packages/LanguageMappingProvider/). You can install it using the package manager console or the NuGet Package Manager in Visual Studio.

1. Open the package manager console by going to **Tools > NuGet Package Manager > Package Manager Console**.

2. Run the following command to install the Language Mapping Provider package:

```shell
Install-Package LanguageMappingProvider
```

Once the package is installed, you can start using the Language Mapping Provider library in your project.

## Usage

To use Language Mapping Provider in your project, follow these steps:

1. Create an instance of the **LanguageMappingDatabase** class by providing a unique identifier for the plugin and a collection of supported languages:

```
string pluginName = "YourPluginName";
List<MappedLanguage> supportedLanguages = new List<MappedLanguage>
{
    // Add your supported languages here
};

LanguageMappingDatabase database = new LanguageMappingDatabase(pluginName, supportedLanguages);
```
2. Use the available methods of the LanguageMappingDatabase class to perform operations on the language mappings:

* **InsertLanguage**: Insert a new language mapping into the database.
* **UpdateAll**: Update multiple language mappings with modified values.
* **UpdateAt**: Update a specific field of a language mapping at a given index.
* **HasMappedLanguagesChanged**: Check if language mappings have changed compared to a given collection.
* **GetMappedLanguages**: Retrieve language mappings from the database.
* **ResetToDefault**: Reset language mappings to their default values.

```csharp
// Create a database instance
LanguageMappingDatabase database = new LanguageMappingDatabase("YourPluginName", supportedLanguages);

// Insert a new language mapping
MappedLanguage newMapping = new MappedLanguage { Name = "English", Region = "US", TradosCode = "en-US", LanguageCode = "en" };
database.InsertLanguage(newMapping);

// Retrieve all language mappings
IEnumerable<MappedLanguage> mappings = database.GetMappedLanguages();

// Update a specific language mapping
var mappingToUpdate = mappings.FirstOrDefault(m => m.Name == "English" && m.Region == "US");
if (mappingToUpdate != null)
{
    mappingToUpdate.LanguageCode = "eng";
    database.UpdateAt(mappingToUpdate.Index, nameof(mappingToUpdate.LanguageCode), mappingToUpdate.LanguageCode);
}

// Check if language mappings have changed
bool hasChanges = database.HasMappedLanguagesChanged(mappings);

// Reset language mappings to default values
database.ResetToDefault();
```

## Contributing

Contributions are welcome! If you find any issues or have suggestions for improvements, please create a new issue or submit a pull request on the GitHub repository.
