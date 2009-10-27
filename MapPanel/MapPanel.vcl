<html>
<body>
<pre>
<h1>Build Log</h1>
<h3>
--------------------Configuration: MapPanel - Win32 (WCE ARMV4I) Release--------------------
</h3>
<h3>Command Lines</h3>
Creating temporary file "C:\DOKUME~1\tom\LOKALE~1\Temp\RSPF3.tmp" with contents
[
/nologo /W3 /D _WIN32_WCE=500 /D "ARM" /D "_ARM_" /D "WCE_PLATFORM_STANDARDSDK_500" /D "ARMV4I" /D UNDER_CE=500 /D "UNICODE" /D "_UNICODE" /D "NDEBUG" /D "_USRDLL" /D "MAPPANEL_EXPORTS" /Fp"ARMV4IRel/MapPanel.pch" /Yu"stdafx.h" /Fo"ARMV4IRel/" /QRarch4T /QRinterwork-return /O2 /MC  /c 
"E:\Coding\Projekte\34U\MapPanel\MapPanel.cpp"
]
Creating command line "clarm.exe @C:\DOKUME~1\tom\LOKALE~1\Temp\RSPF3.tmp" 
Creating temporary file "C:\DOKUME~1\tom\LOKALE~1\Temp\RSPF4.tmp" with contents
[
/nologo /W3 /D _WIN32_WCE=500 /D "ARM" /D "_ARM_" /D "WCE_PLATFORM_STANDARDSDK_500" /D "ARMV4I" /D UNDER_CE=500 /D "UNICODE" /D "_UNICODE" /D "NDEBUG" /D "_USRDLL" /D "MAPPANEL_EXPORTS" /Fp"ARMV4IRel/MapPanel.pch" /Yc"stdafx.h" /Fo"ARMV4IRel/" /QRarch4T /QRinterwork-return /O2 /MC  /c 
"E:\Coding\Projekte\34U\MapPanel\StdAfx.cpp"
]
Creating command line "clarm.exe @C:\DOKUME~1\tom\LOKALE~1\Temp\RSPF4.tmp" 
Creating temporary file "C:\DOKUME~1\tom\LOKALE~1\Temp\RSPF5.tmp" with contents
[
commctrl.lib coredll.lib   /nologo /base:"0x00100000" /stack:0x10000,0x1000 /entry:"_DllMainCRTStartup" /dll /incremental:no /pdb:"ARMV4IRel/MapPanel.pdb" /nodefaultlib:"libc.lib /nodefaultlib:libcd.lib /nodefaultlib:libcmt.lib /nodefaultlib:libcmtd.lib /nodefaultlib:msvcrt.lib /nodefaultlib:msvcrtd.lib" /def:".\MapPanel.def" /out:"ARMV4IRel/MapPanel.dll" /implib:"ARMV4IRel/MapPanel.lib" /subsystem:windowsce,5.00 /MACHINE:THUMB   
.\ARMV4IRel\StdAfx.obj
.\ARMV4IRel\MapPanel.obj
]
Creating command line "link.exe @C:\DOKUME~1\tom\LOKALE~1\Temp\RSPF5.tmp"
<h3>Output Window</h3>
Compiling...
StdAfx.cpp
Compiling...
MapPanel.cpp
Linking...
   Creating library ARMV4IRel/MapPanel.lib and object ARMV4IRel/MapPanel.exp




<h3>Results</h3>
MapPanel.dll - 0 error(s), 0 warning(s)
</pre>
</body>
</html>
