Imports System.IO
Imports System.Text
Imports System.Text.RegularExpressions
Imports ThisIsADoomLauncher.Models

Namespace Views
    Class MainWindow


#Region "Startup"

        'TODO? Think about async
        Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)

            Title = GetFormattedAppTitle()




            Try
                CheckProjectSubdirectories() 'V3

                '---------------------------------------------------------------------------------
                ' LOAD JSON DATA
                '---------------------------------------------------------------------------------
                'TODO: Make it solid, robust, resilient

                Dim savedSettings As Setting = LoadFromJsonData()

                With savedSettings

                    If File.Exists(.Port) Then TextBox_Summary_Port.Text = .Port
                    If File.Exists(.Iwad) Then TextBox_Summary_Iwad.Text = .Iwad

                End With

                '---------------------------------------------------------------------------------

                'SetIniFiles() 'TODO V3
                'LoadSettings()
                PopulateBaseLevelPresets() 'V3
                PopulateBaseModsPresets() 'V3

                'Performance eval : reliable? TODO? Improve
                Dim dateTimeReady As Date = Date.Now
                Dim timeSpan As TimeSpan = dateTimeReady.Subtract(My.Settings.DateTimeAtLaunch)
                WriteToLog(Date.Now & " - Time elapsed from Launch to Ready : " & timeSpan.Milliseconds & " milliseconds")

            Catch ex As Exception
                WriteToLog(Date.Now & " - Error in 'MainWindow:Window_Loaded()'. Exception : " & ex.ToString)
            End Try

        End Sub

        Private Sub PopulateBaseLevelPresets()

            'V3
            Try
                ListView_Levels_BasePresets.ItemsSource = GetLevelPresets_FromCsv("base_levels") 'TODO V3 : Change COMMON to BASE

            Catch ex As Exception
                WriteToLog(Date.Now & " - Error in 'SetCommonPresets()'. Exception : " & ex.ToString)
            End Try

        End Sub

        Private Sub PopulateBaseModsPresets()

            'V3
            Try
                ListView_Mods_BasePresets.ItemsSource = GetModPresets_FromCSV("base_mods")

            Catch ex As Exception
                WriteToLog(Date.Now & " - Error in 'SetCommonPresets()'. Exception : " & ex.ToString)
            End Try

        End Sub

#End Region



#Region "Add new preset"

        'Private Sub TextBox_NewPreset_Name_GotFocus(sender As Object, e As RoutedEventArgs) Handles TextBox_NewPreset_Name.GotFocus

        '    If TextBox_NewPreset_Name.Text = "Enter preset name..." Then
        '        TextBox_NewPreset_Name.Text = Nothing
        '        TextBox_NewPreset_Name.ClearValue(FontStyleProperty)
        '        TextBox_NewPreset_Name.ClearValue(ForegroundProperty)
        '    End If

        'End Sub

        'Private Sub TextBox_NewPreset_Name_LostFocus(sender As Object, e As RoutedEventArgs) Handles TextBox_NewPreset_Name.LostFocus

        '    If TextBox_NewPreset_Name.Text = Nothing Then
        '        TextBox_NewPreset_Name.Text = "Enter preset name..."
        '        TextBox_NewPreset_Name.FontStyle = FontStyles.Italic
        '        TextBox_NewPreset_Name.Foreground = Brushes.DarkGray
        '    End If

        'End Sub

#End Region



#Region "v2.3 stuff"


        Private Sub CopyCommandToClipboard()

            Try
                Dim commandText = New TextRange(RichTextBox_Command.Document.ContentStart, RichTextBox_Command.Document.ContentEnd).Text
                Clipboard.SetText(commandText)

            Catch ex As Exception
                WriteToLog(Date.Now & " - Error in 'CopyCommandToClipboard()'. Exception : " & ex.ToString)
            End Try

        End Sub

        Private Sub ExportCommandAsBat()

            Try
                Dim commandText = New TextRange(RichTextBox_Command.Document.ContentStart, RichTextBox_Command.Document.ContentEnd).Text

                Dim now_formatted As String = Date.Now.ToString("yyyy-MM-dd_HH-mm-ss")
                Dim batPath As String = Path.Combine(GetDirectoryPath(""), now_formatted & "_command.bat") 'TODO : Change/Add RootDirPath variable

                Using writer As New StreamWriter(batPath, False, Encoding.Default)
                    writer.WriteLine("@echo off")
                    writer.WriteLine("start """" " & commandText)
                End Using

            Catch ex As Exception
                WriteToLog(Date.Now & " - Error in 'ExportCommandAsBat()'. Exception : " & ex.ToString)
            End Try

        End Sub


#End Region



#Region "TiaDL v3"


        Private Sub TextBox_Port_PreviewDragOver(sender As Object, e As DragEventArgs)
            e.Handled = True
        End Sub

        Private Sub TextBox_Port_Drop(sender As Object, e As DragEventArgs)

            Try
                Dim droppedFile As String = e.Data.GetData(DataFormats.FileDrop)(0)
                Dim fileExtension As String = New FileInfo(droppedFile).Extension.ToLowerInvariant

                If fileExtension = ".exe" Then
                    FillTextBox(TextBox_Port, droppedFile)
                    UpdateSummary()
                    UpdateCommand()
                    DecorateCommand()
                End If

            Catch ex As Exception
                WriteToLog(Date.Now & " - Error in 'TextBox_Port_Drop()'. Exception : " & ex.ToString)
            End Try

        End Sub

        Private Sub Button_Port_Clear_Click(sender As Object, e As RoutedEventArgs)

            UnfillTextBox(TextBox_Port, "Drop Doom port .exe file here... (GZDoom, Zandronum, etc.)")
            UpdateSummary()
            UpdateCommand()
            DecorateCommand()

        End Sub






        Private Sub GroupBox_Levels_PreviewDragOver(sender As Object, e As DragEventArgs)
            e.Handled = True
        End Sub

        ''' <summary>
        ''' This feature allows to drop multiples files into the GroupBox "Levels"
        ''' </summary>
        Private Sub GroupBox_Levels_Drop(sender As Object, e As DragEventArgs)

            Try
                '1) Collect files
                Dim filePaths As String() = e.Data.GetData(DataFormats.FileDrop)

                '2) Check & order files
                Dim confirmedFiles As List(Of String) = OrderDroppedFiles_Levels(filePaths)

                '------------- Template of confirmedFiles, according to .Count :
                '-------------  1 = Iwad
                '-------------  2 = Iwad, Level
                '-------------  3 = Iwad, Level, Misc
                '-------------  4 = Iwad, Level, Misc, Image

                '3) Feed GUI if Count > 0
                If confirmedFiles.Count = 0 Then Return
                TabControl_Levels.SelectedIndex = 2

                If confirmedFiles.Count > 0 Then FillTextBox(TextBox_NewLevel_Iwad, confirmedFiles(0))
                If confirmedFiles.Count > 1 Then FillTextBox(TextBox_NewLevel_Level, confirmedFiles(1))
                If confirmedFiles.Count > 2 Then FillTextBox(TextBox_NewLevel_Misc, confirmedFiles(2))
                If confirmedFiles.Count > 3 Then FillTextBox(TextBox_NewLevel_Image, confirmedFiles(3))

            Catch ex As Exception
                WriteToLog(Date.Now & " - Error in 'GroupBox_Levels_Drop()'. Exception : " & ex.ToString)
            End Try

        End Sub

        ''' <summary>
        ''' Check and order the files dropped into the GroupBox "Levels"
        ''' </summary>
        Private Function OrderDroppedFiles_Levels(filePaths As String()) As List(Of String)

            Dim orderedFiles As New List(Of String)

            Try
                'Do as many outer loops as there are files, to obtain the correct order in the end (for instance 3 times for 3 files)
                For i As Integer = 1 To filePaths.Length
                    For Each path As String In filePaths

                        If orderedFiles.Contains(path) Then Continue For

                        If ValidateFile_Iwad(path) Then
                            orderedFiles.Add(path)
                            Continue For
                        End If

                        If ValidateFile_Level(path) And orderedFiles.Count > 0 Then
                            orderedFiles.Add(path)
                            Continue For
                        End If

                        If ValidateFile_Misc(path) And orderedFiles.Count > 1 Then
                            orderedFiles.Add(path)
                            Continue For
                        End If

                        If ValidateFile_Image(path) And orderedFiles.Count > 2 Then
                            orderedFiles.Add(path)
                            Continue For
                        End If

                    Next
                Next

            Catch ex As Exception
                WriteToLog(Date.Now & " - Error in 'OrderDroppedFiles_Levels()'. Exception : " & ex.ToString)
            End Try

            Return orderedFiles

        End Function






        Private Sub TextBox_NewLevel_Iwad_PreviewDragOver(sender As Object, e As DragEventArgs)
            e.Handled = True
        End Sub

        Private Sub TextBox_NewLevel_Iwad_Drop(sender As Object, e As DragEventArgs)

            Dim droppedFile As String = e.Data.GetData(DataFormats.FileDrop)(0)
            If ValidateFile_Iwad(droppedFile) Then FillTextBox(sender, droppedFile)

        End Sub

        Private Sub Button_NewLevel_Iwad_Clear_Click(sender As Object, e As RoutedEventArgs)

            UnfillTextBox(TextBox_NewLevel_Iwad, "Drop an IWAD file here...")

        End Sub



        Private Sub TextBox_NewLevel_Level_PreviewDragOver(sender As Object, e As DragEventArgs)
            e.Handled = True
        End Sub

        Private Sub TextBox_NewLevel_Level_Drop(sender As Object, e As DragEventArgs)

            Dim droppedFile As String = e.Data.GetData(DataFormats.FileDrop)(0)
            If ValidateFile_Level(droppedFile) Then FillTextBox(sender, droppedFile)

        End Sub

        Private Sub Button_NewLevel_Level_Clear_Click(sender As Object, e As RoutedEventArgs)

            UnfillTextBox(TextBox_NewLevel_Level, "Drop a .wad/.pk3 file here...")

        End Sub



        Private Sub TextBox_NewLevel_Misc_PreviewDragOver(sender As Object, e As DragEventArgs)
            e.Handled = True
        End Sub

        Private Sub TextBox_NewLevel_Misc_Drop(sender As Object, e As DragEventArgs)

            Dim droppedFile As String = e.Data.GetData(DataFormats.FileDrop)(0)
            If ValidateFile_Misc(droppedFile) Then FillTextBox(sender, droppedFile)

        End Sub

        Private Sub Button_NewLevel_Misc_Clear_Click(sender As Object, e As RoutedEventArgs)

            UnfillTextBox(TextBox_NewLevel_Misc, "Drop a .deh/.bex file here...")

        End Sub



        Private Sub TextBox_NewLevel_Image_PreviewDragOver(sender As Object, e As DragEventArgs)
            e.Handled = True
        End Sub

        Private Sub TextBox_NewLevel_Image_Drop(sender As Object, e As DragEventArgs)

            Dim droppedFile As String = e.Data.GetData(DataFormats.FileDrop)(0)
            If ValidateFile_Image(droppedFile) Then FillTextBox(sender, droppedFile)

        End Sub

        Private Sub Button_NewLevel_Image_Clear_Click(sender As Object, e As RoutedEventArgs)

            UnfillTextBox(TextBox_NewLevel_Image, "Drop a .jpg/.png file here...")

        End Sub



        Private Sub Button_NewLevel_Try_Click(sender As Object, e As RoutedEventArgs)

        End Sub

        Private Sub Button_NewLevel_SaveAs_Click(sender As Object, e As RoutedEventArgs)

        End Sub

        Private Sub Button_NewLevel_ClearAll_Click(sender As Object, e As RoutedEventArgs)

            UnfillTextBox(TextBox_NewLevel_Iwad, "Drop an IWAD file here...")
            UnfillTextBox(TextBox_NewLevel_Level, "Drop a .wad/.pk3 file here...")
            UnfillTextBox(TextBox_NewLevel_Misc, "Drop a .deh/.bex file here...")

        End Sub






        Private Sub ListView_Levels_BasePresets_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)

            UpdateSummary()
            UpdateCommand()
            DecorateCommand()

        End Sub

        Private Sub ListView_Levels_UserPresets_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)

        End Sub

        Private Sub ListView_Mods_BasePresets_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)

            Try
                Dim p As ModPreset = CType(sender.SelectedItem, ModPreset)

                TextBlock_Mods_Desc.Text = p.Desc
                ListView_Mods_Files.ItemsSource = p.Files

                UpdateSummary()
                UpdateCommand()
                DecorateCommand()

            Catch ex As Exception
                WriteToLog(Date.Now & " - Error in 'ListView_Mods_BasePresets_SelectionChanged()'. Exception : " & ex.ToString)
            End Try

        End Sub



        Private Function ReturnSelectedLevels(Optional fromSettings As Boolean = False) As LevelPreset

            Dim preset As LevelPreset = Nothing

            Try
                If fromSettings Then
                    If Not My.Settings.SelectedIwad = Nothing Then

                        Return New LevelPreset() With
                        {
                            .Iwad = My.Settings.SelectedIwad,
                            .Level = My.Settings.SelectedLevel,
                            .Misc = My.Settings.SelectedMisc
                        }

                    End If
                End If

                Select Case TabControl_Levels.SelectedIndex
                    Case 0
                        preset = CType(ListView_Levels_BasePresets.SelectedItem, LevelPreset)
                    Case 1
                        preset = CType(ListView_Levels_UserPresets.SelectedItem, LevelPreset)
                    Case 2
                        preset = New LevelPreset() With {.Iwad = "", .Level = "", .Misc = "", .ImagePath = ""} 'TODO
                End Select

            Catch ex As Exception
                WriteToLog(Date.Now & " - Error in 'ReturnSelectedLevels()'. Exception : " & ex.ToString)
            End Try

            Return preset

        End Function


        Private Function ReturnSelectedMods(Optional fromSettings As Boolean = False) As ModPreset

            Dim preset As ModPreset = Nothing

            Try
                If fromSettings Then
                    If Not My.Settings.FilesMods Is Nothing Then

                        Dim filesList As New List(Of String)
                        For Each modFile In My.Settings.FilesMods
                            filesList.Add(modFile)
                        Next

                        Return New ModPreset() With {.Files = filesList}

                    End If
                End If

                Select Case TabControl_Mods.SelectedIndex
                    Case 0
                        preset = CType(ListView_Mods_BasePresets.SelectedItem, ModPreset)
                    Case 1
                        preset = CType(ListView_Mods_BasePresets.SelectedItem, ModPreset) 'TODO
                    Case 2
                        preset = New ModPreset() With {.Files = New List(Of String)} 'TODO
                    Case Else

                End Select

            Catch ex As Exception
                WriteToLog(Date.Now & " - Error in 'ReturnSelectedMods()'. Exception : " & ex.ToString)
            End Try

            Return preset

        End Function


        Private Sub UpdateCommand(Optional fromSettings As Boolean = False)

            Try
                Dim port As String = Nothing
                Dim portParams As String = Nothing
                Dim iwad As String = Nothing
                Dim filesList As New List(Of String) 'will contain Level + Misc + Mod files, as List
                Dim files As String = Nothing 'will contain Level + Misc + Mod files, as single String

                'Port
                port = If(ReturnSelectedPort() = Nothing, Nothing, String.Format("""{0}""", ReturnSelectedPort()))
                portParams = Nothing 'TODO

                Dim lp As LevelPreset = ReturnSelectedLevels(fromSettings)
                If lp IsNot Nothing Then
                    'Iwad
                    Dim iwadAbsolutePath As String = ConvertPathRelativeToAbsolute_Iwad(lp.Iwad)
                    iwad = String.Format(" -iwad ""{0}""", iwadAbsolutePath)
                    'Level & Misc
                    If Not lp.Level = Nothing Then filesList.Add(ConvertPathRelativeToAbsolute_Level(lp.Level))
                    If Not lp.Misc = Nothing Then filesList.Add(ConvertPathRelativeToAbsolute_Misc(lp.Misc))
                End If

                'Mods
                Dim mp As ModPreset = ReturnSelectedMods(fromSettings)
                If mp IsNot Nothing Then
                    Dim modFilePaths As List(Of String) = ConvertModPath_RelativeToAbsolute(mp.Files)
                    For Each modFile As String In modFilePaths
                        filesList.Add(modFile)
                    Next
                End If

                For Each file As String In filesList
                    files &= String.Format(" -file ""{0}""", file)
                Next

                Dim command As String = String.Format("{0}{1}{2}{3}", port, portParams, iwad, files)
                FillRichTextBox_Command(command)

            Catch ex As Exception
                WriteToLog(Date.Now & " - Error in 'UpdateCommand()'. Exception : " & ex.ToString)
            End Try

        End Sub

        Private Sub DecorateCommand()

            Try
                Dim completeRange As New TextRange(RichTextBox_Command.Document.ContentStart, RichTextBox_Command.Document.ContentEnd)
                Dim matches As MatchCollection = Regex.Matches(completeRange.Text, "-iwad|-file")
                Dim quotesCount As Integer = 0 'Enclosing quotes " must be skipped = 4 for each path as in ""complete_path""

                For Each m As Match In matches
                    For Each c As Capture In m.Captures

                        Dim startIndex As TextPointer = completeRange.Start.GetPositionAtOffset(c.Index + quotesCount * 4)
                        Dim endIndex As TextPointer = completeRange.Start.GetPositionAtOffset(c.Index + quotesCount * 4 + c.Length)
                        Dim rangeToEdit As New TextRange(startIndex, endIndex)

                        rangeToEdit.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.DarkBlue)
                        rangeToEdit.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold)

                    Next
                    quotesCount += 1
                Next

            Catch ex As Exception
                WriteToLog(Date.Now & " - Error in 'DecorateCommandPreview()'. Exception : " & ex.ToString)
            End Try

        End Sub


        Private Sub UpdateSummary(Optional fromSettings As Boolean = False)

            Try
                'Port
                TextBox_Summary_Port.Text = ReturnSelectedPort()

                'Port Parameters
                'TODO

                StackPanel_Summary_FilesMods.Children.Clear()

                Dim lp As LevelPreset = ReturnSelectedLevels(fromSettings)
                If lp IsNot Nothing Then
                    'Iwad
                    TextBox_Summary_Iwad.Text = ConvertPathRelativeToAbsolute_Iwad(lp.Iwad)
                    'Level & Misc
                    Dim levelFileNames As New List(Of String)
                    If Not lp.Level = Nothing Then levelFileNames.Add(lp.Level)
                    If Not lp.Misc = Nothing Then levelFileNames.Add(lp.Misc)
                    DisplayLevels_Summary(levelFileNames)
                End If

                Dim mp As ModPreset = ReturnSelectedMods(fromSettings)
                If mp IsNot Nothing Then DisplayMods_Summary(mp.Files)

            Catch ex As Exception
                WriteToLog(Date.Now & " - Error in 'UpdateSummary()'. Exception : " & ex.ToString)
            End Try

        End Sub


        Private Sub DisplayLevels_Summary(fileNames As List(Of String))

            Try
                If fileNames.Count = 0 Then Return

                For Each name As String In fileNames
                    StackPanel_Summary_FilesMods.Children.Add(
                        New TextBox() With
                            {
                                .Margin = New Thickness(0, 0, 6, 0),
                                .Background = Brushes.LightGray,
                                .Text = name
                            }
                        )
                Next

            Catch ex As Exception
                WriteToLog(Date.Now & " - Error in 'HandleLevels_Summary()'. Exception : " & ex.ToString)
            End Try

        End Sub

        Private Sub DisplayMods_Summary(fileNames As List(Of String))

            Try
                If fileNames.Count = 0 Then Return

                For Each name As String In fileNames
                    'Ignore file not found or not specified
                    If name = Nothing Then Continue For

                    StackPanel_Summary_FilesMods.Children.Add(
                        New TextBox() With
                            {
                                .Margin = New Thickness(0, 0, 6, 0),
                                .Background = Brushes.DarkGray,
                                .Text = name
                            }
                        )
                Next

            Catch ex As Exception
                WriteToLog(Date.Now & " - Error in 'DisplayMods_Summary()'. Exception : " & ex.ToString)
            End Try

        End Sub


        ''' <summary>
        ''' Note 1 : Generic/Common function -> to be moved to Helpers files
        ''' Note 2 : Really useful to have it separated from the Display Subs ?
        ''' </summary>
        Private Function ConvertFilePath_AbsoluteToRelative(filePaths As List(Of String)) As List(Of String)

            Dim fileNames As New List(Of String)

            Try
                For Each path As String In filePaths
                    fileNames.Add(New FileInfo(path).Name)
                Next

            Catch ex As Exception
                WriteToLog(Date.Now & " - Error in 'ConvertFilePath_AbsoluteToRelative()'. Exception : " & ex.ToString)
            End Try

            Return fileNames

        End Function


        Private Function ConvertModPath_RelativeToAbsolute(modFilesList As List(Of String)) As List(Of String)

            Dim absoluteModPaths As List(Of String) = New List(Of String)

            Try
                For Each modFile As String In modFilesList

                    'Read absolute path
                    If File.Exists(modFile) Then absoluteModPaths.Add(modFile)

                    'Build absolute path with modsSubDir & filename 
                    Dim probablePath As String = Path.Combine(GetDirectoryPath("mods"), modFile)
                    If File.Exists(probablePath) Then absoluteModPaths.Add(probablePath)

                Next

            Catch ex As Exception
                WriteToLog(Date.Now & " - Error in 'ConvertModPath_RelativeToAbsolute()'. Exception : " & ex.ToString)
            End Try

            Return absoluteModPaths

        End Function


        Private Sub FillTextBox(tbx As TextBox, content As String)

            Try
                With tbx
                    .Text = content
                    .FontStyle = FontStyles.Normal
                    .Foreground = Brushes.Black
                End With

            Catch ex As Exception
                WriteToLog(Date.Now & " - Error in 'FillTextBox()'. Exception : " & ex.ToString)
            End Try

        End Sub

        Private Sub UnfillTextBox(tbx As TextBox, content As String)

            Try
                With tbx
                    .Text = content 'TODO? Use constants
                    .FontStyle = FontStyles.Italic
                    .Foreground = Brushes.Gray
                End With

            Catch ex As Exception
                WriteToLog(Date.Now & " - Error in 'UnfillTextBox()'. Exception : " & ex.ToString)
            End Try

        End Sub



        Private Sub FillRichTextBox_Command(content As String)

            Try
                Dim flow As New FlowDocument()
                Dim para As New Paragraph()
                para.Inlines.Add(content)
                flow.Blocks.Add(para)

                RichTextBox_Command.Document = flow

            Catch ex As Exception
                WriteToLog(Date.Now & " - Error in 'FillRichTextBox()'. Exception : " & ex.ToString)
            End Try

        End Sub



        Private Sub Button_Options_ToggleView_Click(sender As Object, e As RoutedEventArgs)

            Try
                If Grid_Summary.Visibility = Visibility.Visible Then
                    Grid_Summary.Visibility = Visibility.Collapsed
                    Grid_Command.Visibility = Visibility.Visible
                Else
                    Grid_Summary.Visibility = Visibility.Visible
                    Grid_Command.Visibility = Visibility.Collapsed
                End If

            Catch ex As Exception
                WriteToLog(Date.Now & " - Error in 'Button_Options_ToggleView_Click()'. Exception : " & ex.ToString)
            End Try

        End Sub



        Private Sub Button_Options_LaunchSave_Click(sender As Object, e As RoutedEventArgs)

            Try
                If ReadyToLaunch() Then
                    LaunchGame()
                    SaveSettings()
                End If

            Catch ex As Exception
                WriteToLog(Date.Now & " - Error in 'Button_Options_LaunchSave_Click()'. Exception : " & ex.ToString)
            End Try

        End Sub

        Private Function ReadyToLaunch() As Boolean

            Try
                If ReturnSelectedPort() = Nothing Then
                    MessageBox.Show("Error : you need to define a Port")
                    Return False
                End If

                'If ReturnSelectedLevels() Is Nothing Then
                If TextBox_Summary_Iwad.Text = Nothing Then
                    MessageBox.Show("Error : you need to choose Levels")
                    Return False
                End If

                Return True

            Catch ex As Exception
                WriteToLog(Date.Now & " - Error in 'ReadyToLaunch()'. Exception : " & ex.ToString)
                Return False
            End Try

        End Function

        Private Sub LaunchGame()

            Try
                Dim rtbText As String = New TextRange(RichTextBox_Command.Document.ContentStart, RichTextBox_Command.Document.ContentEnd).Text
                Dim command As String = String.Format("/c start """" {0}", rtbText)
                LaunchProcessV3(command)

            Catch ex As Exception
                WriteToLog(Date.Now & " - Error in 'LaunchGame()'. Exception : " & ex.ToString)
            End Try

        End Sub

        Private Sub SaveSettings()

            Try
                With My.Settings
                    'TODO? Convert to absolute path ?

                    .SelectedPort = ReturnSelectedPort()
                    .SelectedIwad = ReturnSelectedLevels().Iwad
                    .SelectedLevel = ReturnSelectedLevels().Level
                    .SelectedMisc = ReturnSelectedLevels().Misc

                    If ReturnSelectedMods() IsNot Nothing Then
                        .FilesMods = New Specialized.StringCollection
                        .FilesMods.AddRange(ReturnSelectedMods().Files.ToArray)
                    End If

                    .Save()
                End With

            Catch ex As Exception
                WriteToLog(Date.Now & " - Error in 'SaveSettings()'. Exception : " & ex.ToString)
            End Try

        End Sub

        Private Sub LoadSettings()

            Try
                If Not My.Settings.SelectedPort = Nothing Then FillTextBox(TextBox_Port, My.Settings.SelectedPort)
                UpdateSummary(True)
                UpdateCommand(True)
                DecorateCommand()

            Catch ex As Exception
                WriteToLog(Date.Now & " - Error in 'LoadSettings()'. Exception : " & ex.ToString)
            End Try

        End Sub

        Private Function ReturnSelectedPort() As String

            Dim portPath As String = Nothing

            Try
                Dim placeholder As String = "Drop Doom port .exe file here... (GZDoom, Zandronum, etc.)"
                If Not TextBox_Port.Text = placeholder Then portPath = TextBox_Port.Text

            Catch ex As Exception
                WriteToLog(Date.Now & " - Error in 'ReturnSelectedPort()'. Exception : " & ex.ToString)
            End Try

            Return portPath

        End Function


        Private Sub Button_TestMySettings_Check_Click(sender As Object, e As RoutedEventArgs)

            With My.Settings

                Dim sb As New StringBuilder()

                sb.AppendLine("RootDir :")
                sb.AppendLine($"{GetDirectoryPath("")}") 'Empty string as param ?
                sb.AppendLine()
                sb.AppendLine("Iwads subdir :")
                sb.AppendLine($"{GetDirectoryPath("iwads")}")
                sb.AppendLine()
                sb.AppendLine("Levels subdir :")
                sb.AppendLine($"{GetDirectoryPath("levels")}")
                sb.AppendLine()
                sb.AppendLine("Misc subdir :")
                sb.AppendLine($"{GetDirectoryPath("misc")}")
                sb.AppendLine()
                sb.AppendLine("Mods subdir :")
                sb.AppendLine($"{GetDirectoryPath("mods")}")
                sb.AppendLine()
                sb.AppendLine()
                sb.AppendLine(".SelectedPort :")
                sb.AppendLine($"{ .SelectedPort}")
                sb.AppendLine()
                sb.AppendLine(".SelectedIwad:")
                sb.AppendLine($"{ .SelectedIwad}")
                sb.AppendLine()
                sb.AppendLine(".SelectedLevel :")
                sb.AppendLine($"{ .SelectedLevel}")
                sb.AppendLine()
                sb.AppendLine(".SelectedMisc :")
                sb.AppendLine($"{ .SelectedMisc}")
                sb.AppendLine()
                sb.AppendLine(".FilesMods :")
                For Each file As String In .FilesMods
                    sb.AppendLine($"{file}")
                Next

                MessageBox.Show(sb.ToString)

            End With

        End Sub

        Private Sub Button_TestMySettings_Reset_Click(sender As Object, e As RoutedEventArgs)

            My.Settings.Reset()

        End Sub

        Private Sub Button_Options_HelpAbout_Click(sender As Object, e As RoutedEventArgs)

            Try
                Dim mainWindow As MainWindow = Windows.Application.Current.Windows(0)
                Dim helpWindow As New HelpWindow() With {.Owner = mainWindow}
                helpWindow.ShowDialog()

            Catch ex As Exception
                WriteToLog(Date.Now & " - Error in 'Button_Menu_Help_Click()'. Exception : " & ex.ToString)
            End Try

        End Sub

#End Region


    End Class
End Namespace

