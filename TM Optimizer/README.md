# Number Verifier

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
