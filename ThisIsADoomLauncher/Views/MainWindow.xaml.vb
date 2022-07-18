Imports System.IO
Imports System.Reflection
Imports System.Text
Imports System.Text.RegularExpressions
Imports Microsoft.Win32
Imports ThisIsADoomLauncher.Models

Namespace Views
    Public Class MainWindow


#Region "GUI related constants"
        Private Const TBX_SELECT_PORT As String = "Drop port executable... (GZDoom, Zandronum, etc.)"
        Private Const TBX_SELECT_IWAD As String = "Drop IWAD file... (Doom, Doom2, Freedoom, etc.)"
        Private Const TBX_SELECT_LEVEL As String = "Drop Level file... (.wad/.pk3)" '+ /.zip ? TODO: Rename LEVEL into MAPS eventually
        Private Const TBX_SELECT_MISC As String = "Drop extra file... (.deh/.bex/.txt)"
        Private Const TBX_SELECT_PICT As String = "Drop picture file... (.jpg/.png)" '+ others ?

        Private Const ERR_MISSING_INPUT As String = "Error : missing input data"
        Private Const ERR_MISSING_PORT As String = "You have to define a port to run Doom"
        Private Const ERR_MISSING_IWAD As String = "You need an Iwad as game base content"

        Private Const ERR_INVALID_INPUT As String = "Error : invalid input data"
        Private Const ERR_INPUT_NOT_FILE As String = "Submitted element was not a file"
#End Region


#Region "GUI related enums"

        Private Function GetActiveLvlTab() As LVLPRESET_TAB
            Select Case TabControl_Levels.SelectedIndex
                Case 0 : Return LVLPRESET_TAB.Base
                Case 1 : Return LVLPRESET_TAB.User
                Case 2 : Return LVLPRESET_TAB.AddNew
                Case Else : Return LVLPRESET_TAB.None
            End Select
        End Function

        Private Sub SetActiveLvlTab(value As LVLPRESET_TAB)
            TabControl_Levels.SelectedIndex = value
        End Sub

        Private Function GetActiveModTab() As MODPRESET_TAB
            Select Case TabControl_Mods.SelectedIndex
                Case 0 : Return MODPRESET_TAB.Base
                Case 1 : Return MODPRESET_TAB.User
                Case 2 : Return MODPRESET_TAB.AddNew
                Case Else : Return MODPRESET_TAB.None
            End Select
        End Function

        Private Sub SetActiveModTab(value As MODPRESET_TAB)
            TabControl_Mods.SelectedIndex = value
        End Sub

#End Region


#Region "Startup"

        Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
            Title = GetFormattedAppTitle()

            Try
                CheckProjectDirectories()
                LoadSettings()

                'SetIniFiles() 'TODO? V3
                PopulateBaseLevelPresets()
                PopulateBaseModsPresets()

                'Performance eval
                Dim dateTimeReady As Date = Date.Now
                Dim timeSpan As TimeSpan = dateTimeReady.Subtract(My.Settings.DateTimeAtLaunch)
                WriteToLog(Date.Now & " - Time elapsed from Launch to Ready : " & timeSpan.Milliseconds & " milliseconds")
            Catch ex As Exception
                Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
                WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}")
            End Try

        End Sub

        Private Sub PopulateBaseLevelPresets()
            Try
                ListView_Levels_BasePresets.ItemsSource = GetLevelPresets_FromCsv("base_levels")
            Catch ex As Exception
                Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
                WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}")
            End Try
        End Sub

        Private Sub PopulateBaseModsPresets()
            Try
                ListView_Mods_BasePresets.ItemsSource = GetModPresets_FromCsv("base_mods")
            Catch ex As Exception
                Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
                WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}")
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
                Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
                WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}")
            End Try
        End Sub

        Private Sub ExportCommandAsBat()
            Try
                Dim commandText = New TextRange(RichTextBox_Command.Document.ContentStart, RichTextBox_Command.Document.ContentEnd).Text

                Dim now_formatted As String = Date.Now.ToString("yyyy-MM-dd_HH-mm-ss")
                Dim batPath As String = Path.Combine(GetDirectoryPath(), now_formatted & "_command.bat") 'TODO : Change/Add RootDirPath variable

                Using writer As New StreamWriter(batPath, False, Encoding.Default)
                    writer.WriteLine("@echo off")
                    writer.WriteLine("start """" " & commandText)
                End Using
            Catch ex As Exception
                Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
                WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}")
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
                Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
                WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}")
            End Try
        End Sub

        Private Sub Button_Port_Browse_Click(sender As Object, e As RoutedEventArgs)
            Try
                Dim dialog As New OpenFileDialog With
                {
                    .Filter = "EXE file (*.exe)|*.exe",
                    .InitialDirectory = GetDirectoryPath(), 'TiaDL root directory
                    .Title = "Select a Doom port executable"
                }

                If dialog.ShowDialog() Then
                    FillTextBox(TextBox_Port, dialog.FileName)
                    FillTextBox(TextBox_Summary_Port, dialog.FileName)
                    UpdateCommand()
                    DecorateCommand()
                End If
            Catch ex As Exception
                Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
                WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}")
            End Try
        End Sub

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
                'TODO: Make sure dropped elements are files

                '2) Check & order files
                Dim confirmedFiles As List(Of String) = OrderDroppedFiles_Levels(filePaths)

                '------------- Template of confirmedFiles, depending on .Count :
                '-------------  1 = Iwad
                '-------------  2 = Iwad, Level
                '-------------  3 = Iwad, Level, Misc
                '-------------  4 = Iwad, Level, Misc, Image

                '3) Feed GUI if Count > 0
                If confirmedFiles.Count = 0 Then Return
                SetActiveLvlTab(LVLPRESET_TAB.AddNew)

                If confirmedFiles.Count > 0 Then FillTextBox(TextBox_NewLevel_Iwad, confirmedFiles(0))
                If confirmedFiles.Count > 1 Then FillTextBox(TextBox_NewLevel_Level, confirmedFiles(1))
                If confirmedFiles.Count > 2 Then FillTextBox(TextBox_NewLevel_Misc, confirmedFiles(2))
                If confirmedFiles.Count > 3 Then FillTextBox(TextBox_NewLevel_Image, confirmedFiles(3))
            Catch ex As Exception
                Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
                WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}")
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
                Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
                WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}")
            End Try
        End Sub



        Private Sub ListView_Levels_UserPresets_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
            'TODO
        End Sub



        Private Sub TextBox_NewLevel_PreviewDragOver(sender As Object, e As DragEventArgs)
            e.Handled = True
        End Sub

        Private Sub TextBox_NewLevel_Drop(sender As Object, e As DragEventArgs)
            Dim tbx As TextBox = sender

            Dim droppedFile As String = e.Data.GetData(DataFormats.FileDrop)(0)
            If Not File.Exists(droppedFile) Then
                MessageBox.Show(ERR_INPUT_NOT_FILE, ERR_INVALID_INPUT, MessageBoxButton.OK, MessageBoxImage.Error)
                Return
            End If

            Dim sourceTbx As String = tbx.Name.Split("_")(2) 'Iwad, Level, Misc, Image
            If ValidateFile(droppedFile, sourceTbx) Then FillTextBox(sender, droppedFile)
        End Sub

        Private Sub Button_NewLevel_Browse_Click(sender As Object, e As RoutedEventArgs)
            Dim btn As Button = sender

            Dim sourceBtn As String = btn.Name.Split("_")(2)
            Select Case sourceBtn
                Case "Iwad"
                    Dim dialog As New OpenFileDialog With
                    {
                        .Filter = "IWAD file (*.wad)|*.wad",
                        .InitialDirectory = GetDirectoryPath("iwads"),
                        .Title = "Select an IWAD as base content for the new preset"
                    }
                    If dialog.ShowDialog() Then FillTextBox(TextBox_NewLevel_Iwad, dialog.FileName)

                Case "Level"
                    Dim dialog As New OpenFileDialog With
                    {
                        .Filter = "Level file (*.wad;*.pk3)|*.wad;*.pk3", '.zip needs testing, never used such format for levels
                        .InitialDirectory = GetDirectoryPath("levels"),
                        .Title = "Select a Level file for the new preset"
                    }
                    If dialog.ShowDialog() Then FillTextBox(TextBox_NewLevel_Level, dialog.FileName)

                Case "Misc"
                    Dim dialog As New OpenFileDialog With
                    {
                        .Filter = "Misc. file (*.deh;*.bex;*.txt)|*.deh;*.bex;*.txt",
                        .InitialDirectory = GetDirectoryPath("misc"),
                        .Title = "Select a Misc file for the new preset"
                    }
                    If dialog.ShowDialog() Then FillTextBox(TextBox_NewLevel_Misc, dialog.FileName)

                Case "Image"
                    Dim dialog As New OpenFileDialog With
                    {
                        .Filter = "Image file (*.jpg;*.png)|*.jpg;*.png",
                        .InitialDirectory = GetDirectoryPath(), 'Or any other place in user computer?
                        .Title = "Select an image for the new preset"
                    }
                    If dialog.ShowDialog() Then FillTextBox(TextBox_NewLevel_Image, dialog.FileName)

            End Select
        End Sub

        Private Sub Button_NewLevel_Clear_Click(sender As Object, e As RoutedEventArgs)
            Dim btn As Button = sender

            'Restore default placeholder
            Dim sourceBtn As String = btn.Name.Split("_")(2)
            Select Case sourceBtn
                Case "Iwad" : UnfillTextBox(TextBox_NewLevel_Iwad, TBX_SELECT_IWAD)
                Case "Level" : UnfillTextBox(TextBox_NewLevel_Level, TBX_SELECT_LEVEL)
                Case "Misc" : UnfillTextBox(TextBox_NewLevel_Misc, TBX_SELECT_MISC)
                Case "Image" : UnfillTextBox(TextBox_NewLevel_Image, TBX_SELECT_PICT)
            End Select
        End Sub

        Private Sub Button_NewLevel_Try_Click(sender As Object, e As RoutedEventArgs)
            Dim iwadInput As String = TextBox_NewLevel_Iwad.Text
            Dim levelInput As String = TextBox_NewLevel_Level.Text
            Dim miscInput As String = TextBox_NewLevel_Misc.Text

            'At least 2 contents (including Iwad) are required
            If iwadInput = TBX_SELECT_IWAD Then Return
            If levelInput = TBX_SELECT_LEVEL And miscInput = TBX_SELECT_MISC Then
                MessageBox.Show("You only submitted an Iwad. Please select it from the ""Base presets"" tab instead", ERR_MISSING_INPUT, MessageBoxButton.OK, MessageBoxImage.Error)
                Return
            End If

            'Update Summary "Fields" view
            TextBox_Summary_Iwad.Text = iwadInput
            UpdateLevelAndMisc_Summary(New List(Of String) From {levelInput, miscInput})
            UpdateCommand()
            DecorateCommand()
        End Sub

        Private Sub Button_NewLevel_SaveAs_Click(sender As Object, e As RoutedEventArgs)
            'TODO
        End Sub

        Private Sub Button_NewLevel_ClearAll_Click(sender As Object, e As RoutedEventArgs)
            UnfillTextBox(TextBox_NewLevel_Iwad, TBX_SELECT_IWAD)
            UnfillTextBox(TextBox_NewLevel_Level, TBX_SELECT_LEVEL)
            UnfillTextBox(TextBox_NewLevel_Misc, TBX_SELECT_MISC)
            UnfillTextBox(TextBox_NewLevel_Image, TBX_SELECT_PICT)
        End Sub

#End Region


#Region "Events - Mods"

        Private Sub ListView_Mods_BasePresets_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
            Try
                Dim mp As ModPreset = ReturnSelectedMods()

                TextBlock_Mods_Desc.Text = mp.Desc
                ListView_Mods_Files.ItemsSource = mp.Files
                UpdateMods_Summary(mp.Files)

                UpdateCommand()
                DecorateCommand()
            Catch ex As Exception
                Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
                WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}")
            End Try
        End Sub


#End Region


#Region "Actions : Sidebar"

        Private Sub Button_Options_HelpAbout_Click(sender As Object, e As RoutedEventArgs)
            Try
                Dim mainWindow As MainWindow = Windows.Application.Current.Windows(0)
                Dim helpWindow As New HelpWindow() With {.Owner = mainWindow}
                helpWindow.ShowDialog()

            Catch ex As Exception
                Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
                WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}")
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
                Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
                WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}")
            End Try
        End Sub

        Private Sub Button_Options_LaunchSave_Click(sender As Object, e As RoutedEventArgs)
            Try
                If ReadyToLaunch() Then
                    'LaunchGame()
                    SaveSettings()
                End If
            Catch ex As Exception
                Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
                WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}")
            End Try
        End Sub

#End Region


#Region "GUI operations & helpers"

        Private Function CreateFileModsTbx(filepath As String, type As String) As TextBox
            Dim fi As New FileInfo(filepath)
            Dim Color As SolidColorBrush = IIf(type = "Level",
                                               Brushes.White,
                                               Brushes.LightGray)
            Dim sBuilder As New StringBuilder()
            sBuilder.AppendLine($"File type : {type}")
            sBuilder.Append($"Directory : {fi.DirectoryName}")

            Return New TextBox() With
            {
                .Background = Color,
                .Cursor = Cursors.Arrow,
                .IsReadOnly = True,
                .Margin = New Thickness(0, 0, 6, 0),
                .Text = fi.Name,
                .ToolTip = sBuilder.ToString
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
                Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
                Dim fileNamesJoined As String = String.Join(", ", fileNames)
                WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}{vbCrLf} Parameter(s) : {fileNamesJoined}")
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
                Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
                Dim fileNamesJoined As String = String.Join(", ", fileNames)
                WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}{vbCrLf} Parameter(s) : {fileNamesJoined}")
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
                Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
                WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}{vbCrLf} Parameter(s) : {content}")
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

                        If orderedFiles.Count > 0 AndAlso ValidateFile(path, "Level") Then
                            orderedFiles.Add(path)
                            Continue For
                        End If

                        If orderedFiles.Count > 1 AndAlso ValidateFile(path, "Misc") Then
                            orderedFiles.Add(path)
                            Continue For
                        End If

                        If orderedFiles.Count > 2 AndAlso ValidateFile(path, "Image") Then
                            orderedFiles.Add(path)
                            Continue For
                        End If

                    Next
                Next
            Catch ex As Exception
                Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
                Dim filePathsJoined As String = String.Join(", ", filePaths)
                WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}{vbCrLf} Parameter(s) : {filePathsJoined}")
            End Try

            Return orderedFiles
        End Function



        Private Function ReturnSelectedLevels() As LevelPreset
            Dim preset As LevelPreset = Nothing

            Try
                Select Case GetActiveLvlTab()
                    Case LVLPRESET_TAB.Base
                        preset = CType(ListView_Levels_BasePresets.SelectedItem, LevelPreset)

                    Case LVLPRESET_TAB.User
                        preset = CType(ListView_Levels_UserPresets.SelectedItem, LevelPreset) 'TODO

                    Case LVLPRESET_TAB.AddNew
                        preset = New LevelPreset() With {.Iwad = "", .Level = "", .Misc = "", .ImagePath = ""} 'TODO

                End Select
            Catch ex As Exception
                Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
                WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}")
            End Try

            Return preset
        End Function

        Private Function ReturnSelectedMods() As ModPreset
            Dim preset As ModPreset = Nothing

            Try
                Select Case GetActiveModTab()
                    Case MODPRESET_TAB.Base
                        preset = CType(ListView_Mods_BasePresets.SelectedItem, ModPreset)

                    Case MODPRESET_TAB.User
                        preset = CType(ListView_Mods_BasePresets.SelectedItem, ModPreset) 'TODO

                    Case MODPRESET_TAB.AddNew
                        preset = New ModPreset() With {.Files = New List(Of String)} 'TODO

                End Select
            Catch ex As Exception
                Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
                WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}")
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



        ''' <summary>
        ''' Update the "Command" Summary from the "Fields" Summary view
        ''' </summary>
        Private Sub UpdateCommand()
            Dim command As String = String.Empty
            Try
                Dim port As String = TextBox_Summary_Port.Text
                Dim iwad As String = TextBox_Summary_Iwad.Text

                If port = String.Empty Or iwad = String.Empty Then
                    RichTextBox_Command.Document.Blocks.Clear() 'TODO? Display text about Missing Port & Iwad ?
                    Return
                End If

                command &= $"""{port}"" -iwad ""{iwad}"""
                'TODO: Manage port parameters, to be added before " -iwad"

                'Add Level/Misc/Mods to the command line
                Dim allTbxs As List(Of TextBox) = StackPanel_Summary_FilesMods.Children.OfType(Of TextBox).ToList
                allTbxs.ForEach(Sub(tbx) command &= $" -file ""{ExtractFileFullPath(tbx)}""")

                FillRichTextBox_Command(command)
            Catch ex As Exception
                Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
                WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}{vbCrLf} Command : {command}")
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
                Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
                WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}")
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
                Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
                WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}")
            End Try
        End Sub

        Private Sub UnfillTextBox(tbx As TextBox, content As String)
            Try
                With tbx
                    .Text = content
                    .FontStyle = FontStyles.Italic
                    .Foreground = Brushes.Gray
                End With

            Catch ex As Exception
                Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
                WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}")
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
                Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
                WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}")
                Return False
            End Try

            Return True
        End Function

        Private Sub LaunchGame()
            Try
                Dim rtbText As String = New TextRange(RichTextBox_Command.Document.ContentStart, RichTextBox_Command.Document.ContentEnd).Text
                Dim cmdExe As New ProcessStartInfo("cmd.exe") With
                {
                    .UseShellExecute = False,
                    .CreateNoWindow = True,
                    .Arguments = $"/c start """" {rtbText}"
                }
                Process.Start(cmdExe)
            Catch ex As Exception
                Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
                WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}")
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
                Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
                WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}{vbCrLf} Parameter(s) : {type}")
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
                Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
                WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}")
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
                Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
                WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}")
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

                    If File.Exists(.Iwad) Then TextBox_Summary_Iwad.Text = .Iwad
                    'TODO: Handle case of invalid .Iwad

                    UpdateLevelAndMisc_Summary(New List(Of String) From { .Level, .Misc})
                    UpdateMods_Summary(.Mods)
                End With

                UpdateCommand()
                DecorateCommand()
            Catch ex As Exception
                Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
                WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}")
            End Try
        End Sub

#End Region


    End Class
End Namespace

