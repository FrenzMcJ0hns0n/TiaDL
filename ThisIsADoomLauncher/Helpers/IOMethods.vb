Imports System.IO
Imports System.Reflection
Imports System.Text

Module IOMethods

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
    ''' Get the path of a TiaDL directory. 
    ''' Leave subDirName = Nothing to get RootDirPath
    ''' </summary>
    ''' 
    Function GetDirectoryPath(subDirName As String) As String

        Dim subDirPath As String = Nothing

        Try
            Dim rootDirPath As String = Path.GetDirectoryName(Assembly.GetEntryAssembly.Location)
            Dim combinedPath As String = Path.Combine(rootDirPath, subDirName) 'Use subDirName.ToLowerInvariant ?

            If Directory.Exists(combinedPath) Then subDirPath = combinedPath

        Catch ex As Exception
            WriteToLog(Date.Now & " - Error in 'GetDirectoryPath()'. Exception : " & ex.ToString)
        End Try

        Return subDirPath

    End Function

    ''' <summary>
    ''' Build absolute path from Iwad relative filename
    ''' </summary>
    ''' 
    Function ConvertPathRelativeToAbsolute_Iwad(iwadFilename As String) As String

        Dim iwadAbsolutePath As String = Nothing

        Try
            Dim iwadsDir As String = GetDirectoryPath("iwads")

            'Try files directly in iwadsDir
            Dim probablePath As String = Path.Combine(iwadsDir, iwadFilename)
            If File.Exists(probablePath) Then Return probablePath

            'Search recursively in subdirs
            For Each dir As String In Directory.GetDirectories(iwadsDir)
                For Each fi As FileInfo In New DirectoryInfo(dir).GetFiles
                    If fi.Name = iwadFilename Then Return fi.FullName
                Next
            Next

        Catch ex As Exception
            WriteToLog(Date.Now & " - Error in 'ConvertPathRelativeToAbsolute_Iwad()'. Exception : " & ex.ToString)
        End Try

        Return iwadAbsolutePath

    End Function

    ''' <summary>
    ''' Build absolute path from Level relative filename
    ''' </summary>
    ''' 
    Function ConvertPathRelativeToAbsolute_Level(levelFilename As String) As String

        Dim levelAbsolutePath As String = Nothing

        Try
            Dim levelsDir As String = GetDirectoryPath("levels")

            'Try files directly in levelsDir
            Dim probablePath As String = Path.Combine(levelsDir, levelFilename)
            If File.Exists(probablePath) Then Return probablePath

            'Search recursively in subdirs
            For Each dir As String In Directory.GetDirectories(levelsDir)
                For Each fi As FileInfo In New DirectoryInfo(dir).GetFiles

                    'Handle wildcard for "Wolf3D_*.pk3"
                    If levelFilename.Contains("*") Then
                        Dim wildcardIndex As Integer = levelFilename.IndexOf("*")
                        Dim cutFilename As String = levelFilename.Substring(0, wildcardIndex)
                        If fi.Name.Contains(cutFilename) Then Return Path.Combine(fi.DirectoryName, levelFilename)
                    End If

                    'Normal processing
                    If fi.Name = levelFilename Then Return fi.FullName

                Next
            Next

        Catch ex As Exception
            WriteToLog(Date.Now & " - Error in 'ConvertPathRelativeToAbsolute_Level()'. Exception : " & ex.ToString)
        End Try

        Return levelAbsolutePath

    End Function

    ''' <summary>
    ''' Build absolute path from Misc relative filename
    ''' </summary>
    ''' 
    Function ConvertPathRelativeToAbsolute_Misc(miscFilename As String) As String

        Dim miscAbsolutePath As String = Nothing

        Try
            Dim miscDir As String = GetDirectoryPath("misc")

            'Try files directly in miscDir
            Dim probablePath As String = Path.Combine(miscDir, miscFilename)
            If File.Exists(probablePath) Then Return probablePath

            'Search recursively in subdirs
            For Each dir As String In Directory.GetDirectories(miscDir)
                For Each fi As FileInfo In New DirectoryInfo(dir).GetFiles
                    If fi.Name = miscFilename Then Return fi.FullName
                Next
            Next

        Catch ex As Exception
            WriteToLog(Date.Now & " - Error in 'ConvertPathRelativeToAbsolute_Misc()'. Exception : " & ex.ToString)
        End Try

        Return miscAbsolutePath

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
