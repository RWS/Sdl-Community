# memoQ Translation Memory Provider #
## Outline ##
This plugin for Trados Studio allows users to access translation memories from memoQ servers.  The original source code was provided by the memoQ team and is based on the plugin that used to be available through the memoQ website.
The original plugin provided access to translation memories from Language Terminal and also from memoQ servers.  The source code made available here has been provided without the code to access Language Terminal.
## Connecting to a memoQ server ##
To connect to a memoQ server you will need to be provided with the following details from the server's owner:
- the address of the server (for example: https://memoQ.company.com)
- your username
- your password

**Important:** to receive matches from a memoQ server TM you must be a member of the Resorce lookup via API/plugins group on the memoQ server.  If you cannot connect to the memoQ server with the username and password you received you will need to contact the server administrator and ask them to resolve this for you.
In addition the memoQ TM Provider plugin takes a memoQ web licence from the memoQ server if there is one available.  If there are none available you will not be able to use translation memories on that server and again will need to contact the server administrator.
## Additional Information ##
In this repository you will find the [original PDF document](https://github.com/RWS/Sdl-Community/blob/master/MemoQ/memoQ_SDLTradosTMPlugin_GettingStarted_1_0.pdf "memoQ plugin for SDL Trados Studio") provided by the memoQ team.  This document shold be used for now for any additional information required to understad how the plugin works.  But please keep in mind the points above.
We will update this README in due course, or the PDF provided.
