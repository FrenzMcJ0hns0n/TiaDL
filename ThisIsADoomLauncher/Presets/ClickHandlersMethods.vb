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
                MainWindow_Instance().TextBox_IwadToLaunch.Text = iwadPath
                MainWindow_Instance().TextBox_LevelToLaunch.Text = levelPath
                MainWindow_Instance().TextBox_MiscToLaunch.Text = miscPath
            End With

        Catch ex As Exception
            WriteToLog(DateTime.Now & " - Error in 'HandleUserPresetClick()'. Exception : " & ex.ToString)
        End Try

    End Sub

    ''' <summary>
    ''' 
    ''' <para>Triggered when user right-clicks a custom preset</para>
    ''' <para>Delete a preset by its name</para>
    ''' </summary>
    '''
    Sub HandleUserPreset_Delete(presetName As String)

        Dim message As String = String.Format("Delete preset ""{0}"" ?", presetName)

        If MessageBox.Show(message, "Delete user preset", MessageBoxButton.OKCancel) = MessageBoxResult.OK Then
            DeletePreset(presetName)
            DisplayPresets(FormatPresetsData_FromCsv("user")) 'Update GUI
        End If

    End Sub


End Module
