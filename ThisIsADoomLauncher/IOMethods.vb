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
    ''' 
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

    ''' <summary>
    ''' Is this filepath refer to an IWAD or a Level ? Return answer
    ''' As string
    ''' </summary>
    ''' 
    Function ValidateFile(path As String) As String

        Try
            Dim fileInfo As FileInfo = New FileInfo(path)
            If Not File.Exists(path) Then
                Return "does not exist"
            End If

            Dim iwadList As List(Of String) = New List(Of String) From
            {
                "doom.wad", "doom2.wad", "tnt.wad", "plutonia.wad", "freedoom1.wad", "freedoom2.wad"
            }
            If iwadList.Contains(fileInfo.Name.ToLowerInvariant) Then
                Return "iwad"
            End If

            Dim extension As String = fileInfo.Extension.ToLowerInvariant
            If extension = ".wad" Or extension = ".pk3" Then
                Return "level"
            ElseIf extension = ".bex" Or extension = ".deh" Then
                Return "misc"
            End If

            Return "unrecognized"

        Catch ex As Exception
            Return "invalid"
        End Try

    End Function

    ''' <summary>
    ''' Return the filename for all files found in /mods/ directory
    ''' As list of string
    ''' </summary>
    ''' 
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
    ''' Build absolute path from iwad relative filename (Common presets)
    ''' </summary>
    ''' 
    Public Function BuildIwadPath(iwad As String) As String

        With My.Settings
            Return If(File.Exists(.IwadsDir & "\" & iwad), .IwadsDir & "\" & iwad, Nothing)
        End With

    End Function

    ''' <summary>
    ''' Build absolute path from level relative filename (Common presets)
    ''' </summary>
    ''' 
    Public Function BuildLevelPath(level As String) As String

        With My.Settings
            Return If(File.Exists(.LevelsDir & "\" & level), .LevelsDir & "\" & level, Nothing)
        End With

    End Function

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
    ''' Logging - Debug
    ''' </summary>
    Sub WriteToLog(content As String)

        Using streamWriter As New StreamWriter(My.Settings.RootDirPath & "\log.txt", True)
            streamWriter.WriteLine(content)
        End Using

    End Sub

End Module
