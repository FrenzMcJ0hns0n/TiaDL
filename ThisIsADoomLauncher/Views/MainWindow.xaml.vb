Imports System.IO
Imports System.Text
Imports System.Text.RegularExpressions
Imports ThisIsADoomLauncher.Models

Namespace Views
    Public Class MainWindow


#Region "Constants"
        Private Const TBX_SELECT_PORT As String = "Drop port executable... (GZDoom, Zandronum, etc.)"
        Private Const TBX_SELECT_IWAD As String = "Drop IWAD file... (Doom, Doom2, Freedoom, etc.)"
        Private Const TBX_SELECT_LEVEL As String = "Drop Level file... (.wad/.pk3)" '+ /.zip ? TODO: Rename LEVEL into MAPS eventually
        Private Const TBX_SELECT_MISC As String = "Drop extra file... (.deh/.bex/.txt)"
        Private Const TBX_SELECT_PICT As String = "Drop picture file... (.jpg/.png)" '+ others ?

        Private Const ERR_MISSING_INPUT As String = "Error : missing input data"
        Private Const ERR_MISSING_PORT As String = "You have to define a port to run Doom"
        Private Const ERR_MISSING_IWAD As String = "You need an Iwad as game base content"

#End Region


#Region "Startup"

        Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
            Title = GetFormattedAppTitle()

            Try
                CheckProjectSubdirectories()
                LoadSettings()

                'SetIniFiles() 'TODO? V3
                PopulateBaseLevelPresets()
                PopulateBaseModsPresets()

                'Performance eval
                Dim dateTimeReady As Date = Date.Now
                Dim timeSpan As TimeSpan = dateTimeReady.Subtract(My.Settings.DateTimeAtLaunch)
                WriteToLog(Date.Now & " - Time elapsed from Launch to Ready : " & timeSpan.Milliseconds & " milliseconds")
            Catch ex As Exception
                WriteToLog(Date.Now & " - Error in 'MainWindow:Window_Loaded()'. Exception : " & ex.ToString)
            End Try

        End Sub

        Private Sub PopulateBaseLevelPresets()
            Try
                ListView_Levels_BasePresets.ItemsSource = GetLevelPresets_FromCsv("base_levels")
            Catch ex As Exception
                WriteToLog(Date.Now & " - Error in 'SetCommonPresets()'. Exception : " & ex.ToString)
            End Try
        End Sub

        Private Sub PopulateBaseModsPresets()
            Try
                ListView_Mods_BasePresets.ItemsSource = GetModPresets_FromCsv("base_mods")
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


#Region "To (re?)implement"

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


#Region "Events : Port"

        Private Sub TextBox_Port_PreviewDragOver(sender As Object, e As DragEventArgs)
            e.Handled = True
        End Sub

        Private Sub TextBox_Port_Drop(sender As Object, e As DragEventArgs)
            Try
                Dim portFilepath As String = e.Data.GetData(DataFormats.FileDrop)(0)

                If ValidateFile(portFilepath, "Port") Then
                    FillTextBox(TextBox_Port, portFilepath)
                    FillTextBox(TextBox_Summary_Port, portFilepath)
                    UpdateCommand()
                    DecorateCommand()
                End If
            Catch ex As Exception
                WriteToLog(Date.Now & " - Error in 'TextBox_Port_Drop()'. Exception : " & ex.ToString)
            End Try
        End Sub

        'TODO
        'Implement button "Browse..."

        Private Sub Button_Port_Clear_Click(sender As Object, e As RoutedEventArgs)
            UnfillTextBox(TextBox_Port, TBX_SELECT_PORT)
            UnfillTextBox(TextBox_Summary_Port, String.Empty)
            UpdateCommand()
            DecorateCommand()
        End Sub

        'TODO
        'Implement edition of Port parameters

#End Region


#Region "Events : Levels"

        'NOT READY
        Private Sub GroupBox_Levels_PreviewDragOver(sender As Object, e As DragEventArgs)
            e.Handled = True
        End Sub

        'NOT READY
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



        Private Sub ListView_Levels_BasePresets_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
            Try
                Dim lp As LevelPreset = ReturnSelectedLevels()

                TextBox_Summary_Iwad.Text = GetFileAbsolutePath("iwads", lp.Iwad)
                UpdateLevelAndMisc_Summary(New List(Of String) From {lp.Level, lp.Misc})

                UpdateCommand()
                DecorateCommand()
            Catch ex As Exception
                WriteToLog(Date.Now & " - Error in 'ListView_Mods_BasePresets_SelectionChanged()'. Exception : " & ex.ToString)
            End Try
        End Sub

        Private Sub ListView_Mods_BasePresets_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
            Try
                Dim mp As ModPreset = ReturnSelectedMods()

                TextBlock_Mods_Desc.Text = mp.Desc
                ListView_Mods_Files.ItemsSource = mp.Files
                UpdateMods_Summary(mp.Files)

                UpdateCommand()
                DecorateCommand()
            Catch ex As Exception
                WriteToLog(Date.Now & " - Error in 'ListView_Mods_BasePresets_SelectionChanged()'. Exception : " & ex.ToString)
            End Try
        End Sub



        Private Sub ListView_Levels_UserPresets_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
            'TODO
        End Sub

        Private Sub TextBox_NewLevel_Iwad_PreviewDragOver(sender As Object, e As DragEventArgs)
            e.Handled = True
        End Sub

        Private Sub TextBox_NewLevel_Iwad_Drop(sender As Object, e As DragEventArgs)
            Dim droppedFile As String = e.Data.GetData(DataFormats.FileDrop)(0)
            If ValidateFile(droppedFile, "Iwad") Then FillTextBox(sender, droppedFile)
        End Sub

        Private Sub Button_NewLevel_Iwad_Clear_Click(sender As Object, e As RoutedEventArgs)
            UnfillTextBox(TextBox_NewLevel_Iwad, TBX_SELECT_IWAD)
        End Sub

        Private Sub TextBox_NewLevel_Level_PreviewDragOver(sender As Object, e As DragEventArgs)
            e.Handled = True
        End Sub

        Private Sub TextBox_NewLevel_Level_Drop(sender As Object, e As DragEventArgs)
            Dim droppedFile As String = e.Data.GetData(DataFormats.FileDrop)(0)
            If ValidateFile(droppedFile, "Level") Then FillTextBox(sender, droppedFile)
        End Sub

        Private Sub Button_NewLevel_Level_Clear_Click(sender As Object, e As RoutedEventArgs)
            UnfillTextBox(TextBox_NewLevel_Level, TBX_SELECT_LEVEL)
        End Sub

        Private Sub TextBox_NewLevel_Misc_PreviewDragOver(sender As Object, e As DragEventArgs)
            e.Handled = True
        End Sub

        Private Sub TextBox_NewLevel_Misc_Drop(sender As Object, e As DragEventArgs)
            Dim droppedFile As String = e.Data.GetData(DataFormats.FileDrop)(0)
            If ValidateFile(droppedFile, "Misc") Then FillTextBox(sender, droppedFile)
        End Sub

        Private Sub Button_NewLevel_Misc_Clear_Click(sender As Object, e As RoutedEventArgs)
            UnfillTextBox(TextBox_NewLevel_Misc, TBX_SELECT_MISC)
        End Sub

        Private Sub TextBox_NewLevel_Image_PreviewDragOver(sender As Object, e As DragEventArgs)
            e.Handled = True
        End Sub

        Private Sub TextBox_NewLevel_Image_Drop(sender As Object, e As DragEventArgs)
            Dim droppedFile As String = e.Data.GetData(DataFormats.FileDrop)(0)
            If ValidateFile(droppedFile, "Image") Then FillTextBox(sender, droppedFile)
        End Sub

        Private Sub Button_NewLevel_Image_Clear_Click(sender As Object, e As RoutedEventArgs)
            UnfillTextBox(TextBox_NewLevel_Image, TBX_SELECT_PICT)
        End Sub

        Private Sub Button_NewLevel_Try_Click(sender As Object, e As RoutedEventArgs)
            'TODO
        End Sub

        Private Sub Button_NewLevel_SaveAs_Click(sender As Object, e As RoutedEventArgs)
            'TODO
        End Sub

        Private Sub Button_NewLevel_ClearAll_Click(sender As Object, e As RoutedEventArgs)
            UnfillTextBox(TextBox_NewLevel_Iwad, TBX_SELECT_IWAD)
            UnfillTextBox(TextBox_NewLevel_Level, TBX_SELECT_LEVEL)
            UnfillTextBox(TextBox_NewLevel_Misc, TBX_SELECT_MISC)
        End Sub

#End Region


#Region "Actions : Sidebar"

        Private Sub Button_Options_HelpAbout_Click(sender As Object, e As RoutedEventArgs)
            Try
                Dim mainWindow As MainWindow = Windows.Application.Current.Windows(0)
                Dim helpWindow As New HelpWindow() With {.Owner = mainWindow}
                helpWindow.ShowDialog()

            Catch ex As Exception
                WriteToLog(Date.Now & " - Error in 'Button_Menu_Help_Click()'. Exception : " & ex.ToString)
            End Try
        End Sub

        Private Sub Button_Options_OpenRootDir_Click(sender As Object, e As RoutedEventArgs)
            Process.Start(GetDirectoryPath())
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
                    'LaunchGame()
                    SaveSettings()
                End If
            Catch ex As Exception
                WriteToLog(Date.Now & " - Error in 'Button_Options_LaunchSave_Click()'. Exception : " & ex.ToString)
            End Try
        End Sub

#End Region


#Region "GUI operations & helpers"

        Private Function CreateFileModsTbx(filepath As String, type As String) As TextBox
            Dim fi As New FileInfo(filepath)
            Dim Color As SolidColorBrush = IIf(type = "Level",
                                               Brushes.White,
                                               Brushes.LightGray)
            Return New TextBox() With
            {
                .Background = Color,
                .Cursor = Cursors.Arrow,
                .IsReadOnly = True,
                .Margin = New Thickness(0, 0, 6, 0),
                .Text = fi.Name,
                .ToolTip = New StringBuilder(
                    $"File type : {type}" & vbCrLf &
                    $"Directory : {fi.DirectoryName}"
                )
            }
        End Function

        'TODO(later): Gather two Sub into one, with 2 List(Of String) as input parameters
        Private Sub UpdateLevelAndMisc_Summary(fileNames As List(Of String))
            Try
                If fileNames.Count = 0 Then Return 'TODO: Determine if necessary

                'Get all TextBoxes from the StackPanel
                Dim allTbxs As List(Of TextBox) = StackPanel_Summary_FilesMods.Children.OfType(Of TextBox).ToList

                'Get the mods TextBoxes (the ones with the LightGray background)
                Dim modTbxs As List(Of TextBox) = allTbxs.Where(Function(tbx) tbx.Background Is Brushes.LightGray).ToList

                'Create the fileMods TextBoxes
                Dim fileModsTbx As New List(Of TextBox)
                For Each name As String In fileNames
                    If name = String.Empty Then Continue For
                    Dim filepath As String = If(File.Exists(name), name, GetFileAbsolutePath("", name)) 'Fullpath is required
                    fileModsTbx.Add(CreateFileModsTbx(filepath, "Level"))
                Next

                'Clean the StackPanel content and add FileMods TextBoxes
                StackPanel_Summary_FilesMods.Children.Clear()
                fileModsTbx.ForEach(Sub(tbx) StackPanel_Summary_FilesMods.Children.Add(tbx))
                modTbxs.ForEach(Sub(tbx) StackPanel_Summary_FilesMods.Children.Add(tbx))

            Catch ex As Exception
                WriteToLog(Date.Now & " - Error in 'DisplayLevels_Summary()'. Exception : " & ex.ToString)
            End Try
        End Sub

        'TODO? Rewrite, as only mods need to be edited in this Sub
        Private Sub UpdateMods_Summary(fileNames As List(Of String))
            Try
                If fileNames.Count = 0 Then Return 'TODO: Determine if necessary

                'Get all TextBoxes from the StackPanel
                Dim allTbxs As List(Of TextBox) = StackPanel_Summary_FilesMods.Children.OfType(Of TextBox).ToList

                'Get the Level&Misc TextBoxes (the ones with the White background)
                Dim lvlTbxs As List(Of TextBox) = allTbxs.Where(Function(tbx) tbx.Background Is Brushes.White).ToList

                'Create the fileMods TextBoxes
                Dim fileModsTbx As New List(Of TextBox)
                For Each name As String In fileNames
                    If name = String.Empty Then Continue For
                    Dim filepath As String = If(File.Exists(name), name, GetFileAbsolutePath("", name)) 'Fullpath is required
                    fileModsTbx.Add(CreateFileModsTbx(filepath, "Mod"))
                Next

                'Clean the StackPanel content and add FileMods TextBoxes
                StackPanel_Summary_FilesMods.Children.Clear()
                lvlTbxs.ForEach(Sub(tbx) StackPanel_Summary_FilesMods.Children.Add(tbx))
                fileModsTbx.ForEach(Sub(tbx) StackPanel_Summary_FilesMods.Children.Add(tbx))

            Catch ex As Exception
                WriteToLog(Date.Now & " - Error in 'DisplayMods_Summary()'. Exception : " & ex.ToString)
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



        'NOT READY
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

                        If ValidateFile(path, "Iwad") Then
                            orderedFiles.Add(path)
                            Continue For
                        End If

                        If ValidateFile(path, "Level") And orderedFiles.Count > 0 Then
                            orderedFiles.Add(path)
                            Continue For
                        End If

                        If ValidateFile(path, "Misc") And orderedFiles.Count > 1 Then
                            orderedFiles.Add(path)
                            Continue For
                        End If

                        If ValidateFile(path, "Image") And orderedFiles.Count > 2 Then
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



        Private Function ReturnSelectedLevels() As LevelPreset
            Dim preset As LevelPreset = Nothing

            Try
                Select Case TabControl_Levels.SelectedIndex
                    Case 0
                        preset = CType(ListView_Levels_BasePresets.SelectedItem, LevelPreset)
                    Case 1
                        preset = CType(ListView_Levels_UserPresets.SelectedItem, LevelPreset) 'TODO
                    Case 2
                        preset = New LevelPreset() With {.Iwad = "", .Level = "", .Misc = "", .ImagePath = ""} 'TODO
                End Select
            Catch ex As Exception
                WriteToLog(Date.Now & " - Error in 'ReturnSelectedLevels()'. Exception : " & ex.ToString)
            End Try

            Return preset
        End Function

        Private Function ReturnSelectedMods() As ModPreset
            Dim preset As ModPreset = Nothing

            Try
                Select Case TabControl_Mods.SelectedIndex
                    Case 0
                        preset = CType(ListView_Mods_BasePresets.SelectedItem, ModPreset)
                    Case 1
                        preset = CType(ListView_Mods_BasePresets.SelectedItem, ModPreset) 'TODO
                    Case 2
                        preset = New ModPreset() With {.Files = New List(Of String)} 'TODO
                    Case Else
                        'TODO?
                End Select
            Catch ex As Exception
                WriteToLog(Date.Now & " - Error in 'ReturnSelectedMods()'. Exception : " & ex.ToString)
            End Try

            Return preset
        End Function

        Private Function ReturnSelectedPort() As String
            Dim portPath As String = String.Empty

            Try
                If Not TextBox_Port.Text = TBX_SELECT_PORT Then portPath = TextBox_Port.Text
            Catch ex As Exception
                WriteToLog(Date.Now & " - Error in 'ReturnSelectedPort()'. Exception : " & ex.ToString)
            End Try

            Return portPath
        End Function



        'Summary : Command line view
        Private Sub UpdateCommand()
            Try
                Dim port As String = TextBox_Summary_Port.Text
                Dim iwad As String = TextBox_Summary_Iwad.Text

                If String.IsNullOrWhiteSpace(port) OrElse String.IsNullOrWhiteSpace(iwad) Then
                    RichTextBox_Command.Document.Blocks.Clear()
                    Return
                End If

                Dim command As String = $"""{port}"" -iwad ""{iwad}"""
                'TODO: Manage port parameters, to be added before " -iwad"

                'Handle Levels/Misc/Mods
                Dim tbxs As List(Of TextBox) = StackPanel_Summary_FilesMods.Children.OfType(Of TextBox).ToList
                For Each tbx As TextBox In tbxs
                    command &= $" -file ""{ExtractFileFullPath(tbx)}"""
                Next

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



        Private Function ReadyToLaunch() As Boolean
            Try
                If String.IsNullOrEmpty(TextBox_Summary_Port.Text) Then
                    MessageBox.Show(ERR_MISSING_PORT, ERR_MISSING_INPUT, MessageBoxButton.OK, MessageBoxImage.Error)
                    Return False
                End If

                If String.IsNullOrEmpty(TextBox_Summary_Iwad.Text) Then
                    MessageBox.Show(ERR_MISSING_IWAD, ERR_MISSING_INPUT, MessageBoxButton.OK, MessageBoxImage.Error)
                    Return False
                End If

            Catch ex As Exception
                WriteToLog(Date.Now & " - Error in 'ReadyToLaunch()'. Exception : " & ex.ToString)
                Return False
            End Try

            Return True
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

        ''' <summary>
        ''' Retrieve full path of files "Level" or "Misc", using the TextBox tooltip.
        ''' Maybe not the best way, but OK for now...
        ''' </summary>
        ''' <param name="type">Target : "Level" or "Misc"</param>
        ''' <returns></returns>
        Private Function GetLvlsMiscFullPath(type As String) As String
            Dim fullPath As String = String.Empty

            Try
                Dim allTbxs As List(Of TextBox) = StackPanel_Summary_FilesMods.Children.OfType(Of TextBox).ToList

                'Get the Level&Misc TextBoxes (the ones with the White background)
                Dim lvlTbxs As List(Of TextBox) = allTbxs.Where(Function(tbx) tbx.Background Is Brushes.White).ToList

                If lvlTbxs.Count > 0 And type = "Level" Then
                    fullPath = ExtractFileFullPath(lvlTbxs(0))
                End If

                If lvlTbxs.Count > 1 And type = "Misc" Then
                    fullPath = ExtractFileFullPath(lvlTbxs(1))
                End If

            Catch ex As Exception
                WriteToLog(Date.Now & " - Error in 'GetLvlsMiscFullPath()'. Exception : " & ex.ToString)
            End Try

            Return fullPath
        End Function

        Private Function ExtractFileFullPath(tbx As TextBox) As String
            Dim directoryPath As String = tbx.ToolTip.ToString.Split(vbLf)(1).Replace("Directory : ", "")
            Dim filename As String = tbx.Text

            Return Path.Combine(directoryPath, filename)
        End Function

        Private Function GetModsFullPath() As List(Of String)
            Dim fullPaths As New List(Of String)

            Try
                Dim allTbxs As List(Of TextBox) = StackPanel_Summary_FilesMods.Children.OfType(Of TextBox).ToList

                'Get the mods TextBoxes (the ones with the LightGray background)
                Dim modTbxs As List(Of TextBox) = allTbxs.Where(Function(tbx) tbx.Background Is Brushes.LightGray).ToList

                For Each tbx In modTbxs
                    fullPaths.Add(ExtractFileFullPath(tbx))
                Next

            Catch ex As Exception
                WriteToLog(Date.Now & " - Error in 'GetLevelOrMiscFullPath()'. Exception : " & ex.ToString)
            End Try

            Return fullPaths
        End Function

        ''' <summary>
        ''' Save "Settings" (currently selected contents in Summary) to JSON
        ''' </summary>
        Private Sub SaveSettings()
            Try
                Dim lastLaunched As New Setting With
                {
                    .Port = TextBox_Summary_Port.Text,
                    .Iwad = TextBox_Summary_Iwad.Text,
                    .Level = GetLvlsMiscFullPath("Level"),
                    .Misc = GetLvlsMiscFullPath("Misc"),
                    .Mods = GetModsFullPath(),
                    .PortParameters = New List(Of String)
                }
                SaveToJsonData(lastLaunched)
            Catch ex As Exception
                WriteToLog(Date.Now & " - Error in 'SaveSettings()'. Exception : " & ex.ToString)
            End Try
        End Sub

        ''' <summary>
        ''' Load "Settings" (last launched contents) from JSON to update Summary views
        ''' </summary>
        Private Sub LoadSettings()
            Try
                Dim savedSettings As Setting = LoadFromJsonData()

                With savedSettings
                    If File.Exists(.Port) Then
                        FillTextBox(TextBox_Port, .Port)
                        FillTextBox(TextBox_Summary_Port, .Port)
                    End If
                    'TODO: Handle case of invalid .Port 
                    If File.Exists(Path.Combine(GetDirectoryPath("iwads"), .Iwad)) Then TextBox_Summary_Iwad.Text = .Iwad
                    'TODO: Handle case of invalid .Iwad 
                    If Not .Level = String.Empty Then UpdateLevelAndMisc_Summary(New List(Of String) From { .Level, .Misc})
                    If Not .Mods.Count = 0 Then UpdateMods_Summary(.Mods)
                End With

                UpdateCommand()
                DecorateCommand()
            Catch ex As Exception
                WriteToLog(Date.Now & " - Error in 'LoadSettings()'. Exception : " & ex.ToString)
            End Try
        End Sub

#End Region


    End Class
End Namespace

