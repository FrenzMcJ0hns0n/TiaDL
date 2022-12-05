''' <summary>
''' This module holds values regularly used across the TiaDL app
''' </summary>
Friend Module Constants

    Public ReadOnly DIRECTORIES_LIST As New List(Of String) From {"Iwad", "Maps", "Misc", "Mods", "Port"}
    Public Const ERR_MISSING_DIR As String = "The following project directories were not found :"
    Public Const ERR_STARTUP As String = "Startup error"

    Public ReadOnly VALID_EXTENSIONS_MAPS As New List(Of String) From {".pk3", ".wad"} '+ .zip ?
    Public ReadOnly VALID_EXTENSIONS_MISC As New List(Of String) From {".bex", ".deh", ".txt"}
    Public ReadOnly VALID_EXTENSIONS_PICT As New List(Of String) From {".jpg", ".jpeg", ".png"}
    Public ReadOnly VALID_EXTENSIONS_MODS As New List(Of String) From {".pk3", ".zip"}

    'TODO v3? Remove
    Public ReadOnly USER_LEVELS_HEADER As New List(Of String) From
    {
        "# Lines starting with ""#"" are ignored by the program",
        "",
        "# Preset pattern:",
        "# <Preset Name>, <Iwad path>, [<Maps path>], [<Misc path>]",
        "",
        "# <Preset Name> and <Iwad path> are mandatory",
        "# <Iwad path> : absolute path to .wad file",
        "# <Maps path> : absolute path to " & String.Join("/", VALID_EXTENSIONS_MAPS) & " file",
        "# <Misc path> : absolute path to " & String.Join("/", VALID_EXTENSIONS_MISC) & " file",
        ""
    }

    Public Enum LVLPRESET_TAB
        None = -1
        Base = 0
        User = 1
        AddNew = 2
    End Enum

    Public Enum MODPRESET_TAB
        None = -1
        Base = 0
        User = 1
        AddNew = 2
    End Enum

    Public Enum SortCriterion
        None = 0
        Name = 1
        Type = 2
        Year = 3
    End Enum

End Module
