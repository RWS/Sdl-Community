![Build Status](https://sdl.visualstudio.com/TradosStudio/_apis/build/status/AppStore/TradosStudio%20-%20AppStore)
![GitHub repo size in bytes](https://img.shields.io/github/repo-size/sdl/sdl-community.svg)
![GitHub](https://img.shields.io/github/license/sdl/sdl-community.svg)
 
## Table of contents 

1. [Intro](#intro)
2. [Getting started](#getting-started)
3. [List of plugins](#list-of-plugins)
4. [We want your feedback](#we-want-your-feedback)

## Intro

This repository contains around 50 plugins developed for [Trados Studio](https://www.trados.com/product/studio/). A lot of the plugins were developed by Trados Appstore team, but you might also find a few of them which were initially developed by someone else. For all these plugins we now have full source code ownership according to our [License agreement](https://github.com/RWS/Sdl-Community/blob/master/LICENSE). You can find the complete list of plugins [here](#list-of-plugins).

We encourage everyone who is interested to contribute, either by fixing some issues, implementing new features or improving the documentation. To contribute, clone this repository, make the changes and send us a [pull request](http://www.codenewbie.org/blogs/how-to-make-a-pull-request) with your changes.

If you have any questions, don't hesitate to ask on the [Language Developer Community](https://community.rws.com/archive/language-developers).

## Getting started


#### Required Tools
To get started with this repository, install the following tools:
1. **Trados Studio 2024**. These libraries are built on top of Trados Studio APIs and thus using them also requires having it installed.
2. **Developer licence**. If you don't have a licence please send an email to App Signing <app-signing@rws.com>.
3. **.NET 4.8**
4. **Microsoft Visual Studio 2022 or higher**. If you don't have Microsoft Visual Studio you can install the community edition  for free [here](https://www.visualstudio.com/).
5. **Git**. Find more details on installing git [here](https://www.atlassian.com/git/tutorials/install-git#windows). If you prefer a more visual approach you can either use the [github extension for Microsoft Visual Studio](https://visualstudio.github.com/), [Github Desktop](https://desktop.github.com/) or [SourceTree](https://www.sourcetreeapp.com/).

Once you have installed the tools, follow the steps below:

1. Clone this repository (more details [here](https://help.github.com/articles/cloning-a-repository/)). In order to clone the repository using Source Tree, from menu select "Clone/New". In the Source Path paste this path: "https://github.com/RWS/Sdl-Community.git". In the Destination Path, select a path where you want to store the repository on your local drive.

2. After the repository was cloned, navigate to the repository path you've specified when cloning the repository. Each plugin has a dedicated folder. All you need to do is to find the plugin you're looking for, enter the folder and open the solution file using Microsoft Visual Studio.

3. Build the solution. After the build has succeeded, open Trados Studio 2024. Click "Yes" when the warning message appears. After Studio loads, the built plugin will be available in Studio.



#### Visual Studio templates extensions

Starting with Visual Studio 2017, the Trados Studio plugin templates are provided through Visual Studio extension which can be downloaded from [Marketplace](https://marketplace.visualstudio.com/items?itemName=sdl.tradosstudio18) for **Visual Studio 2022**. The extension allow developers to create plugins for Trados Studio 2024 using the predefined templates.

The Github source code for Visual Studio templates extension, can be downloaded from the [Github](https://github.com/RWS/trados-studio-vs-extension) repository.


## List of plugins

The following table shows all the plugins available in the repository. 

By clicking on each plugin name in the table, you'll be redirected to the plugin's download page in RWS AppStore. The Source Code column will redirect to the code project for the selected plugin.

| Plugin | Source Code | API Integration(s) |
| --- | --- | --- |
| [Amazon Translate MT provider](https://appstore.rws.com/Plugin/18) | [Source Code](https://github.com/RWS/Sdl-Community/tree/master/Amazon%20Translate%20MT%20provider) | Translation Memory API |
| [ApplyTM Template](https://appstore.rws.com/Plugin/21) | [Source Code](https://github.com/RWS/Sdl-Community/tree/master/ApplyTM%20Template) | Translation Memory API |
| [DeepL Translation Provider](https://appstore.rws.com/Plugin/24) | [Source Code](https://github.com/RWS/Sdl-Community/tree/master/DeepL%20ranslation%20Provider) | Translation Memory API |
| [Google Cloud Translation Provider](https://appstore.rws.com/Plugin/174) | [Source Code](https://github.com/RWS/Sdl-Community/tree/master/Google%20Cloud%20Translation%20Provider) | Translation Memory API |
| [Language Weaver Provider](https://appstore.rws.com/Plugin/240) | [Source Code](https://github.com/RWS/Sdl-Community/tree/master/Language%20Weaver%20Provider) | Translation Memory API |
| [Microsoft Translator Provider](https://appstore.rws.com/Plugin/179) | [Source Code](https://github.com/RWS/Sdl-Community/tree/master/Microsoft%20Translator%20Provider) | Translation Memory API |
| [SDLTM Import Plus](https://appstore.rws.com/Plugin/89) | [Source Code](https://github.com/RWS/Sdl-Community/tree/master/SDLTM%20Import%20Plus) | Translation Memory API |
| [SDLTM Repair](https://appstore.rws.com/Plugin/41) | [Source Code](https://github.com/RWS/Sdl-Community/tree/master/SDLTM%20Repair) | Translation Memory API |
| [Trados Translation Memory Management Utility](https://appstore.rws.com/Plugin/78) | [Source Code](https://github.com/RWS/Sdl-Community/tree/master/Trados%20Translation%20Memory%20Management%20Utility) | Translation Memory API |
| [TuToTm](https://appstore.rws.com/Plugin/79) | [Source Code](https://github.com/RWS/Sdl-Community/tree/master/TuToTm) | Translation Memory API |
| memoQ Translation Memory Provider | [Source Code](https://github.com/RWS/Sdl-Community/tree/master/memoQ%20Translation%20Memory%20Provider) | Translation Memory API |
| [IATE Real-time Terminology](https://appstore.rws.com/Plugin/30) | [Source Code](https://github.com/RWS/Sdl-Community/tree/master/IATE%20Real-time%20Terminology) | Terminology Provider API |
| [InterpretBank Terminology Provider](https://appstore.rws.com/Plugin/243) | [Source Code](https://github.com/RWS/Sdl-Community/tree/master/InterpretBank%20Terminology%20Provider) | Terminology Provider API |
| [Trados Studio InQuote](https://appstore.rws.com/Plugin/55/) | [Source Code](https://github.com/RWS/Sdl-Community/tree/master/Trados%20Studio%20InQuote) | Terminology Provider API |
| [TermExcelerator](https://appstore.rws.com/Plugin/75) | [Source Code](https://github.com/RWS/Sdl-Community/tree/master/TermExcelerator) | Terminology Provider API |
| [AutoHotKey Manager](https://appstore.rws.com/Plugin/22) | [Source Code](https://github.com/RWS/Sdl-Community/tree/master/AutoHotKey%20Manager) | Integration API |
| [Trados Copy Tags](https://appstore.rws.com/Plugin/23) | [Source Code](https://github.com/RWS/Sdl-Community/tree/master/Trados%20Copy%20Tags) | Integration API |
| [DSI Viewer](https://appstore.rws.com/Plugin/25) | [Source Code](https://github.com/RWS/Sdl-Community/tree/master/DSI%20Viewer) | Integration API |
| [Post-Edit Compare](https://appstore.rws.com/Plugin/15) | [Source Code](https://github.com/RWS/Sdl-Community/tree/master/Post-Edit%20Compare) | Integration API |
| [projectTermExtract](https://appstore.rws.com/Plugin/34) | [Source Code](https://github.com/RWS/Sdl-Community/tree/master/ProjectTermExtract) | Integration API |
| [Qualitivity](https://appstore.rws.com/Plugin/16) | [Source Code](https://github.com/RWS/Sdl-Community/tree/master/Qualitivity) | Integration API |
| [Rapid Add Term](https://appstore.rws.com/Plugin/35) | [Source Code](https://github.com/RWS/Sdl-Community/tree/master/Rapid%20Add%20Term) | Integration API |
| [Reports Viewer Plus](https://appstore.rws.com/Plugin/37) | [Source Code](https://github.com/RWS/Sdl-Community/tree/master/Reports%20Viewer%20Plus) | Integration API |
| [Trados Data Protection Suite](https://appstore.rws.com/Plugin/39) | [Source Code](https://github.com/RWS/Sdl-Community/tree/master/Trados%20Data%20Protection%20Suite) | Integration API |
| [Trados Transcreate](https://appstore.rws.com/Plugin/42) | [Source Code](https://github.com/RWS/Sdl-Community/tree/master/Trados%20Transcreate) | Integration API |
| [Segment Status Switcher](https://appstore.rws.com/Plugin/44) | [Source Code](https://github.com/RWS/Sdl-Community/tree/master/Segment%20Status%20Switcher) | Integration API |
| [TransitPackage Handler](https://appstore.rws.com/Plugin/45) | [Source Code](https://github.com/RWS/Sdl-Community/tree/master/TransitPackage%20Handler) | Integration API |
| [studioViews](https://appstore.rws.com/Plugin/12) | [Source Code](https://github.com/RWS/Sdl-Community/tree/master/StudioViews) | Integration API |
| [Stylesheet Verifier](https://appstore.rws.com/Plugin/47) | [Source Code](https://github.com/RWS/Sdl-Community/tree/master/Stylesheet%20Verifier) | Integration API |
| [Time Tracker](https://appstore.rws.com/Plugin/76) | [Source Code](https://github.com/RWS/Sdl-Community/tree/master/Time%20Tracker) | Integration API |
| [SDLXLIFF Toolkit](https://appstore.rws.com/Plugin/77) | [Source Code](https://github.com/RWS/Sdl-Community/tree/master/SDLXLIFF%20Toolkit) | Integration API |
| [XLIFF Manager for Trados Studio](https://appstore.rws.com/Plugin/67) | [Source Code](https://github.com/RWS/Sdl-Community/tree/master/XLIFF%20Manager%20for%20Trados%20Studio) | Integration API |
| [XML Reader](https://appstore.rws.com/Plugin/81) | [Source Code](https://github.com/RWS/Sdl-Community/tree/master/XML%20Reader) | Integration API |
| [YourProductivity](https://appstore.rws.com/Plugin/82) | [Source Code](https://github.com/RWS/Sdl-Community/tree/master/YourProductivity) | Integration API |
| [MXLIFF File Type](https://appstore.rws.com/Plugin/29) | [Source Code](https://github.com/RWS/Sdl-Community/tree/master/MXLIFF%20File%20Type) | FileType Support API |
| [Multilingual Excel FileType](https://appstore.rws.com/Plugin/17) | [Source Code](https://github.com/RWS/Sdl-Community/tree/master/Multilingual%20Excel%20FileType) | FileType Support API |
| [Multilingual XML FileType](https://appstore.rws.com/Plugin/13) | [Source Code](https://github.com/RWS/Sdl-Community/tree/master/Multilingual%20XML%20FileType) | FileType Support API |
| [File type definition for TMX](https://appstore.rws.com/Plugin/61) | [Source Code](https://github.com/RWS/Sdl-Community/tree/master/File%20type%20definition%20for%20TMX) | FileType Support API |
| [Filetype for Wordfast TXML](https://appstore.rws.com/Plugin/62) | [Source Code](https://github.com/RWS/Sdl-Community/tree/master/Filetype%20for%20Wordfast%20TXML) | FileType Support API |
| [Antidote Verifier](https://appstore.rws.com/Plugin/3) | [Source Code](https://github.com/RWS/Sdl-Community/tree/master/Antidote%20Verifier) | Verification API |
| [Trados Number Verifier](https://appstore.rws.com/Plugin/33) | [Source Code](https://github.com/RWS/Sdl-Community/tree/master/Trados%20Number%20Verifier) | Verification API |
| [CleanUp Tasks](https://appstore.rws.com/Plugin/23) | [Source Code](https://github.com/RWS/Sdl-Community/tree/master/CleanUp%20Tasks) | Batch Task API |
| [Export to Excel](https://appstore.rws.com/Plugin/27) | [Source Code](https://github.com/RWS/Sdl-Community/tree/master/Export%20to%20Excel) | Batch Task API |
| [Fail Safe Task](https://appstore.rws.com/Plugin/28) | [Source Code](https://github.com/RWS/Sdl-Community/tree/master/Fail%20Safe%20Task) | Batch Task API |
| [Trados Batch Anonymizer](https://appstore.rws.com/Plugin/38) | [Source Code](https://github.com/RWS/Sdl-Community/tree/master/Trados%20Batch%20Anonymizer) | Batch Task API |
| [Trados Studio Target Renamer](https://appstore.rws.com/Plugin/73) | [Source Code](https://github.com/RWS/Sdl-Community/tree/master/Trados%20Studio%20Target%20Renamer) | Batch Task API |
| [Target Word Count](https://appstore.rws.com/Plugin/74) | [Source Code](https://github.com/RWS/Sdl-Community/tree/master/Target%20Word%20Count) | Batch Task API |
| [Apply Studio Project Template](https://appstore.rws.com/Plugin/20) | [Source Code](https://github.com/RWS/Sdl-Community/tree/master/Apply%20Studio%20Project%20Template) | Project Automation API |
| [Google API Validator](https://appstore.rws.com/Plugin/53) | [Source Code](https://github.com/RWS/Sdl-Community/tree/master/Google%20API%20Validator) | Standalone Application |
| [Hunspell Dictionary Manager](https://appstore.rws.com/Plugin/54) | [Source Code](https://github.com/sdl/Sdl-Community/tree/master/Hunspell%20Dictionary%20Manager) | Standalone Application |
| [SDLXLIFF to Legacy Converter](https://appstore.rws.com/Plugin/56) | [Source Code](https://github.com/RWS/Sdl-Community/tree/master/SDLXLIFF%20to%20Legacy%20Converter) | Standalone Application |
| [Language Mapping Provider](https://www.nuget.org/packages/LanguageMappingProvider/) | [Source Code](https://github.com/RWS/Sdl-Community/tree/master/Language%20Mapping%20Provider) | Standalone Application |
| Trados Proxy Settings | [Source Code](https://github.com/RWS/Sdl-Community/tree/master/Trados%20Proxy%20Settings) | Standalone Application |



## We want your feedback

If you have any suggestions or find any issues please go [here](https://github.com/RWS/Sdl-Community/issues) and let us know.
