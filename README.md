![Build Status](https://sdl.visualstudio.com/TradosStudio/_apis/build/status/AppStore/TradosStudio%20-%20AppStore)
![GitHub repo size in bytes](https://img.shields.io/github/repo-size/sdl/sdl-community.svg)
![GitHub](https://img.shields.io/github/license/sdl/sdl-community.svg)

## Table of contents 

1. [Intro](#intro)
2. [Getting started](#getting-started)
3. [List of plugins](#list-of-plugins)
4. [We want your feedback](#we-want-your-feedback)

## Intro

This repository contains around 30 plugins developed for [Trados Studio](http://www.sdl.com/solution/language/translation-productivity/trados-studio/). Most of the plugins were developed by SDL, but you might also find a few of them which were initially developed by someone else. For all these plugins we now have full source code ownership according to our [License agreement](https://github.com/sdl/Sdl-Community/blob/master/License.md). You can find the complete list of plugins [here](#list-of-plugins).

We encourage everyone who is interested to contribute, either by fixing some issues, implementing new features or improving the documentation. To contribute, clone this repository, make the changes and send us a [pull request](http://www.codenewbie.org/blogs/how-to-make-a-pull-request) with your changes.

You can also use this repository for learning by reading and tinkering with real Trados Studio plugins. Please find the documentation [here](http://appstore.sdl.com/developers/sdk.html).

If you have any questions, don't hesitate to ask on the [Sdl Language Developer Community](https://community.sdl.com/developers/language-developers/).

## Getting started


#### Required Tools
To get started with this repository, install the following tools:
1. **SDL Trados Studio 2019**. These libraries are built on top of SDL Trados Studio APIs and thus using them also requires having it installed.
2. **Developer licence**. If you don't have a licence please send an email to app-signing@sdl.com.
3. **.NET 4.7.2**
4. **Microsoft Visual Studio 2013, 2015 or 2017**. If you don't have Microsoft Visual Studio you can install the community edition  for free [here](https://www.visualstudio.com/).
5. **Git**. Find more details on installing git [here](https://www.atlassian.com/git/tutorials/install-git#windows). If you prefer a more visual approach you can either use the [github extension for Microsoft Visual Studio](https://visualstudio.github.com/), [Github Desktop](https://desktop.github.com/) or [SourceTree](https://www.sourcetreeapp.com/).

Once you have installed the tools, follow the steps below:

1. Clone this repository (more details [here](https://help.github.com/articles/cloning-a-repository/)). In order to clone the repository using Source Tree, from menu select "Clone/New". In source path paste this path: "https://github.com/sdl/Sdl-Community.git". In destination path, select a path where you want to store the repository on your drive:
[](https://raw.githubusercontent.com/sdl/Sdl-Community/master/cloneRepository.png)

2. After the repository was cloned, navigate to the repository path you've specified when cloning the repository. Each plugin has a dedicated folder. All you need to do is to find the plugin you're looking for, enter the folder and open the solution file using Microsoft Visual Studio.

3. Build the solution. After the build has succeeded, open SDL Trados Studio 2019. Click "Yes" when the warning message appears. After Studio loads, the built plugin will be available in Studio.

 ![](https://raw.githubusercontent.com/sdl/Sdl-Community/gh-pages/unsignedPlugin.png)



#### Visual Studio SDL templates extensions

Starting with Visual Studio 2017, the Trados Studio plugin templates are provided through Visual Studio extension:
1. For **Visual Studio 2017**, the SDL plugins templates extension can be downloaded from here: https://marketplace.visualstudio.com/items?itemName=sdl.project-templates-for-trados-studio-2019.
2. For **Visual Studio 2019**, the SDL plugins templates extension can be downloaded from here:(link to be added after publication was done).

The Github source code for Visual Studio 2019 templates extension, can be downloaded from: https://github.com/sdl/trados-studio-vs-extension


## List of plugins

The following table shows all the plugins available in the repository. 

By clicking on each plugin name in the table, you'll be redirected to the plugin's download page in SDL App Store. Documentation column will redirect to the source code for the selected plugin.

## Batch Task Api

| Plugin Name | Description |
| --- | --- |
| [Export to Excel](http://appstore.sdl.com/app/export-to-excel/532/) |  [Documentation](https://github.com/sdl/Sdl-Community/blob/master/Export%20to%20Excel/)|
| [Project Anonymizer](https://appstore.sdl.com/language/app/projectanonymizer/895/) |  [Documentation](https://github.com/sdl/Sdl-Community/tree/master/Anonymizer)|
| [CleanUp Tasks](https://appstore.sdl.com/language/app/cleanup-tasks/963/) |  [Documentation](https://github.com/sdl/Sdl-Community/tree/master/CleanUpTasks)|
| [Fail Safe Task](https://appstore.sdl.com/language/app/fail-safe-task/964/) |  [Documentation](https://github.com/sdl/Sdl-Community/tree/master/FailSafeTask)|
| [Target Word Count](https://appstore.sdl.com/language/app/target-word-count/965/) |  [Documentation](https://github.com/sdl/Sdl-Community/tree/master/TargetWordCount)|



## Core Api

| Plugin Name | Description |
| --- | --- |
| [Apply Studio Project Template](http://appstore.sdl.com/app/apply-studio-project-template/391/) |  [Documentation](https://github.com/sdl/Sdl-Community/tree/master/ApplyStudioProjectTemplate)|
| [Export to Excel](http://appstore.sdl.com/app/export-to-excel/532/) |  [Documentation](https://github.com/sdl/Sdl-Community/blob/master/Export%20to%20Excel/)|
| [Legacy Converter](http://appstore.sdl.com/app/sdlxliff-to-legacy-converter/134/) |  [Documentation](https://github.com/sdl/Sdl-Community/tree/master/Legacy%20Converter)|
| [Number verifier](http://appstore.sdl.com/app/sdl-number-verifier/440/) |  [Documentation](https://github.com/sdl/Sdl-Community/tree/master/Number%20Verifier)|
| [Post Edit Compare](https://appstore.sdl.com/language/app/post-edit-compare/610/) |  [Documentation](https://github.com/sdl/Sdl-Community/tree/master/Post%20Edit%20Compare)|
| [Segment Status Switcher](http://appstore.sdl.com/app/segment-status-switcher/754/) |  [Documentation](https://github.com/sdl/Sdl-Community/tree/master/SegmentStatusSwitcher)|
| [Star Transit](http://appstore.sdl.com/app/transitpackage-handler/573/) |  [Documentation](https://github.com/sdl/Sdl-Community/tree/master/StarTransit)|
| [SDL TM Anonymizer]() |  [Documentation](https://github.com/sdl/Sdl-Community/tree/master/TmAnonymizer)|


## Project Automation Api

| Plugin Name | Description |
| --- | --- |
| [Apply Studio Project Template](http://appstore.sdl.com/app/apply-studio-project-template/391/) |  [Documentation](https://github.com/sdl/Sdl-Community/tree/master/ApplyStudioProjectTemplate)|
| [Post Edit Compare](https://appstore.sdl.com/language/app/post-edit-compare/610/) |  [Documentation](https://github.com/sdl/Sdl-Community/tree/master/Post%20Edit%20Compare)|
| [Project Anonymizer](https://appstore.sdl.com/language/app/projectanonymizer/895/) |  [Documentation](https://github.com/sdl/Sdl-Community/tree/master/Anonymizer)|
| [InSource](http://appstore.sdl.com/app/sdl-insource/548/) |  [Documentation](https://github.com/sdl/Sdl-Community/blob/master/InSource)|
| [Project Terms](https://appstore.sdl.com/language/app/projecttermextract/817/) |  [Documentation](https://github.com/sdl/Sdl-Community/tree/master/ProjectTerms)|
| [Record Source TU](http://appstore.sdl.com/app/record-source-tu/504/) |  [Documentation](https://github.com/sdl/Sdl-Community/tree/master/Record%20Source%20TU)|
| [Report Exporter](https://appstore.sdl.com/language/app/sdl-trados-studio-export-analysis-reports/3/) |  [Documentation](https://github.com/sdl/Sdl-Community/tree/master/Report%20Exporter)|
| [Studio Migration Utility](http://appstore.sdl.com/app/studio-migration-utility/481/) |  [Documentation](https://github.com/sdl/Sdl-Community/tree/master/Studio%20Migration%20Utility)|

## Global Verifiers Api

| Plugin Name | Description |
| --- | --- |
| [Number verifier](http://appstore.sdl.com/app/sdl-number-verifier/440/) |  [Documentation](https://github.com/sdl/Sdl-Community/tree/master/Number%20Verifier)|


## Terminology Provider Api

| Plugin Name | Description |
| --- | --- |
| [Excel Termonology](https://appstore.sdl.com/language/app/termexcelerator/534/) |  [Documentation](https://github.com/sdl/Sdl-Community/tree/master/Sdl.Community.ExcelTerminology)|
| [IATE Termonology Provider](https://appstore.sdl.com/language/app/iate-terminology/950/) |  [Documentation](https://github.com/sdl/Sdl-Community/tree/master/IATETerminologyProvider)|

## Translation Memory Api

| Plugin Name | Description |
| --- | --- |
| [SDL TM Anonymizer]() |  [Documentation](https://github.com/sdl/Sdl-Community/tree/master/TmAnonymizer)|
| [Record Source TU](http://appstore.sdl.com/app/record-source-tu/504/) |  [Documentation](https://github.com/sdl/Sdl-Community/tree/master/Record%20Source%20TU)|
| [DeepL Translation Provider](https://appstore.sdl.com/language/app/deepl-translation-provider/847/) |  [Documentation](https://github.com/sdl/Sdl-Community/tree/master/DeepLMTProvider)|
| [MT Enhanced Provider](http://appstore.sdl.com/app/mt-enhanced-plugin-for-trados-studio/604/) |  [Documentation](https://github.com/sdl/Sdl-Community/tree/master/MT%20Enhanced%20Provider)|
| [Term Injector](http://appstore.sdl.com/app/terminjector/97/) |  [Documentation](https://github.com/sdl/Sdl-Community/tree/master/TermInjector)|
| [Record Source TU](http://appstore.sdl.com/app/record-source-tu/504/) |  [Documentation](https://github.com/sdl/Sdl-Community/tree/master/Record%20Source%20TU)|
| [TM Optimizer](http://appstore.sdl.com/app/tm-optimizer/347/) |  [Documentation](https://github.com/sdl/Sdl-Community/tree/master/TM%20Optimizer)|
| [TM Lifting](http://appstore.sdl.com/app/tm-lifting/419/) |  [Documentation](https://github.com/sdl/Sdl-Community/tree/master/TMLifting)|
| [BeGlobalV4 Translation Provider](https://appstore.sdl.com/language/app/sdl-beglobal-nmt/941/) |  [Documentation](https://github.com/sdl/Sdl-Community/tree/master/Be%20GlobalV4%20Translation%20Provider)|
| [Amazon Translate](https://appstore.sdl.com/language/app/amazon-translate-mt-provider/925/) |  [Documentation](https://github.com/sdl/Sdl-Community/tree/master/AmazonTranslateTradosPlugin)|
| [ETS Translation Provider](https://appstore.sdl.com/language/app/sdl-ets/843/) |  [Documentation](https://github.com/sdl/Sdl-Community/tree/master/ETS%20Translation%20Provider)|
| [ApplyTM Template](https://appstore.sdl.com/language/app/applytm-template/966/) |  [Documentation](https://github.com/sdl/Sdl-Community/tree/master/ApplyTMTemplate)|

## Integration Api

| Plugin Name | Description |
| --- | --- |
| [Apply Studio Project Template](http://appstore.sdl.com/app/apply-studio-project-template/391/) |  [Documentation](https://github.com/sdl/Sdl-Community/tree/master/ApplyStudioProjectTemplate)|
| [Export to Excel](http://appstore.sdl.com/app/export-to-excel/532/) |  [Documentation](https://github.com/sdl/Sdl-Community/blob/master/Export%20to%20Excel/)|
| [Post Edit Compare](https://appstore.sdl.com/language/app/post-edit-compare/610/) |  [Documentation](https://github.com/sdl/Sdl-Community/tree/master/Post%20Edit%20Compare)|
| [Segment Status Switcher](http://appstore.sdl.com/app/segment-status-switcher/754/) |  [Documentation](https://github.com/sdl/Sdl-Community/tree/master/SegmentStatusSwitcher)|
| [InSource](http://appstore.sdl.com/app/sdl-insource/548/) |  [Documentation](https://github.com/sdl/Sdl-Community/blob/master/InSource)|
| [Record Source TU](http://appstore.sdl.com/app/record-source-tu/504/) |  [Documentation](https://github.com/sdl/Sdl-Community/tree/master/Record%20Source%20TU)|
| [Term Injector](http://appstore.sdl.com/app/terminjector/97/) |  [Documentation](https://github.com/sdl/Sdl-Community/tree/master/TermInjector)|
| [Community Advenced Display Filter](https://appstore.sdl.com/language/app/community-advanced-display-filter/849/) | [Documentation](https://github.com/sdl/Sdl-Community/tree/master/AdvancedDisplayFilter) |
| [AutoHotKey Manager](https://appstore.sdl.com/language/app/autohotkey-manager/893/) | [Documentation](https://github.com/sdl/Sdl-Community/tree/master/AHK%20plugin) |
| [Qualitivity](http://appstore.sdl.com/app/qualitivity/612/) |  [Documentation](https://github.com/sdl/Sdl-Community/tree/master/Qualitivity)|
| [AutoHotKey Manager](https://appstore.sdl.com/language/app/autohotkey-manager/893/) |  [Documentation](https://github.com/sdl/Sdl-Community/tree/master/AHK%20plugin)|
| [DSI Viewer](https://appstore.sdl.com/language/app/dsi-viewer/995/) |  [Documentation](https://github.com/sdl/Sdl-Community/tree/master/DSI%20Viewer)|
| [GrpupShare Version Fetch](https://appstore.sdl.com/language/app/groupshare-version-fetch/993/) |  [Documentation](https://github.com/sdl/Sdl-Community/tree/master/GroupShare%20VersionFetch)|


## FileType Support API

| Plugin Name | Description |
| --- | --- |
| [Export to Excel](http://appstore.sdl.com/app/export-to-excel/532/) |  [Documentation](https://github.com/sdl/Sdl-Community/blob/master/Export%20to%20Excel/)|
| [Legacy Converter](http://appstore.sdl.com/app/sdlxliff-to-legacy-converter/134/) |  [Documentation](https://github.com/sdl/Sdl-Community/tree/master/Legacy%20Converter)|
| [Number verifier](http://appstore.sdl.com/app/sdl-number-verifier/440/) |  [Documentation](https://github.com/sdl/Sdl-Community/tree/master/Number%20Verifier)|
| [Post Edit Compare](https://appstore.sdl.com/language/app/post-edit-compare/610/) |  [Documentation](https://github.com/sdl/Sdl-Community/tree/master/Post%20Edit%20Compare)|
| [Star Transit](http://appstore.sdl.com/app/transitpackage-handler/573/) |  [Documentation](https://github.com/sdl/Sdl-Community/tree/master/StarTransit)|
| [Project Anonymizer](https://appstore.sdl.com/language/app/projectanonymizer/895/) |  [Documentation](https://github.com/sdl/Sdl-Community/tree/master/Anonymizer)|
| [Project Terms](https://appstore.sdl.com/language/app/projecttermextract/817/) |  [Documentation](https://github.com/sdl/Sdl-Community/tree/master/ProjectTerms)|
| [Qualitivity](http://appstore.sdl.com/app/qualitivity/612/) |  [Documentation](https://github.com/sdl/Sdl-Community/tree/master/Qualitivity)|
| [Community Advenced Display Filter](https://appstore.sdl.com/language/app/community-advanced-display-filter/849/) | [Documentation](https://github.com/sdl/Sdl-Community/tree/master/AdvancedDisplayFilter) |
| [Antidote Verifier](http://appstore.sdl.com/app/antidote-verifier/583/) | [Documentation](https://github.com/sdl/Sdl-Community/tree/master/Antidote%20Verifier) |
| [SDLXLIFF Compare](https://appstore.sdl.com/language/app/sdlxliff-compare/89/) | [Documentation](https://github.com/sdl/Sdl-Community/tree/master/SdlXliff%20Compare) |
| [SDLXLIFF Split/Merge](https://appstore.sdl.com/language/app/sdlxliff-split-merge/20/) | [Documentation](https://github.com/sdl/Sdl-Community/tree/master/SDLXLIFFSplitMerge) |
| [File type definition for TMX](https://appstore.sdl.com/language/app/file-type-definition-for-tmx/317/) | [Documentation](https://github.com/sdl/Sdl-Community/tree/master/TMX) |
| [Toolkit](http://appstore.sdl.com/app/sdlxliff-toolkit/296/) |  [Documentation](https://github.com/sdl/Sdl-Community/tree/master/Toolkit)|
| [Wordfast TXML](http://appstore.sdl.com/app/file-type-definition-for-wordfast-txml/247/) |  [Documentation](https://github.com/sdl/Sdl-Community/tree/master/Wordfast%20TXML)|
| [Word Cloud](http://appstore.sdl.com/app/sdl-trados-studio-word-cloud/402/) |  [Documentation](https://github.com/sdl/Sdl-Community/tree/master/Word%20Cloud)|
| [MXLIFF File Type](https://appstore.sdl.com/language/app/mxliff-file-type/962/) |  [Documentation](https://github.com/sdl/Sdl-Community/tree/master/FileTypeSupport.MXLIFF)|
  
## Other plugins

| Plugin Name | Description |
| --- | --- |
| [Hunspell Dictionary Manager](https://appstore.sdl.com/language/app/hunspell-dictionary-manager/928/) |  [Documentation](https://github.com/sdl/Sdl-Community/tree/master/HunspellDictionaryManager)|


## We want your feedback

If you have any suggestions or find any issues please go [here](https://github.com/sdl/SDL-Community/issues) and let us know.
