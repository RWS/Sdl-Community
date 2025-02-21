# Legit


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


