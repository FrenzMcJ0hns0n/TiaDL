Imports System.IO

Module IOMethods

    ''' <summary>
    ''' Check if all directories can be found
    ''' Validate paths
    ''' </summary>
    ''' 
    Sub ValidateDirectories()

        Dim errorText As String = ""

        Try
            With My.Settings

                Dim directoriesList As List(Of String) = New List(Of String) From {
                    "\engine\gzdoom", "\engine\zandronum", "\iwads", "\levels", "\misc", "\mods", "\music", "\tc"
                }

                For Each dir As String In directoriesList
                    If Not Directory.Exists(.RootDirPath & dir) Then
                        errorText &= Environment.NewLine & dir
                    End If
                Next

                If errorText = Nothing Then
                    .GzdoomDir = .RootDirPath & directoriesList(0)
                    .ZandronumDir = .RootDirPath & directoriesList(1)
                    .IwadsDir = .RootDirPath & directoriesList(2)
                    .LevelsDir = .RootDirPath & directoriesList(3)
                    .MiscDir = .RootDirPath & directoriesList(4)
                    .ModDir = .RootDirPath & directoriesList(5)
                    .MusicDir = .RootDirPath & directoriesList(6)
                    .WolfDir = .RootDirPath & directoriesList(7)
                Else
                    MessageBox.Show("Setup error. Unable to find the following directories :" & errorText)
                    WriteToLog(DateTime.Now & " - Setup error. Directories not found :" & errorText)
                End If

            End With

        Catch ex As Exception
            WriteToLog(DateTime.Now & " - Error in 'CheckDirectories()'. Exception : " & ex.ToString)
        End Try

    End Sub

    ''' <summary>
    ''' Is this filepath refer to an IWAD, a Level or a Misc file ? Return answer as string
    ''' </summary>
    ''' 
    Function ValidateFile(path As String) As String

        Try
            If Not File.Exists(path) Then
                Return "does not exist"
            End If

            Dim iwadList As List(Of String) = New List(Of String) From {"doom.wad", "doom2.wad", "tnt.wad", "plutonia.wad", "freedoom1.wad", "freedoom2.wad"}
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
            For Each file As String In Directory.GetFiles(My.Settings.ModDir)
                versionsFound.Add(File_GetName(file))
            Next
            Return versionsFound

        Catch ex As Exception
            WriteToLog(DateTime.Now & " - Error in 'GetLocalBrutalDoomVersions()'. Exception : " & ex.ToString)
            Return Nothing
        End Try

    End Function

    ''' <summary>
    ''' Set .ini files from models, if they exist
    ''' TODO (v2+) : .ini file selection in GUI ?
    ''' </summary>
    ''' 
    Sub SetIniFiles()

        Try
            With My.Settings
                If File.Exists(.GzdoomDir & "\gzdoom-model.ini") Then
                    File.Move(
                        .GzdoomDir & "\gzdoom-model.ini",
                        .GzdoomDir & "\gzdoom-" & Environment.UserName & ".ini")
                End If

                If File.Exists(.ZandronumDir & "\zandronum-model.ini") Then
                    File.Move(
                        .ZandronumDir & "\zandronum-model.ini",
                        .ZandronumDir & "\zandronum-" & Environment.UserName & ".ini")
                End If

                If File.Exists(.WolfDir & "\gzdoom-model-wolf3D.ini") Then
                    File.Move(
                        .WolfDir & "\gzdoom-model-wolf3D.ini",
                        .WolfDir & "\gzdoom-" & Environment.UserName & "-wolf3D.ini")
                End If
            End With

        Catch ex As Exception
            WriteToLog(DateTime.Now & " - Error in 'SetIniFiles()'. Exception : " & ex.ToString)
        End Try

    End Sub

    ''' <summary>
    ''' Create file 'presets.csv' with some commented lines, if it does not exist
    ''' </summary>
    ''' 
    Sub WritePresetsFileHeader()

        Dim presetFile As String = My.Settings.RootDirPath & "\presets.csv"

        Using writer As StreamWriter = New StreamWriter(presetFile, True, Text.Encoding.UTF8)
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

    End Sub

    ''' <summary>
    ''' Log content in file 'log.txt'
    ''' </summary>
    ''' 
    Sub WriteToLog(content As String)

        Using streamWriter As New StreamWriter(My.Settings.RootDirPath & "\log.txt", True)
            streamWriter.WriteLine(content)
        End Using

    End Sub

End Module
