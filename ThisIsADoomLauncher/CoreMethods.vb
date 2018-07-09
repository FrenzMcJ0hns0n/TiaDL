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
                        'TODO
                        'commandLine = String.Format("{0}{1}",value0, value1) ...
                End Select


                '2. - Additional command line instructions

                If .UseBrutalDoom Then
                    commandLine &= If(.BrutalDoomVersion = Nothing, Nothing, String.Format(" -file ""{0}""", .BrutalDoomVersion))
                End If
                commandLine &= If(.SelectedLevel = Nothing, Nothing, String.Format(" -file ""{0}""", .SelectedLevel))
                'commandLine &= If(.SelectedMisc = Nothing, Nothing, String.Format(" -file ""{0}""", .SelectedMisc))
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
    ''' For each valid preset : create a button + return values
    ''' As List of IEnumerable
    ''' 
    ''' To be valid, each preset (as a line of 'presets.txt') must have at least a Name and an Iwad
    ''' values(0) : Preset Name
    ''' values(1) : Preset Iwad
    ''' values(2) : Preset Level
    ''' values(3) : Preset Misc.
    ''' </summary>
    Function FormatPresetsData(presetsData As List(Of List(Of String))) As List(Of IEnumerable(Of Object))

        Dim buttonsList As List(Of Button) = New List(Of Button)
        Dim valuesList As List(Of String) = New List(Of String)

        For Each values As List(Of String) In presetsData
            If values.Count < 2 Then
                Continue For 'invalid preset. Ignore it
            End If

            buttonsList.Add(New Button With {
                .Margin = New Thickness(0, 0, 0, 2),
                .Height = 28,
                .FontSize = 14,
                .Content = values(0)
            })

            valuesList.Add(values(1))

            For i As Integer = 1 To values.Count - 1
                If values(i) = Nothing Then
                    Exit For
                End If
                valuesList.Add(values(i))
            Next
        Next

        Return New List(Of IEnumerable(Of Object)) From {buttonsList, valuesList}

    End Function

    Sub DisplayLoadedPresets(readPresetsData As List(Of List(Of String)))

        For Each values As List(Of String) In readPresetsData
            If values.Count < 2 Then
                Continue For 'invalid preset. Ignore it
            End If

            Dim button As New Button With
            {
                .Margin = New Thickness(0, 0, 0, 2),
                .Height = 28,
                .FontSize = 14,
                .Content = values(0)
            }

            'Not sure
            Dim valuesList As List(Of String) = New List(Of String)
            For i As Integer = 1 To values.Count - 1
                If values(i) = Nothing Then
                    Exit For
                End If
                valuesList.Add(values(i))
            Next

            'TODO Give to HandleUserPresetClick a variable amount of args ? String[]

            'Build an anonymous function to get clickable button for each valid preset
            AddHandler button.Click,
                Sub(sender, e)
                    HandleUserPresetClick(
                        presetName,
                        presetIwad,
                        presetLevel
                    )
                End Sub

        Next

    End Sub

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
