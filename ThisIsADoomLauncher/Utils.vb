Module Utils

    Function MainWindow_Instance() As MainWindow

        Dim mainWindow = Windows.Application.Current.Windows(0)

        Return mainWindow

    End Function

    'TODO? : Extend to all presets
    Sub ValidateCommonPreset(iwad As String, Optional level As String = Nothing, Optional misc As String = Nothing)

        Dim errorText As String = ""
        Dim enl As String = Environment.NewLine

        Dim iwadPath As String = Path_Iwad_RelativeToAbsolute(iwad)
        Dim levelPath As String = Path_Level_RelativeToAbsolute(level)
        Dim miscPath As String = Path_Misc_RelativeToAbsolute(misc)

        With My.Settings
            If iwadPath = Nothing Then
                errorText &= String.Format("File ""{0}"" doesn't exist in :{1}{2}{3}", iwad, enl, .IwadsDir, enl)
            End If

            If levelPath = Nothing And Not level = Nothing Then
                errorText &= String.Format("File ""{0}"" doesn't exist in :{1}{2}{3}", level, enl, .IwadsDir, enl)
            End If

            If miscPath = Nothing And Not misc = Nothing Then
                errorText &= String.Format("File ""{0}"" doesn't exist in :{1}{2}{3}", misc, enl, .MiscDir, enl)
            End If

            If errorText = Nothing Then
                .SelectedIwad = iwadPath
                .SelectedLevel = levelPath
                .SelectedMisc = miscPath
                .Save()

                MainWindow_Instance().TextBox_IwadToLaunch.Text = .SelectedIwad
                MainWindow_Instance().TextBox_LevelToLaunch.Text = .SelectedLevel
                MainWindow_Instance().TextBox_MiscToLaunch.Text = .SelectedMisc
            Else
                MessageBox.Show("Error : " & enl & enl & errorText)
            End If
        End With

    End Sub

End Module
