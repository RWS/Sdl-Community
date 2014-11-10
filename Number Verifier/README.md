# Number Verifier

##Introduction

 While the standard number verification in SDL Trados Studio may often be sufficient there are some occasions when a bit more control would be preferable, for example when translating documents that contain lots of numbers. This Number Verifier plug-in allows you to fine-tune settings to provide you with the desired balance between amount of false positives and potentially missed errors.
 
The first thing you can do is to select to what extent removed or deleted numbers should be considered errors. For example, if you need to add metric measurements while keeping the imperial measurments in your translation you may want to disregard added numbers altogether or at least to have a warning message displayed rather than an error.
The second thing is that you can make settings related to the localization of numbers. These settings allow you to be anything from strict (with the risk of many false positives) to permissive (with the risk of missing some errors). \par
You can select the thousands and decimal separators you want to allow. For example, you may allow one or more thousands separators depending on language standards. Similarly you may allow for both a period and a comma to be used as a decimal separator, in order to allow for cases where for some reason more than one language standard should be allowed. The selected separators will then be combined with the Localizations setting to determine what should be considered a modified/unlocalized number.

**Require localizations** If a number in the source is identified as using one of the possible thousands or decimal separators specified for the source separators, the translation *must*  contain corresponding numbers with any of the separators specified for target separators or else the number will be considered modified/unlocalized.

**Allow localizations** If a number in the source is identified as using one of the possible thousands or decimal separators specified for the source separators, the translation may contain corresponding numbers with either the same separator as the source number or with any of the separators specified for target separators, or else the number will be considered modified/unlocalized.

**Prevent localizations** The same thousands or decimal separators as in source must be retained in the translation or else the number will be considered modified.

In addition to plain numbers the Number Verifier plug-in can also be used to find changes to alphanumeric names. For example, if VT500 has accidentally been translated as VR500 an error can be displayed. Here an alphanumeric name is defined as a string of characters starting with one or more uppercase letters (A-Z) followed by any combination of digits (0-9) and uppercase letters. (Please note that if VT500 has been translated as VT300 this will also be identified as a modified number.)
You can select **Exclude tag text**  if you find that you get duplicate error messages since the change of a number in a tag constitutes a tag change that is reported by the tag verifier.
Finally, you can select the **Extended** option for **Messages** if you want the source and target text to be included in the log file.

##Contribution

You want to add a new functionality or you spot a bug please fill free to create a [pull request](https://guides.github.com/activities/contributing-to-open-source/) with your changes.

##Development Prerequisites

* [Studio 2014](https://oos.sdl.com/asp/products/ssl/account/mydownloads.asp) - if you don't have a licence please use this [link](http://www.translationzone.com/openexchange/developer/index.html) and sign-up into SDL OpenExchange Developer Program
* [Studio 2014 SDK](http://www.translationzone.com/openexchange/developer/sdk.html)
* [Visual Studio 2013](http://www.visualstudio.com/downloads/download-visual-studio-vs) - express edition can be used

##Issues

If you find an issue you report it [here](https://github.com/sdl/SDL-Community/issues).
