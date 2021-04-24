Imports System.IO

Module CliMethods

    Function ReturnEngine() As String

        With My.Settings
            If .SelectedEngine.ToLowerInvariant = "gzdoom" Then
                Return String.Format("""{0}""", Path.Combine(.GzdoomDir, "gzdoom.exe"))
            Else
                Return String.Format("""{0}""", Path.Combine(.ZandronumDir, "zandronum.exe"))
            End If
        End With

    End Function

    Function ReturnFullscreen() As String

        With My.Settings
            If .SelectedEngine.ToLowerInvariant = "gzdoom" Then
                Return String.Format(" +vid_fullscreen {0}", Int(.FullscreenEnabled).ToString)
            Else
                Return String.Format(" +fullscreen {0}", Int(.FullscreenEnabled).ToString)
            End If
        End With

    End Function

    Function ReturnWidth() As String

        With My.Settings
            If .SelectedEngine.ToLowerInvariant = "gzdoom" Then
                Return String.Format(" +vid_scale_customwidth {0}", .ScreenWidth.ToString)
            Else
                Return String.Format(" +vid_defwidth {0}", .ScreenWidth.ToString)
            End If
        End With

    End Function

    Function ReturnHeight() As String

        With My.Settings
            If .SelectedEngine.ToLowerInvariant = "gzdoom" Then
                Return String.Format(" +vid_scale_customheight {0}", .ScreenHeight.ToString)
            Else
                Return String.Format(" +vid_defheight {0}", .ScreenHeight.ToString)
            End If
        End With

    End Function

    Function ReturnScalemode() As String

        With My.Settings
            If .SelectedEngine.ToLowerInvariant = "gzdoom" Then
                Return " +vid_scalemode 5"
            Else
                Return Nothing
            End If
        End With

    End Function

    Function ReturnIwad()

        With My.Settings
            Return String.Format(" -iwad ""{0}""", .SelectedIwad)
        End With

    End Function

    Function ReturnLevel() As String

        With My.Settings
            Return If(.SelectedLevel = Nothing, Nothing, String.Format(" -file ""{0}""", .SelectedLevel))
        End With

    End Function

    Function ReturnMisc() As String
        With My.Settings
            Return If(.SelectedMisc = Nothing, Nothing, String.Format(" -file ""{0}""", .SelectedMisc))
        End With
    End Function

    Function ReturnBdVersion() As String

        With My.Settings
            If .UseBrutalDoom = True Then
                Return If(.BrutalDoomVersion = Nothing, Nothing, String.Format(" -file ""{0}""", .BrutalDoomVersion))
            Else
                Return Nothing
            End If
        End With

    End Function

    Function ReturnMusic()

        With My.Settings
            If .UseAltSoundtrack = True Then
                Return If(.SelectedMusic = Nothing, Nothing, String.Format(" -file ""{0}""", .SelectedMusic))
            Else
                Return Nothing
            End If
        End With

    End Function

    Function ReturnTurbo()

        With My.Settings
            If .UseTurbo = True Then
                Return " -turbo 125"
            Else
                Return Nothing
            End If
        End With

    End Function


    Function ReturnWolfData() As String

        With My.Settings
            Dim wolfPath1 As String = Path.Combine(.WolfDir, "Wolf3D.pk3")
            Dim wolfPath2 As String = Path.Combine(.WolfDir, "Wolf3D_*.pk3")
            Dim wolfPath3 As String = Path.Combine(.WolfDir, "Wolf3DGL.pk3")

            Return String.Format(" -iwad ""{0}"" -file ""{1}"" ""{2}""", wolfPath1, wolfPath2, wolfPath3)
        End With

    End Function

    ''' <summary>
    ''' Build then return the command line to be executed with cmd.exe.
    ''' </summary>
    ''' 
    Function BuildCommandLine(wolf As Boolean) As String

        'TODO v3 : Use this to access UI elements : MainWindow_Instance().element = ...
        'WYSIWYG !

        Try
            Dim commandLine As String = Nothing

            Dim engine As String = Nothing
            Dim config As String = Nothing
            Dim fullscreen As String = Nothing
            Dim width As String = Nothing
            Dim height As String = Nothing
            Dim scalemode As String = Nothing
            Dim iwad As String = Nothing
            Dim level As String = Nothing
            Dim misc As String = Nothing
            Dim bdVersion As String = Nothing
            Dim music As String = Nothing
            Dim turbo As String = Nothing

            With My.Settings

                If wolf Then

                    engine = String.Format("""{0}""", Path.Combine(.GzdoomDir, "gzdoom.exe"))
                    config = String.Format(" -config ""{0}""", Path.Combine(.WolfDir, "gzdoom-" & Environment.UserName & "-wolf3D.ini"))
                    fullscreen = String.Format(" +vid_fullscreen {0}", Int(.FullscreenEnabled).ToString)
                    width = String.Format(" +vid_scale_customwidth {0}", .ScreenWidth.ToString)
                    height = String.Format(" +vid_scale_customheight {0}", .ScreenWidth.ToString)
                    scalemode = " +vid_scalemode 5"
                    iwad = ReturnWolfData()

                    commandLine = $"{engine}{config}{fullscreen}{width}{height}{scalemode}{iwad}"
                    'WriteToLog(DateTime.Now & " - CommandLine (debug) = " & Environment.NewLine & commandLine)
                    Return commandLine

                Else
                    engine = ReturnEngine()
                    fullscreen = ReturnFullscreen()
                    width = ReturnWidth()
                    height = ReturnHeight()
                    scalemode = ReturnScalemode()
                    iwad = ReturnIwad()
                    level = ReturnLevel()
                    misc = ReturnMisc()
                    bdVersion = ReturnBdVersion()
                    music = ReturnMusic()
                    turbo = ReturnTurbo()

                    commandLine = $"{engine}{config}{fullscreen}{width}{height}{scalemode}{iwad}{level}{misc}{bdVersion}{music}{turbo}"
                    'WriteToLog(DateTime.Now & " - CommandLine (debug) = " & Environment.NewLine & commandLine)
                    Return commandLine

                End If

            End With

        Catch ex As Exception
            WriteToLog(DateTime.Now & " - Error in 'BuildCommandLineInstructions()'. Exception : " & ex.ToString)
            Return Nothing
        End Try

    End Function

    ''' <summary>
    ''' Launch an instance of cmd.exe as a process with the specified command as parameter. The final result looks like the following :
    ''' /c start "" "engine_path" [+param1 value1, +param2 value2 ...] -iwad "iwad_path" [-file "level_path" | -file "misc_path" | -file "mod_path"] ...
    ''' </summary>
    ''' 
    Sub LaunchProcess(command As String)

        Dim cmd As ProcessStartInfo = New ProcessStartInfo("cmd.exe")

        With cmd
            .UseShellExecute = False
            .CreateNoWindow = True
            .Arguments = "/c start """" " & command
        End With

        Process.Start(cmd)

    End Sub

End Module
