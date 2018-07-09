Module CoreMethods

    ''' <summary>
    ''' Build the command line to send to cmd.exe
    ''' As a string
    ''' </summary>
    Function BuildCommandLineInstructions() As String

        Dim commandLine As String = ""

        Try
            With My.Settings

                '1. - Base command line instructions

                Select Case .SelectedEngine.ToLowerInvariant

                    Case "gzdoom"
                        commandLine = String.Format(
                            "/c start """" ""{0}"" -width {1} -height {2} +fullscreen {3} -iwad ""{4}""",
                            .GzdoomDir & "\gzdoom.exe",
                            .ScreenWidth.ToString,
                            .ScreenHeight.ToString,
                            Int(.FullscreenEnabled).ToString,
                            .SelectedIwad
                        )

                    Case "zandronum"
                        'TODO!
                        'commandLine = String.Format("{0}{1}",value0, value1) ...

                End Select


                '2. - Additional command line instructions
                commandLine &= If(.SelectedLevel = Nothing, Nothing, String.Format(" -file ""{0}""", .SelectedLevel))

                'commandLine &= If(.SelectedMisc = Nothing, Nothing, String.Format(" -file ""{0}""", .SelectedMisc))

                If .UseBrutalDoom Then
                    commandLine &= If(.BrutalDoomVersion = Nothing, Nothing, String.Format(" -file ""{0}""", .BrutalDoomVersion))
                End If

                commandLine &= If(.SelectedMusic = Nothing, Nothing, String.Format(" -file ""{0}""", .SelectedMusic))

                commandLine &= If(.UseTurbo = True, " -turbo 125", Nothing)
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
    ''' Create buttons then return Button and String objects
    ''' As List of IEnumerable
    ''' 
    ''' tuple example : MyPresetName, Doom2, LevelPath
    ''' </summary>
    Function FormatPresetsData(presets As List(Of List(Of String))) As List(Of IEnumerable(Of Object))

        'Destination
        Dim buttons As List(Of Button) = New List(Of Button)
        Dim values As List(Of String) = New List(Of String)
        Dim buttonsAndValues As List(Of IEnumerable(Of Object)) = New List(Of IEnumerable(Of Object))


        'Parse data then return it
        Dim tuple As List(Of String)
        For Each tuple In presets

            If Not tuple(0) = "" And Not tuple(0) Is Nothing Then
                WriteToLog("Test args/tuples : " & tuple(0) & tuple(1) & tuple(2)) 'debug

                Dim button As Button = New Button With {
                    .Margin = New Thickness(0, 0, 0, 2),
                    .Height = 28,
                    .FontSize = 14,
                    .Content = tuple(0)
                }

                buttons.Add(button)
                'TODO : better values storage
                'For now it's values(0-1-2) for preset1, values(3,4,5) for preset2, etc.
                values.Add(tuple(0))
                values.Add(tuple(1))
                values.Add(tuple(2))

                buttonsAndValues.Add(buttons)
                buttonsAndValues.Add(values)
            Else
                MessageBox.Show("was """" or nothing") ' TODO : Choose between "" and Nothing
            End If
        Next

        Return buttonsAndValues

    End Function




    ''' <summary>
    ''' Launch an instance of cmd.exe as a process, with specified args
    ''' 
    ''' args refers to the command line, which looks like the following :
    ''' /c start "" ".../engine.exe" -width -height +fullscreen iwad [level] [mod] [music]
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
