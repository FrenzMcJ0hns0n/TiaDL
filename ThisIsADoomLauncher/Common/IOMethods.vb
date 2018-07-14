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

        Try
            Dim errorText As String = ""

            With My.Settings

                If Directory.Exists(.RootDirPath & "\engine\gzdoom\") Then
                    .GzdoomDir = .RootDirPath & "\engine\gzdoom\"
                Else
                    errorText &= Environment.NewLine & "- engine/gzdoom"
                End If

                If Directory.Exists(.RootDirPath & "\engine\zandronum\") Then
                    .ZandronumDir = .RootDirPath & "\engine\zandronum\"
                Else
                    errorText &= Environment.NewLine & "- engine/zandronum"
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

                If Directory.Exists(.RootDirPath & "\misc") Then
                    .MiscDir = .RootDirPath & "\misc"
                Else
                    errorText &= Environment.NewLine & "- misc"
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

            If Not errorText = Nothing Then
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
            If Not File.Exists(path) Then
                Return "does not exist"
            End If

            Dim iwadList As List(Of String) = New List(Of String) From
            {
                "doom.wad", "doom2.wad", "tnt.wad", "plutonia.wad", "freedoom1.wad", "freedoom2.wad"
            }
            If iwadList.Contains(File_GetName(path).ToLowerInvariant) Then
                Return "iwad"
            End If

            Dim extension As String = File_GetExtension(path).ToLowerInvariant
            If extension = ".wad" Or extension = ".pk3" Then
                Return "level"
            ElseIf extension = ".bex" Or extension = ".deh" Then
                Return "misc"
            End If

            Return "unrecognized"

        Catch ex As Exception
            WriteToLog(DateTime.Now & " - Error in 'ValidateFile()'. Exception : " & ex.ToString)
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
                'Dim name As String = New FileInfo(file).Name
                'versionsFound.Add(name)
                versionsFound.Add(File_GetName(file))
            Next
            Return versionsFound

        Catch ex As Exception
            WriteToLog(DateTime.Now & " - Error in 'GetLocalBrutalDoomVersions()'. Exception : " & ex.ToString)
            Return Nothing
        End Try

    End Function




    ''' <summary>
    ''' Handle .ini files management
    ''' TODO (v2+) : Improve custom settings selection with .ini files
    ''' TODO : rename to SetIni() ?
    ''' </summary>
    Sub SetEngineIni()

        Try
            With My.Settings
                Select Case .SelectedEngine.ToLowerInvariant

                    Case "gzdoom"
                        If File.Exists(.GzdoomDir & "\gzdoom-model.ini") Then
                            File.Move(
                                .GzdoomDir & "\gzdoom-model.ini",
                                .GzdoomDir & "\gzdoom-" & Environment.UserName & ".ini")
                        End If

                    Case "zandronum"
                        If File.Exists(.ZandronumDir & "\zandronum-model.ini") Then
                            File.Move(
                                .ZandronumDir & "\zandronum-model.ini",
                                .ZandronumDir & "\zandronum-" & Environment.UserName & ".ini")
                        End If

                End Select
            End With
        Catch ex As Exception
            WriteToLog(DateTime.Now & " - Error in 'SetEngineIni()'. Exception : " & ex.ToString)
        End Try

    End Sub


    Sub WritePresetsFileHeader()

        Dim presetFile As String = My.Settings.RootDirPath & "\presets.txt"

        Using writer As StreamWriter = New StreamWriter(presetFile, True, Text.Encoding.UTF8)
            writer.WriteLine("# Lines starting with ""#"" are ignored by the program")
            writer.WriteLine()
            writer.WriteLine("# Preset pattern :")
            writer.WriteLine("# Name = <value> IWAD = <path> [Level = <path> Misc. = <path>]")
            writer.WriteLine("# 'Name' and 'IWAD' are mandatory values")
            writer.WriteLine("# 'Misc.' refers to .deh or .bex files")
            writer.WriteLine("")
        End Using

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
