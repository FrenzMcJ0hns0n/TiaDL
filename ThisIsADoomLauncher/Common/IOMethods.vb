Imports System.IO

Module IOMethods

    ''' <summary>
    ''' Return the filename for all files found in /mods/ directory
    ''' As list of string
    ''' </summary>
    ''' 
    Function GetLocalBrutalDoomVersions() As List(Of String)

        Try
            Dim versionsFound As List(Of String) = New List(Of String)

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
    ''' Build absolute path from Iwad relative filename
    ''' </summary>
    ''' 
    Function ConvertIwadPath_RelativeToAbsolute(iwad As String) As String

        Try
            If iwad = "Wolf3D" Then Return iwad

            Dim iwadPath As String = Path.Combine(My.Settings.IwadsDir, iwad)
            Return If(File.Exists(iwadPath), iwadPath, Nothing)

        Catch ex As Exception
            WriteToLog(DateTime.Now & " - Error in 'ConvertIwadPath_RelativeToAbsolute()'. Exception : " & ex.ToString)
            Return Nothing
        End Try

    End Function

    ''' <summary>
    ''' Build absolute path from Level relative filename
    ''' </summary>
    ''' 
    Function ConvertLevelPath_RelativeToAbsolute(level As String) As String

        Try
            Dim levelPath = Path.Combine(My.Settings.LevelsDir, level)
            Return If(File.Exists(levelPath), levelPath, Nothing)

        Catch ex As Exception
            WriteToLog(DateTime.Now & " - Error in 'ConvertLevelPath_RelativeToAbsolute()'. Exception : " & ex.ToString)
            Return Nothing
        End Try

    End Function

    ''' <summary>
    ''' Build absolute path from Misc relative filename
    ''' </summary>
    ''' 
    Function ConvertMiscPath_RelativeToAbsolute(misc As String) As String

        Try
            Dim miscPath = Path.Combine(My.Settings.MiscDir, misc)
            Return If(File.Exists(miscPath), miscPath, Nothing)

        Catch ex As Exception
            WriteToLog(DateTime.Now & " - Error in 'ConvertMiscPath_RelativeToAbsolute()'. Exception : " & ex.ToString)
            Return Nothing
        End Try

    End Function

    ''' <summary>
    ''' Set .ini files from models, at first TiaDL launch
    ''' TODO (v2+) : .ini file selection in GUI ?
    ''' </summary>
    ''' 
    Sub SetIniFiles()

        Try
            With My.Settings
                If File.Exists(Path.Combine(.GzdoomDir, "gzdoom-model.ini")) Then
                    File.Move(
                        Path.Combine(.GzdoomDir, "gzdoom-model.ini"),
                        Path.Combine(.GzdoomDir, "gzdoom-" & Environment.UserName & ".ini"))
                End If

                If File.Exists(Path.Combine(.ZandronumDir, "zandronum-model.ini")) Then
                    File.Move(
                        Path.Combine(.ZandronumDir, "zandronum-model.ini"),
                        Path.Combine(.ZandronumDir, "zandronum-" & Environment.UserName & ".ini"))
                End If

                If File.Exists(Path.Combine(.WolfDir, "gzdoom-model-wolf3D.ini")) Then
                    File.Move(
                        Path.Combine(.WolfDir, "gzdoom-model-wolf3D.ini"),
                        Path.Combine(.WolfDir, "gzdoom-model-" & Environment.UserName & "-wolf3D.ini"))
                End If
            End With

        Catch ex As Exception
            WriteToLog(DateTime.Now & " - Error in 'SetIniFiles()'. Exception : " & ex.ToString)
        End Try

    End Sub

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
                    If Not Directory.Exists(Path.Combine(.RootDirPath, dir)) Then
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
            If Not File.Exists(path) Then Return "does not exist"

            Dim iwadList As List(Of String) = New List(Of String) From {"doom.wad", "doom2.wad", "tnt.wad", "plutonia.wad", "freedoom1.wad", "freedoom2.wad"}
            If iwadList.Contains(File_GetName(path).ToLowerInvariant) Then Return "iwad" 'Better check file headers ? TODO : investigate

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
    ''' Create file 'presets.csv' with some commented lines, if it does not exist
    ''' </summary>
    ''' 
    Sub WritePresetsFileHeader()

        Try
            Dim presetFile As String = Path.Combine(My.Settings.RootDirPath, "presets.csv")

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
        Catch ex As Exception
            WriteToLog(DateTime.Now & " - Error in 'WritePresetsFileHeader()'. Exception : " & ex.ToString)
        End Try


    End Sub

    ''' <summary>
    ''' Log content in file 'log.txt'
    ''' </summary>
    ''' 
    Sub WriteToLog(content As String)

        Dim logfilePath = Path.Combine(My.Settings.RootDirPath, "log.txt")

        Using streamWriter As New StreamWriter(logfilePath, True)
            streamWriter.WriteLine(content)
        End Using

    End Sub

End Module
