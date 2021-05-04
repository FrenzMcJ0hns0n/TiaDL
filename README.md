# TiaDL : This is a Doom Launcher

## About
This is a Doom Launcher. A quick and easy-to-use way to play Doom on modern computers.

## Application tree
TiaDL is designed as a portable application, which means all required elements are gathered in the same folder. You need to respect a certain structure in order to use the program as intended : "iwads", "levels", "misc" and "mods" are mandatory folders.

Tested contents as of v3 alpha :
```
TiaDL root folder
|   [log.txt]
|   [presets.csv]
|   ThisIsADoomLauncher.exe
|   
+---config
|       gzdoom-USERNAME-wolf3D.ini
|       gzdoom-USERNAME.ini
|       
+---iwads
|   |   doom.wad
|   |   Doom2.wad
|   |   freedoom1.wad
|   |   freedoom2.wad
|   |   Plutonia.wad
|   |   TNT.wad
|   |   
|   \---Wolf3D
|           Wolf3D.pk3
|           
+---levels
|   |   1klinecp2.wad
|   |   2002ad10.wad
|   |   D2RELOAD.WAD
|   |   D2TWID.wad
|   |   DoomZero.wad
|   |   drkenctr2019.wad
|   |   DTS-T.pk3
|   |   hr.wad
|   |   hr2final.wad
|   |   ICARUS.wad
|   |   PL2.WAD
|   |   PRCP.wad
|   |   Requiem.wad
|   |   SIGIL_v1_21.wad
|   |   zone300.wad
|   |   
|   \---Wolf3D
|           Wolf3D_E1.pk3
|           Wolf3D_E2.pk3
|           Wolf3D_E3.pk3
|           Wolf3D_E4.pk3
|           Wolf3D_E5.pk3
|           Wolf3D_E6.pk3
|           
+---misc
|   |   1klinecp2.deh
|   |   D2TWID.deh
|   |   DOOMZERO.DEH
|   |   
|   \---Wolf3D
|           Wolf3DGL.pk3
|           
+---mods
|   |   Brutal Doom Power Fantasy Final Beta.pk3
|   |   BrutalDoom PowerFantasy v2.pk3
|   |   brutalv20b_R.pk3
|   |   brutalv21.pk3
|   |   DoxsPersonalizedAddon.pk3
|   |   EOA Assets.pk3
|   |   EOA Code.pk3
|   |   STR_EOA_D4_MonstersFixed.pk3
|   |   
|   \---music
|           DoomMetalVol3.wad
|           DoomMetalVol4.wad
|           DoomMetalVol5.wad
|           IDKFAv2.wad
|           
\---ports
    \---gzdoom 4.5.0
        |   brightmaps.pk3
        |   game_support.pk3
        |   game_widescreen_gfx.pk3
        |   gzdoom.exe
        |   gzdoom.pk3
        |   libfluidsynth64.dll
        |   libmpg123-0.dll
        |   libsndfile-1.dll
        |   licenses.zip
        |   lights.pk3
        |   openal32.dll
        |   zmusic.dll
        |   
        +---fm_banks
        |       fmmidi-readme.txt
        |       fmmidi.wopn
        |       gems-fmlib-gmize-readme.txt
        |       gems-fmlib-gmize.wopn
        |       GENMIDI-readme.txt
        |       GENMIDI.GS.wopl
        |       gs-by-papiezak-and-sneakernets-readme.txt
        |       gs-by-papiezak-and-sneakernets.wopn
        |       
        \---soundfonts
                gzdoom.sf2
```

Iwad files aren't free to use, Freedoom 1&2 excepted : you'll need to acquire your own copy of Doom, Doom 2, etc.
The other files (levels, misc, mods, etc.) are community contents, you can get most of them on moddb.com
