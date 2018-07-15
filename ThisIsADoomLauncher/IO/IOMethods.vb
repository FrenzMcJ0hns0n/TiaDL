﻿Imports System.IO
Imports System.Reflection

Module IOMethods

    ''' <summary>
    ''' Get the parent directory of TiaDL executable
    ''' </summary>
    ''' 
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

                If Directory.Exists(.RootDirPath & "\tc") Then
                    .WolfDir = .RootDirPath & "\tc"
                Else
                    errorText &= Environment.NewLine & "- tc"
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
            Dim files() = Directory.GetFiles(My.Settings.ModDir)
            For Each file As String In files
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
    ''' Create file 'presets.txt' with some commented lines, if it does not exist
    ''' </summary>
    ''' 
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
    ''' Log content in file 'log.txt'
    ''' </summary>
    ''' 
    Sub WriteToLog(content As String)

        Using streamWriter As New StreamWriter(My.Settings.RootDirPath & "\log.txt", True)
            streamWriter.WriteLine(content)
        End Using

    End Sub

End Module
