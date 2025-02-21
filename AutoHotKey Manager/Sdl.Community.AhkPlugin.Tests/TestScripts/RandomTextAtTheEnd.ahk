SetTimer,UPDATEDSCRIPT,1000

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
;Clear AT Statuses
;===
;Description
; Clear AT statuses in SDL Trados Studio by using Ctrl+F.
;===
;Content
#IfWinActive SDL Trados Studio
^+f::
Send ^a^c!{Ins}^a^v
Return
#IfWinActive
;===
;Active
;endScript
;===
;Active
;endScript

Random text on the end