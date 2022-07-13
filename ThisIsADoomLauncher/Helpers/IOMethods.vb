Imports System.IO
Imports System.Reflection
Imports System.Text

Module IOMethods

    Function GetJsonFilepath() As String
        Return Path.Combine(GetDirectoryPath(), "try.json")
    End Function

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

    'TODO: Determine if useful
    Private Function ConvertFilePath_AbsoluteToRelative(filePaths As List(Of String)) As List(Of String)
        Dim fileNames As New List(Of String)

        Try
            For Each path As String In filePaths
                fileNames.Add(New FileInfo(path).Name)
            Next
        Catch ex As Exception
            WriteToLog(Date.Now & " - Error in 'ConvertFilePath_AbsoluteToRelative()'. Exception : " & ex.ToString)
        End Try

        Return fileNames
    End Function

    ''' <summary>
    ''' Get the path of a TiaDL directory.
    ''' </summary>
    ''' <param name="subDirName">Target directory. Leave empty ("") to get the root dir</param>
    ''' <returns></returns>
    Function GetDirectoryPath(Optional subDirName As String = Nothing) As String

        Dim directoryPath As String = Path.GetDirectoryName(Assembly.GetEntryAssembly.Location)

        Try
            Dim combinedPath As String = Path.Combine(directoryPath, subDirName)
            If Directory.Exists(combinedPath) Then directoryPath = combinedPath

        Catch ex As Exception
            WriteToLog(Date.Now & " - Error in 'GetDirectoryPath()'. Exception : " & ex.ToString)
        End Try

        Return directoryPath

    End Function

    'TODO: Use consistent names everywhere (Port, Iwad, Level, Misc, Mods)
    'TODO: Investigate about old code > ConvertPathRelativeToAbsolute_Level : Handle wildcard for "Wolf3D_*.pk3"
    ''' <summary>
    ''' Get absolute path of a file from relative one.
    ''' Requires a directory to search in, as target.
    ''' No target ("") means search everywhere
    ''' </summary>
    ''' <param name="targetName">One of the project directories (port, iwads, levels, misc, mods)</param>
    ''' <param name="filename">The filename to get the absolute path of</param>
    ''' <returns></returns>
    Public Function GetFileAbsolutePath(targetName As String, filename As String) As String
        Dim absolutePath As String = String.Empty

        Try
            Dim targetDirectory As String = GetDirectoryPath(targetName)

            'Try file directly in target directory
            Dim probablePath As String = Path.Combine(targetDirectory, filename)
            If File.Exists(probablePath) Then absolutePath = probablePath

            'Search recursively in subdirs
            For Each dir As String In Directory.GetDirectories(targetDirectory)
                Dim fileInfoList As FileInfo() = New DirectoryInfo(dir).GetFiles
                For Each fi As FileInfo In fileInfoList
                    If fi.Name = filename Then absolutePath = fi.FullName
                Next
            Next
        Catch ex As Exception
            WriteToLog(Date.Now & " - Error in 'GetFileAbsolutePath()'. Exception : " & ex.ToString)
        End Try

        Return absolutePath
    End Function

    ''' <summary>
    ''' Set .ini files from models, at first TiaDL launch
    ''' TODO (v2+) : .ini file selection in GUI ?
    ''' </summary>
    ''' 
    Sub SetIniFiles()

        Try
            'TODO : Write v3 equivalent

            'With My.Settings
            '    If File.Exists(Path.Combine(.GzdoomDir, "gzdoom-model.ini")) Then
            '        File.Move(
            '            Path.Combine(.GzdoomDir, "gzdoom-model.ini"),
            '            Path.Combine(.GzdoomDir, "gzdoom-" & Environment.UserName & ".ini"))
            '    End If

            '    If File.Exists(Path.Combine(.ZandronumDir, "zandronum-model.ini")) Then
            '        File.Move(
            '            Path.Combine(.ZandronumDir, "zandronum-model.ini"),
            '            Path.Combine(.ZandronumDir, "zandronum-" & Environment.UserName & ".ini"))
            '    End If

            '    If File.Exists(Path.Combine(.WolfDir, "gzdoom-model-wolf3D.ini")) Then
            '        File.Move(
            '            Path.Combine(.WolfDir, "gzdoom-model-wolf3D.ini"),
            '            Path.Combine(.WolfDir, "gzdoom-model-" & Environment.UserName & "-wolf3D.ini"))
            '    End If
            'End With

        Catch ex As Exception
            WriteToLog(Date.Now & " - Error in 'SetIniFiles()'. Exception : " & ex.ToString)
        End Try

    End Sub

    'TODO: Use constants and maybe rewrite
    ''' <summary>
    ''' Check if all directories can be found
    ''' Validate paths
    ''' </summary>
    ''' 
    Sub CheckProjectSubdirectories()

        Dim errorText As String = Nothing

        Try
            For Each dir As String In New List(Of String) From {"iwads", "levels", "misc", "mods"}
                If GetDirectoryPath(dir) = Nothing Then errorText &= Environment.NewLine & dir
            Next

            If Not errorText = Nothing Then
                MessageBox.Show("Startup error. Unable to find the following subdirectories :" & errorText)
                WriteToLog(Date.Now & " - Startup error. Subdirectories not found :" & errorText)
            End If

        Catch ex As Exception
            WriteToLog(Date.Now & " - Error in 'CheckProjectSubdirectories()'. Exception : " & ex.ToString)
        End Try

    End Sub

    'TODO: Rewrite as only one function
    Function ValidateFile_Iwad(filepath As String) As Boolean

        Try
            Using reader As New BinaryReader(File.OpenRead(filepath))
                Dim bytes As Byte() = reader.ReadBytes(4)
                Dim fileh As String = Encoding.Default.GetString(bytes)

                If fileh = "IWAD" Then Return True
            End Using

        Catch ex As Exception
            WriteToLog(Date.Now & " - Error in 'ValidateFileIwad()'. Exception : " & ex.ToString)
        End Try

        Return False

    End Function

    'TODO: Rewrite as only one function
    Function ValidateFile_Level(filepath As String) As Boolean

        Try
            Dim file_info As New FileInfo(filepath)
            Dim extension As String = file_info.Extension.ToLowerInvariant
            Dim valid_ext As String() = {".pk3", ".wad"}

            If valid_ext.Contains(extension) Then Return True

        Catch ex As Exception
            WriteToLog(Date.Now & " - Error in 'ValidateFileLevel()'. Exception : " & ex.ToString)
        End Try

        Return False

    End Function

    'TODO: Rewrite as only one function
    Function ValidateFile_Misc(filepath As String) As Boolean

        Try
            Dim file_info As New FileInfo(filepath)
            Dim extension As String = file_info.Extension.ToLowerInvariant
            Dim valid_ext As String() = {".bex", ".deh"}

            If valid_ext.Contains(extension) Then Return True

        Catch ex As Exception
            WriteToLog(Date.Now & " - Error in 'ValidateFileMisc()'. Exception : " & ex.ToString)
        End Try

        Return False

    End Function

    'TODO: Rewrite as only one function
    Function ValidateFile_Image(filepath As String) As Boolean

        Try
            Dim file_info As New FileInfo(filepath)
            Dim extension As String = file_info.Extension.ToLowerInvariant
            Dim valid_ext As String() = {".jpg", ".jpeg", ".png"}

            If valid_ext.Contains(extension) Then Return True

        Catch ex As Exception
            WriteToLog(Date.Now & " - Error in 'ValidateFileImage()'. Exception : " & ex.ToString)
        End Try

        Return False

    End Function

    'TODO: Use constants and maybe rewrite
    ''' <summary>
    ''' Create file 'presets.csv' with some commented lines, if it does not exist
    ''' </summary>
    ''' 
    Sub WritePresetsFileHeader()

        Try
            Dim rootDirPath = GetDirectoryPath("")
            Dim presetFile As String = Path.Combine(rootDirPath, "presets.csv")

            Using writer As New StreamWriter(presetFile, True, Encoding.Default)
                writer.WriteLine("# Lines starting with ""#"" are ignored by the program")
                writer.WriteLine()
                writer.WriteLine("# Preset pattern :")
                writer.WriteLine("# <Preset Name>, <Iwad path> [,<Level path>] [,<Misc. path>]")
                writer.WriteLine()
                writer.WriteLine("# <Preset Name> and <Iwad path> are mandatory")
                writer.WriteLine("# <Iwad path> : absolute path to .wad file")
                writer.WriteLine("# <Level path> : absolute path to .wad/.pk3 file")
                writer.WriteLine("# <Misc. path> : absolute path to .deh/.dex file")
                writer.WriteLine()
            End Using

        Catch ex As Exception
            WriteToLog(Date.Now & " - Error in 'WritePresetsFileHeader()'. Exception : " & ex.ToString)
        End Try

    End Sub

    'TODO: Write Date.Now here, not in parameter "content"
    ''' <summary>
    ''' Log content in file 'log.txt'
    ''' </summary>
    ''' 
    Sub WriteToLog(content As String)

        Dim rootDirPath = GetDirectoryPath("")
        Dim logfilePath = Path.Combine(rootDirPath, "log.txt")

        Using streamWriter As New StreamWriter(logfilePath, True)
            streamWriter.WriteLine(content)
        End Using

    End Sub

End Module
