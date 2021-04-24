Imports System.IO
Imports System.Text.RegularExpressions
Imports ThisIsADoomLauncher.Models
'Imports ThisIsADoomLauncher.Helpers

Namespace Views
    Class MainWindow




#Region "Startup"

        'TODO? Think about async
        Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)

            Try
                SetRootDirPath()
                ValidateDirectories()
                SetIniFiles()
                SetCommonPresets()
                UpdateGUI()

                'Performance eval
                Dim dateTimeReady As DateTime = DateTime.Now
                Dim timeSpan As TimeSpan = dateTimeReady.Subtract(My.Settings.DateTimeAtLaunch)
                WriteToLog(DateTime.Now & " - Time elapsed from Launch to Ready : " & timeSpan.Milliseconds & " milliseconds")

            Catch ex As Exception
                WriteToLog(DateTime.Now & " - Error in 'MainWindow:Window_Loaded()'. Exception : " & ex.ToString)
            End Try

        End Sub

        Private Sub SetCommonPresets()
            Try
                'DisplayPresets("common", FormatPresetsData_FromCsv("common")) PREVIOUS
                ListView_CommonPresets.ItemsSource = FormatPresetsData_FromCsv("common")

            Catch ex As Exception
                WriteToLog(DateTime.Now & " - Error in 'SetCommonPresets()'. Exception : " & ex.ToString)
            End Try

        End Sub

        Sub UpdateGUI()

            Try
                With My.Settings
                    'Auto-set native resolution at first launch
                    If .ScreenWidth = 0 And .ScreenHeight = 0 Then
                        .ScreenWidth = GetResolution_Width()
                        .ScreenHeight = GetResolution_Height()
                    End If

                    Label_EngineToLaunch.Content = .SelectedEngine
                    Label_ResolutionToLaunch.Content = "Resolution : " & My.Settings.ScreenWidth.ToString & " x " & My.Settings.ScreenHeight.ToString
                    Label_FullscreenToLaunch.Content = "Fullscreen : " & If(My.Settings.FullscreenEnabled, "Yes", "No")

                    TextBox_ModToLaunch.Text = If(.UseBrutalDoom = False Or .BrutalDoomVersion = Nothing, Nothing, .BrutalDoomVersion)

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

            Catch ex As Exception
                WriteToLog(DateTime.Now & " - Error in 'UpdateGUI()'. Exception : " & ex.ToString)
            End Try

        End Sub

#End Region




#Region "Sidebar buttons"

        Private Sub Button_Menu_Help_Click(sender As Object, e As RoutedEventArgs) Handles Button_Help.Click

            Try
                Dim helpWindow As HelpWindow = New HelpWindow()
                helpWindow.Owner = MainWindow_Instance()
                helpWindow.ShowDialog()

            Catch ex As Exception
                WriteToLog(DateTime.Now & " - Error in 'Button_Menu_Help_Click()'. Exception : " & ex.ToString)
            End Try

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

            Try
                Dim settingsWindow As SettingsWindow = New SettingsWindow()
                settingsWindow.Owner = MainWindow_Instance()
                settingsWindow.ShowDialog()

            Catch ex As Exception
                WriteToLog(DateTime.Now & " - Error in 'Button_Menu_Settings_Click()'. Exception : " & ex.ToString)
            End Try

        End Sub

        Private Sub Button_Launch_Click(sender As Object, e As RoutedEventArgs) Handles Button_Launch.Click

            Try
                'Save level choices into Settings
                With My.Settings
                    .SelectedIwad = TextBox_IwadToLaunch.Text
                    .SelectedLevel = TextBox_LevelToLaunch.Text
                    .SelectedMisc = TextBox_MiscToLaunch.Text
                End With

                If TextBox_IwadToLaunch.Text = Nothing Then
                    MessageBox.Show("Error : an IWAD must be defined in the launch parameters")
                    Return
                Else
                    Dim cli As String = If(TextBox_IwadToLaunch.Text = "Wolf3D", BuildCommandLine(True), BuildCommandLine(False))

                    LaunchProcess(cli)
                    WriteToLog(DateTime.Now & " - CommandLine = " & cli)

                    My.Settings.Save()
                    WriteToLog(DateTime.Now & " - Saved settings")
                End If

            Catch ex As Exception
                WriteToLog(DateTime.Now & " - Error in 'Button_Launch_Click()'. Exception : " & ex.ToString)
            End Try

        End Sub

#End Region




        Private Sub TabControl_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)

            Try
                Dim tabControl As TabControl = sender
                Dim item As TabItem = tabControl.SelectedValue

                If item.Name = "User" Then
                    If File.Exists(Path.Combine(My.Settings.RootDirPath, "presets.csv")) Then
                        DisplayPresets(FormatPresetsData_FromCsv("user")) 'TODO? Think about async
                    End If
                End If

            Catch ex As Exception
                WriteToLog(DateTime.Now & " - Error in 'TabControl_SelectionChanged()'. Exception : " & ex.ToString)
            End Try

        End Sub

        Private Sub ListView_CommonPresets_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)

            Dim p As Preset = CType(sender.SelectedItem, Preset)

            TextBox_IwadToLaunch.Text = ConvertIwadPath_RelativeToAbsolute(p.Iwad)
            TextBox_LevelToLaunch.Text = ConvertLevelPath_RelativeToAbsolute(p.Level)
            TextBox_MiscToLaunch.Text = ConvertMiscPath_RelativeToAbsolute(p.Misc)

        End Sub




#Region "Add new preset"

        Private Sub Button_NewPreset_SetDoomIwad_Click(sender As Object, e As RoutedEventArgs) Handles Button_NewPreset_SetDoomIwad.Click

            If File.Exists(My.Settings.IwadsDir & "\Doom.wad") Then
                Button_NewPreset_SetDoomIwad.Background = Brushes.LightGreen
                Button_NewPreset_SetDoom2Iwad.Background = Brushes.Transparent
                Button_NewPreset_SetFreedoomIwad.Background = Brushes.Transparent
                Button_NewPreset_SetFreedoom2Iwad.Background = Brushes.Transparent
            Else
                MessageBox.Show("Error : File 'Doom.wad' not found in :" & Environment.NewLine & My.Settings.IwadsDir)
            End If

        End Sub

        Private Sub Button_NewPreset_SetDoom2Iwad_Click(sender As Object, e As RoutedEventArgs) Handles Button_NewPreset_SetDoom2Iwad.Click

            If File.Exists(My.Settings.IwadsDir & "\Doom2.wad") Then
                Button_NewPreset_SetDoom2Iwad.Background = Brushes.LightGreen
                Button_NewPreset_SetDoomIwad.Background = Brushes.Transparent
                Button_NewPreset_SetFreedoomIwad.Background = Brushes.Transparent
                Button_NewPreset_SetFreedoom2Iwad.Background = Brushes.Transparent
            Else
                MessageBox.Show("Error : File 'Doom2.wad' not found in :" & Environment.NewLine & My.Settings.IwadsDir)
            End If

        End Sub

        Private Sub Button_NewPreset_SetFreedoomIwad_Click(sender As Object, e As RoutedEventArgs) Handles Button_NewPreset_SetFreedoomIwad.Click

            If File.Exists(My.Settings.IwadsDir & "\freedoom1.wad") Then
                Button_NewPreset_SetFreedoomIwad.Background = Brushes.LightGreen
                Button_NewPreset_SetDoomIwad.Background = Brushes.Transparent
                Button_NewPreset_SetDoom2Iwad.Background = Brushes.Transparent
                Button_NewPreset_SetFreedoom2Iwad.Background = Brushes.Transparent
            Else
                MessageBox.Show("Error : File 'freedoom1.wad' not found in :" & Environment.NewLine & My.Settings.IwadsDir)
            End If

        End Sub

        Private Sub Button_NewPreset_SetFreedoom2Iwad_Click(sender As Object, e As RoutedEventArgs) Handles Button_NewPreset_SetFreedoom2Iwad.Click

            If File.Exists(My.Settings.IwadsDir & "\freedoom2.wad") Then
                Button_NewPreset_SetFreedoom2Iwad.Background = Brushes.LightGreen
                Button_NewPreset_SetDoomIwad.Background = Brushes.Transparent
                Button_NewPreset_SetDoom2Iwad.Background = Brushes.Transparent
                Button_NewPreset_SetFreedoomIwad.Background = Brushes.Transparent
            Else
                MessageBox.Show("Error : File 'freedoom2.wad' not found in :" & Environment.NewLine & My.Settings.IwadsDir)
            End If

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
                    MessageBox.Show("Error : this file refers to a 'Level' file")
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
                TextBox_NewPreset_Name.Foreground = Brushes.DarkGray
            End If

        End Sub


        Private Sub Button_NewPreset_Try_Click(sender As Object, e As RoutedEventArgs) Handles Button_NewPreset_Try.Click

            'Note : KnowSelected**** don't care about path validity : that is done later on TextBox.TextChanged events

            TextBox_IwadToLaunch.Text = KnowSelectedIwad_NewPreset()
            TextBox_LevelToLaunch.Text = KnowSelectedLevel_NewPreset()
            TextBox_MiscToLaunch.Text = KnowSelectedMisc_NewPreset()

        End Sub

        Private Sub Button_NewPreset_Reset_Click(sender As Object, e As RoutedEventArgs) Handles Button_NewPreset_Reset.Click

            ResetFields_NewPreset()

        End Sub

        Private Sub Button_NewPreset_Save_Click(sender As Object, e As RoutedEventArgs) Handles Button_NewPreset_Save.Click

            Save_NewPreset()

        End Sub

#End Region




#Region "Launch parameters"

        Private Sub TextBox_IwadToLaunch_TextChanged(sender As Object, e As TextChangedEventArgs) Handles TextBox_IwadToLaunch.TextChanged

            'weak. TODO : Improve Wolf3D integration
            If TextBox_IwadToLaunch.Text = "Wolf3D" Then
                Return
            End If

            If File.Exists(TextBox_IwadToLaunch.Text) Then
                TextBox_IwadToLaunch.Foreground = Brushes.Black
            Else
                TextBox_IwadToLaunch.Foreground = Brushes.Red
            End If

        End Sub

        Private Sub TextBox_LevelToLaunch_TextChanged(sender As Object, e As TextChangedEventArgs) Handles TextBox_LevelToLaunch.TextChanged

            If File.Exists(TextBox_LevelToLaunch.Text) Then
                TextBox_LevelToLaunch.Foreground = Brushes.Black
            Else
                TextBox_LevelToLaunch.Foreground = Brushes.Red
            End If

        End Sub

        Private Sub TextBox_MiscToLaunch_TextChanged(sender As Object, e As TextChangedEventArgs) Handles TextBox_MiscToLaunch.TextChanged

            If File.Exists(TextBox_MiscToLaunch.Text) Then
                TextBox_MiscToLaunch.Foreground = Brushes.Black
            Else
                TextBox_MiscToLaunch.Foreground = Brushes.Red
            End If

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



#Region "Testing tab"

        Private Sub TextBox_TestingEngine_PreviewDragOver(sender As Object, e As DragEventArgs)
            e.Handled = True
        End Sub

        Private Sub TextBox_TestingEngine_Drop(sender As Object, e As DragEventArgs)

            Try
                Dim file() As String = CType(e.Data.GetData(DataFormats.FileDrop), String())
                Dim info As FileInfo = New FileInfo(file(0))
                Dim ext As String = info.Extension.ToLowerInvariant

                If info.Extension.ToLowerInvariant = ".exe" Then TextBox_TestingEngine.Text = file(0)

            Catch ex As Exception
                WriteToLog(DateTime.Now & " - Error in 'TextBox_TestingEngine_Drop()'. Exception : " & ex.ToString)
            End Try

        End Sub

        Private Sub TextBox_TestingIwad_PreviewDragOver(sender As Object, e As DragEventArgs)
            e.Handled = True
        End Sub

        Private Sub TextBox_TestingIwad_Drop(sender As Object, e As DragEventArgs)

            Try
                Dim file() As String = CType(e.Data.GetData(DataFormats.FileDrop), String())
                If ValidateFile(file(0)) = "iwad" Then TextBox_TestingIwad.Text = file(0)

            Catch ex As Exception
                WriteToLog(DateTime.Now & " - Error in 'TextBox_TestingIwad_Drop()'. Exception : " & ex.ToString)
            End Try

        End Sub

        Private Sub TextBox_TestingFile1_PreviewDragOver(sender As Object, e As DragEventArgs)
            e.Handled = True
        End Sub

        Private Sub TextBox_TestingFile1_Drop(sender As Object, e As DragEventArgs)

            Try
                Dim file() As String = CType(e.Data.GetData(DataFormats.FileDrop), String())
                TextBox_TestingFile1.Text = file(0)

            Catch ex As Exception
                WriteToLog(DateTime.Now & " - Error in 'TextBox_TestingFile1_Drop()'. Exception : " & ex.ToString)
            End Try

        End Sub

        Private Sub TextBox_TestingFile2_PreviewDragOver(sender As Object, e As DragEventArgs)
            e.Handled = True
        End Sub

        Private Sub TextBox_TestingFile2_Drop(sender As Object, e As DragEventArgs)

            Try
                Dim file() As String = CType(e.Data.GetData(DataFormats.FileDrop), String())
                TextBox_TestingFile2.Text = file(0)

            Catch ex As Exception
                WriteToLog(DateTime.Now & " - Error in 'TextBox_TestingFile2_Drop()'. Exception : " & ex.ToString)
            End Try

        End Sub

        Private Sub TextBox_TestingFile3_PreviewDragOver(sender As Object, e As DragEventArgs)
            e.Handled = True
        End Sub

        Private Sub TextBox_TestingFile3_Drop(sender As Object, e As DragEventArgs)

            Try
                Dim file() As String = CType(e.Data.GetData(DataFormats.FileDrop), String())
                TextBox_TestingFile3.Text = file(0)

            Catch ex As Exception
                WriteToLog(DateTime.Now & " - Error in 'TextBox_TestingFile3_Drop()'. Exception : " & ex.ToString)
            End Try

        End Sub

        Private Sub TextBox_TestingFile4_PreviewDragOver(sender As Object, e As DragEventArgs)
            e.Handled = True
        End Sub

        Private Sub TextBox_TestingFile4_Drop(sender As Object, e As DragEventArgs)

            Try
                Dim file() As String = CType(e.Data.GetData(DataFormats.FileDrop), String())
                TextBox_TestingFile4.Text = file(0)

            Catch ex As Exception
                WriteToLog(DateTime.Now & " - Error in 'TextBox_TestingFile4_Drop()'. Exception : " & ex.ToString)
            End Try

        End Sub

        Private Sub TextBox_TestingFile5_PreviewDragOver(sender As Object, e As DragEventArgs)
            e.Handled = True
        End Sub

        Private Sub TextBox_TestingFile5_Drop(sender As Object, e As DragEventArgs)

            Try
                Dim file() As String = CType(e.Data.GetData(DataFormats.FileDrop), String())
                TextBox_TestingFile5.Text = file(0)

            Catch ex As Exception
                WriteToLog(DateTime.Now & " - Error in 'TextBox_TestingFile5_Drop()'. Exception : " & ex.ToString)
            End Try

        End Sub

        Private Sub TextBox_TestingEngine_TextChanged(sender As Object, e As TextChangedEventArgs)
            UpdateCommandPreview()
            DecorateCommandPreview()
        End Sub

        Private Sub TextBox_TestingEngineParameters_TextChanged(sender As Object, e As TextChangedEventArgs)
            UpdateCommandPreview()
            DecorateCommandPreview()
        End Sub

        Private Sub TextBox_TestingIwad_TextChanged(sender As Object, e As TextChangedEventArgs)
            UpdateCommandPreview()
            DecorateCommandPreview()
        End Sub

        Private Sub TextBox_TestingFile1_TextChanged(sender As Object, e As TextChangedEventArgs)
            UpdateCommandPreview()
            DecorateCommandPreview()
        End Sub

        Private Sub TextBox_TestingFile2_TextChanged(sender As Object, e As TextChangedEventArgs)
            UpdateCommandPreview()
            DecorateCommandPreview()
        End Sub

        Private Sub TextBox_TestingFile3_TextChanged(sender As Object, e As TextChangedEventArgs)
            UpdateCommandPreview()
            DecorateCommandPreview()
        End Sub

        Private Sub TextBox_TestingFile4_TextChanged(sender As Object, e As TextChangedEventArgs)
            UpdateCommandPreview()
            DecorateCommandPreview()
        End Sub

        Private Sub TextBox_TestingFile5_TextChanged(sender As Object, e As TextChangedEventArgs)
            UpdateCommandPreview()
            DecorateCommandPreview()
        End Sub

        Private Sub TextBox_TestingExtraParameters_TextChanged(sender As Object, e As TextChangedEventArgs)
            UpdateCommandPreview()
            DecorateCommandPreview()
        End Sub

        Private Sub UpdateCommandPreview()

            Try
                'Build
                Dim engine As String = String.Format("""{0}""", TextBox_TestingEngine.Text)
                Dim engineParams As String = If(TextBox_TestingEngineParameters.Text = Nothing, Nothing, String.Format(" {0}", TextBox_TestingEngineParameters.Text))
                Dim iwad As String = If(TextBox_TestingIwad.Text = Nothing, Nothing, String.Format(" -iwad ""{0}""", TextBox_TestingIwad.Text))
                Dim file1 As String = If(TextBox_TestingFile1.Text = Nothing, Nothing, String.Format(" -file ""{0}""", TextBox_TestingFile1.Text))
                Dim file2 As String = If(TextBox_TestingFile2.Text = Nothing, Nothing, String.Format(" -file ""{0}""", TextBox_TestingFile2.Text))
                Dim file3 As String = If(TextBox_TestingFile3.Text = Nothing, Nothing, String.Format(" -file ""{0}""", TextBox_TestingFile3.Text))
                Dim file4 As String = If(TextBox_TestingFile4.Text = Nothing, Nothing, String.Format(" -file ""{0}""", TextBox_TestingFile4.Text))
                Dim file5 As String = If(TextBox_TestingFile5.Text = Nothing, Nothing, String.Format(" -file ""{0}""", TextBox_TestingFile5.Text))
                'Or built list from multiples inputs (several files dropped in the same zone) and use a For each
                Dim extraParams As String = If(TextBox_TestingExtraParameters.Text = Nothing, Nothing, String.Format(" {0}", TextBox_TestingExtraParameters.Text))

                Dim command As String = String.Format("{0}{1}{2}{3}{4}{5}{6}{7}", engine, engineParams, iwad, file1, file2, file3, file4, file5)
                FillRichTextBox(command)

            Catch ex As Exception
                WriteToLog(DateTime.Now & " - Error in 'UpdateCommandPreview()'. Exception : " & ex.ToString)
            End Try

        End Sub

        Private Sub FillRichTextBox(content As String)

            Dim flow As FlowDocument = New FlowDocument()
            Dim para As Paragraph = New Paragraph()

            para.Inlines.Add(content)
            flow.Blocks.Add(para)

            RichTextBox_TestingCommandPreview.Document = flow

        End Sub

        Private Sub DecorateCommandPreview()

            Try
                Dim completeRange As TextRange = New TextRange(RichTextBox_TestingCommandPreview.Document.ContentStart, RichTextBox_TestingCommandPreview.Document.ContentEnd)
                Dim matches As MatchCollection = Regex.Matches(completeRange.Text, "-iwad|-file")
                Dim quotesCount As Integer = 0 'Enclosing quotes " must be skipped (4 for each path : ""complete_path"")

                For Each m As Match In matches
                    For Each c As Capture In m.Captures

                        Dim startIndex As TextPointer = completeRange.Start.GetPositionAtOffset(c.Index + quotesCount * 4)
                        Dim endIndex As TextPointer = completeRange.Start.GetPositionAtOffset(c.Index + quotesCount * 4 + c.Length)
                        Dim rangeToEdit As TextRange = New TextRange(startIndex, endIndex)

                        rangeToEdit.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.DarkBlue)
                        rangeToEdit.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold)

                    Next
                    quotesCount += 1
                Next

            Catch ex As Exception
                WriteToLog(DateTime.Now & " - Error in 'DecorateCommandPreview()'. Exception : " & ex.ToString)
            End Try


        End Sub

        Private Sub Button_TestingExecute_Click(sender As Object, e As RoutedEventArgs)

            ExecuteCommandPreview()

        End Sub

        Private Sub Button_TestingCopy_Click(sender As Object, e As RoutedEventArgs)

            CopyCommandToClipboard()

        End Sub

        Private Sub Button_TestingExport_Click(sender As Object, e As RoutedEventArgs)

            ExportCommandAsBat()

        End Sub

        Private Sub ExecuteCommandPreview()

            Try
                Dim commandText = New TextRange(RichTextBox_TestingCommandPreview.Document.ContentStart, RichTextBox_TestingCommandPreview.Document.ContentEnd).Text
                If commandText = Nothing Then Return
                LaunchProcess(commandText)
                WriteToLog(DateTime.Now & " - CommandLine :" & Environment.NewLine & commandText)

            Catch ex As Exception
                WriteToLog(DateTime.Now & " - Error in 'ExecuteCommandPreview()'. Exception : " & ex.ToString)
            End Try

        End Sub

        Private Sub CopyCommandToClipboard()

            Try
                Dim commandText = New TextRange(RichTextBox_TestingCommandPreview.Document.ContentStart, RichTextBox_TestingCommandPreview.Document.ContentEnd).Text
                Clipboard.SetText(commandText)

            Catch ex As Exception
                WriteToLog(DateTime.Now & " - Error in 'CopyCommandToClipboard()'. Exception : " & ex.ToString)
            End Try

        End Sub

        Private Sub ExportCommandAsBat()

            Try
                Dim commandText = New TextRange(RichTextBox_TestingCommandPreview.Document.ContentStart, RichTextBox_TestingCommandPreview.Document.ContentEnd).Text

                Dim now_formatted As String = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss")
                Dim batPath As String = Path.Combine(My.Settings.RootDirPath, now_formatted & "_command.bat")

                Using writer As StreamWriter = New StreamWriter(batPath, False, Text.Encoding.UTF8)
                    writer.WriteLine("@echo off")
                    writer.WriteLine("/c start """" " & commandText)
                End Using

            Catch ex As Exception
                WriteToLog(DateTime.Now & " - Error in 'ExportCommandAsBat()'. Exception : " & ex.ToString)
            End Try

        End Sub

#End Region



#Region "Quick tests"

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
End Namespace

