Imports System.IO
Imports System.Reflection

Module IOMethods

    Sub SetRootDirPath()

        My.Settings.RootDirPath = Path.GetDirectoryName(Assembly.GetEntryAssembly.Location)

    End Sub

    ''' <summary>
    ''' Check if all directories can be found
    ''' Validate paths
    ''' </summary>

    Sub ValidateDirectories()
        'TODO
        'engine\gzdoom
        'engine\zandronum

        Try
            Dim errorText As String = ""

            With My.Settings
                If Directory.Exists(.RootDirPath & "\gzdoom") Then
                    .GzdoomDir = .RootDirPath & "\gzdoom"
                Else
                    errorText &= Environment.NewLine & "- gzdoom"
                End If

                If Directory.Exists(.RootDirPath & "\zandronum") Then
                    .ZandronumDir = .RootDirPath & "\zandronum"
                Else
                    errorText &= Environment.NewLine & "- zandronum"
                End If

                If Directory.Exists(.RootDirPath & "\iwads") Then
                    .IwadsDir = .RootDirPath & "\iwads"
                Else
                    errorText &= Environment.NewLine & "- iwads"
                End If

                If Directory.Exists(.RootDirPath & "\levels") Then
                    .LevelsDir = .RootDirPath & "\levels"
                Else
                    errorText &= Environment.NewLine & "- levels"
                End If

                If Directory.Exists(.RootDirPath & "\mods") Then
                    .ModDir = .RootDirPath & "\mods"
                Else
                    errorText &= Environment.NewLine & "- mods"
                End If

                If Directory.Exists(.RootDirPath & "\music") Then
                    .MusicDir = .RootDirPath & "\music"
                Else
                    errorText &= Environment.NewLine & "- music"
                End If
            End With

            If Not errorText = "" Then
                MessageBox.Show("Setup error. Unable to find the following directories :" & errorText)
                WriteToLog(DateTime.Now & " - Setup error. Directories not found :" & errorText)
            End If

        Catch ex As Exception
            WriteToLog(DateTime.Now & " - Error in 'CheckDirectories()'. Exception : " & ex.ToString)
        End Try

    End Sub

    'Function CheckFile(wadInfo As FileInfo) As String

    '    Dim name As String = wadInfo.Name.ToLowerInvariant
    '    If name = "doom.wad" Or name = "doom2.wad" Or name = "tnt.wad" Or name = "plutonia.wad" Then
    '        Return "iwad"
    '    End If

    '    Dim ext As String = wadInfo.Extension.ToLowerInvariant
    '    If ext = ".wad" Or ext = ".pk3" Then
    '        Return "level"
    '    End If

    '    Return "invalid"

    'End Function

    ''' <summary>
    ''' Is this filepath refer to an IWAD or a Level ? Return answer
    ''' As string
    ''' </summary>
    Function ValidateFile(path As String) As String

        Try
            Dim fileInfo As FileInfo = New FileInfo(path)
            If Not File.Exists(path) Then
                Return "does not exist"
            End If

            Dim name As String = fileInfo.Name.ToLowerInvariant
            If name = "doom.wad" Or name = "doom2.wad" Or name = "tnt.wad" Or name = "plutonia.wad" Then
                Return "iwad"
            End If

            Dim extension As String = fileInfo.Extension.ToLowerInvariant
            If extension = ".wad" Or extension = ".pk3" Then
                Return "level"
            End If
            Return "unrecognized"

        Catch ex As Exception
            Return "invalid"
        End Try

    End Function

    'Not used
    Function CheckPath(filename As String) As Boolean

        If File.Exists(filename) Then
            Return True
        End If
        Return False

    End Function

    ''' <summary>
    ''' Return the filename for all files found in /mods/ directory
    ''' As list of string
    ''' </summary>
    Function GetLocalBrutalDoomVersions() As List(Of String)

        Dim versionsFound As List(Of String) = New List(Of String)

        Try
            Dim files() = Directory.GetFiles(My.Settings.ModDir)
            For Each file As String In files
                Dim name As String = New FileInfo(file).Name
                versionsFound.Add(name)
            Next
            Return versionsFound

        Catch ex As Exception
            WriteToLog(DateTime.Now & " - Error in 'GetLocalBrutalDoomVersions()'. Exception : " & ex.ToString)
            Return Nothing
        End Try

    End Function

    ''' <summary>
    ''' Return parsed values, from file "presets.txt"
    ''' As a list of list of string
    ''' 
    ''' For instance : 
    ''' ("MyFirstPreset","Doom2","Level.wad"), ("MySecondPreset", "Doom2", "AnotherLevel.wad")
    ''' </summary>
    Function GetPresetsFromFile(presetFile As String) As List(Of List(Of String))

        Dim presetLines As List(Of List(Of String)) = New List(Of List(Of String))

        If Not File.Exists(My.Settings.RootDirPath & "\presets.txt") Then
            Return presetLines 'presetLines is Nothing
        End If

        Try
            For Each line As String In File.ReadLines(My.Settings.RootDirPath & "\presets.txt")

                If line.Trim.StartsWith("#") Then
                    Continue For 'Ignore lines with "#" as first read char
                End If

                If line.Contains("Name =") And line.Contains("IWAD =") Then

                    Dim argLine As List(Of String) = New List(Of String)
                    Dim _start As Integer
                    Dim _end As Integer

                    'Name -------------------------------------------------------------------------------------
                    _start = line.IndexOf("Name =") + 6
                    _end = line.IndexOf("IWAD =") - 6
                    Dim presetName As String = line.Substring(_start, _end).Trim 'need test
                    argLine.Add(presetName)

                    'IWAD -------------------------------------------------------------------------------------
                    Dim presetIwad As String = ""
                    If Not line.Contains("Level =") Then
                        _start = line.IndexOf("IWAD =") + 6
                        presetIwad = line.Substring(_start).Trim 'need test
                        argLine.Add(presetIwad)
                        presetLines.Add(argLine) '=> Return preset with : Name, IWAD
                        Continue For
                    End If
                    _start = line.IndexOf("IWAD =") + 6
                    _end = line.IndexOf("Level =")
                    presetIwad = line.Substring(_start, _end - _start).Trim 'need test
                    argLine.Add(presetIwad)

                    'Level -------------------------------------------------------------------------------------
                    Dim presetLevel As String = ""
                    If Not line.Contains("Misc. =") Then
                        _start = line.IndexOf("Level =") + 7
                        presetLevel = line.Substring(_start).Trim 'need test
                        argLine.Add(presetLevel)
                        presetLines.Add(argLine) '=> Return preset with : Name, IWAD, Level
                        Continue For
                    End If
                    _start = line.IndexOf("Level =") + 7
                    _end = line.IndexOf("Misc. =")
                    presetLevel = line.Substring(_start, _end - _start).Trim 'need test
                    argLine.Add(presetLevel)

                    'Misc. -------------------------------------------------------------------------------------
                    _start = line.IndexOf("Misc. =") + 7
                    Dim presetMisc As String = line.Substring(_start).Trim 'need test
                    argLine.Add(presetMisc)
                    presetLines.Add(argLine) '=> Return preset with : Name, IWAD, Level, Misc.
                End If
            Next

        Catch ex As Exception
            MessageBox.Show("Error. Exception :" & Environment.NewLine & ex.ToString)
        End Try

        Return presetLines

    End Function

    Public Function BuildIwadPath(iwad As String) As String

        With My.Settings

            If File.Exists(.IwadsDir & "\" & iwad) Then
                Return .IwadsDir & "\" & iwad
            End If
            Return Nothing

        End With

    End Function

    Public Function BuildLevelPath(level As String) As String

        With My.Settings

            If File.Exists(.LevelsDir & "\" & level) Then
                Return .LevelsDir & "\" & level
            End If
            Return Nothing

        End With

    End Function

    'Public Function BuildMiscPath(misc As String) As String

    '    With My.Settings

    '        If File.Exists(misc) Then
    '            Return .LevelsDir & "\" & level
    '        End If
    '        Return Nothing

    '    End With

    'End Function

    ''' <summary>
    ''' Handle .ini files management
    ''' TODO (v2+) : Improve custom settings selection with .ini files
    ''' TODO : rename to SetIni() ?
    ''' </summary>
    Sub SetEngineIni()

        With My.Settings
            Select Case .SelectedEngine.ToLowerInvariant
                Case "gzdoom"
                    If File.Exists(.GzdoomDir & "\gzdoom-model.ini") Then
                        File.Move(
                            .GzdoomDir & "\gzdoom-model.ini",
                            .GzdoomDir & "\gzdoom-" & Environment.UserName & ".ini")
                    End If
                Case "zandronum"
                    'TODO
                    '...
            End Select
        End With

    End Sub

    ''' <summary>
    ''' Write attributes for New user preset
    ''' As line in 'presets.txt'
    ''' </summary>
    Sub WritePresetToFile(name As String, iwad As String, level As String, misc As String)

        Try
            Dim pathToPreset As String = My.Settings.RootDirPath & "\presets.txt"

            'A DEPLACER >
            'Create file with some lines
            If Not File.Exists(pathToPreset) Then
                Using streamWriter As New StreamWriter(pathToPreset, True, Text.Encoding.Unicode)
                    streamWriter.WriteLine("# Lines starting with ""#"" are ignored by the program")
                    streamWriter.WriteLine()
                    streamWriter.WriteLine("# Preset pattern :")
                    streamWriter.WriteLine("# Name = <value> IWAD = <path> [Level = <path> Misc. = <path>]")
                    streamWriter.WriteLine("# 'Name' and 'IWAD' are mandatory values")
                    streamWriter.WriteLine("# 'Misc.' refers to .deh or .dex files")
                    streamWriter.WriteLine("")
                    streamWriter.WriteLine("# Presets")
                End Using
            End If

            'Write the preset
            Using streamWriter As New StreamWriter(pathToPreset, True, Text.Encoding.Unicode)
                streamWriter.WriteLine(String.Format(
                    "Name = {0} IWAD = {1} Level = {2} Misc = {3}",
                    name, iwad, level, misc))
            End Using

        Catch ex As Exception
            WriteToLog(DateTime.Now & " - Error in 'WritePresetToFile()'. Exception : " & ex.ToString)
        End Try


    End Sub

    ''' <summary>
    ''' Logging - Debug
    ''' </summary>
    Sub WriteToLog(content As String)

        Using streamWriter As New StreamWriter(My.Settings.RootDirPath & "\log.txt", True)
            streamWriter.WriteLine(content)
        End Using

    End Sub

End Module
