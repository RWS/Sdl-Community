SDL-Community-WPF-Helpers
===========

The SDL Community WPF Helpers is a collection of helper functions. It simplifies and demonstrates common developer tasks building SDL Studio plugins.
## Supported platforms

* .NET 4.7 (Desktop / Server)
* Windows 7 / 8 / 8.1 /10 Store Apps
* SDL Studio 2019 or later

## Getting started

This libraries are build on top of SDL Studio APIs and thus using them also requires having SDL Studio installed.

1. Install Visual Studio 2019. The community edition is available for free [here](https://www.visualstudio.com/).

2. Open the solution for an existing SDL Studio plugin or create a new one.

3. In Solution Explorer panel, right click on your project name and select **Manage NuGet packages**. Search for **Sdl.CommunityWpfHelpers**, and choose your desired [NuGet Packages](https://www.nuget.org/packages?q=Sdl.CommunityWpfHelpers) from the list.


4. In your C# class, add the namespaces to the Sdl.CommunityWpfHelpers, for example:

```c#
using Sdl.CommunityWpfHelpers
```

5. Use the extensions for existing API classes.

## Nuget Packages

NuGet is a standard package manager for .NET applications that is built into Visual Studio. From your open solution choose the *Tools* menu, *NuGet Package Manager*, *Manage NuGet packages for solution...* to open the UI.  Enter one of the package names below to search for it online.


| NuGet Package Name | description |
| --- | --- |
| [Sdl.CommunityWpfHelpers](https://www.nuget.org/packages/Sdl.CommunityWpfHelpers/) | NuGet package which includes helper methods to build common WPF functions for Studio plugins. |


## Features
* Helper functions for working with Commands, Converters, Window behaviors

## Feedback and Requests

Please use [GitHub issues](https://github.com/sdl/Sdl-Community/issues) for questions or comments.

## Principles

 - Principle #1: The Sdl.CommunityWpfHelpers will be kept simple.
 - Principle #2: As soon as a comparable feature is available in the SDL Studio, it will be marked as deprecated.
 - Principle #3: All features will be supported for the latest SDL Studio release cycles or until another principle supersedes it.