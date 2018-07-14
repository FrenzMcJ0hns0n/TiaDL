Imports System.IO

Class MainWindow




#Region "Startup"

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)

        Try
            SetRootDirPath()
            ValidateDirectories()
            SetIniFiles()
            InitializeGUI()

        Catch ex As Exception
            WriteToLog(DateTime.Now & " - Error in 'Window_Loaded()'. Exception : " & ex.ToString)
        End Try

    End Sub

    Private Sub InitializeGUI()

        With My.Settings
            TextBox_IwadToLaunch.Text = .SelectedIwad
            TextBox_LevelToLaunch.Text = .SelectedLevel
            TextBox_MiscToLaunch.Text = .SelectedMisc
            CheckBox_Load_DoomMetal.IsChecked = If(.SelectedMusic = Nothing, False, True)
            CheckBox_EnableTurbo.IsChecked = .UseTurbo

            'Load user presets (2nd tab)
            DisplayLoadedPresets(GetPresetsFromFile(.RootDirPath & "\presets.txt"))
        End With

    End Sub

#End Region




#Region "Sidebar buttons"

    Private Sub Button_Menu_Help_Click(sender As Object, e As RoutedEventArgs) Handles Button_Help.Click

        'TODO : Help window with explanations about files, engines, etc.
        '...

    End Sub

    Private Sub Button_Levels_Click(sender As Object, e As RoutedEventArgs) Handles Button_Levels.Click

        Try
            Process.Start("https://www.doomworld.com/idgames/levels/")

        Catch ex As Exception
            WriteToLog(DateTime.Now & " - Error in 'Button_Levels_Click()'. Exception : " & ex.ToString)
        End Try

    End Sub

    Private Sub Button_DoomResources_Click(sender As Object, e As RoutedEventArgs) Handles Button_DoomResources.Click

        Try
            Process.Start("https://zdoom.org/index")
            Process.Start("https://zandronum.com/")
            Process.Start("https://www.moddb.com/mods/brutal-doom")

        Catch ex As Exception
            WriteToLog(DateTime.Now & " - Error in 'Button_DoomResources_Click()'. Exception : " & ex.ToString)
        End Try

    End Sub

    Private Sub Button_ExploreFolder_Click(sender As Object, e As RoutedEventArgs) Handles Button_ExploreFolder.Click

        Try
            Process.Start(My.Settings.RootDirPath)
        Catch ex As Exception
            WriteToLog(DateTime.Now & " - Error in 'Button_ExploreFolder_Click()'. Exception : " & ex.ToString)
        End Try

    End Sub

    Private Sub Button_Menu_Settings_Click(sender As Object, e As RoutedEventArgs) Handles Button_Menu_Settings.Click

        Dim settingsWindow As SettingsWindow = New SettingsWindow()
        settingsWindow.ShowDialog()

    End Sub

    Private Sub Button_Launch_Click(sender As Object, e As RoutedEventArgs) Handles Button_Launch.Click

        Try
            If TextBox_IwadToLaunch.Text = Nothing Then
                MessageBox.Show("Error : an IWAD must be selected")
                Return
            Else
                Dim cli As String =
                    If(TextBox_IwadToLaunch.Text = "Wolf3D", BuildCommandLineInstructions(True), BuildCommandLineInstructions(False))

                LaunchProcess(cli)
                WriteToLog(DateTime.Now & " - CommandLine :" & Environment.NewLine & cli)
            End If

        Catch ex As Exception
            WriteToLog(DateTime.Now & " - Error in 'Button_Launch_Click()'. Exception : " & ex.ToString)
        End Try

    End Sub

#End Region




#Region "Common presets"

    'TODO : Factorize ?

    Private Sub Button_Preset_UltimateDoom_Click(sender As Object, e As RoutedEventArgs) Handles Button_Preset_UltimateDoom.Click

        With My.Settings

            Dim iwad = "Doom.wad"
            Dim level As String = Nothing
            Dim misc As String = Nothing
            Dim iwadPath As String = Path_Iwad_RelativeToAbsolute(iwad)
            Dim levelPath As String = Path_Level_RelativeToAbsolute(level)
            Dim miscPath As String = Path_Misc_RelativeToAbsolute(misc)


            Dim errorText As String = ""

            If iwadPath = Nothing Then
                errorText &= String.Format("Error : File ""{0}"" doesn't exist in :{1}{2}{3}", iwad, Environment.NewLine, .IwadsDir, Environment.NewLine)
            End If

            If levelPath = Nothing And Not level = Nothing Then
                errorText &= String.Format("Error : File ""{0}"" doesn't exist in :{1}{2}{3}", level, Environment.NewLine, .IwadsDir, Environment.NewLine)
            End If

            If miscPath = Nothing And Not misc = Nothing Then
                errorText &= String.Format("Error : File ""{0}"" doesn't exist in :{1}{2}{3}", misc, Environment.NewLine, .MiscDir, Environment.NewLine)
            End If


            If errorText = Nothing Then
                .SelectedIwad = iwadPath
                .SelectedLevel = levelPath
                .SelectedMisc = miscPath
                .Save()

                TextBox_IwadToLaunch.Text = .SelectedIwad
                TextBox_LevelToLaunch.Text = .SelectedLevel
                TextBox_MiscToLaunch.Text = .SelectedMisc
            Else
                MessageBox.Show(errorText)
            End If

        End With

    End Sub

    Private Sub Button_Preset_Doom2_Click(sender As Object, e As RoutedEventArgs) Handles Button_Preset_Doom2.Click

        With My.Settings
            Dim iwad = "Doom2.wad"
            Dim level As String = Nothing
            Dim misc As String = Nothing
            Dim iwadPath As String = Path_Iwad_RelativeToAbsolute(iwad)
            Dim levelPath As String = Path_Level_RelativeToAbsolute(level)
            Dim miscPath As String = Path_Misc_RelativeToAbsolute(misc)


            Dim errorText As String = ""

            If iwadPath = Nothing Then
                errorText &= String.Format("Error : File ""{0}"" doesn't exist in :{1}{2}{3}", iwad, Environment.NewLine, .IwadsDir, Environment.NewLine)
            End If

            If levelPath = Nothing And Not level = Nothing Then
                errorText &= String.Format("Error : File ""{0}"" doesn't exist in :{1}{2}{3}", level, Environment.NewLine, .IwadsDir, Environment.NewLine)
            End If

            If miscPath = Nothing And Not misc = Nothing Then
                errorText &= String.Format("Error : File ""{0}"" doesn't exist in :{1}{2}{3}", misc, Environment.NewLine, .MiscDir, Environment.NewLine)
            End If


            If errorText = Nothing Then
                .SelectedIwad = iwadPath
                .SelectedLevel = levelPath
                .SelectedMisc = miscPath
                .Save()

                TextBox_IwadToLaunch.Text = .SelectedIwad
                TextBox_LevelToLaunch.Text = .SelectedLevel
                TextBox_MiscToLaunch.Text = .SelectedMisc
            Else
                MessageBox.Show(errorText)
            End If
        End With

    End Sub

    Private Sub Button_Preset_TNT_Click(sender As Object, e As RoutedEventArgs) Handles Button_Preset_TNT.Click

        With My.Settings
            Dim iwad = "TNT.wad"
            Dim level As String = Nothing
            Dim misc As String = Nothing
            Dim iwadPath As String = Path_Iwad_RelativeToAbsolute(iwad)
            Dim levelPath As String = Path_Level_RelativeToAbsolute(level)
            Dim miscPath As String = Path_Misc_RelativeToAbsolute(misc)


            Dim errorText As String = ""

            If iwadPath = Nothing Then
                errorText &= String.Format("Error : File ""{0}"" doesn't exist in :{1}{2}{3}", iwad, Environment.NewLine, .IwadsDir, Environment.NewLine)
            End If

            If levelPath = Nothing And Not level = Nothing Then
                errorText &= String.Format("Error : File ""{0}"" doesn't exist in :{1}{2}{3}", level, Environment.NewLine, .IwadsDir, Environment.NewLine)
            End If

            If miscPath = Nothing And Not misc = Nothing Then
                errorText &= String.Format("Error : File ""{0}"" doesn't exist in :{1}{2}{3}", misc, Environment.NewLine, .MiscDir, Environment.NewLine)
            End If


            If errorText = Nothing Then
                .SelectedIwad = iwadPath
                .SelectedLevel = levelPath
                .SelectedMisc = miscPath
                .Save()

                TextBox_IwadToLaunch.Text = .SelectedIwad
                TextBox_LevelToLaunch.Text = .SelectedLevel
                TextBox_MiscToLaunch.Text = .SelectedMisc
            Else
                MessageBox.Show(errorText)
            End If
        End With

    End Sub

    Private Sub Button_Preset_Plutonia_Click(sender As Object, e As RoutedEventArgs) Handles Button_Preset_Plutonia.Click

        With My.Settings
            Dim iwad = "Plutonia.wad"
            Dim level As String = Nothing
            Dim misc As String = Nothing
            Dim iwadPath As String = Path_Iwad_RelativeToAbsolute(iwad)
            Dim levelPath As String = Path_Level_RelativeToAbsolute(level)
            Dim miscPath As String = Path_Misc_RelativeToAbsolute(misc)


            Dim errorText As String = ""

            If iwadPath = Nothing Then
                errorText &= String.Format("Error : File ""{0}"" doesn't exist in :{1}{2}{3}", iwad, Environment.NewLine, .IwadsDir, Environment.NewLine)
            End If

            If levelPath = Nothing And Not level = Nothing Then
                errorText &= String.Format("Error : File ""{0}"" doesn't exist in :{1}{2}{3}", level, Environment.NewLine, .IwadsDir, Environment.NewLine)
            End If

            If miscPath = Nothing And Not misc = Nothing Then
                errorText &= String.Format("Error : File ""{0}"" doesn't exist in :{1}{2}{3}", misc, Environment.NewLine, .MiscDir, Environment.NewLine)
            End If


            If errorText = Nothing Then
                .SelectedIwad = iwadPath
                .SelectedLevel = levelPath
                .SelectedMisc = miscPath
                .Save()

                TextBox_IwadToLaunch.Text = .SelectedIwad
                TextBox_LevelToLaunch.Text = .SelectedLevel
                TextBox_MiscToLaunch.Text = .SelectedMisc
            Else
                MessageBox.Show(errorText)
            End If
        End With

    End Sub

    Private Sub Button_Preset_2002ADO_Click(sender As Object, e As RoutedEventArgs) Handles Button_Preset_2002ADO.Click

        With My.Settings
            Dim iwad As String = "Doom.wad"
            Dim level As String = "2002ad10.wad"
            Dim misc As String = Nothing
            Dim iwadPath As String = Path_Iwad_RelativeToAbsolute(iwad)
            Dim levelPath As String = Path_Level_RelativeToAbsolute(level)
            Dim miscPath As String = Path_Misc_RelativeToAbsolute(misc)


            Dim errorText As String = ""

            If iwadPath = Nothing Then
                errorText &= String.Format("Error : File ""{0}"" doesn't exist in :{1}{2}{3}", iwad, Environment.NewLine, .IwadsDir, Environment.NewLine)
            End If

            If levelPath = Nothing And Not level = Nothing Then
                errorText &= String.Format("Error : File ""{0}"" doesn't exist in :{1}{2}{3}", level, Environment.NewLine, .IwadsDir, Environment.NewLine)
            End If

            If miscPath = Nothing And Not misc = Nothing Then
                errorText &= String.Format("Error : File ""{0}"" doesn't exist in :{1}{2}{3}", misc, Environment.NewLine, .MiscDir, Environment.NewLine)
            End If


            If errorText = Nothing Then
                .SelectedIwad = iwadPath
                .SelectedLevel = levelPath
                .SelectedMisc = miscPath
                .Save()

                TextBox_IwadToLaunch.Text = .SelectedIwad
                TextBox_LevelToLaunch.Text = .SelectedLevel
                TextBox_MiscToLaunch.Text = .SelectedMisc
            Else
                MessageBox.Show(errorText)
            End If
        End With

    End Sub

    Private Sub Button_Preset_Icarus_Click(sender As Object, e As RoutedEventArgs) Handles Button_Preset_Icarus.Click

        With My.Settings
            Dim iwad As String = "Doom2.wad"
            Dim level As String = "ICARUS.wad"
            Dim misc As String = Nothing
            Dim iwadPath As String = Path_Iwad_RelativeToAbsolute(iwad)
            Dim levelPath As String = Path_Level_RelativeToAbsolute(level)
            Dim miscPath As String = Path_Misc_RelativeToAbsolute(misc)


            Dim errorText As String = ""

            If iwadPath = Nothing Then
                errorText &= String.Format("Error : File ""{0}"" doesn't exist in :{1}{2}{3}", iwad, Environment.NewLine, .IwadsDir, Environment.NewLine)
            End If

            If levelPath = Nothing And Not level = Nothing Then
                errorText &= String.Format("Error : File ""{0}"" doesn't exist in :{1}{2}{3}", level, Environment.NewLine, .IwadsDir, Environment.NewLine)
            End If

            If miscPath = Nothing And Not misc = Nothing Then
                errorText &= String.Format("Error : File ""{0}"" doesn't exist in :{1}{2}{3}", misc, Environment.NewLine, .MiscDir, Environment.NewLine)
            End If


            If errorText = Nothing Then
                .SelectedIwad = iwadPath
                .SelectedLevel = levelPath
                .SelectedMisc = miscPath
                .Save()

                TextBox_IwadToLaunch.Text = .SelectedIwad
                TextBox_LevelToLaunch.Text = .SelectedLevel
                TextBox_MiscToLaunch.Text = .SelectedMisc
            Else
                MessageBox.Show(errorText)
            End If
        End With

    End Sub

    Private Sub Button_Preset_Requiem_Click(sender As Object, e As RoutedEventArgs) Handles Button_Preset_Requiem.Click

        With My.Settings
            Dim iwad As String = "Doom2.wad"
            Dim level As String = "Requiem.wad"
            Dim misc As String = Nothing
            Dim iwadPath As String = Path_Iwad_RelativeToAbsolute(iwad)
            Dim levelPath As String = Path_Level_RelativeToAbsolute(level)
            Dim miscPath As String = Path_Misc_RelativeToAbsolute(misc)


            Dim errorText As String = ""

            If iwadPath = Nothing Then
                errorText &= String.Format("Error : File ""{0}"" doesn't exist in :{1}{2}{3}", iwad, Environment.NewLine, .IwadsDir, Environment.NewLine)
            End If

            If levelPath = Nothing And Not level = Nothing Then
                errorText &= String.Format("Error : File ""{0}"" doesn't exist in :{1}{2}{3}", level, Environment.NewLine, .IwadsDir, Environment.NewLine)
            End If

            If miscPath = Nothing And Not misc = Nothing Then
                errorText &= String.Format("Error : File ""{0}"" doesn't exist in :{1}{2}{3}", misc, Environment.NewLine, .MiscDir, Environment.NewLine)
            End If


            If errorText = Nothing Then
                .SelectedIwad = iwadPath
                .SelectedLevel = levelPath
                .SelectedMisc = miscPath
                .Save()

                TextBox_IwadToLaunch.Text = .SelectedIwad
                TextBox_LevelToLaunch.Text = .SelectedLevel
                TextBox_MiscToLaunch.Text = .SelectedMisc
            Else
                MessageBox.Show(errorText)
            End If
        End With

    End Sub

    Private Sub Button_Preset_Plutonia2_Click(sender As Object, e As RoutedEventArgs) Handles Button_Preset_Plutonia2.Click

        With My.Settings
            Dim iwad As String = "Doom2.wad"
            Dim level As String = "PL2.wad"
            Dim misc As String = Nothing
            Dim iwadPath As String = Path_Iwad_RelativeToAbsolute(iwad)
            Dim levelPath As String = Path_Level_RelativeToAbsolute(level)
            Dim miscPath As String = Path_Misc_RelativeToAbsolute(misc)


            Dim errorText As String = ""

            If iwadPath = Nothing Then
                errorText &= String.Format("Error : File ""{0}"" doesn't exist in :{1}{2}{3}", iwad, Environment.NewLine, .IwadsDir, Environment.NewLine)
            End If

            If levelPath = Nothing And Not level = Nothing Then
                errorText &= String.Format("Error : File ""{0}"" doesn't exist in :{1}{2}{3}", level, Environment.NewLine, .IwadsDir, Environment.NewLine)
            End If

            If miscPath = Nothing And Not misc = Nothing Then
                errorText &= String.Format("Error : File ""{0}"" doesn't exist in :{1}{2}{3}", misc, Environment.NewLine, .MiscDir, Environment.NewLine)
            End If


            If errorText = Nothing Then
                .SelectedIwad = iwadPath
                .SelectedLevel = levelPath
                .SelectedMisc = miscPath
                .Save()

                TextBox_IwadToLaunch.Text = .SelectedIwad
                TextBox_LevelToLaunch.Text = .SelectedLevel
                TextBox_MiscToLaunch.Text = .SelectedMisc
            Else
                MessageBox.Show(errorText)
            End If
        End With

    End Sub

    Private Sub Button_Preset_HellRevealed_Click(sender As Object, e As RoutedEventArgs) Handles Button_Preset_HellRevealed.Click

        With My.Settings
            Dim iwad As String = "Doom2.wad"
            Dim level As String = "hr.wad"
            Dim misc As String = Nothing
            Dim iwadPath As String = Path_Iwad_RelativeToAbsolute(iwad)
            Dim levelPath As String = Path_Level_RelativeToAbsolute(level)
            Dim miscPath As String = Path_Misc_RelativeToAbsolute(misc)


            Dim errorText As String = ""

            If iwadPath = Nothing Then
                errorText &= String.Format("Error : File ""{0}"" doesn't exist in :{1}{2}{3}", iwad, Environment.NewLine, .IwadsDir, Environment.NewLine)
            End If

            If levelPath = Nothing And Not level = Nothing Then
                errorText &= String.Format("Error : File ""{0}"" doesn't exist in :{1}{2}{3}", level, Environment.NewLine, .IwadsDir, Environment.NewLine)
            End If

            If miscPath = Nothing And Not misc = Nothing Then
                errorText &= String.Format("Error : File ""{0}"" doesn't exist in :{1}{2}{3}", misc, Environment.NewLine, .MiscDir, Environment.NewLine)
            End If


            If errorText = Nothing Then
                .SelectedIwad = iwadPath
                .SelectedLevel = levelPath
                .SelectedMisc = miscPath
                .Save()

                TextBox_IwadToLaunch.Text = .SelectedIwad
                TextBox_LevelToLaunch.Text = .SelectedLevel
                TextBox_MiscToLaunch.Text = .SelectedMisc
            Else
                MessageBox.Show(errorText)
            End If
        End With

    End Sub

    Private Sub Button_Preset_HellRevealed2_Click(sender As Object, e As RoutedEventArgs) Handles Button_Preset_HellRevealed2.Click

        With My.Settings
            Dim iwad As String = "Doom2.wad"
            Dim level As String = "hr2final.wad"
            Dim misc As String = Nothing
            Dim iwadPath As String = Path_Iwad_RelativeToAbsolute(iwad)
            Dim levelPath As String = Path_Level_RelativeToAbsolute(level)
            Dim miscPath As String = Path_Misc_RelativeToAbsolute(misc)


            Dim errorText As String = ""

            If iwadPath = Nothing Then
                errorText &= String.Format("Error : File ""{0}"" doesn't exist in :{1}{2}{3}", iwad, Environment.NewLine, .IwadsDir, Environment.NewLine)
            End If

            If levelPath = Nothing And Not level = Nothing Then
                errorText &= String.Format("Error : File ""{0}"" doesn't exist in :{1}{2}{3}", level, Environment.NewLine, .IwadsDir, Environment.NewLine)
            End If

            If miscPath = Nothing And Not misc = Nothing Then
                errorText &= String.Format("Error : File ""{0}"" doesn't exist in :{1}{2}{3}", misc, Environment.NewLine, .MiscDir, Environment.NewLine)
            End If


            If errorText = Nothing Then
                .SelectedIwad = iwadPath
                .SelectedLevel = levelPath
                .SelectedMisc = miscPath
                .Save()

                TextBox_IwadToLaunch.Text = .SelectedIwad
                TextBox_LevelToLaunch.Text = .SelectedLevel
                TextBox_MiscToLaunch.Text = .SelectedMisc
            Else
                MessageBox.Show(errorText)
            End If
        End With

    End Sub

    Private Sub Button_Preset_DTST_Click(sender As Object, e As RoutedEventArgs) Handles Button_Preset_DTST.Click

        With My.Settings
            Dim iwad As String = "Doom2.wad"
            Dim level As String = "DTS-T.pk3"
            Dim misc As String = Nothing
            Dim iwadPath As String = Path_Iwad_RelativeToAbsolute(iwad)
            Dim levelPath As String = Path_Level_RelativeToAbsolute(level)
            Dim miscPath As String = Path_Misc_RelativeToAbsolute(misc)


            Dim errorText As String = ""

            If iwadPath = Nothing Then
                errorText &= String.Format("Error : File ""{0}"" doesn't exist in :{1}{2}{3}", iwad, Environment.NewLine, .IwadsDir, Environment.NewLine)
            End If

            If levelPath = Nothing And Not level = Nothing Then
                errorText &= String.Format("Error : File ""{0}"" doesn't exist in :{1}{2}{3}", level, Environment.NewLine, .IwadsDir, Environment.NewLine)
            End If

            If miscPath = Nothing And Not misc = Nothing Then
                errorText &= String.Format("Error : File ""{0}"" doesn't exist in :{1}{2}{3}", misc, Environment.NewLine, .MiscDir, Environment.NewLine)
            End If


            If errorText = Nothing Then
                .SelectedIwad = iwadPath
                .SelectedLevel = levelPath
                .SelectedMisc = miscPath
                .Save()

                TextBox_IwadToLaunch.Text = .SelectedIwad
                TextBox_LevelToLaunch.Text = .SelectedLevel
                TextBox_MiscToLaunch.Text = .SelectedMisc
            Else
                MessageBox.Show(errorText)
            End If
        End With

    End Sub

    Private Sub Button_Preset_PlutoniaRevisited_Click(sender As Object, e As RoutedEventArgs) Handles Button_Preset_PlutoniaRevisited.Click

        With My.Settings
            Dim iwad As String = "Doom2.wad"
            Dim level As String = "PRCP.wad"
            Dim misc As String = Nothing
            Dim iwadPath As String = Path_Iwad_RelativeToAbsolute(iwad)
            Dim levelPath As String = Path_Level_RelativeToAbsolute(level)
            Dim miscPath As String = Path_Misc_RelativeToAbsolute(misc)


            Dim errorText As String = ""

            If iwadPath = Nothing Then
                errorText &= String.Format("Error : File ""{0}"" doesn't exist in :{1}{2}{3}", iwad, Environment.NewLine, .IwadsDir, Environment.NewLine)
            End If

            If levelPath = Nothing And Not level = Nothing Then
                errorText &= String.Format("Error : File ""{0}"" doesn't exist in :{1}{2}{3}", level, Environment.NewLine, .IwadsDir, Environment.NewLine)
            End If

            If miscPath = Nothing And Not misc = Nothing Then
                errorText &= String.Format("Error : File ""{0}"" doesn't exist in :{1}{2}{3}", misc, Environment.NewLine, .MiscDir, Environment.NewLine)
            End If


            If errorText = Nothing Then
                .SelectedIwad = iwadPath
                .SelectedLevel = levelPath
                .SelectedMisc = miscPath
                .Save()

                TextBox_IwadToLaunch.Text = .SelectedIwad
                TextBox_LevelToLaunch.Text = .SelectedLevel
                TextBox_MiscToLaunch.Text = .SelectedMisc
            Else
                MessageBox.Show(errorText)
            End If
        End With

    End Sub

    Private Sub Button_Preset_Doom2Reloaded_Click(sender As Object, e As RoutedEventArgs) Handles Button_Preset_Doom2Reloaded.Click

        With My.Settings
            Dim iwad As String = "Doom2.wad"
            Dim level As String = "D2RELOAD.WAD"
            Dim misc As String = Nothing
            Dim iwadPath As String = Path_Iwad_RelativeToAbsolute(iwad)
            Dim levelPath As String = Path_Level_RelativeToAbsolute(level)
            Dim miscPath As String = Path_Misc_RelativeToAbsolute(misc)


            Dim errorText As String = ""

            If iwadPath = Nothing Then
                errorText &= String.Format("Error : File ""{0}"" doesn't exist in :{1}{2}{3}", iwad, Environment.NewLine, .IwadsDir, Environment.NewLine)
            End If

            If levelPath = Nothing And Not level = Nothing Then
                errorText &= String.Format("Error : File ""{0}"" doesn't exist in :{1}{2}{3}", level, Environment.NewLine, .IwadsDir, Environment.NewLine)
            End If

            If miscPath = Nothing And Not misc = Nothing Then
                errorText &= String.Format("Error : File ""{0}"" doesn't exist in :{1}{2}{3}", misc, Environment.NewLine, .MiscDir, Environment.NewLine)
            End If


            If errorText = Nothing Then
                .SelectedIwad = iwadPath
                .SelectedLevel = levelPath
                .SelectedMisc = miscPath
                .Save()

                TextBox_IwadToLaunch.Text = .SelectedIwad
                TextBox_LevelToLaunch.Text = .SelectedLevel
                TextBox_MiscToLaunch.Text = .SelectedMisc
            Else
                MessageBox.Show(errorText)
            End If
        End With

    End Sub

    Private Sub Button_Preset_Doom2TheWayIdDid_Click(sender As Object, e As RoutedEventArgs) Handles Button_Preset_Doom2TheWayIdDid.Click

        With My.Settings
            Dim iwad As String = "Doom2.wad"
            Dim level As String = "D2TWID.wad"
            Dim misc As String = "D2TWID.deh"
            Dim iwadPath As String = Path_Iwad_RelativeToAbsolute(iwad)
            Dim levelPath As String = Path_Level_RelativeToAbsolute(level)
            Dim miscPath As String = Path_Misc_RelativeToAbsolute(misc)


            Dim errorText As String = ""

            If iwadPath = Nothing Then
                errorText &= String.Format("Error : File ""{0}"" doesn't exist in :{1}{2}{3}", iwad, Environment.NewLine, .IwadsDir, Environment.NewLine)
            End If

            If levelPath = Nothing And Not level = Nothing Then
                errorText &= String.Format("Error : File ""{0}"" doesn't exist in :{1}{2}{3}", level, Environment.NewLine, .IwadsDir, Environment.NewLine)
            End If

            If miscPath = Nothing And Not misc = Nothing Then
                errorText &= String.Format("Error : File ""{0}"" doesn't exist in :{1}{2}{3}", misc, Environment.NewLine, .MiscDir, Environment.NewLine)
            End If


            If errorText = Nothing Then
                .SelectedIwad = iwadPath
                .SelectedLevel = levelPath
                .SelectedMisc = miscPath
                .Save()

                TextBox_IwadToLaunch.Text = .SelectedIwad
                TextBox_LevelToLaunch.Text = .SelectedLevel
                TextBox_MiscToLaunch.Text = .SelectedMisc
            Else
                MessageBox.Show(errorText)
            End If
        End With

    End Sub

    Private Sub Button_Preset_Zone300_Click(sender As Object, e As RoutedEventArgs) Handles Button_Preset_Zone300.Click

        With My.Settings
            Dim iwad As String = "Doom2.wad"
            Dim level As String = "zone300.wad"
            Dim misc As String = Nothing
            Dim iwadPath As String = Path_Iwad_RelativeToAbsolute(iwad)
            Dim levelPath As String = Path_Level_RelativeToAbsolute(level)
            Dim miscPath As String = Path_Misc_RelativeToAbsolute(misc)


            Dim errorText As String = ""

            If iwadPath = Nothing Then
                errorText &= String.Format("Error : File ""{0}"" doesn't exist in :{1}{2}{3}", iwad, Environment.NewLine, .IwadsDir, Environment.NewLine)
            End If

            If levelPath = Nothing And Not level = Nothing Then
                errorText &= String.Format("Error : File ""{0}"" doesn't exist in :{1}{2}{3}", level, Environment.NewLine, .IwadsDir, Environment.NewLine)
            End If

            If miscPath = Nothing And Not misc = Nothing Then
                errorText &= String.Format("Error : File ""{0}"" doesn't exist in :{1}{2}{3}", misc, Environment.NewLine, .MiscDir, Environment.NewLine)
            End If


            If errorText = Nothing Then
                .SelectedIwad = iwadPath
                .SelectedLevel = levelPath
                .SelectedMisc = miscPath
                .Save()

                TextBox_IwadToLaunch.Text = .SelectedIwad
                TextBox_LevelToLaunch.Text = .SelectedLevel
                TextBox_MiscToLaunch.Text = .SelectedMisc
            Else
                MessageBox.Show(errorText)
            End If
        End With

    End Sub

    Private Sub Button_Preset_Wolfenstein3D_Click(sender As Object, e As RoutedEventArgs) Handles Button_Preset_Wolfenstein3D.Click

        'With My.Settings
        TextBox_IwadToLaunch.Text = "Wolf3D"
        TextBox_LevelToLaunch.Text = Nothing
        TextBox_MiscToLaunch.Text = Nothing

        'Dim brush As Brush = New ImageBrush With {
        '    .ImageSource = New BitmapImage(New Uri("../../Resources/Blazkowicz_sprite.png", UriKind.Relative)),
        '    .Stretch = Stretch.Uniform
        '}
        'Button_Preset_Wolfenstein3D.Background = brush

    End Sub

#End Region




#Region "Add new preset"

    Private Sub Button_NewPreset_SetDoomIwad_Click(sender As Object, e As RoutedEventArgs) Handles Button_NewPreset_SetDoomIwad.Click

        With My.Settings
            If File.Exists(.IwadsDir & "\Doom.wad") Then
                Button_NewPreset_SetDoomIwad.Background = Brushes.LightGreen
                Button_NewPreset_SetDoom2Iwad.Background = Brushes.Transparent
                Button_NewPreset_SetFreedoomIwad.Background = Brushes.Transparent
                Button_NewPreset_SetFreedoom2Iwad.Background = Brushes.Transparent
            Else
                MessageBox.Show("Error : File 'Doom.wad' not found in :" & Environment.NewLine & .IwadsDir)
            End If
        End With

    End Sub

    Private Sub Button_NewPreset_SetDoom2Iwad_Click(sender As Object, e As RoutedEventArgs) Handles Button_NewPreset_SetDoom2Iwad.Click

        With My.Settings
            If File.Exists(.IwadsDir & "\Doom2.wad") Then
                Button_NewPreset_SetDoom2Iwad.Background = Brushes.LightGreen
                Button_NewPreset_SetDoomIwad.Background = Brushes.Transparent
                Button_NewPreset_SetFreedoomIwad.Background = Brushes.Transparent
                Button_NewPreset_SetFreedoom2Iwad.Background = Brushes.Transparent
            Else
                MessageBox.Show("Error : File 'Doom2.wad' not found in :" & Environment.NewLine & .IwadsDir)
            End If
        End With

    End Sub

    Private Sub Button_NewPreset_SetFreedoomIwad_Click(sender As Object, e As RoutedEventArgs) Handles Button_NewPreset_SetFreedoomIwad.Click

        With My.Settings
            If File.Exists(.IwadsDir & "\freedoom1.wad") Then
                Button_NewPreset_SetFreedoomIwad.Background = Brushes.LightGreen
                Button_NewPreset_SetDoomIwad.Background = Brushes.Transparent
                Button_NewPreset_SetDoom2Iwad.Background = Brushes.Transparent
                Button_NewPreset_SetFreedoom2Iwad.Background = Brushes.Transparent

            Else
                MessageBox.Show("Error : File 'freedoom1.wad' not found in :" & Environment.NewLine & .IwadsDir)
            End If
        End With

    End Sub

    Private Sub Button_NewPreset_SetFreedoom2Iwad_Click(sender As Object, e As RoutedEventArgs) Handles Button_NewPreset_SetFreedoom2Iwad.Click

        With My.Settings
            If File.Exists(.IwadsDir & "\freedoom2.wad") Then
                Button_NewPreset_SetFreedoom2Iwad.Background = Brushes.LightGreen
                Button_NewPreset_SetDoomIwad.Background = Brushes.Transparent
                Button_NewPreset_SetDoom2Iwad.Background = Brushes.Transparent
                Button_NewPreset_SetFreedoomIwad.Background = Brushes.Transparent

            Else
                MessageBox.Show("Error : File 'freedoom2.wad' not found in :" & Environment.NewLine & .IwadsDir)
            End If
        End With

    End Sub

    Private Sub TextBox_DropWadFile_PreviewDragOver(sender As Object, e As DragEventArgs)

        e.Handled = True

    End Sub

    Private Sub TextBox_DropWadFile_Drop(sender As Object, e As DragEventArgs)

        Try
            Dim file() As String = CType(e.Data.GetData(DataFormats.FileDrop), String())

            If ValidateFile(file(0)) = "level" Then
                TextBox_DropWadFile.Background = Brushes.LightGreen
                TextBox_DropWadFile.ClearValue(FontStyleProperty)
                TextBox_DropWadFile.ClearValue(ForegroundProperty)
                TextBox_DropWadFile.Text = file(0)

            ElseIf ValidateFile(file(0)) = "iwad" Then
                MessageBox.Show("Error : this file is an IWAD")

            ElseIf ValidateFile(file(0)) = "misc" Then
                MessageBox.Show("Error : this file refers to a 'Misc.' file")

            Else
                MessageBox.Show("Error : not a .wad/.pk3 file")

            End If

        Catch ex As Exception
            WriteToLog(DateTime.Now & " - Error in 'TextBox_wad_file_Drop()'. Exception : " & ex.ToString)
        End Try

    End Sub

    Private Sub TextBox_DropMiscFile_PreviewDragOver(sender As Object, e As DragEventArgs)

        e.Handled = True

    End Sub

    Private Sub TextBox_DropMiscFile_Drop(sender As Object, e As DragEventArgs)

        Try
            Dim file() As String = CType(e.Data.GetData(DataFormats.FileDrop), String())

            If ValidateFile(file(0)) = "misc" Then
                TextBox_DropMiscFile.Background = Brushes.LightGreen
                TextBox_DropMiscFile.ClearValue(FontStyleProperty)
                TextBox_DropMiscFile.ClearValue(ForegroundProperty)
                TextBox_DropMiscFile.Text = file(0)

            ElseIf ValidateFile(file(0)) = "iwad" Then
                MessageBox.Show("Error : this file is an IWAD")

            ElseIf ValidateFile(file(0)) = "level" Then
                MessageBox.Show("Error : this file refers to 'Level'")

            Else
                MessageBox.Show("Error : not a .deh/.bex file")

            End If

        Catch ex As Exception
            WriteToLog(DateTime.Now & " - Error in 'TextBox_wad_file_Drop()'. Exception : " & ex.ToString)
        End Try

    End Sub

    Private Sub TextBox_NewPreset_Name_GotFocus(sender As Object, e As RoutedEventArgs) Handles TextBox_NewPreset_Name.GotFocus

        If TextBox_NewPreset_Name.Text = "Enter preset name ..." Then
            TextBox_NewPreset_Name.Text = ""
            TextBox_NewPreset_Name.ClearValue(FontStyleProperty)
            TextBox_NewPreset_Name.ClearValue(ForegroundProperty)
        End If

    End Sub

    Private Sub TextBox_NewPreset_Name_LostFocus(sender As Object, e As RoutedEventArgs) Handles TextBox_NewPreset_Name.LostFocus

        If TextBox_NewPreset_Name.Text Is Nothing Or TextBox_NewPreset_Name.Text = "" Then
            TextBox_NewPreset_Name.Text = "Enter preset name ..."
            TextBox_NewPreset_Name.FontStyle = FontStyles.Italic
            TextBox_NewPreset_Name.Foreground = New BrushConverter().ConvertFrom("#444")
        End If

    End Sub


    Private Function NewPreset_GetSelectedIwad() As String

        'IWAD
        Dim iwadButton As SolidColorBrush

        iwadButton = Button_NewPreset_SetDoomIwad.Background
        If iwadButton.Color = Colors.LightGreen Then
            Return My.Settings.IwadsDir & "\Doom.wad"
        End If

        iwadButton = Button_NewPreset_SetDoom2Iwad.Background
        If iwadButton.Color = Colors.LightGreen Then
            Return My.Settings.IwadsDir & "\Doom2.wad"
        End If

        iwadButton = Button_NewPreset_SetFreedoomIwad.Background
        If iwadButton.Color = Colors.LightGreen Then
            Return My.Settings.IwadsDir & "\freedoom1.wad"
        End If

        iwadButton = Button_NewPreset_SetFreedoom2Iwad.Background
        If iwadButton.Color = Colors.LightGreen Then
            Return My.Settings.IwadsDir & "\freedoom2.wad"
        End If

        Return Nothing

    End Function

    Private Function NewPreset_GetSelectedLevel() As String

        Return If(TextBox_DropWadFile.Text = "Drop a .wad/.pk3 file here ...", Nothing, TextBox_DropWadFile.Text)

    End Function

    Private Function NewPreset_GetSelectedMisc() As String

        Return If(TextBox_DropMiscFile.Text = "Drop a .deh/.bex file here ...", Nothing, TextBox_DropMiscFile.Text)

    End Function


    Private Sub Button_NewPreset_Try_Click(sender As Object, e As RoutedEventArgs) Handles Button_NewPreset_Try.Click

        'IWAD
        TextBox_IwadToLaunch.Text = NewPreset_GetSelectedIwad()
        My.Settings.SelectedIwad = NewPreset_GetSelectedIwad()

        'Level
        TextBox_LevelToLaunch.Text = NewPreset_GetSelectedLevel()
        My.Settings.SelectedLevel = NewPreset_GetSelectedLevel()

        'Misc
        TextBox_MiscToLaunch.Text = NewPreset_GetSelectedMisc()
        My.Settings.SelectedMisc = NewPreset_GetSelectedMisc()

    End Sub

    Private Sub Button_NewPreset_Reset_Click(sender As Object, e As RoutedEventArgs) Handles Button_NewPreset_Reset.Click

        Button_NewPreset_SetDoomIwad.Background = Brushes.Transparent
        Button_NewPreset_SetDoom2Iwad.Background = Brushes.Transparent
        Button_NewPreset_SetFreedoomIwad.Background = Brushes.Transparent
        Button_NewPreset_SetFreedoom2Iwad.Background = Brushes.Transparent

        TextBox_DropWadFile.FontStyle = FontStyles.Italic
        TextBox_DropWadFile.Background = New SolidColorBrush(Colors.Transparent)
        TextBox_DropWadFile.Foreground = New BrushConverter().ConvertFrom("#444")
        TextBox_DropWadFile.Text = "Drop a .wad/.pk3 file here ..."

        TextBox_NewPreset_Name.FontStyle = FontStyles.Italic
        TextBox_NewPreset_Name.Foreground = New BrushConverter().ConvertFrom("#444")
        TextBox_NewPreset_Name.Text = "Enter preset name ..."

    End Sub

    Private Sub Button_NewPreset_Save_Click(sender As Object, e As RoutedEventArgs) Handles Button_NewPreset_Save.Click

        Try
            Dim nameToSave As String = TextBox_NewPreset_Name.Text
            If nameToSave = "Enter preset name ..." Or nameToSave = Nothing Then
                MessageBox.Show("New user preset requires a name to be saved")
                Return
            End If

            Dim iwadToSave As String = NewPreset_GetSelectedIwad()
            If iwadToSave = Nothing Then
                MessageBox.Show("New user preset requires an IWAD to be saved")
                Return
            End If

            Dim levelToSave As String = NewPreset_GetSelectedLevel()
            Dim miscToSave As String = NewPreset_GetSelectedMisc()

            WritePresetToFile(nameToSave, iwadToSave, levelToSave, miscToSave)
            MessageBox.Show(String.Format("Preset ""{0}"" saved !", nameToSave))

        Catch ex As Exception
            WriteToLog(DateTime.Now & " - Error in 'Button_NewPreset_Save_Click()'. Exception : " & ex.ToString)
        End Try

    End Sub

#End Region




#Region "Extra launch parameters"

    Private Sub CheckBox_Load_DoomMetal_Checked(sender As Object, e As RoutedEventArgs) Handles CheckBox_Load_DoomMetal.Checked

        With My.Settings
            If File.Exists(.MusicDir & "\DoomMetalVol4.wad") Then
                .SelectedMusic = .MusicDir & "\DoomMetalVol4.wad"
                .Save()

            Else
                MessageBox.Show("Error : File ""DoomMetalVol4.wad"" not found in :" & Environment.NewLine & .MusicDir)
                CheckBox_Load_DoomMetal.IsChecked = False
            End If
        End With

    End Sub

    Private Sub CheckBox_Load_DoomMetal_Unchecked(sender As Object, e As RoutedEventArgs) Handles CheckBox_Load_DoomMetal.Unchecked

        With My.Settings
            .SelectedMusic = Nothing
            .Save()
        End With

    End Sub

    Private Sub CheckBox_EnableTurbo_Checked(sender As Object, e As RoutedEventArgs) Handles CheckBox_EnableTurbo.Checked

        With My.Settings
            .UseTurbo = True
            .Save()
        End With

    End Sub

    Private Sub CheckBox_EnableTurbo_Unchecked(sender As Object, e As RoutedEventArgs) Handles CheckBox_EnableTurbo.Unchecked

        With My.Settings
            .UseTurbo = False
            .Save()
        End With

    End Sub

#End Region




#Region "Crappy tests"

    Private Sub Button_TestProperties_Click(sender As Object, e As RoutedEventArgs) Handles Button_TestProperties.Click

        With My.Settings

            MessageBox.Show(String.Format(
                "ScreenWidth = {0}{1}ScreenHeight = {2}{3}FullscreenEnabled = {4}{5}SelectedIwad = {6}{7}SelectedLevel = {8}{9}UseBrutalDoom = {10}{11}BrutalDoomVersion = {12}{13}SelectedMusicMod = {14}{15}UseTurbo = {16}{17}SelectedEngine = {18}",
                .ScreenWidth.ToString, Environment.NewLine & Environment.NewLine,
                .ScreenHeight.ToString, Environment.NewLine & Environment.NewLine,
                .FullscreenEnabled.ToString, Environment.NewLine & Environment.NewLine,
                .SelectedIwad, Environment.NewLine & Environment.NewLine,
                .SelectedLevel, Environment.NewLine & Environment.NewLine,
                .UseBrutalDoom.ToString, Environment.NewLine & Environment.NewLine,
                .BrutalDoomVersion, Environment.NewLine & Environment.NewLine,
                .SelectedMusic, Environment.NewLine & Environment.NewLine,
                .UseTurbo, Environment.NewLine & Environment.NewLine,
                .SelectedEngine
            ))
            '.SelectedMisc, Environment.NewLine,

        End With

        'SaveNewSettings()

    End Sub

    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        Test()
    End Sub

    Private Sub Test()

        'My.Settings.ScreenHeight = 1440
        'My.Settings.BrutalDoomVersion = "bd21testApr25.pk3"
        'My.Settings.Save()

        'MessageBox.Show(ValidateFile("C:\Users\Frenz\source\repos\AnotherYoutubeAudioDownloader\AnotherYoutuion Of DOOM.mp3"))

    End Sub

#End Region




End Class
