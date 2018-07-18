Module ClickHandlersMethods

    ''' <summary>
    ''' <para>Triggered when user clicks a user-preset-button.</para>
    ''' <para>Set SelectedIwad, SelectedLevel, Selected Misc. for launch</para>
    ''' <para>Display red text on filepath if does not exist</para>
    ''' </summary>
    ''' 
    Sub HandleUserPresetClick(iwadPath As String, Optional levelPath As String = Nothing, Optional miscPath As String = Nothing)

        'MessageBox.Show(String.Format(
        '    "iwadPath: '{0}'{1}levelPath:'{2}'{3}miscPath:'{4}'",
        '    iwadPath, Environment.NewLine & Environment.NewLine, levelPath, Environment.NewLine & Environment.NewLine, miscPath
        '))

        Try
            With My.Settings

                Dim mainWindow As MainWindow = Windows.Application.Current.Windows(0)

                'Reset TextBox.Foreground
                mainWindow.TextBox_IwadToLaunch.ClearValue(Control.ForegroundProperty)
                mainWindow.TextBox_LevelToLaunch.ClearValue(Control.ForegroundProperty)
                mainWindow.TextBox_MiscToLaunch.ClearValue(Control.ForegroundProperty)

                If ValidateFile(iwadPath) = "iwad" Then
                    .SelectedIwad = iwadPath
                Else
                    mainWindow.TextBox_IwadToLaunch.Foreground = New SolidColorBrush(Colors.Red)
                End If
                mainWindow.TextBox_IwadToLaunch.Text = iwadPath

                If ValidateFile(levelPath) = "level" Then
                    .SelectedLevel = levelPath
                Else
                    mainWindow.TextBox_LevelToLaunch.Foreground = New SolidColorBrush(Colors.Red)
                End If
                mainWindow.TextBox_LevelToLaunch.Text = levelPath

                If ValidateFile(miscPath) = "misc" Then
                    .SelectedMisc = miscPath
                Else
                    mainWindow.TextBox_MiscToLaunch.Foreground = New SolidColorBrush(Colors.Red)
                End If
                mainWindow.TextBox_MiscToLaunch.Text = miscPath

            End With

        Catch ex As Exception
            WriteToLog(DateTime.Now & " - Error in 'HandleUserPresetClick()'. Exception : " & ex.ToString)
        End Try

    End Sub

    ''' <summary>
    ''' 
    ''' <para>Triggered when user right-clicks a user-preset-button.</para>
    ''' <para>Delete a preset by its name</para>
    ''' </summary>
    '''
    Sub HandleRightClick(presetName As String)

        Dim message As String = String.Format(
            "You are about to delete preset ""{0}"".{1}Continue ?",
            presetName,
            Environment.NewLine & Environment.NewLine
        )

        If MessageBox.Show(message, "Delete user preset", MessageBoxButton.OKCancel) = MessageBoxResult.OK Then
            DeletePreset(presetName)

            'Update GUI
            Dim mainWindow As MainWindow = Windows.Application.Current.Windows(0)
            mainWindow.StackPanel_DisplayUserPresets.Children.Clear()
            DisplayLoadedPresets(FormatPresetsData_FromCsv)
        End If

    End Sub

End Module
