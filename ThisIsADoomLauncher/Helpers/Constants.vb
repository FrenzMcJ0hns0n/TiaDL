''' <summary>
''' This module holds values regularly used across the TiaDL app
''' </summary>
Friend Module Constants

    Public ReadOnly DIRECTORIES_LIST As New List(Of String) From {"ports", "iwads", "levels", "misc", "mods"} 'TODO(v3 final): Rename
    Public Const ERR_MISSING_DIR As String = "The following project directories were not found : "
    Public Const ERR_STARTUP As String = "Startup error"

End Module
