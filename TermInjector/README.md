# Sdl.Community.TermInjector plugin for SDL Trados Studio 

### What is this repository for? 

This is the repo for the Sdl.Community.TermInjector plugin, which I published some years ago.
I'm not able to continue development myself, and as there's been some interest 
in updating the plugin for newer Studio versions, I've decided to make the code
available as open source under the MIT license.

The plugin reportedly works in all versions of Studio up to 2017, including 2017.

The plugin documentation can be accessed here:
http://www.tntranslations.com/TermInjectorHelp.html

### How do I get set up?

The repo contains a Visual Studio solution, which includes the Sdl.Community.TermInjector
project (adapted from Studio SDK examples).

More information on setup can be found in the Trados Studio SDK documentation. 

### RegexKeyTrie data structure

Sdl.Community.TermInjector uses RegexKeyTrie to perform replacements with regular expression
operators. RegexKeyTrie is available as a standalone library, but it's
embedded into TermInjector as including external libraries in Studio plugins
was not possibly in early Studio versions. RegexKeyTrie is available here:
https://bitbucket.org/tjnieminen/regexkeytrie


Credits to : [Tommi Nieminen](https://bitbucket.org/tjnieminen/)
