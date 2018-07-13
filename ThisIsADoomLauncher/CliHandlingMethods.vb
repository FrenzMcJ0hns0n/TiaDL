Module CliHandlingMethods

    ''' <summary>
    ''' Build then return the command line to send to cmd.exe.
    ''' </summary>
    Function BuildCommandLineInstructions() As String

        Dim commandLine As String = ""

        Try
            With My.Settings

                '1. - Base command line instructions

                Select Case .SelectedEngine.ToLowerInvariant

                    Case "gzdoom"
                        commandLine = String.Format(
                            "/c start """" ""{0}"" +fullscreen {1} -width {2} -height {3} -iwad ""{4}""",
                            .GzdoomDir & "\gzdoom.exe",
                            Int(.FullscreenEnabled).ToString,
                            .ScreenWidth.ToString,
                            .ScreenHeight.ToString,
                            .SelectedIwad
                        )

                    Case "zandronum"
                        'IOMethods.HandleCfg()
                        commandLine = String.Format(
                            "/c start """" ""{0}"" +fullscreen {1} +vid_defwidth {2} +vid_defheight {3} -iwad ""{4}""",
                            .ZandronumDir & "\zandronum.exe",
                            Int(.FullscreenEnabled).ToString,
                            .ScreenWidth.ToString,
                            .ScreenHeight.ToString,
                            .SelectedIwad
                        )

                End Select


                '2. - Additional command line instructions

                If .UseBrutalDoom Then
                    commandLine &= If(.BrutalDoomVersion = Nothing, Nothing, String.Format(" -file ""{0}""", .BrutalDoomVersion))
                End If
                commandLine &= If(.SelectedLevel = Nothing, Nothing, String.Format(" -file ""{0}""", .SelectedLevel))
                commandLine &= If(.SelectedMisc = Nothing, Nothing, String.Format(" -file ""{0}""", .SelectedMisc))
                commandLine &= If(.SelectedMusic = Nothing, Nothing, String.Format(" -file ""{0}""", .SelectedMusic))
                commandLine &= If(.UseTurbo = True, " -turbo 125", Nothing)

                'TODO (v2+)
                'commandLine &= If(.UseTurbo = True, String.Format(" -turbo {0}", turboValue), Nothing)

                Return commandLine
            End With

        Catch ex As Exception

            WriteToLog(DateTime.Now & " - Error in 'BuildCommandLineInstructions()'. Exception : " & ex.ToString &
                       Environment.NewLine & "CommandLine built :" & Environment.NewLine & commandLine)
            Return ""

        End Try

    End Function

    ''' <summary>
    ''' Launch an instance of cmd.exe as a process, with specified args. 
    ''' args refer to the command line, which looks like the following :
    ''' '/c start "" ".../engine.exe" -width -height +fullscreen iwad [level] [mod] [music]'
    ''' </summary>
    Sub LaunchProcess(args As String)

        Dim cmd As ProcessStartInfo = New ProcessStartInfo("cmd.exe")

        With cmd
            .UseShellExecute = False
            .CreateNoWindow = True
            .Arguments = args
        End With

        Process.Start(cmd)

    End Sub

End Module
