﻿Imports Microsoft.Win32
Imports System.Collections.ObjectModel
Imports System.IO
Imports System.Reflection
Imports System.Text
Imports System.Text.RegularExpressions
Imports ThisIsADoomLauncher.Models

Namespace Views
    Public Class MainWindow


#Region "GUI related Constants"
        Private Const TBX_DROP_PORT As String = "Drop port executable... (GZDoom, Zandronum, etc.)"
        Private Const TBX_DROP_IWAD As String = "Drop IWAD file... (Doom, Doom2, Freedoom, etc.)"
        Private Const TBX_DROP_MAPS As String = "Drop Maps file... (.wad/.pk3)" '+ /.zip ?
        Private Const TBX_DROP_MISC As String = "Drop Misc. file... (.deh/.bex/.txt)"
        Private Const TBX_DROP_PICT As String = "Drop Image file... (.jpg/.png)" '+ others ?

        Private Const TBX_SELECT_PORT As String = "Select a Doom port executable"
        Private Const TBX_SELECT_IWAD As String = "Select an Iwad file for the new preset"
        Private Const TBX_SELECT_MAPS As String = "Select a Maps file for the new preset"
        Private Const TBX_SELECT_MISC As String = "Select a Misc. file for the new preset"
        Private Const TBX_SELECT_PICT As String = "Select an Image file for the new preset"

        Private Const ERR_MISSING_INPUT As String = "Error : missing input data"
        Private Const ERR_MISSING_PORT As String = "You have to define a port to run Doom"
        Private Const ERR_MISSING_IWAD As String = "You need an Iwad as game base content"
        Private Const ERR_ONLY_IWAD As String = "You only submitted an ""Iwad"" file: at least a ""Maps"" file is required as well"

        Private Const ERR_INVALID_INPUT As String = "Error : invalid input data"
        Private Const ERR_INPUT_NOT_FILE As String = "Submitted element was not a file"
#End Region


#Region "GUI related Enums"

        Private Function GetActiveLvlTab() As LVLPRESET_TAB
            Select Case Tbc_Levels.SelectedIndex
                Case 0 : Return LVLPRESET_TAB.Base
                Case 1 : Return LVLPRESET_TAB.User
                Case 2 : Return LVLPRESET_TAB.AddNew
                Case Else : Return LVLPRESET_TAB.None
            End Select
        End Function

        Private Sub SetActiveLvlTab(value As LVLPRESET_TAB)
            Tbc_Levels.SelectedIndex = value
        End Sub

        Private Function GetActiveModTab() As MODPRESET_TAB
            Select Case Tbc_Mods.SelectedIndex
                Case 0 : Return MODPRESET_TAB.Base
                Case 1 : Return MODPRESET_TAB.User
                Case 2 : Return MODPRESET_TAB.AddNew
                Case Else : Return MODPRESET_TAB.None
            End Select
        End Function

        Private Sub SetActiveModTab(value As MODPRESET_TAB)
            Tbc_Mods.SelectedIndex = value
        End Sub

#End Region


#Region "Startup"

        Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
            Title = GetFormattedAppTitle()

            Try
                CheckProjectDirectories()
                LoadSettings()

                'SetIniFiles() 'TODO? Implement later in v3+
                PopulateBaseLevelPresets()
                PopulateBaseModsPresets()

                'Performance eval 'TODO? Remove that
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
                Lvw_LevelsBasePresets.ItemsSource = GetLevelPresets_FromCsv("base_levels")
            Catch ex As Exception
                Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
                WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}")
            End Try
        End Sub

        Private Sub PopulateBaseModsPresets()
            Try
                Lvw_ModsBasePresets.ItemsSource = GetModPresets_FromCsv("base_mods")
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
                Dim commandText = New TextRange(Rtb_Command.Document.ContentStart, Rtb_Command.Document.ContentEnd).Text
                Clipboard.SetText(commandText)
            Catch ex As Exception
                Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
                WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}")
            End Try
        End Sub

        Private Sub ExportCommandAsBat()
            Try
                Dim commandText = New TextRange(Rtb_Command.Document.ContentStart, Rtb_Command.Document.ContentEnd).Text

                Dim now_formatted As String = Date.Now.ToString("yyyy-MM-dd_HH-mm-ss")
                Dim batPath As String = Path.Combine(GetDirectoryPath(), now_formatted & "_command.bat")

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

        Private Sub Tbx_Port_PreviewDragOver(sender As Object, e As DragEventArgs)
            e.Handled = True
        End Sub

        Private Sub Tbx_Port_Drop(sender As Object, e As DragEventArgs)
            Try
                'Accept files only
                If Not e.Data.GetDataPresent(DataFormats.FileDrop) Then Return

                Dim portFilepath As String = e.Data.GetData(DataFormats.FileDrop)(0)

                If ValidateFile(portFilepath, "Port") Then
                    FillTextBox(Tbx_Port, portFilepath)
                    FillTextBox(TextBox_Summary_Port, portFilepath)
                    UpdateCommand()
                    DecorateCommand()
                End If

            Catch ex As Exception
                Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
                WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}")
            End Try
        End Sub

        Private Sub Btn_PortBrowse_Click(sender As Object, e As RoutedEventArgs)
            Try
                Dim dialog As New OpenFileDialog With
                {
                    .Filter = "EXE file (*.exe)|*.exe",
                    .InitialDirectory = GetDirectoryPath("Port"),
                    .Title = TBX_SELECT_PORT
                }

                If dialog.ShowDialog() Then
                    FillTextBox(Tbx_Port, dialog.FileName)
                    FillTextBox(TextBox_Summary_Port, dialog.FileName)
                    UpdateCommand()
                    DecorateCommand()
                End If
            Catch ex As Exception
                Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
                WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}")
            End Try
        End Sub

        Private Sub Btn_PortClear_Click(sender As Object, e As RoutedEventArgs)
            UnfillTextBox(Tbx_Port, TBX_DROP_PORT)
            UnfillTextBox(TextBox_Summary_Port, String.Empty)
            UpdateCommand()
            DecorateCommand()
        End Sub

        Private Sub Btn_PortParamsEdit_Click(sender As Object, e As RoutedEventArgs)
            Dim portParamsWindow As New PortParamsWindow() With {.Owner = Me, .OldValuesDict = GetPortParamsFromStackPanel()}
            portParamsWindow.ShowDialog()

            If portParamsWindow.UserValidation Then
                Dim oldParamsDict As Dictionary(Of String, String) = portParamsWindow.OldValuesDict
                Dim newParamsDict As Dictionary(Of String, String) = If(portParamsWindow.NewValuesDict, New Dictionary(Of String, String))
                Dim bothIdentical As Boolean = oldParamsDict.Count = newParamsDict.Count AndAlso Not oldParamsDict.Except(newParamsDict).Any()
                If Not bothIdentical Then
                    UpdatePortParams(newParamsDict)
                    UpdatePortParams_Summary(newParamsDict)
                End If
            End If

            UpdateCommand()
            DecorateCommand()
        End Sub

        ''' <summary>
        ''' Update the Port parameters in Port GUI area
        ''' </summary>
        ''' <param name="portParamsDict">Dictionary (Parameter:String, ParameterValue:String) </param>
        Private Sub UpdatePortParams(portParamsDict As Dictionary(Of String, String))
            If portParamsDict.Count > 0 Then
                Tbx_PortParameters.FontWeight = FontWeights.Bold

                Dim parametersList As New List(Of String)
                For Each kvp As KeyValuePair(Of String, String) In portParamsDict
                    parametersList.Add($"-{kvp.Key}" & If(kvp.Value.Length = 0, "", $" {kvp.Value}"))
                Next

                Tbx_PortParameters.ToolTip = String.Join(vbCrLf, parametersList)
            Else
                Tbx_PortParameters.ClearValue(TextBlock.FontWeightProperty)
                Tbx_PortParameters.ToolTip = Nothing
            End If

            Tbx_PortParameters.Text = $"+ {portParamsDict.Count} parameters"
        End Sub

#End Region


#Region "Events : Levels"

        Private Sub Tbc_Levels_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
            Stkp_BaseLevelsSorting.Visibility = If(GetActiveLvlTab() = LVLPRESET_TAB.Base, Visibility.Visible, Visibility.Hidden)

            Select Case GetActiveLvlTab()
                Case LVLPRESET_TAB.Base '...
                Case LVLPRESET_TAB.User : PopulateUserLevels() 'Refresh each time
                Case LVLPRESET_TAB.AddNew '...
            End Select

        End Sub

        Private Sub Cmbx_BaseLevelsSorting_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
            If Not GetActiveLvlTab() = LVLPRESET_TAB.Base Then Return 'Fix as Visibility.Visible is required.

            If Cmbx_BaseLevelsSorting.SelectedIndex > 0 Then
                'Sorting = Name/Type/Year
                Rbtn_SortAsc.IsEnabled = True
                Rbtn_SortDesc.IsEnabled = True

                'At launch, sorting is disabled, radio buttons are unchecked. Once sorting is enabled, Ascending becomes the default sorting order
                If Not Rbtn_SortAsc.IsChecked And Not Rbtn_SortDesc.IsChecked Then
                    Rbtn_SortAsc.IsChecked = True
                End If

                SortBaseLevels()
            Else
                'Sorting = None
                Rbtn_SortAsc.IsEnabled = False
                Rbtn_SortDesc.IsEnabled = False

                Lvw_LevelsBasePresets.ItemsSource = GetLevelPresets_FromCsv("base_levels")
            End If
        End Sub

        Private Sub ShowNoUserLevels()
            Lvw_LevelsUserPresets.Visibility = Visibility.Collapsed
            Lbl_LevelsNoUserPresets.Visibility = Visibility.Visible
        End Sub

        Private Sub SortBaseLevels()
            Dim sortCriterion As SortCriterion = Cmbx_BaseLevelsSorting.SelectedIndex
            Dim isAscending As Boolean = Rbtn_SortAsc.IsChecked

            With Lvw_LevelsBasePresets
                Dim currentLevelPresets As List(Of LevelPreset) = DirectCast(.ItemsSource, List(Of LevelPreset)) 'TODO: check with Compiler option Strict On

                .ItemsSource = SortLevelPresets(currentLevelPresets, sortCriterion, isAscending)
            End With
        End Sub

        Private Sub PopulateUserLevels()
            Try
                Dim jsonFilepath As String = GetJsonFilepath("UserLevels")
                If Not File.Exists(jsonFilepath) Then
                    MessageBox.Show("No local JSON file to load data from: use the ""Add new preset"" tab to create some", ERR_INVALID_INPUT, MessageBoxButton.OK, MessageBoxImage.Information)
                    Return 'Early return
                End If

                Dim jsonString As String = GetJsonData(jsonFilepath)

                If Not CanLoadJsonArray(jsonString) Then
                    MessageBox.Show("Unable to read user level presets from local JSON file", ERR_INVALID_INPUT, MessageBoxButton.OK, MessageBoxImage.Error)
                    ShowNoUserLevels()
                    Return 'Early return
                End If

                Dim userLevels As List(Of LevelPreset) = LoadUserLevels(jsonString)
                If userLevels.Count = 0 Then
                    ShowNoUserLevels()
                    Return 'Early return
                End If

                'Display
                With Lvw_LevelsUserPresets
                    .Visibility = Visibility.Visible
                    .ItemsSource = userLevels
                End With

            Catch ex As Exception
                Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
                WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}")

                ShowNoUserLevels()
            End Try
        End Sub

        Private Sub Lvw_LevelsBasePresets_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
            Try
                Dim lp As LevelPreset = ReturnSelectedLevels()

                TextBox_Summary_Iwad.Text = GetFileAbsolutePath("Iwad", lp.Iwad)
                UpdateLevels_Summary(lp.Maps, lp.Misc)
                UpdateCommand()
                DecorateCommand()

                e.Handled = True 'Prevent escalating up to "parent" event Tbc_Levels_SelectionChanged()

            Catch ex As Exception
                Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
                WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}")
            End Try
        End Sub

        'TODO? Factorize in common function?

        Private Sub Lvw_LevelsUserPresets_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
            Try
                'Safety fix. SelectedItem is not kept accross tabs, as the content of this one is updated everytime
                'TODO? Improve
                If Lvw_LevelsUserPresets.SelectedItem Is Nothing Then Return

                Dim lp As LevelPreset = ReturnSelectedLevels()

                TextBox_Summary_Iwad.Text = GetFileAbsolutePath("Iwad", lp.Iwad)
                UpdateLevels_Summary(lp.Maps, lp.Misc)
                UpdateCommand()
                DecorateCommand()

                e.Handled = True 'Prevent escalating up to "parent" event Tbc_Levels_SelectionChanged()

            Catch ex As Exception
                Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
                WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}")
            End Try
        End Sub

        'TODO? Generate window with XAML
        Private Sub MenuItemLevelView_Click(sender As Object, e As RoutedEventArgs)
            Try
                Dim preset As LevelPreset = Lvw_LevelsUserPresets.SelectedItem

                Dim myTextBlock As New TextBlock With {.Margin = New Thickness(10)}
                For Each pi As PropertyInfo In preset.GetType().GetProperties()
                    myTextBlock.Inlines.Add(New Bold(New Run($"{pi.Name}")))
                    myTextBlock.Inlines.Add(New Run($" : {pi.GetValue(preset)}{vbCrLf}"))
                Next

                Dim myWindow As New Window With
                {
                    .Content = myTextBlock,
                    .Height = 200,
                    .Owner = Me,
                    .Title = "User level preset",
                    .Width = 600,
                    .WindowStartupLocation = WindowStartupLocation.CenterOwner
                }
                myWindow.ShowDialog()

            Catch ex As Exception
                Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
                WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}")
            End Try
        End Sub

        Private Sub MenuItemLevelDelete_Click(sender As Object, e As RoutedEventArgs)
            Try
                Dim preset As LevelPreset = Lvw_LevelsUserPresets.SelectedItem

                If MessageBox.Show($"Delete preset ""{preset.Name}"" ?", "Delete preset", MessageBoxButton.YesNo, MessageBoxImage.Question) = Forms.DialogResult.Yes Then
                    'TODO: Secure I/O
                    Dim jsonFilepath As String = GetJsonFilepath("UserLevels")
                    Dim jsonString As String = GetJsonData(jsonFilepath)
                    Dim currentLevels As List(Of LevelPreset) = LoadUserLevels(jsonString) 'TODO? Load from ListView instead as we are here actually

                    Dim presetIdx As Integer = Lvw_LevelsUserPresets.Items.IndexOf(preset)
                    currentLevels.RemoveAt(presetIdx)

                    SaveUserLevels(currentLevels, jsonFilepath)
                    PopulateUserLevels()
                End If
            Catch ex As Exception
                Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
                WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}")
            End Try
        End Sub

        Private Sub Gbx_Levels_PreviewDragOver(sender As Object, e As DragEventArgs)
            e.Handled = True
        End Sub

        ''' <summary>
        ''' Handle multiple file drops onto GroupBox "Levels"
        ''' </summary>
        Private Sub Gbx_Levels_Drop(sender As Object, e As DragEventArgs)
            Try
                'Accept files only
                If Not e.Data.GetDataPresent(DataFormats.FileDrop) Then Return

                Dim filePaths As String() = e.Data.GetData(DataFormats.FileDrop)
                Dim confirmedFiles As List(Of String) = OrderDroppedLevels(filePaths)

                '------------- Template of confirmedFiles, depending on .Count :
                '-------------  1 = Iwad
                '-------------  2 = Iwad, Maps
                '-------------  3 = Iwad, Maps, Misc
                '-------------  4 = Iwad, Maps, Misc, Pict

                If confirmedFiles.Count = 0 Then Return
                SetActiveLvlTab(LVLPRESET_TAB.AddNew)

                If confirmedFiles.Count > 0 Then FillTextBox(Tbx_NewLevelIwad, confirmedFiles(0))
                If confirmedFiles.Count > 1 Then FillTextBox(Tbx_NewLevelMaps, confirmedFiles(1))
                If confirmedFiles.Count > 2 Then FillTextBox(Tbx_NewLevelMisc, confirmedFiles(2))
                If confirmedFiles.Count > 3 Then FillTextBox(Tbx_NewLevelPict, confirmedFiles(3))
            Catch ex As Exception
                Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
                WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}")
            End Try
        End Sub

        Private Sub TextBox_NewLevel_PreviewDragOver(sender As Object, e As DragEventArgs)
            e.Handled = True
        End Sub

        Private Sub TextBox_NewLevel_Drop(sender As Object, e As DragEventArgs)
            Try
                Dim tbx As TextBox = sender

                'Accept files only
                If Not e.Data.GetDataPresent(DataFormats.FileDrop) Then Return

                Dim droppedFile As String = e.Data.GetData(DataFormats.FileDrop)(0)
                If Not File.Exists(droppedFile) Then
                    MessageBox.Show(ERR_INPUT_NOT_FILE, ERR_INVALID_INPUT, MessageBoxButton.OK, MessageBoxImage.Error)
                    Return
                End If

                Dim sourceTbx As String = tbx.Tag 'Iwad, Maps, Misc, Pict
                If ValidateFile(droppedFile, sourceTbx) Then FillTextBox(sender, droppedFile)

            Catch ex As Exception
                Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
                WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}")
            End Try
        End Sub

        Private Sub Btn_NewLevelBrowse_Click(sender As Object, e As RoutedEventArgs)
            Dim btn As Button = sender

            Dim sourceBtn As String = btn.Tag.ToString
            Select Case sourceBtn
                Case "Iwad"
                    Dim dialog As New OpenFileDialog With
                    {
                        .Filter = "IWAD file (*.wad)|*.wad",
                        .InitialDirectory = GetDirectoryPath("Iwad"),
                        .Title = TBX_SELECT_IWAD
                    }
                    If dialog.ShowDialog() Then FillTextBox(Tbx_NewLevelIwad, dialog.FileName)

                Case "Maps"
                    Dim dialog As New OpenFileDialog With
                    {
                        .Filter = "Maps file (*.wad;*.pk3)|*.wad;*.pk3", '.zip needs testing, never used such format for Maps
                        .InitialDirectory = GetDirectoryPath("Maps"),
                        .Title = TBX_SELECT_MAPS
                    }
                    If dialog.ShowDialog() Then FillTextBox(Tbx_NewLevelMaps, dialog.FileName)

                Case "Misc"
                    Dim dialog As New OpenFileDialog With
                    {
                        .Filter = "Misc. file (*.deh;*.bex;*.txt)|*.deh;*.bex;*.txt",
                        .InitialDirectory = GetDirectoryPath("Misc"),
                        .Title = TBX_SELECT_MISC
                    }
                    If dialog.ShowDialog() Then FillTextBox(Tbx_NewLevelMisc, dialog.FileName)

                Case "Pict"
                    Dim dialog As New OpenFileDialog With
                    {
                        .Filter = "Image file (*.jpg;*.png)|*.jpg;*.png",
                        .InitialDirectory = GetDirectoryPath(), 'Or any other place in user computer?
                        .Title = TBX_SELECT_PICT
                    }
                    If dialog.ShowDialog() Then FillTextBox(Tbx_NewLevelPict, dialog.FileName)

            End Select
        End Sub

        Private Sub Btn_NewLevelClear_Click(sender As Object, e As RoutedEventArgs)
            Dim btn As Button = sender

            'Restore default placeholder
            Dim sourceBtn As String = btn.Tag.ToString
            Select Case sourceBtn
                Case "Iwad" : UnfillTextBox(Tbx_NewLevelIwad, TBX_DROP_IWAD)
                Case "Maps" : UnfillTextBox(Tbx_NewLevelMaps, TBX_DROP_MAPS)
                Case "Misc" : UnfillTextBox(Tbx_NewLevelMisc, TBX_DROP_MISC)
                Case "Pict" : UnfillTextBox(Tbx_NewLevelPict, TBX_DROP_PICT)
            End Select
        End Sub

        Private Sub Btn_NewLevelTry_Click(sender As Object, e As RoutedEventArgs)
            Dim iwadInput As String = Tbx_NewLevelIwad.Text
            Dim mapsInput As String = Tbx_NewLevelMaps.Text
            Dim miscInput As String = Tbx_NewLevelMisc.Text

            'At least 2 contents (including Iwad) are required
            If iwadInput = TBX_DROP_IWAD Then Return
            If mapsInput = TBX_DROP_MAPS And miscInput = TBX_DROP_MISC Then
                MessageBox.Show(ERR_ONLY_IWAD, ERR_MISSING_INPUT, MessageBoxButton.OK, MessageBoxImage.Error)
                Return
            End If

            'Update Summary "Fields" view
            TextBox_Summary_Iwad.Text = iwadInput
            UpdateLevels_Summary(mapsInput, miscInput)

            UpdateCommand()
            DecorateCommand()
        End Sub

        Private Sub Btn_NewLevelSaveAs_Click(sender As Object, e As RoutedEventArgs)
            Dim iwadInput As String = Tbx_NewLevelIwad.Text
            Dim mapsInput As String = Tbx_NewLevelMaps.Text
            Dim miscInput As String = Tbx_NewLevelMisc.Text

            'At least 2 contents (including Iwad) are required
            If iwadInput = TBX_DROP_IWAD Then Return
            If mapsInput = TBX_DROP_MAPS And miscInput = TBX_DROP_MISC Then
                'TODO: Use string constant
                MessageBox.Show("You only submitted an Iwad. Please select it from the ""Base presets"" tab instead", ERR_MISSING_INPUT, MessageBoxButton.OK, MessageBoxImage.Error)
                Return
            End If

            'Shortcut as saving fake data in order to complete quickly the management of user levels with JSON 
            Dim jsonFilepath As String = GetJsonFilepath("UserLevels")
            Dim jsonString As String = GetJsonData(jsonFilepath)

            Dim FAKENAME As String = $"Fake name (created on {Now:yyyy-MM-dd_HH-mm-ss})"

            Dim currentLevels As List(Of LevelPreset) = LoadUserLevels(jsonString)
            currentLevels.Add(New LevelPreset With
            {
                .Name = FAKENAME,
                .Iwad = iwadInput,
                .Maps = mapsInput,
                .Misc = If(miscInput = TBX_DROP_MISC,
                            String.Empty,
                            miscInput),
                .Type = If(iwadInput.ToLowerInvariant.Contains("doom2.wad"), "Doom 2", "Doom 1"),
                .Year = Now.Year,
                .Pict = "" 'TODO: Manage this input
            })
            SaveUserLevels(currentLevels, jsonFilepath)
            MessageBox.Show($"New user level preset ""{FAKENAME}"" saved", "Saving OK", MessageBoxButton.OK, MessageBoxImage.Information)
        End Sub

        Private Sub Btn_NewLevelClearAll_Click(sender As Object, e As RoutedEventArgs)
            UnfillTextBox(Tbx_NewLevelIwad, TBX_DROP_IWAD)
            UnfillTextBox(Tbx_NewLevelMaps, TBX_DROP_MAPS)
            UnfillTextBox(Tbx_NewLevelMisc, TBX_DROP_MISC)
            UnfillTextBox(Tbx_NewLevelPict, TBX_DROP_PICT)
        End Sub

#End Region


#Region "Events - Mods"

        Private Sub Tbc_Mods_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)

            Select Case GetActiveModTab()
                Case MODPRESET_TAB.Base '...
                Case MODPRESET_TAB.User : PopulateUserMods() 'Refresh each time
                Case MODPRESET_TAB.AddNew '...
            End Select

        End Sub

        Private Sub ShowNoUserMods()
            Lvw_ModsUserPresets.Visibility = Visibility.Collapsed
            Lbl_ModsNoUserPresets.Visibility = Visibility.Visible
        End Sub

        Private Sub PopulateUserMods()
            Try
                Dim jsonFilepath As String = GetJsonFilepath("UserMods")
                If Not File.Exists(jsonFilepath) Then
                    MessageBox.Show("No local JSON file to load data from: use the ""Add new preset"" tab to create some", ERR_INVALID_INPUT, MessageBoxButton.OK, MessageBoxImage.Information)
                    Return 'Early return
                End If

                Dim jsonString As String = GetJsonData(jsonFilepath)

                If Not CanLoadJsonArray(jsonString) Then
                    MessageBox.Show("Unable to read user mod presets from local JSON file", ERR_INVALID_INPUT, MessageBoxButton.OK, MessageBoxImage.Error)
                    ShowNoUserMods()
                    Return 'Early return
                End If

                Dim userMods As List(Of ModPreset) = LoadUserMods(jsonString)
                If userMods.Count = 0 Then
                    ShowNoUserMods()
                    Return 'Early return
                End If

                'Display
                With Lvw_ModsUserPresets
                    .Visibility = Visibility.Visible
                    .ItemsSource = userMods
                End With

            Catch ex As Exception
                Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
                WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}")

                ShowNoUserMods()
            End Try
        End Sub

        Private Sub Lvw_ModsBasePresets_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
            Try
                Dim mp As ModPreset = ReturnSelectedMods()

                Tbk_ModsDesc.Text = mp.Desc
                Lvw_ModsFiles.ItemsSource = mp.Files.Where(Function(f As String) Not f = String.Empty) 'Exclude empty filenames
                UpdateMods_Summary(mp.Files)
                UpdateCommand()
                DecorateCommand()

                e.Handled = True 'Prevent escalating up to "parent" event Tbc_Mods_SelectionChanged()

            Catch ex As Exception
                Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
                WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}")
            End Try
        End Sub

        Private Sub Lvw_ModsUserPresets_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
            Try
                'Safety fix. SelectedItem is not kept accross tabs, as the content of this one is updated everytime
                'TODO? Improve
                If Lvw_ModsUserPresets.SelectedItem Is Nothing Then Return

                Dim mp As ModPreset = ReturnSelectedMods()

                UpdateMods_Summary(mp.Files)
                UpdateCommand()
                DecorateCommand()

                e.Handled = True 'Prevent escalating up to "parent" event Tbc_Mods_SelectionChanged()

            Catch ex As Exception
                Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
                WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}")
            End Try
        End Sub

        Private Sub MenuItemModView_Click(sender As Object, e As RoutedEventArgs)
            'TODO
            MessageBox.Show("You clicked ""View details""")
        End Sub

        Private Sub MenuItemModDelete_Click(sender As Object, e As RoutedEventArgs)
            'TODO
            MessageBox.Show("You clicked ""Delete preset""")
        End Sub

        Private Sub Btn_NewModFilesClear_Click()
            Dtg_NewModFiles.ItemsSource = Nothing
            Dtg_NewModFiles.Visibility = Visibility.Collapsed
            Lbl_NewModFiles.Visibility = Visibility.Visible
        End Sub

        Private Sub Grid_NewModFiles_Drop(sender As Object, e As DragEventArgs)
            Try
                'Accept files only
                If Not e.Data.GetDataPresent(DataFormats.FileDrop) Then Return

                Dim modFiles As ObservableCollection(Of InputFile) = If(Dtg_NewModFiles.ItemsSource Is Nothing,
                                                                       New ObservableCollection(Of InputFile),
                                                                       DirectCast(Dtg_NewModFiles.ItemsSource, ObservableCollection(Of InputFile)))

                If modFiles.Count = 0 Then
                    Lbl_NewModFiles.Visibility = Visibility.Collapsed
                    Dtg_NewModFiles.Visibility = Visibility.Visible
                End If


                For Each droppedFile As String In e.Data.GetData(DataFormats.FileDrop)
                    Dim fi As New FileInfo(droppedFile)

                    'Dim iFile As New InputFile(fi.Name, fi.Directory.ToString)

                    modFiles.Add(New InputFile(fi.Name, fi.Directory.ToString))
                Next
                Dtg_NewModFiles.ItemsSource = modFiles

            Catch ex As Exception
                Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
                WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}")
            End Try
        End Sub

        Private Sub Grid_NewModFiles_PreviewDragOver(sender As Object, e As DragEventArgs)
            e.Handled = True
        End Sub

        Private Sub Dtg_NewModFiles_LoadingRow(sender As Object, e As DataGridRowEventArgs)
            e.Row.Header = (e.Row.GetIndex + 1).ToString
        End Sub

        Private Sub Btn_NewModTry_Click(sender As Object, e As RoutedEventArgs)
            If Dtg_NewModFiles.ItemsSource Is Nothing Then Return

            'Gather modFiles
            Dim modFiles As New List(Of String)
            Dim iFiles As ObservableCollection(Of InputFile) = DirectCast(Dtg_NewModFiles.ItemsSource, ObservableCollection(Of InputFile))
            iFiles.ToList().ForEach(Sub(iFile As InputFile) modFiles.Add(Path.Combine(iFile.Directory, iFile.Name)))

            UpdateMods_Summary(modFiles)
            UpdateCommand()
            DecorateCommand()
        End Sub

        Private Sub Btn_NewModSave_Click(sender As Object, e As RoutedEventArgs)

            If Dtg_NewModFiles.ItemsSource Is Nothing Then
                MessageBox.Show("Missing mod files", ERR_MISSING_INPUT, MessageBoxButton.OK, MessageBoxImage.Error)
                Return 'Early return
            End If

            If Tbx_NewModName.Text = String.Empty Then
                MessageBox.Show("Missing name", ERR_MISSING_INPUT, MessageBoxButton.OK, MessageBoxImage.Error)
                Return 'Early return
            End If

            'Get current user mods
            Dim jsonFilepath As String = GetJsonFilepath("UserMods")
            Dim jsonString As String = GetJsonData(jsonFilepath)
            Dim currentMods As List(Of ModPreset) = LoadUserMods(jsonString)

            'Gather modFiles
            Dim modFiles As New List(Of String)
            Dim iFiles As ObservableCollection(Of InputFile) = DirectCast(Dtg_NewModFiles.ItemsSource, ObservableCollection(Of InputFile))
            iFiles.ToList().ForEach(Sub(iFile As InputFile) modFiles.Add(Path.Combine(iFile.Directory, iFile.Name)))

            'Append new preset
            currentMods.Add(New ModPreset With
            {
                .Name = Tbx_NewModName.Text,
                .Desc = "",
                .Files = modFiles,
                .Pict = "" 'TODO: Manage this input
            })
            SaveUserMods(currentMods, jsonFilepath)
            MessageBox.Show($"New user mod preset ""{Tbx_NewModName.Text}"" saved", "Saving OK", MessageBoxButton.OK, MessageBoxImage.Information)

        End Sub

        Private Sub Btn_NewModClearAll_Click(sender As Object, e As RoutedEventArgs)
            'TODO: Use UnfillTextBox(Tbx, Placeholder) + Set placeholders
            Tbx_NewModName.Text = String.Empty
            Tbx_NewModDesc.Text = String.Empty

            Btn_NewModFilesClear_Click()
        End Sub

#End Region


#Region "Actions : Sidebar"

        Private Sub Button_Options_HelpAbout_Click(sender As Object, e As RoutedEventArgs)
            Try
                Dim helpWindow As New HelpWindow() With {.Owner = Me}
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
                    Grp_Summary.Header = "Command line"
                Else
                    Grid_Summary.Visibility = Visibility.Visible
                    Grid_Command.Visibility = Visibility.Collapsed
                    Grp_Summary.Header = "Summary"
                End If
            Catch ex As Exception
                Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
                WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}")
            End Try
        End Sub

        Private Sub Button_Options_LaunchSave_Click(sender As Object, e As RoutedEventArgs)
            Try
                If ReadyToLaunch() Then
                    LaunchGame()
                    SaveSettings()
                End If
            Catch ex As Exception
                Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
                WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}")
            End Try
        End Sub

#End Region


#Region "Operations / Helpers"

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




        ''' <summary>
        ''' Create new TextBox to represent a file in StackPanel_Summary_FilesMods
        ''' </summary>
        ''' <param name="filepath">Full path of represented file</param>
        ''' <param name="target">Target : "Level" or "Mod"</param>
        ''' <returns></returns>
        Private Function CreateTbx(filepath As String, target As String) As TextBox
            Dim info As New FileInfo(filepath)

            Dim sBuilder As New StringBuilder()
            sBuilder.AppendLine($"File type : {target}")
            sBuilder.Append($"Directory : {info.DirectoryName}")

            Return New TextBox() With
            {
                .Background = If(target = "Level", Brushes.White, Brushes.LightGray),
                .BorderBrush = Brushes.LightGray,
                .Cursor = Cursors.Arrow,
                .IsReadOnly = True,
                .Margin = New Thickness(0, 0, 6, 0),
                .Tag = info.Directory.ToString,
                .Text = info.Name,
                .ToolTip = sBuilder.ToString
            }
        End Function

        ''' <summary>
        ''' Extract file fullpath from TextBox.Tag attribute
        ''' </summary>
        ''' <param name="tbx">TextBox to extract from</param>
        ''' <returns></returns>
        Private Function ExtractFileFullPath(tbx As TextBox) As String
            Dim directoryPath As String = tbx.Tag.ToString
            Dim filename As String = tbx.Text

            Return Path.Combine(directoryPath, filename)
        End Function

        ''' <summary>
        ''' Retrieve full path of files "Maps/Misc" or "Mods" in StackPanel_Summary_FilesMods.
        ''' It is done by reading the TextBox.Tag attribute
        ''' </summary>
        ''' <param name="target">Target : "Level" or "Mod"</param>
        ''' <returns></returns>
        Private Function GetFullPathsFromStackPanelItems(target As String) As List(Of String)
            Dim fullPaths As New List(Of String)

            Try
                Dim allTbxs As List(Of TextBox) = Stkp_SummaryFilesMods.Children.OfType(Of TextBox).ToList

                If target = "Level" Then
                    'Build a List(Of String) of Length = 2, as {Maps, Misc}
                    Dim lvlTbxs As List(Of TextBox) = allTbxs.Where(Function(tbx) tbx.Background Is Brushes.White).ToList

                    If lvlTbxs.Count > 0 Then : fullPaths.Add(ExtractFileFullPath(lvlTbxs(0))) : Else fullPaths.Add(String.Empty) : End If
                    If lvlTbxs.Count > 1 Then : fullPaths.Add(ExtractFileFullPath(lvlTbxs(1))) : Else fullPaths.Add(String.Empty) : End If
                End If

                If target = "Mod" Then
                    'Build a List(Of String) of Length = N, as {ModFile1, ModFile2, ModFile3, etc.}
                    Dim modTbxs As List(Of TextBox) = allTbxs.Where(Function(tbx) tbx.Background Is Brushes.LightGray).ToList

                    modTbxs.ForEach(Sub(tbx) fullPaths.Add(ExtractFileFullPath(tbx)))
                End If

            Catch ex As Exception
                Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
                WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}{vbCrLf} Parameter(s) : {target}")
            End Try

            Return fullPaths
        End Function

        ''' <summary>
        ''' Retrieve Port parameters in StackPanel SummaryPortParameters.
        ''' </summary>
        ''' <returns></returns>
        Private Function GetPortParamsFromStackPanel() As Dictionary(Of String, String)
            Dim paramsDict As New Dictionary(Of String, String)

            Try
                Dim paramTbxs As List(Of TextBox) = Stkp_SummaryPortParameters.Children.OfType(Of TextBox).ToList
                For Each tbx As TextBox In paramTbxs
                    If tbx.Text.Contains(" ") Then
                        paramsDict.Add(tbx.Text.Split(" ")(0).Replace("-", ""), tbx.Text.Split(" ")(1))
                    Else
                        paramsDict.Add(tbx.Text.Replace("-", ""), String.Empty)
                    End If
                Next

            Catch ex As Exception
                Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
                WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}")
            End Try

            Return paramsDict
        End Function

        ''' <summary>
        ''' Update the TextBox items in StackPanel for Levels (Maps and Misc).
        ''' Clear then refill the entire StackPanel
        ''' </summary>
        ''' <param name="level">Maps filename or filepath</param>
        ''' <param name="misc">Misc filename or filepath</param>
        Private Sub UpdateLevels_Summary(level As String, misc As String)
            Try
                'Gather existing contents in StackPanel : update Levels but preserve Mods if any
                Dim allTbxs As List(Of TextBox) = Stkp_SummaryFilesMods.Children.OfType(Of TextBox).ToList
                Dim modTbxs As List(Of TextBox) = allTbxs.Where(Function(tbx) tbx.Background Is Brushes.LightGray).ToList

                'Clear the StackPanel content
                Stkp_SummaryFilesMods.Children.Clear()

                'Create New List(Of TextBox) to be filled
                Dim lvlTbxs As New List(Of TextBox)

                Dim lvls As New List(Of String) From {level, misc}
                For Each nameOrPath As String In lvls
                    If nameOrPath = String.Empty Then Continue For

                    Dim fullpath As String = If(File.Exists(nameOrPath), nameOrPath, GetFileAbsolutePath("", nameOrPath))
                    lvlTbxs.Add(CreateTbx(fullpath, "Level"))
                Next

                'Fill the StackPanel with gathered TextBoxes
                lvlTbxs.ForEach(Sub(tbx) Stkp_SummaryFilesMods.Children.Add(tbx))
                modTbxs.ForEach(Sub(tbx) Stkp_SummaryFilesMods.Children.Add(tbx))

            Catch ex As Exception
                Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
                WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}{vbCrLf} Parameter(s) : {level}, {misc}")
            End Try
        End Sub

        ''' <summary>
        ''' Update the TextBox items in StackPanel for Mods
        ''' Clear and refill mods only
        ''' </summary>
        ''' <param name="mods">Mods filenames or filepaths</param>
        Private Sub UpdateMods_Summary(mods As List(Of String))
            Try
                'Gather existing contents in StackPanel : update Levels but preserve Mods if any
                Dim allTbxs As List(Of TextBox) = Stkp_SummaryFilesMods.Children.OfType(Of TextBox).ToList
                Dim modTbxs As List(Of TextBox) = allTbxs.Where(Function(tbx) tbx.Background Is Brushes.LightGray).ToList

                'Remove previous mods
                modTbxs.ForEach(Sub(tbx) Stkp_SummaryFilesMods.Children.Remove(tbx))

                'Clear the list to be filled again
                modTbxs.Clear()

                For Each nameOrPath As String In mods
                    If nameOrPath = String.Empty Then Continue For

                    Dim fullpath As String = If(File.Exists(nameOrPath), nameOrPath, GetFileAbsolutePath("", nameOrPath))
                    modTbxs.Add(CreateTbx(fullpath, "Mod"))
                Next

                'Fill the StackPanel with gathered TextBoxes
                modTbxs.ForEach(Sub(tbx) Stkp_SummaryFilesMods.Children.Add(tbx))

            Catch ex As Exception
                Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
                Dim modsJoined As String = String.Join(", ", mods)
                WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}{vbCrLf} Parameter(s) : {modsJoined}")
            End Try
        End Sub

        ''' <summary>
        ''' Update the TextBox items in StackPanel for Port parameters
        ''' </summary>
        ''' <param name="portParams">Dictionary (Parameter:String, ParameterValue:String) </param>
        Private Sub UpdatePortParams_Summary(portParams As Dictionary(Of String, String))
            Try
                'Clear StackPanel
                Stkp_SummaryPortParameters.Children.Clear()

                'Fill StackPanel
                For Each kvp As KeyValuePair(Of String, String) In portParams
                    Stkp_SummaryPortParameters.Children.Add(New TextBox() With
                    {
                        .Background = Brushes.AliceBlue,
                        .BorderBrush = Brushes.LightGray,
                        .Cursor = Cursors.Arrow,
                        .IsReadOnly = True,
                        .Margin = New Thickness(0, 0, 6, 0),
                        .Text = $"-{kvp.Key}" & If(kvp.Value.Length = 0, "", $" {kvp.Value}")
                    })
                Next

            Catch ex As Exception
                Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
                Dim paramsJoined As String = String.Join(", ", portParams)
                WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}{vbCrLf} Parameter(s) : {paramsJoined}")
            End Try
        End Sub




        ''' <summary>
        ''' Perform 2 consecutive loops on cmdTxtRange to apply syntax highlighting on 1) keywords 2) port Parameters
        ''' </summary>
        Private Sub DecorateCommand() 'TODO? Improve (Simplify code ? Use only one loop ?)
            Try
                Dim cmdTxtRange As New TextRange(Rtb_Command.Document.ContentStart, Rtb_Command.Document.ContentEnd)

                'Decorate keywords (skip enclosing quotes 4 times per path, like in ""complete_path"")
                Dim fileMatches As MatchCollection = Regex.Matches(cmdTxtRange.Text, "-iwad|-file")
                Dim quotesCount As Integer = 0
                For Each fm As Match In fileMatches
                    For Each c As Capture In fm.Captures
                        Dim startIndex As TextPointer = cmdTxtRange.Start.GetPositionAtOffset(c.Index + (quotesCount * 4))
                        Dim endIndex As TextPointer = cmdTxtRange.Start.GetPositionAtOffset(c.Index + (quotesCount * 4) + c.Length)
                        Dim rangeToEdit As New TextRange(startIndex, endIndex)

                        rangeToEdit.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.DarkBlue)
                        rangeToEdit.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold)
                    Next
                    quotesCount += 1
                Next

                'Decorate Port parameters (at each param, skip the length of the value of the previous one + 1 for the blank space)
                Dim portParams As Dictionary(Of String, String) = GetPortParamsFromStackPanel()
                If portParams.Count > 0 Then
                    Dim paramsKeyWords As New List(Of String)
                    Dim paramsValueLen As New List(Of Integer)
                    For Each kvp As KeyValuePair(Of String, String) In portParams
                        paramsKeyWords.Add($"-{kvp.Key}")
                        paramsValueLen.Add(kvp.Value.Length)
                    Next
                    Dim paramsMatches As MatchCollection = Regex.Matches(cmdTxtRange.Text, String.Join("|", paramsKeyWords))
                    Dim offset As Integer = 0
                    Dim mem As Integer = 0
                    Dim i As Integer = 0
                    For Each pm As Match In paramsMatches
                        For Each c As Capture In pm.Captures
                            Dim startIndex As TextPointer = cmdTxtRange.Start.GetPositionAtOffset(c.Index + offset)
                            Dim endIndex As TextPointer = cmdTxtRange.Start.GetPositionAtOffset(c.Index + offset + c.Length)
                            Dim rangeToEdit As New TextRange(startIndex, endIndex)

                            rangeToEdit.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.DarkMagenta)
                            rangeToEdit.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold)
                        Next
                        mem += paramsValueLen(i)
                        offset += mem + 1
                        i += 1
                    Next
                End If

            Catch ex As Exception
                Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
                WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}")
            End Try
        End Sub

        Private Sub FillRichTextBox_Command(content As String)
            Try
                Dim para As New Paragraph()
                para.Inlines.Add(content)

                Dim flow As New FlowDocument()
                flow.Blocks.Add(para)

                Rtb_Command.Document = flow
            Catch ex As Exception
                Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
                WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}{vbCrLf} Parameter(s) : {content}")
            End Try
        End Sub

        ''' <summary>
        ''' Update the "Command" Summary from the "Fields" Summary view
        ''' </summary>
        Private Sub UpdateCommand()
            Dim command As String = String.Empty
            Try
                Dim port As String = TextBox_Summary_Port.Text
                Dim iwad As String = TextBox_Summary_Iwad.Text

                If port = String.Empty Or iwad = String.Empty Then
                    Rtb_Command.Document.Blocks.Clear() 'TODO? Display text about Missing Port & Iwad ?
                    Return
                End If

                command &= $"""{port}"""

                'Add Port parameters to the command line
                Dim paramTbxs As List(Of TextBox) = Stkp_SummaryPortParameters.Children.OfType(Of TextBox).ToList
                paramTbxs.ForEach(Sub(tbx) command &= $" {tbx.Text}")

                command &= $" -iwad ""{iwad}"""

                'Add Maps/Misc/Mods to the command line
                Dim allTbxs As List(Of TextBox) = Stkp_SummaryFilesMods.Children.OfType(Of TextBox).ToList
                allTbxs.ForEach(Sub(tbx) command &= $" -file ""{ExtractFileFullPath(tbx)}""")

                FillRichTextBox_Command(command)
            Catch ex As Exception
                Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
                WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}{vbCrLf} Command : {command}")
            End Try
        End Sub




        Private Function ReturnSelectedLevels() As LevelPreset
            Dim preset As LevelPreset = Nothing

            Try
                Select Case GetActiveLvlTab()
                    Case LVLPRESET_TAB.Base
                        preset = DirectCast(Lvw_LevelsBasePresets.SelectedItem, LevelPreset)

                    Case LVLPRESET_TAB.User
                        preset = DirectCast(Lvw_LevelsUserPresets.SelectedItem, LevelPreset)

                    Case LVLPRESET_TAB.AddNew
                        'TODO

                End Select
            Catch ex As Exception
                Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
                WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}")
            End Try

            Return preset
        End Function

        ''' <summary>
        ''' Check and order the files dropped into the GroupBox "Levels"
        ''' </summary>
        Private Function OrderDroppedLevels(filePaths As String()) As List(Of String)
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

                        If orderedFiles.Count > 0 AndAlso ValidateFile(path, "Maps") Then
                            orderedFiles.Add(path)
                            Continue For
                        End If

                        If orderedFiles.Count > 1 AndAlso ValidateFile(path, "Misc") Then
                            orderedFiles.Add(path)
                            Continue For
                        End If

                        If orderedFiles.Count > 2 AndAlso ValidateFile(path, "Pict") Then
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

        Private Function ReturnSelectedMods() As ModPreset
            Dim preset As ModPreset = Nothing

            Try
                Select Case GetActiveModTab()
                    Case MODPRESET_TAB.Base
                        preset = DirectCast(Lvw_ModsBasePresets.SelectedItem, ModPreset)

                    Case MODPRESET_TAB.User
                        preset = DirectCast(Lvw_ModsUserPresets.SelectedItem, ModPreset)

                    Case MODPRESET_TAB.AddNew
                        'TODO

                End Select
            Catch ex As Exception
                Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
                WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}")
            End Try

            Return preset
        End Function

        'TODO: Update to v3
        ''' <summary>
        ''' Delete a preset by its name
        ''' Triggered with right click on custom preset
        ''' </summary>
        '''
        Public Sub DeleteUserLevelPreset(presetName As String)
            Dim message As String = String.Format("Delete preset ""{0}"" ?", presetName)

            If MessageBox.Show(message, "Delete user preset", MessageBoxButton.OKCancel) = MessageBoxResult.OK Then
                DeletePreset(presetName)
                'DisplayUserPresets(GetLevelPresets_FromCsv("user")) 'Update GUI
            End If
        End Sub

        'TODO: Update to v3
        Public Function ReturnUserPresetButtons(presetsList As List(Of LevelPreset)) As List(Of Button)
            Dim buttonsList As New List(Of Button)

            For Each preset As LevelPreset In presetsList
                Dim button As New Button() With
                {
                    .Content = preset.Name,
                    .FontSize = 14,
                    .Height = 28,
                    .Margin = New Thickness(0, 0, 0, 2)
                }

                AddHandler button.Click, 'Left click
                Sub(sender, e)
                    'SelectUserLevelPreset(preset.Iwad, preset.Maps, preset.Misc)
                End Sub

                AddHandler button.MouseRightButtonDown, 'Right click
                Sub(sender, e)
                    DeleteUserLevelPreset(preset.Name)
                End Sub

                buttonsList.Add(button)
            Next

            Return buttonsList
        End Function




        Private Function ReadyToLaunch() As Boolean
            Try
                If TextBox_Summary_Port.Text = String.Empty Then
                    MessageBox.Show(ERR_MISSING_PORT, ERR_MISSING_INPUT, MessageBoxButton.OK, MessageBoxImage.Error)
                    Return False
                End If

                If TextBox_Summary_Iwad.Text = String.Empty Then
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
                Dim rtbText As String = New TextRange(Rtb_Command.Document.ContentStart, Rtb_Command.Document.ContentEnd).Text

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
        ''' Save "Settings" (currently selected contents in Summary) to JSON
        ''' </summary>
        Private Sub SaveSettings()
            Try
                Dim levelsPaths As List(Of String) = GetFullPathsFromStackPanelItems("Level")
                Dim modsPaths As List(Of String) = GetFullPathsFromStackPanelItems("Mod")

                Dim lastLaunched As New Setting(GetJsonFilepath("Settings")) With
                {
                    .Port = TextBox_Summary_Port.Text,
                    .PortParameters = GetPortParamsFromStackPanel(),
                    .Iwad = TextBox_Summary_Iwad.Text,
                    .Maps = levelsPaths(0),
                    .Misc = levelsPaths(1),
                    .Mods = modsPaths
                }
                lastLaunched.Save()

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
                Dim settingsFilepath As String = GetJsonFilepath("Settings")
                If Not File.Exists(settingsFilepath) Then Return

                Dim persistedSettings As New Setting(settingsFilepath)
                If Not persistedSettings.CanLoad() Then Return

                persistedSettings.Load()
                With persistedSettings
                    If File.Exists(.Port) Then
                        FillTextBox(Tbx_Port, .Port)
                        FillTextBox(TextBox_Summary_Port, .Port)
                    End If
                    'TODO? Handle case of invalid .Port

                    UpdatePortParams(.PortParameters)
                    UpdatePortParams_Summary(.PortParameters)

                    If File.Exists(.Iwad) Then TextBox_Summary_Iwad.Text = .Iwad
                    'TODO? Handle case of invalid .Iwad

                    UpdateLevels_Summary(.Maps, .Misc)
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
