; PieLaunch Hotkey Script
; Customize these hotkeys as needed

; Win+Alt+Left - Snap to left half
#!Left::
Run, PieLaunch.exe /command snap-left
return

; Win+Alt+Right - Snap to right half
#!Right::
Run, PieLaunch.exe /command snap-right
return

; Win+Alt+Up - Maximize
#!Up::
Run, PieLaunch.exe /command maximize
return

; Win+Alt+Down - Minimize
#!Down::
Run, PieLaunch.exe /command minimize
return

; Win+Shift+1 - Top Left
#+1::
Run, PieLaunch.exe /command snap-topleft
return

; Win+Shift+2 - Top Right
#+2::
Run, PieLaunch.exe /command snap-topright
return

; Win+Shift+3 - Bottom Left
#+3::
Run, PieLaunch.exe /command snap-bottomleft
return

; Win+Shift+4 - Bottom Right
#+4::
Run, PieLaunch.exe /command snap-bottomright
return