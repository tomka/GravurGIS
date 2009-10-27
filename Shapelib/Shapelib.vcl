<html>
<body>
<pre>
<h1>Build Log</h1>
<h3>
--------------------Configuration: Shapelib - Win32 (WCE ARMV4) Release--------------------
</h3>
<h3>Command Lines</h3>
Creating temporary file "C:\DOKUME~1\tom\LOKALE~1\Temp\RSP1EC.tmp" with contents
[
/nologo /W3 /D _WIN32_WCE=420 /D "WIN32_PLATFORM_PSPC=400" /D "ARM" /D "_ARM_" /D "ARMV4" /D UNDER_CE=420 /D "UNICODE" /D "_UNICODE" /D "NDEBUG" /D "_USRDLL" /D "SHAPELIB_EXPORTS" /FR"ARMV4Rel/" /Fp"ARMV4Rel/Shapelib.pch" /YX /Fo"ARMV4Rel/" /O2 /MC /c 
"E:\Coding\Projekte\34U\PDA\Shapelib\quadtree.c"
]
Creating command line "clarm.exe @C:\DOKUME~1\tom\LOKALE~1\Temp\RSP1EC.tmp" 
Creating temporary file "C:\DOKUME~1\tom\LOKALE~1\Temp\RSP1ED.tmp" with contents
[
commctrl.lib coredll.lib /nologo /base:"0x00100000" /stack:0x10000,0x1000 /entry:"_DllMainCRTStartup" /dll /incremental:no /pdb:"ARMV4Rel/Shapelib.pdb" /nodefaultlib:"libc.lib /nodefaultlib:libcd.lib /nodefaultlib:libcmt.lib /nodefaultlib:libcmtd.lib /nodefaultlib:msvcrt.lib /nodefaultlib:msvcrtd.lib" /out:"ARMV4Rel/Shapelib.dll" /implib:"ARMV4Rel/Shapelib.lib" /subsystem:windowsce,4.20 /align:"4096" /MACHINE:ARM 
.\ARMV4Rel\assert_wince.obj
.\ARMV4Rel\dbfopen.obj
.\ARMV4Rel\generator.obj
.\ARMV4Rel\shpopen.obj
.\ARMV4Rel\quadtree.obj
]
Creating command line "link.exe @C:\DOKUME~1\tom\LOKALE~1\Temp\RSP1ED.tmp"
<h3>Output Window</h3>
Compiling...
quadtree.c
Linking...
   Creating library ARMV4Rel/Shapelib.lib and object ARMV4Rel/Shapelib.exp
Creating command line "bscmake.exe /nologo /o"ARMV4Rel/Shapelib.bsc"  .\ARMV4Rel\assert_wince.sbr .\ARMV4Rel\dbfopen.sbr .\ARMV4Rel\generator.sbr .\ARMV4Rel\shpopen.sbr .\ARMV4Rel\quadtree.sbr"
Creating browse info file...
<h3>Output Window</h3>




<h3>Results</h3>
Shapelib.dll - 0 error(s), 0 warning(s)
</pre>
</body>
</html>
