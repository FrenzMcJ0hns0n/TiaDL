Imports System.IO

Class MainWindow




#Region "Start up"

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)

        Try

            'Dim str As String = ""
            'For i As Integer = 1 To 1
            '    str &= "hola"
            'Next
            'MessageBox.Show(str)


            'Dim str As String = ""

            'MessageBox.Show(If(str = Nothing, "Nothing", """"))

            'TEST
            'MessageBox.Show(CheckIwadPath("Doom.wad"))

            'MessageBox.Show(str)

            'Directories
            SetRootDirPath()
            'Dim clochard As List(Of List(Of String)) = GetPresetsFromFile(My.Settings.RootDirPath & "\presets.txt")


            'Dim str As String = ""
            'Dim i As Integer = 1

            'For Each argLine As List(Of String) In clochard
            '    str &= Environment.NewLine & i & " - "
            '    For Each arg As String In argLine
            '        str &= arg & Environment.NewLine
            '    Next
            '    i += 1
            'Next
            'MessageBox.Show(str)




            ValidateDirectories()

            'Settings
            InitializeGUI()


            'AutoSetResolution()



        Catch ex As Exception
            WriteToLog(DateTime.Now & " - Error in 'Window_Loaded()'. Exception : " & ex.ToString)
        End Try

    End Sub

    Private Sub InitializeGUI()

        With My.Settings
            TextBox_IwadToLaunch.Text = .SelectedIwad
            TextBox_LevelToLaunch.Text = .SelectedLevel
            'TextBox_MiscToLaunch.Text = "" TODO
            CheckBox_Load_DoomMetal.IsChecked = If(.SelectedMusic = Nothing, False, True)
            CheckBox_EnableTurbo.IsChecked = .UseTurbo

            'User presets (2nd tab)
            Dim presets As List(Of IEnumerable(Of Object)) = New List(Of IEnumerable(Of Object)) From {
                FormatPresetsData(GetPresetsFromFile(.RootDirPath & "\presets.txt"))
            }
            '<=> presets = FormatPresetsData(GetPresetsFromFile(.RootDirPath & "\presets.txt"))
            DisplayUserPresets(presets)
        End With

    End Sub

#End Region




#Region "Sidebar buttons"

    Private Sub Button_Menu_Help_Click(sender As Object, e As RoutedEventArgs) Handles Button_Help.Click

        'TODO : Help window with explanations about files, engines, etc.
        '...

    End Sub

    'Private Sub Button_SearchDoomWorldDB_Click(sender As Object, e As RoutedEventArgs) Handles Button_SearchDoomWorldDB.Click

    '    Dim url As String = "https://www.doomworld.com/idgames/levels/"
    '    Process.Start(url)

    'End Sub

    Private Sub Button_Menu_Settings_Click(sender As Object, e As RoutedEventArgs) Handles Button_Menu_Settings.Click

        Dim settingsWindow As SettingsWindow = New SettingsWindow()
        settingsWindow.ShowDialog()

    End Sub

    Private Sub Button_ExploreFolder_Click(sender As Object, e As RoutedEventArgs) Handles Button_ExploreFolder.Click

        Try
            Process.Start(My.Settings.RootDirPath)
        Catch ex As Exception
            WriteToLog(DateTime.Now & " - Error in 'Button_ExploreFolder_Click()'. Exception : " & ex.ToString)
        End Try


    End Sub

    Private Sub Button_Launch_Click(sender As Object, e As RoutedEventArgs) Handles Button_Launch.Click

        Try
            If TextBox_IwadToLaunch.Text = Nothing Then
                MessageBox.Show("Error : an IWAD must be selected")
                Return
            Else
                SetEngineIni()
                Dim bcli As String = BuildCommandLineInstructions()
                LaunchProcess(bcli)
                WriteToLog(DateTime.Now & " - CommandLine :" & Environment.NewLine & bcli)
            End If

        Catch ex As Exception
            WriteToLog(DateTime.Now & " - Error in 'Button_Launch_Click()'. Exception : " & ex.ToString)
        End Try

    End Sub

#End Region




#Region "Common presets click"

    Private Sub Button_Preset_UltimateDoom_Click(sender As Object, e As RoutedEventArgs) Handles Button_Preset_UltimateDoom.Click

        With My.Settings
            Dim iwad = "Doom.wad"
            Dim level As String = Nothing
            Dim iwadPath As String = BuildIwadPath(iwad)
            Dim levelPath As String = BuildLevelPath(level)

            If iwadPath = Nothing Then
                MessageBox.Show("Error : File " & iwad & " doesn't exist in" & Environment.NewLine & .IwadsDir)
                Return
            Else
                .SelectedIwad = iwadPath
                .SelectedLevel = levelPath
                .Save()
            End If

            TextBox_IwadToLaunch.Text = .SelectedIwad
            TextBox_LevelToLaunch.Text = .SelectedLevel
        End With

    End Sub

    Private Sub Button_Preset_Doom2_Click(sender As Object, e As RoutedEventArgs) Handles Button_Preset_Doom2.Click

        With My.Settings
            Dim iwad = "Doom2.wad"
            Dim level As String = Nothing
            Dim iwadPath As String = BuildIwadPath(iwad)
            Dim levelPath As String = BuildLevelPath(level)

            If iwadPath = Nothing Then
                MessageBox.Show("Error : File " & iwad & " doesn't exist in" & Environment.NewLine & .IwadsDir)
                Return
            Else
                .SelectedIwad = iwadPath
                .SelectedLevel = levelPath
                .Save()
            End If

            TextBox_IwadToLaunch.Text = .SelectedIwad
            TextBox_LevelToLaunch.Text = .SelectedLevel
        End With

    End Sub

    Private Sub Button_Preset_TNT_Click(sender As Object, e As RoutedEventArgs) Handles Button_Preset_TNT.Click

        With My.Settings
            Dim iwad = "TNT.wad"
            Dim level As String = Nothing
            Dim iwadPath As String = BuildIwadPath(iwad)
            Dim levelPath As String = BuildLevelPath(level)

            If iwadPath = Nothing Then
                MessageBox.Show("Error : File " & iwad & " doesn't exist in" & Environment.NewLine & .IwadsDir)
                Return
            Else
                .SelectedIwad = iwadPath
                .SelectedLevel = levelPath
                .Save()
            End If

            TextBox_IwadToLaunch.Text = .SelectedIwad
            TextBox_LevelToLaunch.Text = .SelectedLevel
        End With

    End Sub

    Private Sub Button_Preset_Plutonia_Click(sender As Object, e As RoutedEventArgs) Handles Button_Preset_Plutonia.Click

        With My.Settings
            Dim iwad = "Plutonia.wad"
            Dim level As String = Nothing
            Dim iwadPath As String = BuildIwadPath(iwad)
            Dim levelPath As String = BuildLevelPath(level)

            If iwadPath = Nothing Then
                MessageBox.Show("Error : File " & iwad & " doesn't exist in" & Environment.NewLine & .IwadsDir)
                Return
            Else
                .SelectedIwad = iwadPath
                .SelectedLevel = levelPath
                .Save()
            End If

            TextBox_IwadToLaunch.Text = .SelectedIwad
            TextBox_LevelToLaunch.Text = .SelectedLevel
        End With

    End Sub

    Private Sub Button_Preset_2002ADO_Click(sender As Object, e As RoutedEventArgs) Handles Button_Preset_2002ADO.Click

        With My.Settings
            Dim iwad As String = "Doom.wad"
            Dim level As String = "2002ad10.wad"
            Dim iwadPath As String = BuildIwadPath(iwad)
            Dim levelPath As String = BuildLevelPath(level)

            If iwadPath = Nothing And levelPath = Nothing Then
                MessageBox.Show(
                    "Error :" &
                    "File " & iwad & " doesn't exist in" & Environment.NewLine & .IwadsDir &
                    Environment.NewLine &
                    "File " & level & " doesn't exist in" & Environment.NewLine & .LevelsDir
                )
                Return

            ElseIf iwadPath = Nothing Then
                MessageBox.Show("Error : File " & iwad & " doesn't exist in" & Environment.NewLine & .IwadsDir)
                Return

            Else
                .SelectedIwad = iwadPath
                .SelectedLevel = levelPath
                .Save()
            End If

            TextBox_IwadToLaunch.Text = .SelectedIwad
            TextBox_LevelToLaunch.Text = .SelectedLevel
        End With

    End Sub

    Private Sub Button_Preset_Icarus_Click(sender As Object, e As RoutedEventArgs) Handles Button_Preset_Icarus.Click

        With My.Settings
            Dim iwad As String = "Doom2.wad"
            Dim level As String = "ICARUS.wad"
            Dim iwadPath As String = BuildIwadPath(iwad)
            Dim levelPath As String = BuildLevelPath(level)

            If iwadPath = Nothing And levelPath = Nothing Then
                MessageBox.Show(
                    "Error :" &
                    "File " & iwad & " doesn't exist in" & Environment.NewLine & .IwadsDir &
                    Environment.NewLine &
                    "File " & level & " doesn't exist in" & Environment.NewLine & .LevelsDir
                )
                Return

            ElseIf iwadPath = Nothing Then
                MessageBox.Show("Error : File " & iwad & " doesn't exist in" & Environment.NewLine & .IwadsDir)
                Return

            Else
                .SelectedIwad = iwadPath
                .SelectedLevel = levelPath
                .Save()
            End If

            TextBox_IwadToLaunch.Text = .SelectedIwad
            TextBox_LevelToLaunch.Text = .SelectedLevel
        End With

    End Sub

    Private Sub Button_Preset_Requiem_Click(sender As Object, e As RoutedEventArgs) Handles Button_Preset_Requiem.Click

        With My.Settings
            Dim iwad As String = "Doom2.wad"
            Dim level As String = "Requiem.wad"
            Dim iwadPath As String = BuildIwadPath(iwad)
            Dim levelPath As String = BuildLevelPath(level)

            If iwadPath = Nothing And levelPath = Nothing Then
                MessageBox.Show(
                    "Error :" &
                    "File " & iwad & " doesn't exist in" & Environment.NewLine & .IwadsDir &
                    Environment.NewLine &
                    "File " & level & " doesn't exist in" & Environment.NewLine & .LevelsDir
                )
                Return

            ElseIf iwadPath = Nothing Then
                MessageBox.Show("Error : File " & iwad & " doesn't exist in" & Environment.NewLine & .IwadsDir)
                Return

            Else
                .SelectedIwad = iwadPath
                .SelectedLevel = levelPath
                .Save()
            End If

            TextBox_IwadToLaunch.Text = .SelectedIwad
            TextBox_LevelToLaunch.Text = .SelectedLevel
        End With

    End Sub

    Private Sub Button_Preset_Plutonia2_Click(sender As Object, e As RoutedEventArgs) Handles Button_Preset_Plutonia2.Click

        With My.Settings
            Dim iwad As String = "Doom2.wad"
            Dim level As String = "PL2.wad"
            Dim iwadPath As String = BuildIwadPath(iwad)
            Dim levelPath As String = BuildLevelPath(level)

            If iwadPath = Nothing And levelPath = Nothing Then
                MessageBox.Show(
                    "Error :" &
                    "File " & iwad & " doesn't exist in" & Environment.NewLine & .IwadsDir &
                    Environment.NewLine &
                    "File " & level & " doesn't exist in" & Environment.NewLine & .LevelsDir
                )
                Return

            ElseIf iwadPath = Nothing Then
                MessageBox.Show("Error : File " & iwad & " doesn't exist in" & Environment.NewLine & .IwadsDir)
                Return

            Else
                .SelectedIwad = iwadPath
                .SelectedLevel = levelPath
                .Save()
            End If

            TextBox_IwadToLaunch.Text = .SelectedIwad
            TextBox_LevelToLaunch.Text = .SelectedLevel
        End With

    End Sub

    Private Sub Button_Preset_HellRevealed_Click(sender As Object, e As RoutedEventArgs) Handles Button_Preset_HellRevealed.Click

        With My.Settings
            Dim iwad As String = "Doom2.wad"
            Dim level As String = "hr.wad"
            Dim iwadPath As String = BuildIwadPath(iwad)
            Dim levelPath As String = BuildLevelPath(level)

            If iwadPath = Nothing And levelPath = Nothing Then
                MessageBox.Show(
                    "Error :" &
                    "File " & iwad & " doesn't exist in" & Environment.NewLine & .IwadsDir &
                    Environment.NewLine &
                    "File " & level & " doesn't exist in" & Environment.NewLine & .LevelsDir
                )
                Return

            ElseIf iwadPath = Nothing Then
                MessageBox.Show("Error : File " & iwad & " doesn't exist in" & Environment.NewLine & .IwadsDir)
                Return

            Else
                .SelectedIwad = iwadPath
                .SelectedLevel = levelPath
                .Save()
            End If

            TextBox_IwadToLaunch.Text = .SelectedIwad
            TextBox_LevelToLaunch.Text = .SelectedLevel
        End With

    End Sub

    Private Sub Button_Preset_HellRevealed2_Click(sender As Object, e As RoutedEventArgs) Handles Button_Preset_HellRevealed2.Click

        With My.Settings
            Dim iwad As String = "Doom2.wad"
            Dim level As String = "hr2final.wad"
            Dim iwadPath As String = BuildIwadPath(iwad)
            Dim levelPath As String = BuildLevelPath(level)

            If iwadPath = Nothing And levelPath = Nothing Then
                MessageBox.Show(
                    "Error :" &
                    "File " & iwad & " doesn't exist in" & Environment.NewLine & .IwadsDir &
                    Environment.NewLine &
                    "File " & level & " doesn't exist in" & Environment.NewLine & .LevelsDir
                )
                Return

            ElseIf iwadPath = Nothing Then
                MessageBox.Show("Error : File " & iwad & " doesn't exist in" & Environment.NewLine & .IwadsDir)
                Return

            Else
                .SelectedIwad = iwadPath
                .SelectedLevel = levelPath
                .Save()
            End If

            TextBox_IwadToLaunch.Text = .SelectedIwad
            TextBox_LevelToLaunch.Text = .SelectedLevel
        End With

    End Sub

    Private Sub Button_Preset_DTST_Click(sender As Object, e As RoutedEventArgs) Handles Button_Preset_DTST.Click

        With My.Settings
            Dim iwad As String = "Doom2.wad"
            Dim level As String = "DTS-T.pk3"
            Dim iwadPath As String = BuildIwadPath(iwad)
            Dim levelPath As String = BuildLevelPath(level)

            If iwadPath = Nothing And levelPath = Nothing Then
                MessageBox.Show(
                    "Error :" &
                    "File " & iwad & " doesn't exist in" & Environment.NewLine & .IwadsDir &
                    Environment.NewLine &
                    "File " & level & " doesn't exist in" & Environment.NewLine & .LevelsDir
                )
                Return

            ElseIf iwadPath = Nothing Then
                MessageBox.Show("Error : File " & iwad & " doesn't exist in" & Environment.NewLine & .IwadsDir)
                Return

            Else
                .SelectedIwad = iwadPath
                .SelectedLevel = levelPath
                .Save()
            End If

            TextBox_IwadToLaunch.Text = .SelectedIwad
            TextBox_LevelToLaunch.Text = .SelectedLevel
        End With

    End Sub

    Private Sub Button_Preset_PlutoniaRevisited_Click(sender As Object, e As RoutedEventArgs) Handles Button_Preset_PlutoniaRevisited.Click

        With My.Settings
            Dim iwad As String = "Doom2.wad"
            Dim level As String = "RCP.wad"
            Dim iwadPath As String = BuildIwadPath(iwad)
            Dim levelPath As String = BuildLevelPath(level)

            If iwadPath = Nothing And levelPath = Nothing Then
                MessageBox.Show(
                    "Error :" &
                    "File " & iwad & " doesn't exist in" & Environment.NewLine & .IwadsDir &
                    Environment.NewLine &
                    "File " & level & " doesn't exist in" & Environment.NewLine & .LevelsDir
                )
                Return

            ElseIf iwadPath = Nothing Then
                MessageBox.Show("Error : File " & iwad & " doesn't exist in" & Environment.NewLine & .IwadsDir)
                Return

            Else
                .SelectedIwad = iwadPath
                .SelectedLevel = levelPath
                .Save()
            End If

            TextBox_IwadToLaunch.Text = .SelectedIwad
            TextBox_LevelToLaunch.Text = .SelectedLevel
        End With

    End Sub

    Private Sub Button_Preset_Doom2Reloaded_Click(sender As Object, e As RoutedEventArgs) Handles Button_Preset_Doom2Reloaded.Click

        With My.Settings
            Dim iwad As String = "Doom2.wad"
            Dim level As String = "D2RELOAD.WAD"
            Dim iwadPath As String = BuildIwadPath(iwad)
            Dim levelPath As String = BuildLevelPath(level)

            If iwadPath = Nothing And levelPath = Nothing Then
                MessageBox.Show(
                    "Error :" &
                    "File " & iwad & " doesn't exist in" & Environment.NewLine & .IwadsDir &
                    Environment.NewLine &
                    "File " & level & " doesn't exist in" & Environment.NewLine & .LevelsDir
                )
                Return

            ElseIf iwadPath = Nothing Then
                MessageBox.Show("Error : File " & iwad & " doesn't exist in" & Environment.NewLine & .IwadsDir)
                Return

            Else
                .SelectedIwad = iwadPath
                .SelectedLevel = levelPath
                .Save()
            End If

            TextBox_IwadToLaunch.Text = .SelectedIwad
            TextBox_LevelToLaunch.Text = .SelectedLevel
        End With

    End Sub

    Private Sub Button_Preset_Doom2TheWayIdDid_Click(sender As Object, e As RoutedEventArgs) Handles Button_Preset_Doom2TheWayIdDid.Click

        With My.Settings
            Dim iwad As String = "Doom2.wad"
            Dim level As String = "D2TWID.wad"
            Dim iwadPath As String = BuildIwadPath(iwad)
            Dim levelPath As String = BuildLevelPath(level)

            If iwadPath = Nothing And levelPath = Nothing Then
                MessageBox.Show(
                    "Error :" &
                    "File " & iwad & " doesn't exist in" & Environment.NewLine & .IwadsDir &
                    Environment.NewLine &
                    "File " & level & " doesn't exist in" & Environment.NewLine & .LevelsDir
                )
                Return

            ElseIf iwadPath = Nothing Then
                MessageBox.Show("Error : File " & iwad & " doesn't exist in" & Environment.NewLine & .IwadsDir)
                Return

            Else
                .SelectedIwad = iwadPath
                .SelectedLevel = levelPath
                .Save()
            End If

            TextBox_IwadToLaunch.Text = .SelectedIwad
            TextBox_LevelToLaunch.Text = .SelectedLevel
        End With

    End Sub

#End Region




#Region "Add new preset : GUI items"

    Private Sub Button_NewPreset_SetDoomIwad_Click(sender As Object, e As RoutedEventArgs) Handles Button_NewPreset_SetDoomIwad.Click

        With My.Settings
            If File.Exists(.IwadsDir & "\Doom.wad") Then
                Button_NewPreset_SetDoom2Iwad.Background = Brushes.LightGreen
                Button_NewPreset_SetDoomIwad.Background = Brushes.Transparent
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

    Private Sub TextBox_wad_file_PreviewDragOver(sender As Object, e As DragEventArgs)

        e.Handled = True

    End Sub

    Private Sub TextBox_wad_file_Drop(sender As Object, e As DragEventArgs)

        Try
            Dim file() As String = CType(e.Data.GetData(DataFormats.FileDrop), String())

            If ValidateFile(file(0)) = "level" Then
                TextBox_DropWadFile.Background = Brushes.LightGreen
                TextBox_DropWadFile.ClearValue(FontStyleProperty)
                TextBox_DropWadFile.ClearValue(ForegroundProperty)
                TextBox_DropWadFile.Text = file(0)

            ElseIf ValidateFile(file(0)) = "iwad" Then
                MessageBox.Show("Error : this file is an IWAD")

            Else
                MessageBox.Show("Error : not a .wad/.pk3 file")
                'ResetInput_wad(False) 'Same as above for IWAD

            End If

        Catch ex As Exception
            WriteToLog(DateTime.Now & " - Error in 'TextBox_wad_file_Drop()'. Exception : " & ex.ToString)
        End Try

    End Sub

#End Region




    Private Sub Button_TestProperties_Click(sender As Object, e As RoutedEventArgs) Handles Button_TestProperties.Click

        With My.Settings

            MessageBox.Show(String.Format(
                "ScreenWidth = {0}{1}ScreenHeight = {2}{3}FullscreenEnabled = {4}{5}SelectedIwad = {6}{7}SelectedLevel = {8}{9}UseBrutalDoom = {10}{11}BrutalDoomVersion = {12}{13}SelectedMusicMod = {14}{15}UseTurbo = {16}{17}SelectedEngine = {18}",
                .ScreenWidth.ToString, Environment.NewLine,
                .ScreenHeight.ToString, Environment.NewLine,
                .FullscreenEnabled.ToString, Environment.NewLine,
                .SelectedIwad, Environment.NewLine,
                .SelectedLevel, Environment.NewLine,
                .UseBrutalDoom.ToString, Environment.NewLine,
                .BrutalDoomVersion, Environment.NewLine,
                .SelectedMusic, Environment.NewLine,
                .UseTurbo, Environment.NewLine,
                .SelectedEngine
            ))
            '.SelectedMisc, Environment.NewLine,

        End With

        'SaveNewSettings()

    End Sub

    Private Function NewPreset_GetSelectedIwad() As String

        'IWAD
        Dim iwadButton As SolidColorBrush

        iwadButton = Button_NewPreset_SetDoomIwad.Background
        If iwadButton.Color = Colors.LightGreen Then
            Return "Doom.wad"
        End If

        iwadButton = Button_NewPreset_SetDoom2Iwad.Background
        If iwadButton.Color = Colors.LightGreen Then
            Return "Doom2.wad"
        End If

        iwadButton = Button_NewPreset_SetFreedoomIwad.Background
        If iwadButton.Color = Colors.LightGreen Then
            Return "freedoom1.wad"
        End If

        iwadButton = Button_NewPreset_SetFreedoom2Iwad.Background
        If iwadButton.Color = Colors.LightGreen Then
            Return "freedoom2.wad"
        End If

        Return Nothing

    End Function

    Private Function NewPreset_GetSelectedLevel() As String

        If TextBox_DropWadFile.Text = "Drop a .wad/.pk3 file here ..." Then
            Return Nothing
        End If
        Return TextBox_DropWadFile.Text

    End Function

    Private Function NewPreset_GetSelectedMisc() As String

        'TODO : change text

        If TextBox_DropMiscFile.Text = "Drop a .deh/.??? file here ..." Then
            Return Nothing
        End If
        Return TextBox_DropMiscFile.Text

    End Function

    Private Sub Button_NewPreset_Try_Click(sender As Object, e As RoutedEventArgs) Handles Button_NewPreset_Try.Click

        'IWAD
        TextBox_IwadToLaunch.Text = NewPreset_GetSelectedIwad()
        My.Settings.SelectedIwad = NewPreset_GetSelectedIwad()

        'Level
        TextBox_LevelToLaunch.Text = NewPreset_GetSelectedLevel()
        My.Settings.SelectedLevel = NewPreset_GetSelectedLevel()

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

        If TextBox_NewPreset_Name.Text = "Enter preset name ..." Or TextBox_NewPreset_Name.Text = Nothing Then
            MessageBox.Show("New user preset requires a name to be saved")
            Return
        End If

        If NewPreset_GetSelectedIwad() = Nothing Then
            MessageBox.Show("New user preset requires at least an IWAD to be saved")
            Return
        End If

        'TODO : Check input validity !

        Dim nameToSave As String = TextBox_NewPreset_Name.Text
        Dim iwadToSave As String = NewPreset_GetSelectedIwad()
        Dim levelToSave As String = NewPreset_GetSelectedLevel() 'can be Nothing -> ok for String.Format ?
        Dim miscToSave As String = NewPreset_GetSelectedMisc() 'can be Nothing -> ok for String.Format ?
        WritePresetToFile(nameToSave, iwadToSave, levelToSave, miscToSave)

    End Sub

    'Private Sub Button_LaunchParameters_Reset_Click(sender As Object, e As RoutedEventArgs) Handles Button_LaunchParameters_Reset.Click

    '    TextBox_IwadToLaunch.Text = Nothing
    '    TextBox_LevelToLaunch.Text = Nothing
    '    TextBox_MiscToLaunch.Text = Nothing

    'End Sub

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


    Private Sub Test()

        'My.Settings.ScreenHeight = 1440
        'My.Settings.BrutalDoomVersion = "bd21testApr25.pk3"
        'My.Settings.Save()

        'MessageBox.Show(ValidateFile("C:\Users\Frenz\source\repos\AnotherYoutubeAudioDownloader\AnotherYoutuion Of DOOM.mp3"))

    End Sub

    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        Test()
    End Sub

End Class
