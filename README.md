# TiaDL : This is a Doom Launcher

## About
This is a Doom Launcher. It's meant to provide an easy-to-use and rather organized way to play Doom with or without mods. I know there are several other softwares like that, but I won't care or compare against such competitors until I get fully satisfied with what I do here.

## Application tree
TiaDL is designed as a portable application, which means all required elements are gathered in the same folder. You need to respect the following structure in order to use the program as intended :

```
TiaDL root folder
.
|   [presets.csv]
|   ThisIsADoomLauncher.exe
|   
+---engine
|   +---gzdoom
|   |   |   brightmaps.pk3
|   |   |   [gzdoom-model.ini]
|   |   |   gzdoom.exe
|   |   |   gzdoom.pk3
|   |   |   libfluidsynth64.dll
|   |   |   libmpg123-0.dll
|   |   |   libsndfile-1.dll
|   |   |   licenses.zip
|   |   |   lights.pk3
|   |   |   openal32.dll
|   |   |   zd_extra.pk3
|   |   |   
|   |   \---soundfonts
|   |           gzdoom.sf2
|   |           
|   \---zandronum
|           fmodex.dll
|           skulltag_actors.pk3
|           [zandronum-model.ini]
|           zandronum.exe
|           zandronum.pk3
+---iwads
|       doom.wad
|       Doom2.wad
|       freedoom1.wad
|       freedoom2.wad
|       Plutonia.wad
|       TNT.wad
|       
+---levels
|       2002ad10.wad
|       D2RELOAD.WAD
|       D2TWID.wad
|       DTS-T.pk3
|       hr.wad
|       hr2final.wad
|       ICARUS.wad
|       PL2.WAD
|       PRCP.wad
|       Requiem.wad
|       zone300.wad
|       (to be completed ...)
|       
+---misc
|       D2TWID.deh
|       
+---mods
|       bd21testApr25.pk3
|       brutalv20b_R.pk3
|       
+---music
|       DoomMetalVol4.wad
|       IDKFAv2.wad
|       
+---tc
|       [gzdoom-model-wolf3D.ini]
|       Wolf3D.pk3
|       Wolf3DGL.pk3
|       Wolf3D_E1.pk3
|       Wolf3D_E2.pk3
|       Wolf3D_E3.pk3
|       Wolf3D_E4.pk3
|       Wolf3D_E5.pk3
|       Wolf3D_E6.pk3
```
.ini files (containing correct keybindings mainly) and all Iwads are included in the releases I share with my friends. But I can't submit those on GitHub since they aren't free to use, Freedoom 1&2 excepted : you'll need to acquire your own copy of Doom, Doom 2, ...
