![Build Status](https://sdl.visualstudio.com/TradosStudio/_apis/build/status/AppStore/TradosStudio%20-%20AppStore)
![GitHub repo size in bytes](https://img.shields.io/github/repo-size/sdl/sdl-community.svg)
![GitHub](https://img.shields.io/github/license/sdl/sdl-community.svg)

## Table of contents 

1. [Intro](#intro)
2. [Getting started](#getting-started)
3. [List of plugins](#list-of-plugins)
4. [We want your feedback](#we-want-your-feedback)

## Intro

This repository contains around 70 plugins developed for [Trados Studio](https://www.trados.com/product/studio/). A lot of the plugins were developed by Trados Appstore team, but you might also find a few of them which were initially developed by someone else. For all these plugins we now have full source code ownership according to our [License agreement](https://github.com/RWS/Sdl-Community/blob/master/LICENSE). You can find the complete list of plugins [here](#list-of-plugins).

We encourage everyone who is interested to contribute, either by fixing some issues, implementing new features or improving the documentation. To contribute, clone this repository, make the changes and send us a [pull request](http://www.codenewbie.org/blogs/how-to-make-a-pull-request) with your changes.

You can also use this repository for learning by reading and tinkering with real Trados Studio plugins. Please find the documentation [here](http://appstore.sdl.com/developers/sdk.html).

If you have any questions, don't hesitate to ask on the [Language Developer Community](https://community.rws.com/archive/language-developers).

## Getting started


#### Required Tools
To get started with this repository, install the following tools:
1. **SDL Trados Studio 2021**. These libraries are built on top of SDL Trados Studio APIs and thus using them also requires having it installed.
2. **Developer licence**. If you don't have a licence please send an email to app-signing@sdl.com.
3. **.NET 4.8**
4. **Microsoft Visual Studio 2022 or higher**. If you don't have Microsoft Visual Studio you can install the community edition  for free [here](https://www.visualstudio.com/).
5. **Git**. Find more details on installing git [here](https://www.atlassian.com/git/tutorials/install-git#windows). If you prefer a more visual approach you can either use the [github extension for Microsoft Visual Studio](https://visualstudio.github.com/), [Github Desktop](https://desktop.github.com/) or [SourceTree](https://www.sourcetreeapp.com/).

Once you have installed the tools, follow the steps below:

1. Clone this repository (more details [here](https://help.github.com/articles/cloning-a-repository/)). In order to clone the repository using Source Tree, from menu select "Clone/New". In the Source Path paste this path: "https://github.com/sdl/Sdl-Community.git". In the Destination Path, select a path where you want to store the repository on your local drive:
[](https://raw.githubusercontent.com/sdl/Sdl-Community/master/cloneRepository.png)

2. After the repository was cloned, navigate to the repository path you've specified when cloning the repository. Each plugin has a dedicated folder. All you need to do is to find the plugin you're looking for, enter the folder and open the solution file using Microsoft Visual Studio.

3. Build the solution. After the build has succeeded, open SDL Trados Studio 2021. Click "Yes" when the warning message appears. After Studio loads, the built plugin will be available in Studio.

 ![](https://raw.githubusercontent.com/sdl/Sdl-Community/gh-pages/unsignedPlugin.png)



#### Visual Studio SDL templates extensions

Starting with Visual Studio 2017, the Trados Studio plugin templates are provided through Visual Studio extension which can be downloaded from [Marketplace](https://marketplace.visualstudio.com/items?itemName=sdl.project-templates-for-trados-studio-2021) for both **Visual Studio 2017 and Visual Studio 2019**. The extension allow developers to create plugins for Studio 2021 using the predefined templates.

The Github source code for Visual Studio templates extension, can be downloaded from the [Github](https://github.com/sdl/trados-studio-vs-extension) repository.


## List of plugins

The following table shows all the plugins available in the repository. 

By clicking on each plugin name in the table, you'll be redirected to the plugin's download page in SDL App Store. Documentation column will redirect to the source code for the selected plugin.


## Translation Memory Api

| Plugin Name | Description |
| --- | --- |
| [Amazon Translate MT provider](https://appstore.rws.com/Plugin/18) | [Documentation](https://github.com/RWS/Sdl-Community/tree/master/AmazonTranslateTradosPlugin)|
| [ApplyTM Template](https://appstore.rws.com/Plugin/21) | [Documentation](https://github.com/RWS/Sdl-Community/tree/master/ApplyTMTemplate)|
| [DeepL Translation Provider](https://appstore.rws.com/Plugin/24) |  [Documentation](https://github.com/RWS/Sdl-Community/tree/master/DeepLMTProvider)|
| [Google Cloud Translation Provider](https://appstore.rws.com/Plugin/174) |  [Documentation](https://github.com/RWS/Sdl-Community/tree/master/GoogleApiValidator)|
| [Language Weaver Provider](https://appstore.rws.com/Plugin/240) |  [Documentation](https://github.com/RWS/Sdl-Community/tree/master/LanguageWeaverProvider)|
| [Microsoft Translator Provider](https://appstore.rws.com/Plugin/179) |  [Documentation](https://github.com/RWS/Sdl-Community/tree/master/MicrosoftTranslatorProvider)|
| [SDLTM Import Plus](https://appstore.rws.com/Plugin/89) |  [Documentation](https://github.com/RWS/Sdl-Community/tree/master/SDLTM.Import)|
| [SDLTM Repair](https://appstore.rws.com/Plugin/41) |  [Documentation](https://github.com/RWS/Sdl-Community/tree/master/SDLTMRepair)|
| [TM Lifting](https://appstore.rws.com/Plugin/72) |  [Documentation](https://github.com/RWS/Sdl-Community/tree/master/TMLifting)|
| TMX Translation Provider |  [Documentation](https://github.com/RWS/Sdl-Community/tree/master/TMX_TranslationProvider)|
| [TermInjector](http://appstore.sdl.com/app/terminjector/97/) |  [Documentation](https://github.com/sdl/Sdl-Community/tree/master/TermInjector)|
| [Trados Translation Memory Management Utility](https://appstore.rws.com/Plugin/78) |  [Documentation](https://github.com/RWS/Sdl-Community/tree/master/TranslationMemoryManagementUtility)|
| [TuToTm](https://appstore.rws.com/Plugin/79) |  [Documentation](https://github.com/RWS/Sdl-Community/tree/master/TuToTm)|
| memoQ Translation Memory Provider |  [Documentation](https://github.com/RWS/Sdl-Community/tree/master/memoQ%20TM%20Provider)|



## Terminology Provider Api

| Plugin Name | Description |
| --- | --- |
| [IATE Real-time Terminology](https://appstore.rws.com/Plugin/30) |  [Documentation](https://github.com/RWS/Sdl-Community/tree/master/IATETerminologyProvider)|
| [InterpretBank Terminology Provider](https://appstore.rws.com/Plugin/243) |  [Documentation](https://github.com/RWS/Sdl-Community/tree/master/InterpretBank)|
| [Trados Studio InQuote](https://appstore.rws.com/Plugin/55/) |  [Documentation](https://github.com/RWS/Sdl-Community/tree/master/InvoiceAndQuotes)|
| [TermExcelerator](https://appstore.rws.com/Plugin/75) |  [Documentation](https://github.com/RWS/Sdl-Community/tree/master/TermInjector)|


## Integration Api

| Plugin Name | Description |
| --- | --- |
| [AutoHotKey Manager](https://appstore.rws.com/Plugin/22) | [Documentation](https://github.com/RWS/Sdl-Community/tree/master/AHK%20plugin)|
| [Trados Copy Tags](https://appstore.rws.com/Plugin/23) |  [Documentation](https://github.com/RWS/Sdl-Community/tree/master/CopyTags)|
| [DSI Viewer](https://appstore.rws.com/Plugin/25) |  [Documentation](https://github.com/RWS/Sdl-Community/tree/master/DSI%20Viewer)|
| [Trados InSource!](https://appstore.rws.com/Plugin/31) |  [Documentation](https://github.com/RWS/Sdl-Community/tree/master/InSource)|
| [Trados Jobs](https://appstore.rws.com/Plugin/32) |  [Documentation](https://github.com/RWS/Sdl-Community/tree/master/Jobs)|
| [Trados Legit!](https://appstore.rws.com/Plugin/57) |  [Documentation](https://github.com/RWS/Sdl-Community/tree/master/Legit)|
| [Post-Edit Compare](https://appstore.rws.com/Plugin/15) |  [Documentation](https://github.com/RWS/Sdl-Community/tree/master/Post%20Edit%20Compare)|
| [projectTermExtract](https://appstore.rws.com/Plugin/34) |  [Documentation](https://github.com/RWS/Sdl-Community/tree/master/ProjectTerms)|
| [Qualitivity](https://appstore.rws.com/Plugin/16) |  [Documentation](https://github.com/RWS/Sdl-Community/tree/master/Qualitivity)|
| [Rapid Add Term](https://appstore.rws.com/Plugin/35) | [Documentation](https://github.com/RWS/Sdl-Community/tree/master/RapidAddTerm)|
| [Record Source TU](https://appstore.rws.com/Plugin/36) |  [Documentation](https://github.com/RWS/Sdl-Community/tree/master/Record%20Source%20TU)|
| [Reports Viewer Plus](https://appstore.rws.com/Plugin/37) |  [Documentation](https://github.com/RWS/Sdl-Community/tree/master/Reports.Viewer)|
| [Trados Data Protection Suite](https://appstore.rws.com/Plugin/39) |  [Documentation](https://github.com/RWS/Sdl-Community/tree/master/SDLDataProtectionSuite)|
| [Trados Transcreate](https://appstore.rws.com/Plugin/42) |  [Documentation](https://github.com/RWS/Sdl-Community/tree/master/SDLTranscreate)|
| [Segment Status Switcher](https://appstore.rws.com/Plugin/44) |  [Documentation](https://github.com/RWS/Sdl-Community/tree/master/SegmentStatusSwitcher)|
| [TransitPackage Handler](https://appstore.rws.com/Plugin/45) |  [Documentation](https://github.com/RWS/Sdl-Community/tree/master/StarTransit)|
| [studioViews](https://appstore.rws.com/Plugin/12) |  [Documentation](https://github.com/RWS/Sdl-Community/tree/master/StudioViews)|
| [Stylesheet Verifier](https://appstore.rws.com/Plugin/47) |  [Documentation](https://github.com/RWS/Sdl-Community/tree/master/StyleSheetVerifier)|
| [Time Tracker](https://appstore.rws.com/Plugin/76) |  [Documentation](https://github.com/RWS/Sdl-Community/tree/master/Time%20Tracker)|
| [SDLXLIFF Toolkit](https://appstore.rws.com/Plugin/77) |  [Documentation](https://github.com/RWS/Sdl-Community/tree/master/Toolkit)|
| [Variables Manager](https://appstore.rws.com/Plugin/180) |  [Documentation](https://github.com/RWS/Sdl-Community/tree/master/VariablesManager)|
| [Trados Word Cloud](https://appstore.rws.com/Plugin/80) |  [Documentation](https://github.com/RWS/Sdl-Community/tree/master/Word%20Cloud)|
| [XLIFF Manager for Trados Studio](https://appstore.rws.com/Plugin/67) |  [Documentation](https://github.com/RWS/Sdl-Community/tree/master/XLIFF.Manager)|
| [XML Reader](https://appstore.rws.com/Plugin/81) |  [Documentation](https://github.com/RWS/Sdl-Community/tree/master/XML%20Reader)|
| [YourProductivity](https://appstore.rws.com/Plugin/82) |  [Documentation](https://github.com/RWS/Sdl-Community/tree/master/YourProductivity)|


## FileType Support API

| Plugin Name | Description |
| --- | --- |
| [MXLIFF File Type](https://appstore.rws.com/Plugin/29) |  [Documentation](https://github.com/RWS/Sdl-Community/tree/master/FileTypeSupport.MXLIFF)|
| [Multilingual Excel FileType](https://appstore.rws.com/Plugin/17) |  [Documentation](https://github.com/RWS/Sdl-Community/tree/master/Multilingual.Excel.FileType)|
| [Multilingual XML FileType](https://appstore.rws.com/Plugin/13) |  [Documentation](https://github.com/RWS/Sdl-Community/tree/master/Multilingual.XML.FileType)|
| [File type definition for TMX](https://appstore.rws.com/Plugin/61) | [Documentation](https://github.com/RWS/Sdl-Community/tree/master/TMX) |
| [Filetype for Wordfast TXML](https://appstore.rws.com/Plugin/62) |  [Documentation](https://github.com/RWS/Sdl-Community/tree/master/Wordfast%20TXML)|


## Verification API

| Plugin Name | Description |
| --- | --- |
| [Antidote Verifier](https://appstore.rws.com/Plugin/3) |  [Documentation](https://github.com/RWS/Sdl-Community/tree/master/Antidote%20Verifier)|
| [Trados Number Verifier](https://appstore.rws.com/Plugin/33) |  [Documentation](https://github.com/RWS/Sdl-Community/tree/master/Number%20Verifier)|


## Batch Task Api

| Plugin Name | Description |
| --- | --- |
| [CleanUp Tasks](https://appstore.rws.com/Plugin/23) |  [Documentation](https://github.com/RWS/Sdl-Community/tree/master/CleanUpTasks)|
| [Export to Excel](https://appstore.rws.com/Plugin/27) |  [Documentation](https://github.com/RWS/Sdl-Community/tree/master/Export%20to%20Excel)|
| [Fail Safe Task](https://appstore.rws.com/Plugin/28) |  [Documentation](https://github.com/RWS/Sdl-Community/tree/master/FailSafeTask)|
| [Trados Batch Anonymizer](https://appstore.rws.com/Plugin/38) |  [Documentation](https://github.com/RWS/Sdl-Community/tree/master/SDLBatchAnonymize)|
| TQA Reporting |  [Documentation](https://github.com/RWS/Sdl-Community/tree/master/TQA)|
| [Trados Studio Target Renamer](https://appstore.rws.com/Plugin/73) |  [Documentation](https://github.com/RWS/Sdl-Community/tree/master/TargetRenamer)|
| [Target Word Count](https://appstore.rws.com/Plugin/74) |  [Documentation](https://github.com/RWS/Sdl-Community/tree/master/TargetWordCount)|


## Project Automation Api

| Plugin Name | Description |
| --- | --- |
| [Apply Studio Project Template](https://appstore.rws.com/Plugin/20) |  [Documentation](https://github.com/RWS/Sdl-Community/tree/master/ApplyStudioProjectTemplate)|
| [Trados Studio � Export Analysis Reports](https://appstore.rws.com/Plugin/92) |  [Documentation](https://github.com/RWS/Sdl-Community/tree/master/Export%20Analysis%20Reports)|

	
## Standalone Application Api

| Plugin Name | Description |
| --- | --- |
| [Google API Validator](https://appstore.rws.com/Plugin/53) |  [Documentation](https://github.com/RWS/Sdl-Community/tree/master/GoogleApiValidator)|
| [Hunspell Dictionary Manager](https://appstore.rws.com/Plugin/54) |  [Documentation](https://github.com/sdl/Sdl-Community/tree/master/HunspellDictionaryManager)|
| [SDLXLIFF to Legacy Converter](https://appstore.rws.com/Plugin/56) |  [Documentation](https://github.com/RWS/Sdl-Community/tree/master/Legacy%20Converter)|
| [Language Mapping Provider](https://www.nuget.org/packages/LanguageMappingProvider/) |  [Documentation](https://github.com/RWS/Sdl-Community/tree/master/LanguageMappingProvider)|
| [Trados Freshstart](https://appstore.rws.com/Plugin/107) |  [Documentation](https://github.com/RWS/Sdl-Community/tree/master/SdlFreshstart)|
| Trados Proxy Settings |  [Documentation](https://github.com/RWS/Sdl-Community/tree/master/TradosProxySettings)|



## We want your feedback

If you have any suggestions or find any issues please go [here](https://github.com/sdl/SDL-Community/issues) and let us know.
