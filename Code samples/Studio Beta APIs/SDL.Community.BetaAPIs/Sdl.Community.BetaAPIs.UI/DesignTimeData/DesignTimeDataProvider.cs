﻿using Sdl.Community.BetaAPIs.UI.DataProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.BetaAPIs.UI.Model;

namespace Sdl.Community.BetaAPIs.UI.DesignTimeData
{
    public class DesignTimeDataProvider : IAPIDataProvider
    {
        public IEnumerable<API> LoadAPIs()
        {
            yield return new API { Name = "Alignment API", Description = "Alignment API description", Assemblies = new List<string> { "assemly1", "assembly2" } };
            yield return new API { Name = "Terminology Provider API", Description = "Terminology Provider API" };

            yield return new API { Name = "SpellChecking API", Description = @"# Apply Studio Project Template

This plug-in allows you to apply settings from a template (.sdltpl) or project (.sdlproj) to one or more projects in SDL Trados Studio. The following settings can be applied:
- Translation Memory and Automated Translation*
- Translation Memory
- Terminology*
- Batch Processing
- Verification
- File Types

*it's possible to merge the lists of translation and terminology providers

The settings can be applied to either the active project or all selected projects in the projects view.

Once installed, you will now be able to see the option to Apply Studio Project Template by right clicking on a project in the Projects view. You can also open the Plugin in Studio by pressing Ctrl + Alt + T.

##Contribution

You want to add a new functionality or you spot a bug please fill free to create a [pull request](http://www.codenewbie.org/blogs/how-to-make-a-pull-request) with your changes.

##Development Prerequisites

* [Studio 2014 or 2015](https://oos.sdl.com/asp/products/ssl/account/mydownloads.asp) - if you don't have a licence please use this [link](http://www.translationzone.com/openexchange/developer/index.html) and sign-up into SDL OpenExchange Developer Program
* [Studio 2014 SDK](http://www.translationzone.com/openexchange/developer/sdk.html)
* [Visual Studio 2013](http://www.visualstudio.com/downloads/download-visual-studio-vs) - express/community edition can be used
* [Sdl plugin installer](https://community.sdl.com/developers/language-developers/f/59/t/4107) - there is nothing to develop just to know that this can be used to install the plugins

##Issues

If you find an issue you report it [here](https://github.com/sdl/SDL-Community/issues)." };
        }
    }
}
