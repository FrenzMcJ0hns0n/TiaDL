Imports System.IO
Imports System.Reflection
Imports System.Text

Friend Module IOMethods


#Region "FileInfo helpers : useless ?"

    ''' <summary>
    ''' Return input's parent directory full path
    ''' </summary>
    '''
    Public Function File_GetDir(input As String) As String

        With New FileInfo(input)
            Return If(File.Exists(.FullName), .DirectoryName, Nothing)
        End With

    End Function

    ''' <summary>
    ''' Return input's extension
    ''' </summary>
    ''' 
    Public Function File_GetExtension(input As String) As String

        With New FileInfo(input)
            Return If(File.Exists(.FullName), .Extension, Nothing)
        End With

    End Function

    ''' <summary>
    ''' Return input's filename
    ''' </summary>
    ''' 
    Public Function File_GetName(input As String) As String

        With New FileInfo(input)
            Return If(File.Exists(.FullName), .Name, Nothing)
        End With

    End Function

#End Region


#Region "Other helpers"

    ''' <summary>
    ''' Check presence of project directories
    ''' </summary>
    Public Sub CheckProjectDirectories()
        Dim missingDirectories As New List(Of String)

        Try
            For Each dir As String In DIRECTORIES_LIST
                If Not Directory.Exists(GetDirectoryPath(dir)) Then missingDirectories.Add(dir)
            Next

            If missingDirectories.Count > 0 Then
                MessageBox.Show(ERR_MISSING_DIR & vbCrLf & String.Join(vbCrLf, missingDirectories), ERR_STARTUP, MessageBoxButton.OK, MessageBoxImage.Error)
                WriteToLog(Date.Now & " - " & ERR_STARTUP & ". " & ERR_MISSING_DIR & String.Join(", ", missingDirectories))
            End If
        Catch ex As Exception
            WriteToLog(Date.Now & " - Error in 'CheckProjectDirectories()'. Exception : " & ex.ToString)
        End Try
    End Sub

    ''' <summary>
    ''' Get the path of a TiaDL directory.
    ''' </summary>
    ''' <param name="subDirName">Target. Leave unfilled () or empty ("") to get the project root directory</param>
    ''' <returns></returns>
    Public Function GetDirectoryPath(Optional subDirName As String = "") As String
        Dim directoryPath As String = Path.GetDirectoryName(Assembly.GetEntryAssembly.Location)

        Try
            Dim combinedPath As String = Path.Combine(directoryPath, subDirName)
            directoryPath = combinedPath
        Catch ex As ArgumentNullException
            WriteToLog(Date.Now & " - Warning, potential error in 'GetDirectoryPath()' as parameter 'subDirName' was null")
        Catch ex As Exception
            WriteToLog(Date.Now & " - Error in 'GetDirectoryPath()'. Exception : " & ex.ToString)
        End Try

        Return directoryPath
    End Function

    'TODO: Use consistent names everywhere (Port, Iwad, Level, Misc, Mods)
    'TODO: Figure out a way to handle wildcard * in "Wolf3D_*.pk3" (confirmed issue, as recursive search fails on that)
    ''' <summary>
    ''' Get the absolute path of a file from its relative one within TiaDL project tree
    ''' </summary>
    ''' <param name="targetName">One of the project directories: "port", "iwads", "levels", "misc", "mods", or "" for any</param>
    ''' <param name="filename">The filename to get the absolute path of</param>
    ''' <returns></returns>
    Public Function GetFileAbsolutePath(targetName As String, filename As String) As String
        Dim absolutePath As String = String.Empty

        Try
            Dim targetDirectory As String = GetDirectoryPath(targetName)

            'Get file with targetName provided explicitly from caller
            If Not targetName = String.Empty Then
                absolutePath = Path.Combine(targetDirectory, filename)
                GoTo functionEnd
            End If

            'Search recursively from root directory, as no targetName
            Dim allFiles() As String = Directory.GetFiles(targetDirectory, "*.*", SearchOption.AllDirectories)
            For Each filepath As String In allFiles
                If New FileInfo(filepath).Name.ToLowerInvariant = filename.ToLowerInvariant Then
                    absolutePath = filepath
                    GoTo functionEnd
                End If
            Next

        Catch ex As Exception
            WriteToLog(Date.Now & " - Error in 'GetFileAbsolutePath()'. Exception : " & ex.ToString)
        End Try

functionEnd:
        Return absolutePath
    End Function

    Public Function GetJsonFilepath() As String
        Return Path.Combine(GetDirectoryPath(), "try.json")
    End Function

    ''' <summary>
    ''' Set .ini files from models, at first TiaDL launch
    ''' TODO (v2+) : .ini file selection in GUI ?
    ''' </summary>
    ''' 
    Public Sub SetIniFiles()

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

    'TODO: Use consistent names everywhere (Port, Iwad, Level, Misc, Mods)
    ''' <summary>
    ''' Validate file as proper Doom content
    ''' </summary>
    ''' <param name="filepath">Filepath of the file to validate</param>
    ''' <param name="type">Target, such as : Port, Iwad, Level, Misc, Mods</param>
    ''' <returns></returns>
    Public Function ValidateFile(filepath As String, type As String) As Boolean
        Try
            Dim extension As String = New FileInfo(filepath).Extension.ToLowerInvariant

            Select Case type
                Case "Port"
                    'TODO? Add other validation rules
                    If extension = ".exe" Then Return True

                Case "Iwad"
                    Using reader As New BinaryReader(File.OpenRead(filepath))
                        Dim bytes As Byte() = reader.ReadBytes(4)
                        If Encoding.Default.GetString(bytes) = "IWAD" And extension = ".wad" Then Return True
                    End Using

                Case "Level"
                    Dim validExtensions As New List(Of String) From {".pk3", ".wad", ".zip"} 'TODO: Use constants or similar
                    If validExtensions.Contains(extension) Then Return True

                Case "Misc"
                    Dim validExtensions As New List(Of String) From {".bex", ".deh", ".txt"} 'TODO: Use constants or similar
                    If validExtensions.Contains(extension) Then Return True

                Case "Image"
                    Dim validExtensions As New List(Of String) From {".jpg", ".jpeg", ".png"} 'TODO: Use constants or similar
                    If validExtensions.Contains(extension) Then Return True

                Case Else 'TODO?

            End Select
        Catch ex As Exception
            WriteToLog(Date.Now & " - Error in 'ValidateFile()'. Exception : " & ex.ToString)
        End Try

        Return False
    End Function

    'TODO: Use constants and maybe rewrite
    ''' <summary>
    ''' Create file 'presets.csv' with some commented lines, if it does not exist
    ''' </summary>
    ''' 
    Public Sub WritePresetsFileHeader()

        Try
            Dim rootDirPath = GetDirectoryPath()
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
    Public Sub WriteToLog(content As String)

        Dim rootDirPath = GetDirectoryPath()
        Dim logfilePath = Path.Combine(rootDirPath, "log.txt")

        Using streamWriter As New StreamWriter(logfilePath, True)
            streamWriter.WriteLine(content)
        End Using

    End Sub

#End Region


End Module
