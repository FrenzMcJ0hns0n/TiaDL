Module ClickHandlersMethods

    ''' <summary>
    ''' <para>Triggered when user clicks a user-preset-button.</para>
    ''' <para>Set SelectedIwad, SelectedLevel, Selected Misc. for launch</para>
    ''' <para>Display red text on filepath if does not exist</para>
    ''' </summary>
    ''' 
    Sub HandleUserPreset_Select(iwadPath As String, Optional levelPath As String = Nothing, Optional miscPath As String = Nothing)

        Try
            With My.Settings

                'Reset TextBox.Foreground
                MainWindow_Instance().TextBox_IwadToLaunch.ClearValue(Control.ForegroundProperty)
                MainWindow_Instance().TextBox_LevelToLaunch.ClearValue(Control.ForegroundProperty)
                MainWindow_Instance().TextBox_MiscToLaunch.ClearValue(Control.ForegroundProperty)

                If ValidateFile(iwadPath) = "iwad" Then
                    .SelectedIwad = iwadPath
                Else
                    MainWindow_Instance().TextBox_IwadToLaunch.Foreground = New SolidColorBrush(Colors.Red)
                End If
                MainWindow_Instance().TextBox_IwadToLaunch.Text = iwadPath

                .SelectedLevel = Nothing
                If ValidateFile(levelPath) = "level" Then
                    .SelectedLevel = levelPath
                Else
                    MainWindow_Instance().TextBox_LevelToLaunch.Foreground = New SolidColorBrush(Colors.Red)
                End If
                MainWindow_Instance().TextBox_LevelToLaunch.Text = levelPath

                .SelectedMisc = Nothing
                If ValidateFile(miscPath) = "misc" Then
                    .SelectedMisc = miscPath
                Else
                    MainWindow_Instance().TextBox_MiscToLaunch.Foreground = New SolidColorBrush(Colors.Red)
                End If
                MainWindow_Instance().TextBox_MiscToLaunch.Text = miscPath

                .Save()
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
    Sub HandleUserPreset_Delete(presetName As String)

        Dim message As String = String.Format(
            "You are about to delete preset ""{0}"".{1}Continue ?",
            presetName,
            Environment.NewLine & Environment.NewLine
        )

        If MessageBox.Show(message, "Delete user preset", MessageBoxButton.OKCancel) = MessageBoxResult.OK Then
            DeletePreset(presetName)

            'Update GUI
            DisplayPresets("user", FormatPresetsData_FromCsv(My.Settings.RootDirPath & "\presets.csv"))
        End If

    End Sub

End Module
