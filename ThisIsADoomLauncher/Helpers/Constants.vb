''' <summary>
''' This module holds values regularly used across the TiaDL app
''' </summary>
Friend Module Constants

    Public ReadOnly DIRECTORIES_LIST As New List(Of String) From {"ports", "iwads", "levels", "misc", "mods"} 'TODO(v3 final): Rename
    Public Const ERR_MISSING_DIR As String = "The following project directories were not found : "
    Public Const ERR_STARTUP As String = "Startup error"

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

End Module
