Imports System.IO

Module IOHelper

    ''' <summary>
    ''' Build absolute path from iwad relative filename (Common presets)
    ''' </summary>
    ''' 
    Public Function Path_Iwad_RelativeToAbsolute(iwad As String) As String

        With My.Settings
            Return If(File.Exists(.IwadsDir & "\" & iwad), .IwadsDir & "\" & iwad, Nothing)
        End With

    End Function

    ''' <summary>
    ''' Build absolute path from level relative filename (Common presets)
    ''' </summary>
    ''' 
    Public Function Path_Level_RelativeToAbsolute(level As String) As String

        With My.Settings
            Return If(File.Exists(.LevelsDir & "\" & level), .LevelsDir & "\" & level, Nothing)
        End With

    End Function

    ''' <summary>
    ''' Build absolute path from misc relative filename (Common presets)
    ''' </summary>
    ''' 
    Public Function Path_Misc_RelativeToAbsolute(misc As String) As String

        With My.Settings
            Return If(File.Exists(.MiscDir & "\" & misc), .MiscDir & "\" & misc, Nothing)
        End With

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' 
    Function File_GetDir(input As String) As String

        With New FileInfo(input)
            Return If(File.Exists(.FullName), .DirectoryName, Nothing)
        End With
        'Return New FileInfo(input).DirectoryName

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' 
    Function File_GetExtension(input As String) As String

        With New FileInfo(input)
            Return If(File.Exists(.FullName), .Extension, Nothing)
        End With
        'Return New FileInfo(input).Extension

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' 
    Function File_GetFullName(input As String) As String

        With New FileInfo(input)
            Return If(File.Exists(.FullName), .FullName, Nothing)
        End With
        'Return New FileInfo(input).FullName

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' 
    Function File_GetName(input As String) As String

        With New FileInfo(input)
            Return If(File.Exists(.FullName), .Name, Nothing)
        End With
        'Return New FileInfo(input).Name

    End Function

End Module
