Imports System.Globalization
Imports System.IO
Imports System.Reflection
Imports System.Text

Friend Module IOHelper

    ''' <summary>
    ''' Check presence of project directories
    ''' </summary>
    Public Sub CheckProjectDirectories()
        Dim missingDirectories As New List(Of String)

        Try
            DIRECTORIES_LIST.ForEach(Sub(dir) If Not Directory.Exists(GetDirectoryPath(dir)) Then missingDirectories.Add(dir))

            If missingDirectories.Count > 0 Then
                Dim errorMessage As String = ERR_MISSING_DIR & vbCrLf & String.Join(vbCrLf, missingDirectories)

                MessageBox.Show(errorMessage, ERR_STARTUP, MessageBoxButton.OK, MessageBoxImage.Error)
                WriteToLog(Date.Now & " - " & errorMessage)
            End If

        Catch ex As Exception
            Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
            WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}")
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

        Catch ex As Exception
            Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
            WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}{vbCrLf} Parameter(s) : {subDirName}")
        End Try

        Return directoryPath
    End Function

    'TODO: Figure out a way to handle wildcard * in "Wolf3D_*.pk3" (confirmed issue, as recursive search fails on that)
    ''' <summary>
    ''' Get absolute path of a file within TiaDL project
    ''' </summary>
    ''' <param name="targetDir">One of the project directories: "Iwad", "Maps", "Misc", "Mods", "Port", or "" for any</param>
    ''' <param name="filename">The filename to get the absolute path of</param>
    ''' <returns></returns>
    Public Function GetFileAbsolutePath(targetDir As String, filename As String) As String
        Dim absolutePath As String = String.Empty

        Try
            Dim targetDirectory As String = GetDirectoryPath(targetDir)

            'Get file with targetDir provided explicitly by the caller
            If Not targetDir = String.Empty Then
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
            Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
            WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}{vbCrLf} Parameter(s) : {targetDir}, {filename}")
        End Try

functionEnd:
        Return absolutePath
    End Function


    Public Function GetFileInfo_Directory(filepath As String) As String
        Return New FileInfo(filepath).DirectoryName
    End Function

    Public Function GetFileInfo_Extension(filepath As String) As String
        Return New FileInfo(filepath).Extension
    End Function

    Public Function GetFileInfo_RemoveExtension(filepath As String) As String
        Dim fInfo As New FileInfo(filepath)
        Return fInfo.FullName.Replace(fInfo.Extension, String.Empty)
    End Function

    Public Function GetFileInfo_Name(filepath As String, withExtension As Boolean) As String
        Dim info As New FileInfo(filepath)
        Return If(withExtension, info.Name, info.Name.Replace(info.Extension, String.Empty))
    End Function

    Public Function GetFileInfo_Size(filepath As String) As Long
        Return New FileInfo(filepath).Length
    End Function

    Public Function GetFileInfo_Size2(filepath As String) As String
        Dim units() As String = {"b", "Kb", "Mb", "Gb", "Tb"}
        'Dim units As New List(Of String) From {"b", "Kb", "Mb", "Gb", "Tb"}
        Dim pos As Integer = 0
        Dim fLength As Double = New FileInfo(filepath).Length
        While fLength >= 1024 AndAlso pos < units.Count
            fLength /= 1024
            pos += 1
        End While
        'Dim nfi As New NumberFormatInfo With {.NumberGroupSeparator = " ", .NumberDecimalDigits = 0}
        Return String.Format("{0} {1}", fLength, units(pos))
    End Function

    Public Function GetFileInfo_Size2(size As Long) As String
        Dim units() As String = {"b", "Kb", "Mb", "Gb", "Tb"}
        'Dim units As New List(Of String) From {"b", "Kb", "Mb", "Gb", "Tb"}
        Dim pos As Integer = 0
        Dim fLength As Double = size
        While fLength >= 1024 AndAlso pos < units.Count
            fLength /= 1024
            pos += 1
        End While
        'Dim nfi As New NumberFormatInfo With {.NumberGroupSeparator = " ", .NumberDecimalDigits = 0}
        Return String.Format("{0} {1}", Math.Round(fLength, 2), units(pos))
    End Function

    ''' <summary>
    ''' Get local CSV filepath by its name
    ''' </summary>
    ''' <param name="name">Name of the file (without extension)</param>
    ''' <returns>Filepath as a String</returns>
    Public Function GetCsvFilepath(name As String) As String
        Return Path.Combine(GetDirectoryPath(), $"{name}.csv")
    End Function

    ''' <summary>
    ''' Return the text found in the specified JSON file
    ''' </summary>
    ''' <param name="jsonFilepath">Path of the target JSON file</param>
    ''' <returns>JSON data as String</returns>
    Public Function GetJsonData(jsonFilepath As String) As String
        Dim jsonData As String = String.Empty
        Try
            jsonData = File.ReadAllText(jsonFilepath)
        Catch ex As Exception
            Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
            WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}{vbCrLf} Parameter(s) : {jsonFilepath}")
        End Try
        Return jsonData
    End Function

    Public Sub PersistJsonData(filepath As String, jsonData As String)
        Try
            File.WriteAllText(filepath, jsonData)
        Catch ex As Exception
            Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
            WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}{vbCrLf} Parameter(s) : {jsonData}, {filepath}")
        End Try
    End Sub

    Public Function GetJsonFilepath(type As String) As String
        Return Path.Combine(GetDirectoryPath(), $"{type}.json")
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
            Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
            WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}")
        End Try

    End Sub

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
                Case "Port" : Return extension = ".exe" 'TODO? Add other validation rules
                Case "Iwad" : Return extension = ".wad" AndAlso ValidateIwad(filepath)
                Case "Maps" : Return VALID_EXTENSIONS_MAPS.Contains(extension) AndAlso Not ValidateIwad(filepath)
                Case "Misc" : Return VALID_EXTENSIONS_MISC.Contains(extension)
                Case "Pict" : Return VALID_EXTENSIONS_PICT.Contains(extension)
                Case Else 'TODO?
            End Select

        Catch ex As Exception
            Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
            WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}{vbCrLf} Parameter(s) : {filepath}, {type}")
        End Try

        Return False
    End Function

    ''' <summary>
    ''' Check if "filepath" corresponds to an Iwad
    ''' </summary>
    ''' <param name="filepath"></param>
    ''' <returns></returns>
    Private Function ValidateIwad(filepath As String) As Boolean
        Using reader As New BinaryReader(File.OpenRead(filepath))
            Return Encoding.Default.GetString(reader.ReadBytes(4)) = "IWAD"
        End Using
    End Function

    ''' <summary>
    ''' Log content in file 'log.txt'
    ''' </summary>
    ''' 
    Public Sub WriteToLog(content As String)
        Dim logfilePath As String = Path.Combine(GetDirectoryPath(), "log.txt")

        Using streamWriter As New StreamWriter(logfilePath, True)
            streamWriter.WriteLine(content)
        End Using
    End Sub

End Module
