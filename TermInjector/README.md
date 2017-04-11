# Sdl.Community.TermInjector plugin for SDL Trados Studio #

### What is this repository for? ###

This is the repo for the Sdl.Community.TermInjector plugin, which I published some years ago.
I'm not able to continue development myself, and as there's been some interest 
in updating the plugin for newer Studio versions, I've decided to make the code
available as open source under the MIT license.

This was the first project I had done on C#, so the code quality can be uneven.
However, the plugin has been widely used, so it seems fairly robust.

The plugin reportedly works in all versions of Studio up to 2015, but it's been
developed for the Studio 2009/2011 APIs.

The plugin documentation can be accessed here:
http://www.tntranslations.com/TermInjectorHelp.html

### How do I get set up? ###

The repo contains a Visual Studio solution, which includes the Sdl.Community.TermInjector
project (adapted from Studio SDK examples). API references point to the folder
of an old Studio version, so they should probably be updated before building.


More information on setup can be found in the Trados Studio SDK documentation. 

### RegexKeyTrie data structure###

Sdl.Community.TermInjector uses RegexKeyTrie to perform replacements with regular expression
operators. RegexKeyTrie is available as a standalone library, but it's
embedded into TermInjector as including external libraries in Studio plugins
was not possibly in early Studio versions. RegexKeyTrie is available here:
https://bitbucket.org/tjnieminen/regexkeytrie


Credits to : [Tommi Nieminen](https://bitbucket.org/tjnieminen/)