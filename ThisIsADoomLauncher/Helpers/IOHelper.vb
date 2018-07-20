Imports System.IO
Imports System.Reflection

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
    ''' Get the parent directory of TiaDL executable
    ''' </summary>
    ''' 
    Sub SetRootDirPath()

        My.Settings.RootDirPath = Path.GetDirectoryName(Assembly.GetEntryAssembly.Location)

    End Sub

End Module
