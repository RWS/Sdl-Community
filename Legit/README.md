# Legit

##Introduction

The application uses the Trados 2007 components that are installed with Studio so that the user can batch convert any supported source file to TTX or Bilingual Doc.  It’s important to note that this means supported by Trados 2007 and not by Studio, so newer formats like IDML, ICML etc. cannot be converted in this way.

The advantages this tool provide over using SDL TTXit! (a similar tool installed with Studio) are as follows

- Fully segment files to avoid round trip legacy issues.
- Select to convert into TTX and/or Bilingual Doc.
- Convert using a Translation Memory if available.
- You can select a source and target language.
- If no Translation Memory is selected the app will default to SDL Trados 2007 segmentation rules automatically.

So this is a very useful application because it means you can handle the old legacy workflows very nicely without the need to install and use the old Trados Translators Workbench or Tag Editor.  The process is this:

1.	Convert the source files to TTX/Bilingual DOC with SDL LegIt!
2.	Create your Project in Studio with the TTX/Bilingual DOC files
3.	Translate these files and save the target TTX/Bilingual DOC files

There is no mechanism in here to “clean up” these bilingual files, but if you are using this workflow it can only be because you have customers who are looking for this format.  If they only need the translated files in their final format then you would not need to use this process in the first place!

##Contribution

You want to add a new functionality or you spot a bug please fill free to create a [pull request](http://www.codenewbie.org/blogs/how-to-make-a-pull-request) with your changes.

##Development Prerequisites

* [Studio 2015](https://oos.sdl.com/asp/products/ssl/account/mydownloads.asp) - if you don't have a licence please use this [link](http://www.translationzone.com/openexchange/developer/index.html) and sign-up into SDL OpenExchange Developer Program
* [Studio 2014 SDK](http://www.translationzone.com/openexchange/developer/sdk.html)
* [Visual Studio 2013](http://www.visualstudio.com/downloads/download-visual-studio-vs) - express edition can be used
* [Inno Setup](http://www.jrsoftware.org/isinfo.php) - if you want to generate the installer

##Issues

If you find an issue you report it [here](https://github.com/sdl/SDL-Community/issues).
