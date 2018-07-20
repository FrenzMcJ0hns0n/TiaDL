Imports System.IO

Class MainWindow




#Region "Startup"

    'TODO? Think about async
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

            CheckBox_UseAltSoundtrack.IsChecked = .UseAltSoundtrack
            If .SelectedMusic = .MusicDir & "\DoomMetalVol4.wad" Then
                RadioButton_Soundtrack_DoomMetal.IsChecked = True
            ElseIf .SelectedMusic = .MusicDir & "\IDKFAv2.wad" Then
                RadioButton_Soundtrack_IDKFA.IsChecked = True
            End If

            CheckBox_EnableTurbo.IsChecked = .UseTurbo
        End With

    End Sub

#End Region




#Region "Sidebar buttons"

    Private Sub Button_Menu_Help_Click(sender As Object, e As RoutedEventArgs) Handles Button_Help.Click

        Dim helpWindow As HelpWindow = New HelpWindow()
        helpWindow.ShowDialog()

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




    Private Sub TabControl_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)

        Dim tabControl As TabControl = sender
        Dim item As TabItem = tabControl.SelectedValue

        If item.Name = "User" Then
            DisplayLoadedPresets(FormatPresetsData_FromCsv()) 'TODO? Think about async
        End If

    End Sub




#Region "Common presets"

    Private Sub Button_Preset_UltimateDoom_Click(sender As Object, e As RoutedEventArgs) Handles Button_Preset_UltimateDoom.Click

        ValidateCommonPreset("Doom.wad")

    End Sub

    Private Sub Button_Preset_Doom2_Click(sender As Object, e As RoutedEventArgs) Handles Button_Preset_Doom2.Click

        ValidateCommonPreset("Doom2.wad")

    End Sub

    Private Sub Button_Preset_TNT_Click(sender As Object, e As RoutedEventArgs) Handles Button_Preset_TNT.Click

        ValidateCommonPreset("TNT.wad")

    End Sub

    Private Sub Button_Preset_Plutonia_Click(sender As Object, e As RoutedEventArgs) Handles Button_Preset_Plutonia.Click

        ValidateCommonPreset("Plutonia.wad")

    End Sub

    Private Sub Button_Preset_2002ADO_Click(sender As Object, e As RoutedEventArgs) Handles Button_Preset_2002ADO.Click

        ValidateCommonPreset("Doom.wad", "2002ad10.wad")

    End Sub

    Private Sub Button_Preset_Icarus_Click(sender As Object, e As RoutedEventArgs) Handles Button_Preset_Icarus.Click

        ValidateCommonPreset("Doom2.wad", "ICARUS.wad")

    End Sub

    Private Sub Button_Preset_Requiem_Click(sender As Object, e As RoutedEventArgs) Handles Button_Preset_Requiem.Click

        ValidateCommonPreset("Doom2.wad", "Requiem.wad")

    End Sub

    Private Sub Button_Preset_Plutonia2_Click(sender As Object, e As RoutedEventArgs) Handles Button_Preset_Plutonia2.Click

        ValidateCommonPreset("Doom2.wad", "PL2.wad")

    End Sub

    Private Sub Button_Preset_HellRevealed_Click(sender As Object, e As RoutedEventArgs) Handles Button_Preset_HellRevealed.Click

        ValidateCommonPreset("Doom2.wad", "hr.wad")

    End Sub

    Private Sub Button_Preset_HellRevealed2_Click(sender As Object, e As RoutedEventArgs) Handles Button_Preset_HellRevealed2.Click

        ValidateCommonPreset("Doom2.wad", "hr2final.wad")

    End Sub

    Private Sub Button_Preset_DTST_Click(sender As Object, e As RoutedEventArgs) Handles Button_Preset_DTST.Click

        ValidateCommonPreset("Doom2.wad", "DTS-T.pk3")

    End Sub

    Private Sub Button_Preset_PlutoniaRevisited_Click(sender As Object, e As RoutedEventArgs) Handles Button_Preset_PlutoniaRevisited.Click

        ValidateCommonPreset("Doom2.wad", "PRCP.wad")

    End Sub

    Private Sub Button_Preset_Doom2Reloaded_Click(sender As Object, e As RoutedEventArgs) Handles Button_Preset_Doom2Reloaded.Click

        ValidateCommonPreset("Doom2.wad", "D2RELOAD.WAD")

    End Sub

    Private Sub Button_Preset_Doom2TheWayIdDid_Click(sender As Object, e As RoutedEventArgs) Handles Button_Preset_Doom2TheWayIdDid.Click

        ValidateCommonPreset("Doom2.wad", "D2TWID.wad", "D2TWID.deh")

    End Sub

    Private Sub Button_Preset_Zone300_Click(sender As Object, e As RoutedEventArgs) Handles Button_Preset_Zone300.Click

        ValidateCommonPreset("Doom2.wad", "zone300.wad")

    End Sub

    Private Sub Button_Preset_Wolfenstein3D_Click(sender As Object, e As RoutedEventArgs) Handles Button_Preset_Wolfenstein3D.Click

        TextBox_IwadToLaunch.Text = "Wolf3D"
        TextBox_LevelToLaunch.Text = Nothing
        TextBox_MiscToLaunch.Text = Nothing

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


    Private Sub Button_NewPreset_Try_Click(sender As Object, e As RoutedEventArgs) Handles Button_NewPreset_Try.Click

        'When using GetSelected**** we don't care path validity yet : done later at preset display

        TextBox_IwadToLaunch.Text = KnowSelectedIwad_NewPreset()
        My.Settings.SelectedIwad = KnowSelectedIwad_NewPreset()

        TextBox_LevelToLaunch.Text = KnowSelectedLevel_NewPreset()
        My.Settings.SelectedLevel = KnowSelectedLevel_NewPreset()

        TextBox_MiscToLaunch.Text = KnowSelectedMisc_NewPreset()
        My.Settings.SelectedMisc = KnowSelectedMisc_NewPreset()

    End Sub

    Private Sub Button_NewPreset_Reset_Click(sender As Object, e As RoutedEventArgs) Handles Button_NewPreset_Reset.Click

        ResetFields_NewPreset()

    End Sub

    Private Sub Button_NewPreset_Save_Click(sender As Object, e As RoutedEventArgs) Handles Button_NewPreset_Save.Click

        Save_NewPreset()

    End Sub

#End Region




#Region "Extra launch parameters"

    Private Sub CheckBox_UseAltSoundtrack_Checked(sender As Object, e As RoutedEventArgs) Handles CheckBox_UseAltSoundtrack.Checked

        With My.Settings
            .UseAltSoundtrack = True
            .Save()
        End With
        RadioButton_Soundtrack_DoomMetal.IsEnabled = True
        RadioButton_Soundtrack_DoomMetal.ClearValue(ForegroundProperty)
        RadioButton_Soundtrack_IDKFA.IsEnabled = True
        RadioButton_Soundtrack_IDKFA.ClearValue(ForegroundProperty)

    End Sub

    Private Sub CheckBox_UseAltSoundtrack_Unchecked(sender As Object, e As RoutedEventArgs) Handles CheckBox_UseAltSoundtrack.Unchecked

        With My.Settings
            .UseAltSoundtrack = False
            .Save()
        End With
        RadioButton_Soundtrack_DoomMetal.IsEnabled = False
        RadioButton_Soundtrack_DoomMetal.Foreground = Brushes.LightGray
        RadioButton_Soundtrack_IDKFA.IsEnabled = False
        RadioButton_Soundtrack_IDKFA.Foreground = Brushes.LightGray

    End Sub

    Private Sub RadioButton_Soundtrack_DoomMetal_Checked(sender As Object, e As RoutedEventArgs) Handles RadioButton_Soundtrack_DoomMetal.Checked

        With My.Settings
            If File.Exists(.MusicDir & "\DoomMetalVol4.wad") Then
                .SelectedMusic = .MusicDir & "\DoomMetalVol4.wad"
                .Save()
            Else
                MessageBox.Show("Error : File ""DoomMetalVol4.wad"" not found in :" & Environment.NewLine & .MusicDir)
                RadioButton_Soundtrack_DoomMetal.IsChecked = False
            End If
        End With

    End Sub

    Private Sub RadioButton_Soundtrack_IDKFA_Checked(sender As Object, e As RoutedEventArgs) Handles RadioButton_Soundtrack_IDKFA.Checked

        With My.Settings
            If File.Exists(.MusicDir & "\IDKFAv2.wad") Then
                .SelectedMusic = .MusicDir & "\IDKFAv2.wad"
                .Save()
            Else
                MessageBox.Show("Error : File ""IDKFAv2.wad"" not found in :" & Environment.NewLine & .MusicDir)
                RadioButton_Soundtrack_DoomMetal.IsChecked = False
            End If

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

    'Private Sub Button_TestProperties_Click(sender As Object, e As RoutedEventArgs) Handles Button_TestProperties.Click

    '    With My.Settings

    '        MessageBox.Show(String.Format(
    '            "ScreenWidth = {0}{1}ScreenHeight = {2}{3}FullscreenEnabled = {4}{5}SelectedIwad = {6}{7}SelectedLevel = {8}{9}UseBrutalDoom = {10}{11}BrutalDoomVersion = {12}{13}SelectedMusicMod = {14}{15}UseTurbo = {16}{17}SelectedEngine = {18}",
    '            .ScreenWidth.ToString, Environment.NewLine & Environment.NewLine,
    '            .ScreenHeight.ToString, Environment.NewLine & Environment.NewLine,
    '            .FullscreenEnabled.ToString, Environment.NewLine & Environment.NewLine,
    '            .SelectedIwad, Environment.NewLine & Environment.NewLine,
    '            .SelectedLevel, Environment.NewLine & Environment.NewLine,
    '            .UseBrutalDoom.ToString, Environment.NewLine & Environment.NewLine,
    '            .BrutalDoomVersion, Environment.NewLine & Environment.NewLine,
    '            .SelectedMusic, Environment.NewLine & Environment.NewLine,
    '            .UseTurbo, Environment.NewLine & Environment.NewLine,
    '            .SelectedEngine
    '        ))
    '        '.SelectedMisc, Environment.NewLine,

    '    End With

    'End Sub

    'Private Sub Button_Click(sender As Object, e As RoutedEventArgs)

    '    With My.Settings
    '        MessageBox.Show(.SelectedMusic)
    '        MessageBox.Show(.UseAltSoundtrack)
    '    End With

    'End Sub

#End Region




End Class
