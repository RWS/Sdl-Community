# TM Optimizer

##Using TM Optimizer

After installation, start TM Optimizer from the Start menu under SDL->TM Optimizer.

On the first page of the TM Optimizer wizard, you have two choices, depending on your situation:
            
1. **Convert Workbench translation memory:** Use this option when you have not yet converted your Workbench TM for use in Studio. You will need a TMX export of the Workbench TM you want to convert.
1. **Optimize already converted Workbench translation memory:** Use this option when you have already converted your Workbench TM to a Studio TM but are having problems caused by excessive formatting tags. You will need your Studio TM and a TMX export of your original Workbench TM.
            ![Wizzard Intro](/TM Optimizer/Sdl.Community.TMOptimizer/TMOptimizerHelp/images/wizard_intro.png)

On the second page, select the TMX files you have exported from your existing TRADOS Workbench TM. The TMX files are analyzed and the number of TUs and languages are displayed. If you had already converted the Workbench TMs to Studio TMs using the Studio TM upgrade wizard (and have selected option 2 on the previous page), you also have to select that Studio TM here.
![Wizzard Input](/TM Optimizer/Sdl.Community.TMOptimizer/TMOptimizerHelp/images/wizard_input.png)

On the third page, you select the TM into which the optimized translation units should be imported. This could be either a newly created TM or an existing TM. This TM should be different from the input TM you chose on the previous page.
![Wizzard Output](/TM Optimizer/Sdl.Community.TMOptimizer/TMOptimizerHelp/images/wizard_output.png)

Finally, on the processing page, you will see detailed progress on the TM optimization process. When all the steps are completed, you can browse to the output TM or open the TM in Studio using the buttons that appear at the bottom of the wizard.
![Wizzard Processing](/TM Optimizer/Sdl.Community.TMOptimizer/TMOptimizerHelp/images/wizard_processing.png)           

##How does TM optimizer work ?
###Increases TM leverage

When using TRADOS Workbench with the Microsoft Word macro for translation of RTF and DOC files, translation memories are often polluted with unnecessary formatting tags. If you convert such translation memory into SDL Trados Studio you experience that translations where you expect exact matches will be reported as fuzzy matches, effectively costing you money. TM Optimizer will analyse the content of each translation unit and will optimize the tag content for optimal results in SDL Trados Studio. In addition translation unit content is harmonized so you can achieve the best results and don't spent any extra money on translations you already paid once.

The two screenshots below illustrate what this means in practice. In the first picture, the TM only returns a 99% match where you would have expected a 100% match, purely due to excessive, unnecessary tags.
![Not Optimized](/TM Optimizer/Sdl.Community.TMOptimizer/TMOptimizerHelp/images/NotOptimized.jpg)

Using the optimized TM, you see that the TM now returns a 100% match. Note that TM Optimizer only selectively removes the unnecessary tags.

![Optimized](/TM Optimizer/Sdl.Community.TMOptimizer/TMOptimizerHelp/images/Optimized.jpg)

###Removes excessive formatting, e.g. font change, kerning, tracking, spacing

Due to limitations of the Microsoft Word macro, unnecessary formatting information is stored in the translation memory. TM Optimizer will remove such formatting giving you less work and making translation easier.

###Import of the TMX is much faster

Due the fact that during optimization a vast amount of tags is removed, SDL Trados Studio will perform all operations which are using translation memory much faster. In cetain cases the difference is more than 500%.

###Makes migration from DOC in TagEditor to DOCX in Studio much easier

More and more clients are migrating from the legacy DOC format towards the latest version of Microsoft Word which is using the new DOCX file format. The features mentioned above will allow you to switch from translating DOC files in Microsoft Word to translating DOCX files in SDL Trados Studio without the major issues with leverage you would otherwise see.

##Contribution

You want to add a new functionality or you spot a bug please fill free to create a [pull request](https://guides.github.com/activities/contributing-to-open-source/) with your changes.

##Development Prerequisites

* [Studio 2014](https://oos.sdl.com/asp/products/ssl/account/mydownloads.asp) - if you don't have a licence please use this [link](http://www.translationzone.com/openexchange/developer/index.html) and sign-up into SDL OpenExchange Developer Program
* [Studio 2014 SDK](http://www.translationzone.com/openexchange/developer/sdk.html)
* [Visual Studio 2013](http://www.visualstudio.com/downloads/download-visual-studio-vs) - express edition can be used

##Issues

If you find an issue you report it [here](https://github.com/sdl/SDL-Community/issues).
