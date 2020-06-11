# ETS Trados Plugin

The ETS Trados Plugin's purpose is to allow clients to use their ETS API in
combination with Trados Studio. There are two projects that help do this,
ETSLanguageResourceBuilder and ETSTradosTranslationProvider.

## ETSLanguageResourceBuilder

The purpose of this project is to create a build task (in the form of a dll)
that will be accessed via the other project. The build task will take the
languages.xml file from xmt-externals and parse it into a resources file, which
will then be referenced by ETSTradosTranslationProvider.

## ETSTradosTranslationProvider

### Requirements

Set the environmental variable SDL_EXTERNALS_PATH to your xmt-externals git
repo. This will be passed through to the build task via a line in the csproj.
If you want to customize the xmt-externals path being used by opening the
csproj in a text editor and finding the task line (should start with
GenerateLanguageResourceTask) and manually altering the value of
XMTExternalsDir.

Set the environmental variable MSBUILDDISABLENODEREUSE=1. Without it, msbuild
will try to reuse nodes in order to save time. Unfortunately, this will mean
that if you build the build task and then go to build the plugin, the node the
plugin uses could still have a lock on the build tasks's dll, which would
cause the build task to fail. If you're not building the build task at all,
you can skip setting this environmental variable. Leaving it unset will allow
your builds to build much faster, so it's generally recommended not to set it
unless necessary.

You also must have built ETSLanguageResourceBuilder. 
ETSTradosTranslationProvider will access the dll from that location (referred
to using the UsingTask tag in the csproj).

### Flow of Code

#### TranslationProvider
Here we set all settings related specifically to ETS and the settings we want
to be applied that aren't being entered by the client. This gets used directly
by Trados Studio and features many required settings (required via interface-
ITranslationProvider).

#### TranslationOptions

This is the file to change any client-side options for the translation, for
example host, post, api token etc.

#### TranslationProviderLanguageDirection

In this file, we do the main brunt of the translating. Trados Studio will
create a provider and use SearchSegment (and variations) as well as
SearchTranslationUnit (and variations). The main gist of how translation takes
place is that we're given a segment from the sourceLanguage and we take the
enclosed text, translate it and return a new segment with the translated text
contained within.

Certain functions, such as UpdatingTranslations throw
NotImplementedException(), which Trados Studio will catch and silence. We
throw the exception because we don't train ETS based on corrections that users
make on translations. If we did ever want to support training ETS, we would
modify these functions.

#### ETSApi

The purpose of this folder and all files enclosed within is to create a
centralized location for accessing the ETS API and related classes used for
serializing. Any functions that call to ETS should be contained within
(ideally using ContactETSServer).

#### TranslationProviderWinFormsUI and ProviderConfDialog

This, in combination with the Dialog (ProviderConfDialog) are the front end of
the plugin. Any cosmetic changes to the dialog are made in ProviderConfDialog
and related .cs project. TranslationProviderWinFormsUI calls upon
ProviderConfDialog and should be kept slim in comparison.
