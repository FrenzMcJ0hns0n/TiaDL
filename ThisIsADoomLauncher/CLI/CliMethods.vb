Imports System.IO

Module CliMethods

    Function ReturnEngine() As String

        With My.Settings
            If .SelectedEngine.ToLowerInvariant = "gzdoom" Then
                Return Path.Combine(.GzdoomDir, "gzdoom.exe")
            Else
                Return Path.Combine(.ZandronumDir, "zandronum.exe")
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
                    iwad = String.Format(" -iwad ""{0}""", .SelectedIwad)
                    level = If(.SelectedLevel = Nothing, Nothing, String.Format(" -file ""{0}""", .SelectedLevel))
                    misc = If(.SelectedMisc = Nothing, Nothing, String.Format(" -file ""{0}""", .SelectedMisc))
                    If .UseBrutalDoom Then bdVersion = If(.BrutalDoomVersion = Nothing, Nothing, String.Format(" -file ""{0}""", .BrutalDoomVersion))
                    If .UseAltSoundtrack Then music = If(.SelectedMusic = Nothing, Nothing, String.Format(" -file ""{0}""", .SelectedMusic))
                    If .UseTurbo Then turbo = " -turbo 125"

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
    ''' Launch an instance of cmd.exe as a process, with specified args. 
    ''' args refer to the command line, which looks like the following :
    ''' '/c start "" ".../engine.exe" -width -height +fullscreen iwad [level] [mod] [music]'
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
