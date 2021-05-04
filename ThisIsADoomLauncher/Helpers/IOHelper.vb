Imports System.IO
Imports System.Reflection

Module IOHelper

    '''' <summary>
    '''' Return input's parent directory full path
    '''' </summary>
    '''' 
    'Function File_GetDir(input As String) As String

    '    With New FileInfo(input)
    '        Return If(File.Exists(.FullName), .DirectoryName, Nothing)
    '    End With

    'End Function

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
    Function GetSubdirectoryPath(subDirName As String) As String

        Dim subDirPath As String = Nothing

        Try
            Dim rootDirPath As String = Path.GetDirectoryName(Assembly.GetEntryAssembly.Location)
            Dim combinedPath As String = Path.Combine(rootDirPath, subDirName) 'Use subDirName.ToLowerInvariant ?

            If Directory.Exists(combinedPath) Then subDirPath = combinedPath

        Catch ex As Exception
            WriteToLog(DateTime.Now & " - Error in 'GetSubdirectoryPath()'. Exception : " & ex.ToString)
        End Try

        Return subDirPath

    End Function

End Module
