Ignore the Installer folder, installer is now built by the SdlxTmTranslationProviderWix project.
Just run the solution, and a ready-to-use msi is created.

Do not try to test this with only a debug version on your machine, use a Trados Studio installer.
It does some registry magic without which the plugin will fail.

Change the version number in SdlxTmTranslationProviderWix\Product.wxs when you publish a new version,
and make sure the plugin installation folder has correct location and version, at the moment it is
C:\Users\<user>\AppData\Roaming\SDL\SDL Trados Studio\11\Plugins

This is not built by the nightly build: create the msi, zip it, and submit it to OpenExchange
when ready. The folder "OpenExchange deployment version" contains the latest tested zip.
As of now (10/05/2013) this version (1.1) is not on OpenExchange