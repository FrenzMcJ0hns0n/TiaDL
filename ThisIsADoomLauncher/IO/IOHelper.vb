Imports System.IO

Module IOHelper

    ''' <summary>
    ''' Return input's parent directory full path
    ''' </summary>
    ''' 
    Function File_GetDir(input As String) As String

        With New FileInfo(input)
            Return If(File.Exists(.FullName), .DirectoryName, Nothing)
        End With

    End Function

    ''' <summary>
    ''' Return input's extension
    ''' </summary>
    ''' 
    Function File_GetExtension(input As String) As String

        With New FileInfo(input)
            Return If(File.Exists(.FullName), .Extension, Nothing)
        End With

    End Function

    ''' <summary>
    ''' Return input's full name (full path)
    ''' </summary>
    ''' 
    Function File_GetFullName(input As String) As String

        With New FileInfo(input)
            Return If(File.Exists(.FullName), .FullName, Nothing)
        End With

    End Function

    ''' <summary>
    ''' Return input's filename
    ''' </summary>
    ''' 
    Function File_GetName(input As String) As String

        With New FileInfo(input)
            Return If(File.Exists(.FullName), .Name, Nothing)
        End With

    End Function

    ''' <summary>
    ''' Build absolute path from Iwad relative filename (Common presets)
    ''' </summary>
    ''' 
    Public Function Path_Iwad_RelativeToAbsolute(iwad As String) As String

        With My.Settings
            Return If(File.Exists(.IwadsDir & "\" & iwad), .IwadsDir & "\" & iwad, Nothing)
        End With

    End Function

    ''' <summary>
    ''' Build absolute path from Level relative filename (Common presets)
    ''' </summary>
    ''' 
    Public Function Path_Level_RelativeToAbsolute(level As String) As String

        With My.Settings
            Return If(File.Exists(.LevelsDir & "\" & level), .LevelsDir & "\" & level, Nothing)
        End With

    End Function

    ''' <summary>
    ''' Build absolute path from Misc relative filename (Common presets)
    ''' </summary>
    ''' 
    Public Function Path_Misc_RelativeToAbsolute(misc As String) As String

        With My.Settings
            Return If(File.Exists(.MiscDir & "\" & misc), .MiscDir & "\" & misc, Nothing)
        End With

    End Function

End Module
