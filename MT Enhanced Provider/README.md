## MT Enhanced Provider for SDL Trados Studio

## Table of contents 

1. [Intro](#intro)
2. [Features](#features)
3. [XML Structure](#xml-structure)
4. [Example](#example)

## Intro

This plugin allows you to retrieve translations from either Microsoft Translator or Google Translate in SDL Trados Studio, with added features.


## Features

* Options to perform a batch find/replace on the source text before sending to the MT provider as well on the translated text returned, using files with a specific XML structure to specify find/replace pairs (current version limited to 20 pairs each for pre-lookup and post-lookup). Use this feature to redact sensitive data from your source text or to improve the results.
* Text-only option: gives you the option of sending and receiving text only, and no tags.
The default setting is to not re-send translated segments: only sends segments with a ‘Not Translated’ segment status; conveniently navigate through your document or review it without increasing your character usage. This is optional and can be changed in the settings form.
* The Microsoft Translator API currently allows users to translate 2M characters/month at no charge after signing up. Users can also create their own custom MT engines on the ‘Microsoft Translator Hub’ and access those engines with this plugin by using the ‘Category ID’ setting.

Once installed, you will now find the option to choose MT Enhanced Plugin for Trados Studio as an MT provider to use in your translation projects.

For more info and screenshots, have a look at the [documentation](https://web.archive.org/web/20160801113006/http://www.linguisticproductions.com/mtenhancedplugin/doc).

## XML Structure
How to create the necessary xml structure in a text file for batch find/replace lists:

You can use a text editor (e.g. Notepad) to create files containing the necessary xml structure for use as batch find/replace lists.

The plugin will apply these lists to the source text before sending to the machine translation service and/or to the returned translated text, depending on the options configured in the plugin settings form.

The files can be saved with any file extension. They must have the following structure: ![](https://raw.githubusercontent.com/sdl/Sdl-Community/master/EditCollection1.PNG)

The first node is <EditCollection>. It contains a list of <Items>.

The items list contains EditItem elements, each of which has two attributes Enabled and EditItemType.

The acceptable values for Enabled are true or false. If Enabled is “false” the plugin will ignore the pair. This parameter allows you to temporarily disable a find/replace pair without having to delete it from the file.

The acceptable values for EditItemType are plain_text and regular_expression. You can use regular expression matching/replacement by setting the EditItemType to regular_expression. For simple plain text replacements, it is best to set it to plain_text to avoid unexpected behavior or the need to insert any special regular expression escape characters. The plugin uses .NET regular expressions.

Each EditItem has two elements: <FindText> and <ReplaceText>. They represent the text to find and the replacement text, respectively.

## Important: note that there are several special XML characters that will cause problems if you type them directly into the XML file as the values for FindText and ReplaceText. 
If you need to search and replace things like HTML tags, or any other text containing the < and/or > bracket character, the brackets must be ‘escaped’ with &lt; for <, and &gt; for >. Also, the ampersand character (&) must be escaped with &amp; to avoid problems. Escaping single quotes (&#39;) and double quotes (&quot;) in the text values is not absolutely mandatory, but it is a good idea in order to avoid any unexpected errors. Some XML editor programs will automatically escape these characters for you when saving the file.

After the list of EditItems nodes, the other nodes are closed with the corresponding closing tags: </Items> and </EditCollection>.

## Example

The following is a complete example of the content for a find/replace list file: ![](https://raw.githubusercontent.com/sdl/Sdl-Community/master/EditCollection2.PNG)

Here, there is one plain text pair specifying to find the string “Hello World” and replace it with “Hello world!”.
There is also a regular expression pair specifying to find the regular expression “\bhello\b” (a whole-word match for “hello”) and replace it with “Hello!”.

You can copy and paste the example xml into a text file and modify it as needed. Add EditItem blocks for additional pairs and set the attributes and elements accordingly.

Keep in mind:

* Beginning with version 1.2.3 there is no longer a limit in the number of items that the plugin will read from the edit collection files (version 1.2.2 was limited to 20).
* The find/replace text specified is case sensitive, i.e., specifying “hello” will not find “Hello”. To find both, two separate EditItem elements are necessary.
* The replacement is a direct string replacement and not a whole-word search. For example, specifying “no” / “not” as a find/replace pair will cause the word “none” to become “notne”. To use whole-word matching, regular expressions are necessary.
* The replacements are performed incrementally in the order they appear in the file. Once a replacement is made, the next find/replace will be performed on the updated text. For that reason, with similar strings, it is better to place longer ones first. For example, if you have entries to replace “dogs” with “pets” and “dog” with “a pet”, place the entry for “dogs” first. Otherwise you may get unexpected behavior.





## Credits: 
Initial development done by [Patrick Porter](https://github.com/patrickporter)
