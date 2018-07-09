Imports System.IO

Module GUIMethods

    'TODO : XML description
    'TODO : Improve. For now = 1 button for 3 values
    Sub DisplayUserPresets(presetsData As List(Of IEnumerable(Of Object)))

        Try
            If presetsData.Count = 0 Then
                'Nothing to display
                Return
            End If

            Dim mainWindow As MainWindow = New MainWindow()

            mainWindow.Label_NoUserPresetsFound.Visibility = Visibility.Hidden

            Dim btns As List(Of Button) = presetsData(0) 'buttons
            Dim vals As List(Of String) = presetsData(1) 'values = val(0), val(1), ... val(5), ...

            Dim i As Integer = 0
            For counter As Integer = 0 To btns.Count - 1
                With btns(counter)

                    Dim presetName As String = vals(i + 0)
                    Dim presetIwad As String = vals(i + 1)
                    Dim presetLevel As String = vals(i + 2)

                    AddHandler btns(counter).Click, Sub(sender, e) 'Anonymous function ?
                                                        HandleUserPresetClick(presetName, presetIwad, presetLevel)
                                                    End Sub

                    mainWindow.StackPanel_DisplayUserPresets.Children.Add(btns(counter))
                    i += 3
                End With
            Next

        Catch ex As Exception
            WriteToLog(DateTime.Now & " - Error in 'DisplayUserPresets()'. Exception : " & ex.ToString)
        End Try

    End Sub

    ''' <summary>
    ''' Triggered when user clicks a preset in list(second tab:User presets)
    ''' Handle files validation
    ''' 
    ''' Set SelectedIwad, SelectedLevel, Selected Misc. for launch
    ''' </summary>
    ''' 
    Sub HandleUserPresetClick(name As String, iwadPath As String, levelPath As String) 'other As String (i.e. DEH file)

        Try
            With My.Settings

                Dim mainWindow As MainWindow = New MainWindow()

                .SelectedIwad = If(ValidateFile(iwadPath) = "iwad", iwadPath, Nothing)
                mainWindow.TextBox_IwadToLaunch.Text = .SelectedIwad

                .SelectedLevel = If(ValidateFile(levelPath) = "level", levelPath, Nothing)
                mainWindow.TextBox_LevelToLaunch.Text = .SelectedLevel

                WriteToLog(DateTime.Now & " - From 'HandleUserPresetClick()' : " & Environment.NewLine &
                           "Preset IWAD : " & iwadPath & Environment.NewLine & "Preset level : " & levelPath)
            End With

        Catch ex As Exception
            WriteToLog(DateTime.Now & " - Error in 'HandleUserPresetClick()'. Exception : " & ex.ToString)
        End Try


    End Sub



End Module
