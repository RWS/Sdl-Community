5 references should be placed in this folder to work with the project:

Sdl.Core.Globalization.dll
Sdl.Core.PluginFramework.dll
Sdl.LanguagePlatform.Core.dll
Sdl.LanguagePlatform.TranslationMemory.dll
Sdl.LanguagePlatform.TranslationMemoryApi.dll

There are MSBuild tasks in the project that will place the correct versions of these in this folder, according to the build version selected in the Drop-Down. However, if you experience compile-errors related to the references not being found, try manually placing a version of these files in the folder. 

The locations of the correct references are as follows:

Builds targeting Trados Studio 2011:
(ProgramFiles (x86))\SDL\SDL Trados Studio\Studio2

Builds targeting Trados Studio 2014:
(ProgramFiles (x86))\SDL\SDL Trados Studio\Studio3

Builds targeting Trados Studio 2015:
(ProgramFiles (x86))\SDL\SDL Trados Studio\Studio4

You must have the corresponding version of Trados Studio installed on your development machine for the build task to find these references in the above locations or the project won't build correctly.