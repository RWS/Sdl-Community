﻿SetTimer,UPDATEDSCRIPT,1000

UPDATEDSCRIPT:
FileGetAttrib,attribs,%A_ScriptFullPath%
IfInString,attribs,A
{
FileSetAttrib,-A,%A_ScriptFullPath%
SplashTextOn,,,Updated script,
Sleep,500
Reload
}
Return



;Name
;asfas
;===
;===
;Content
ceva script
;===
;Active
;endScript


