## Table of contents 

1. [Intro](#intro)
2. [Getting started](#getting-started)
3. [List of plugins](#list-of-plugins)
4. [We want your feedback](#we-want-your-feedback)

## Intro

This repository contains around 30 plugins developed for [Trados Studio](http://www.sdl.com/solution/language/translation-productivity/trados-studio/). Most of the plugins were developed by SDL, but you might also find a few of them which were initially developed by someone else. For all this plugins we now have full source code ownership according to our [License agreement](https://github.com/sdl/Sdl-Community/blob/master/License.md). You can find the complete list of plugins [here](#list-of-plugins).

We encourage everyone who is interested to contribute, either by fixing some issues, implement new features or improve the documentation. To do this you will have to clone this repository, make the changes and send us a [pull request](http://www.codenewbie.org/blogs/how-to-make-a-pull-request) with your changes.

You can also use this repository for learning by reading and tinkering with real Trados Studio plugins. For the documentation please go [here](http://appstore.sdl.com/developers/sdk.html).

If you have any questions, don't hesitate to ask on the [Sdl Language Developer Community](https://community.sdl.com/developers/language-developers/).

## Getting started

These libraries are built on top of SDL Trados Studio APIs and thus using them also requires having SDL Trados Studio installed.

If you don't have a licence please send a email to app-signing@sdl.com and ask for a developer licence.

All the plugins require .NET 4.5.2 and SDL Trados Studio 2017.

To get started with this repository follow the following steps:

1. Make sure you have a installed Microsoft Visual Studio 2013, 2015 or 2017. If you don't have Microsoft Visual Studio you can install the community edition, available for free [here](https://www.visualstudio.com/).

2. To clone this repository you need to have Git installed and configured on your machine (more details [here](https://www.atlassian.com/git/tutorials/install-git#windows) and [here](https://help.github.com/articles/cloning-a-repository/)). If you prefer a more visual approach you can either use the [github extension for Microsoft Visual Studio](https://visualstudio.github.com/), [Github Desktop](https://desktop.github.com/) or [SourceTree](https://www.sourcetreeapp.com/).

3. In order to clone the repository using Source Tree, from menu select "Clone/New". In source path paste this path : "https://github.com/sdl/Sdl-Community.git", in destination path select a path where the repository should be stored on your drive![](https://raw.githubusercontent.com/sdl/Sdl-Community/master/cloneRepository.png)

4. After the repository was cloned, navigate to the repository path you've specified when cloning the repository. Each plugin has a dedicated folder so all you need to do is to find the plugin you're looking for, enter the folder and open the solution file using Microsoft Visual Studio.

5. Build the solution, after the build succeded open SDL Trados Studio 2017. A warning message will appear click "Yes", after Studio loads, builded plugin will be available in Studio.

 ![](https://raw.githubusercontent.com/sdl/Sdl-Community/gh-pages/unsignedPlugin.png)

## List of plugins

In the following table are shown all the plugins available in the repository. 

If you click on the plugin name you'll be redirected to SDL App Store, from where you can download the selected plugin. 
Documentation column will redirect to the source code for the selected plugin.
  
| Plugin Name | Description |
| --- | --- |
| [Antidote Verifier](http://appstore.sdl.com/app/antidote-verifier/583/) | [Documentation](https://github.com/sdl/Sdl-Community/tree/master/Antidote%20Verifier) |
| [Advenced Display Filter](https://www.nuget.org/packages/Sdl.Community.Toolkit.FileType/) | [Documentation](https://github.com/sdl/Sdl-Community/tree/master/AdvancedDisplayFilter) |
| Bring Back The Button | [Documentation](https://github.com/sdl/Sdl-Community/blob/master/BringBackTheButton)
| [Export to Excel](http://appstore.sdl.com/app/export-to-excel/532/) |  [Documentation](https://github.com/sdl/Sdl-Community/blob/master/Export%20to%20Excel/)|
| [InSource](http://appstore.sdl.com/app/sdl-insource/548/) |  [Documentation](https://github.com/sdl/Sdl-Community/blob/master/InSource)|
| [Legacy Converter](http://appstore.sdl.com/app/sdlxliff-to-legacy-converter/134/) |  [Documentation](https://github.com/sdl/Sdl-Community/tree/master/Legacy%20Converter)|
| [MT Enhanced Provider](http://appstore.sdl.com/app/mt-enhanced-plugin-for-trados-studio/604/) |  [Documentation](https://github.com/sdl/Sdl-Community/tree/master/MT%20Enhanced%20Provider)|
| [Post Edit Compare](http://appstore.sdl.com/app/post-edit-compare/610/) |  [Documentation](https://github.com/sdl/Sdl-Community/blob/master/Post%20Edit%20Compare)|
| [Qualitivity](http://appstore.sdl.com/app/qualitivity/612/) |  [Documentation](https://github.com/sdl/Sdl-Community/tree/master/Qualitivity)|
| [SDLTM Repair](http://appstore.sdl.com/app/sdltm-repair/298/) |  [Documentation](https://github.com/sdl/Sdl-Community/tree/master/SDLTMRepair)|
| [Star Transit](http://appstore.sdl.com/app/transitpackage-handler/573/) |  [Documentation](https://github.com/sdl/Sdl-Community/tree/master/Sdl.Community.StarTransit)|
| [Segment Status Switcher](http://appstore.sdl.com/app/segment-status-switcher/754/) |  [Documentation](https://github.com/sdl/Sdl-Community/tree/master/SegmentStatusSwitcher)|
| [Studio Migration Utility](http://appstore.sdl.com/app/studio-migration-utility/481/) |  [Documentation](https://github.com/sdl/Sdl-Community/tree/master/Studio%20Migration%20Utility)|
| [TAUS Search](http://appstore.sdl.com/app/taus-search/164/) |  [Documentation](https://github.com/sdl/Sdl-Community/tree/master/TAUSS%20Search)|
| [TM Lifting](http://appstore.sdl.com/app/tm-lifting/419/) |  [Documentation](https://github.com/sdl/Sdl-Community/tree/master/TMLifting)|
| [Term Injector](http://appstore.sdl.com/app/terminjector/97/) |  [Documentation](https://github.com/sdl/Sdl-Community/tree/master/TermInjector)|
| [Jobs](http://appstore.sdl.com/app/jobs/463/) |  [Documentation](https://github.com/sdl/Sdl-Community/tree/master/Jobs)|
| [YourProductivity](http://appstore.sdl.com/app/yourproductivity/491/) |  [Documentation](https://github.com/sdl/Sdl-Community/tree/master/YourProductivity)|
| [Time Tracker](http://appstore.sdl.com/app/studio-time-tracker/361/) |  [Documentation](https://github.com/sdl/Sdl-Community/tree/master/Time%20Tracker)|
| [Toolkit](http://appstore.sdl.com/app/sdlxliff-toolkit/296/) |  [Documentation](https://github.com/sdl/Sdl-Community/tree/master/Toolkit)|
| [Variables Manager](http://appstore.sdl.com/app/variables-manager-for-sdl-trados-studio/297/) |  [Documentation](https://github.com/sdl/Sdl-Community/tree/master/Toolkit)|
| [Apply Studio Project Template](http://appstore.sdl.com/app/apply-studio-project-template/391/) |  [Documentation](https://github.com/sdl/Sdl-Community/tree/master/ApplyStudioProjectTemplate)|
| Controlled Machine Translation Providers |  [Documentation](https://github.com/sdl/Sdl-Community/tree/master/Controlled%20Machine%20Translation%20Providers)|
| [Word Cloud](http://appstore.sdl.com/app/sdl-trados-studio-word-cloud/402/) |  [Documentation](https://github.com/sdl/Sdl-Community/tree/master/Word%20Cloud)|
| [Number verifier](http://appstore.sdl.com/app/sdl-number-verifier/440/) |  [Documentation](https://github.com/sdl/Sdl-Community/tree/master/Number%20Verifier)|
| [TM Optimizer](http://appstore.sdl.com/app/tm-optimizer/347/) |  [Documentation](https://github.com/sdl/Sdl-Community/tree/master/TM%20Optimizer)|
| [Record Source TU](http://appstore.sdl.com/app/record-source-tu/504/) |  [Documentation](https://github.com/sdl/Sdl-Community/tree/master/Record%20Source%20TU)|
| [Studio InQuote](http://appstore.sdl.com/app/sdl-studio-inquote/295/) |  [Documentation](https://github.com/sdl/Sdl-Community/blob/master/InvoiceAndQuotes)|
| [Wordfast TXML](http://appstore.sdl.com/app/file-type-definition-for-wordfast-txml/247/) |  [Documentation](https://github.com/sdl/Sdl-Community/tree/master/Wordfast%20TXML)|
| [Your Studio](http://appstore.sdl.com/app/your-sdlstudio/300/) |  [Documentation](https://github.com/sdl/Sdl-Community/tree/master/YourStudio)|

## We want your feedback

If you have any suggestions or find any issues please go [here](https://github.com/sdl/SDL-Community/issues) and let us know.
