﻿Imports System.Drawing
Imports System.Globalization
Imports System.IO
Imports System.Reflection
Imports ThisIsADoomLauncher.Helpers.DoomWorld
Imports ThisIsADoomLauncher.Helpers.DoomWorld.Models

Namespace Views
    Public Class DoomWorldWindow

        Private _doomworldService As New DoomWorldService()
        Private _resourcePath As String = "levels/"
        Private _selectedSortingCriterion As String

        Private _lstBrowseResults As List(Of Object)
        Private _lstSearchResults As List(Of Level)
        Private _lstInstalledLevelsResults As List(Of InstalledLevel)

        Private _selectedLevel As ThisIsADoomLauncher.Helpers.DoomWorld.Models.Level

        Public Const ERR_NO_INTERNET As String = "No Internet connection."
        Public Const ERR_DW_UNREACHABLE As String = "DoomWorld server unreachable. Error code: "

        Public Property SelectedLevel() As ThisIsADoomLauncher.Helpers.DoomWorld.Models.Level
            Get
                Return _selectedLevel
            End Get
            Set(value As ThisIsADoomLauncher.Helpers.DoomWorld.Models.Level)
                _selectedLevel = value
                Me.ToggleDisplayLevelContent()
            End Set
        End Property


        Public Sub New()
            InitializeComponent()
            InitUI()
            CheckApiAccess()
        End Sub

        Private Async Sub CheckApiAccess()
            Dim responseCode As Integer = Await _doomworldService.CheckDoomWorldAccess()
            If Not responseCode = 200 Then
                If responseCode = 0 Then
                    Txt_CanvasErrorText.Text = ERR_NO_INTERNET
                Else
                    Txt_CanvasErrorText.Text = $"{ERR_DW_UNREACHABLE & responseCode}."
                End If
                Cnv_InternetError.Visibility = Visibility.Visible
                Img_Browse_NoInternet.Visibility = Visibility.Visible
                Img_DWSearch_NoInternet.Visibility = Visibility.Visible
            Else
                Cnv_InternetError.Visibility = Visibility.Collapsed
                Img_Browse_NoInternet.Visibility = Visibility.Collapsed
                Img_DWSearch_NoInternet.Visibility = Visibility.Collapsed
            End If

        End Sub

        Public Function GetDWServiceInstance() As DoomWorldService
            Return _doomworldService
        End Function

        ''' <summary>
        ''' Initializes some UI custom elements.
        ''' </summary>
        Private Sub InitUI()
            SelectedLevel = Nothing

            InitLists()
            UpdateDirPathUI(_resourcePath)

        End Sub

        Private Sub UpdateDirPathUI(resourcePath As String)
            Me.SetDynamicBreadcrumb(resourcePath)
            Me.ToggleParentFolderButtonEnabled(_resourcePath)
        End Sub

        ''' <summary>
        ''' Creates a dynamic clickable path for user to navigate easier in folder tree.
        ''' </summary>
        ''' <param name="resourcePath"></param>
        Private Sub SetDynamicBreadcrumb(resourcePath As String)
            Dim pathForHyperlink As String = String.Empty
            Dim pathArray As String() = resourcePath.Split(CChar("/"))?.Where(
                Function(pathItem As String) Not String.IsNullOrWhiteSpace(pathItem)).ToArray()

            Txt_BrowseDirPath.Inlines.Clear()

            For index As Integer = 0 To pathArray.Length - 1
                If index > 0 Then Txt_BrowseDirPath.Inlines.Add(New Run(" / ") With {.FontWeight = FontWeights.Regular})
                pathForHyperlink += String.Concat(pathArray(index), "/")

                Dim runTextSegment As New System.Windows.Documents.Run(pathArray(index))
                Dim pathHyperlink As New System.Windows.Documents.Hyperlink(runTextSegment) With {
                    .Tag = pathForHyperlink}
                AddHandler pathHyperlink.Click, Sub(hyperlinkSender, hyperlinkEventArgs) HyperlinkPath_Clicked(hyperlinkSender, hyperlinkEventArgs)

                Txt_BrowseDirPath.Inlines.Add(pathHyperlink)
            Next
        End Sub

        ''' <summary>
        ''' Event when UI breadcrumb hyperlink is clicked.
        ''' </summary>
        ''' <param name="hyperlinkSender"></param>
        ''' <param name="hyperlinkEventArgs"></param>
        Private Sub HyperlinkPath_Clicked(hyperlinkSender As Object, hyperlinkEventArgs As RoutedEventArgs)
            Dim hlink As Hyperlink = DirectCast(hyperlinkSender, Hyperlink)

            Dim resPath As String = hlink.Tag.ToString()
            _resourcePath = resPath

            Me.GetBrowseResults(_resourcePath)

        End Sub

        ''' <summary>
        ''' First initialize elements from DoomWorld API.
        ''' Then prepare results to be displayed as ItemsSource.
        ''' </summary>
        Private Async Sub InitLists()
            Try
                _lstBrowseResults = Await _doomworldService.GetContents()
                LoadResultsItemsSource(_lstBrowseResults)
                LoadInstalledLevelsItemsSource()

            Catch ex As Exception
                Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
                WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}{vbCrLf}")

            End Try
        End Sub

        ''' <summary>
        ''' Get content from API,
        ''' Refreshes Browse ListView,
        ''' Refreshes path breadcrumb.
        ''' </summary>
        ''' <param name="resourcePath"></param>
        Private Async Sub GetBrowseResults(resourcePath As String)
            _lstBrowseResults = Await _doomworldService.GetContents(_resourcePath)

            LoadResultsItemsSource(_lstBrowseResults)
            Me.UpdateDirPathUI(_resourcePath)
        End Sub

        ''' <summary>
        ''' Sorts DoomWorld results and sets as ItemsSource.
        ''' </summary>
        ''' <param name="contents"></param>
        Private Sub LoadResultsItemsSource(contents As IEnumerable(Of Object))
            If contents Is Nothing Or contents.Count() = 0 Then
                Me.SetTxtResultText("0 results", Txt_Lvw_BrowseResults_Count)

                Return
            End If

            contents = SortContents(contents, _selectedSortingCriterion)

            Lvw_BrowseResults.ItemsSource = contents
            Me.SetTxtResultText($"{contents.Count} results", Txt_Lvw_BrowseResults_Count)
        End Sub

        ''' <summary>
        ''' Sorts DoomWorld installed levels and sets as ItemsSource.
        ''' </summary>
        Private Sub LoadInstalledLevelsItemsSource()
            _lstInstalledLevelsResults = _doomworldService.GetInstalledLevels(Path.Combine("DoomWorld", "doomworld_registry.json"))
            If _lstInstalledLevelsResults Is Nothing Then
                Me.SetTxtResultText($"0 results", Txt_Lvw_InstalledResults_Count)

                Return
            End If

            Lvw_InstalledResults.ItemsSource = _lstInstalledLevelsResults
            Me.SetTxtResultText($"{_lstInstalledLevelsResults.Count} results", Txt_Lvw_InstalledResults_Count)
        End Sub

        ''' <summary>
        ''' Sorts DoomWorld elements by selected attribute 
        ''' <br>Folder : always by Name</br>
        ''' <br>Level : Title, Filename, ReleaseDate...</br>
        ''' </summary>
        ''' <param name="contents"></param>
        ''' <param name="selectedSortingCriterion"></param>
        ''' <returns></returns>
        Private Function SortContents(contents As IEnumerable(Of Object), selectedSortingCriterion As String, Optional ascending As Boolean = True) As IEnumerable(Of Object)
            If contents Is Nothing OrElse contents.Count() = 0 Then
                Return contents
            End If

            If contents.FirstOrDefault().GetType = GetType(Helpers.DoomWorld.Models.Folder) Then
                Return SortFolders(contents.OfType(Of Folder), ascending)
            Else
                Return SortLevels(contents.OfType(Of Level), selectedSortingCriterion, ascending)
            End If
        End Function

        ''' <summary>
        ''' Sort folders
        ''' </summary>
        ''' <param name="contents">List of Folders</param>
        ''' <param name="isAscending">Is ascending</param>
        ''' <returns></returns>
        Private Function SortFolders(contents As IEnumerable(Of Folder), Optional isAscending As Boolean = True) As IEnumerable(Of Object)
            Return If(isAscending,
                contents.OrderBy(Function(foldr) foldr.Name),
                contents.OrderByDescending(Function(foldr) foldr.Name)
            )
        End Function

        ''' <summary>
        ''' Sort levels regarding sorting criterion and direction.
        ''' </summary>
        ''' <param name="contents">List of Levels</param>
        ''' <param name="selectedSortingCriterion">Sorting criterion</param>
        ''' <param name="isAscending">Is ascending</param>
        ''' <returns></returns>
        Private Function SortLevels(contents As IEnumerable(Of Level), selectedSortingCriterion As String, Optional isAscending As Boolean = True) As IEnumerable(Of Object)
            Select Case selectedSortingCriterion
                Case "Title"
                    Return If(isAscending,
                        contents.OrderBy(Function(levl) levl.Title),
                        contents.OrderByDescending(Function(levl) levl.Title)
                    )
                Case "Filename"
                    Return If(isAscending,
                        contents.OrderBy(Function(levl) levl.Filename),
                        contents.OrderByDescending(Function(levl) levl.Filename)
                    )
                Case "ReleaseDate"
                    Return If(isAscending,
                        contents.OrderBy(Function(levl) levl.ReleaseDate),
                        contents.OrderByDescending(Function(levl) levl.ReleaseDate)
                    )
                Case "Rating"
                    Return If(isAscending,
                        contents.OrderBy(Function(levl) levl.Rating),
                        contents.OrderByDescending(Function(levl) levl.Rating)
                    )
                Case "Author"
                    Return If(isAscending,
                        contents.OrderBy(Function(levl) levl.Author),
                        contents.OrderByDescending(Function(levl) levl.Author)
                    )
                Case "Size"
                    Return If(isAscending,
                        contents.OrderBy(Function(levl) levl.Size),
                        contents.OrderByDescending(Function(levl) levl.Size)
                    )
                Case Else
                    Return contents
            End Select
        End Function

        ''' <summary>
        ''' Sort installed levels regarding sorting criterion and direction.
        ''' </summary>
        ''' <param name="contents">List of Levels</param>
        ''' <param name="selectedSortingCriterion">Sorting criterion</param>
        ''' <param name="isAscending">Is ascending</param>
        ''' <returns></returns>
        Private Function SortInstalledLevels(contents As IEnumerable(Of InstalledLevel), selectedSortingCriterion As String, Optional isAscending As Boolean = True) As IEnumerable(Of Object)
            Select Case selectedSortingCriterion
                Case "Title"
                    Return If(isAscending,
                        contents.OrderBy(Function(levl) levl.Title),
                        contents.OrderByDescending(Function(levl) levl.Title)
                    )
                Case "Filename"
                    Return If(isAscending,
                        contents.OrderBy(Function(levl) levl.FileName),
                        contents.OrderByDescending(Function(levl) levl.FileName)
                    )
                Case Else
                    Return contents
            End Select
        End Function

        Private Sub Btn_ParentFolder_Click(sender As Object, e As RoutedEventArgs)
            Me.BackToParentDirectory()
        End Sub

        Private Sub Lvw_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles Lvw_BrowseResults.SelectionChanged, Lvw_SearchResults.SelectionChanged, Lvw_InstalledResults.SelectionChanged
            Dim lst As ListView = DirectCast(sender, ListView)
            If lst.SelectedIndex <> -1 Then
                Me.HandleSelectedItem(lst.SelectedItem)
            End If

        End Sub

        Private Sub Cbb_Sorting_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles Cbb_Sorting.SelectionChanged
            Dim cbb As ComboBox = DirectCast(sender, ComboBox)
            If cbb.SelectedIndex <> -1 Then
                _selectedSortingCriterion = DirectCast(cbb.SelectedItem, ComboBoxItem).Content.ToString()

                SortAndRefreshAllLists()
            End If
        End Sub

        ''' <summary>
        ''' Sort and refresh all lists after Combobox sorting option has changed.
        ''' </summary>
        Private Sub SortAndRefreshAllLists()
            If _lstBrowseResults IsNot Nothing AndAlso _lstBrowseResults.Count <> 0 Then
                Me.LoadResultsItemsSource(_lstBrowseResults)
            End If

            If Lvw_SearchResults IsNot Nothing AndAlso Lvw_SearchResults.Items IsNot Nothing AndAlso Lvw_SearchResults.Items.Count <> 0 Then
                Lvw_SearchResults.ItemsSource = SortLevels(Lvw_SearchResults.Items.OfType(Of Level), _selectedSortingCriterion)
            End If

            If Lvw_InstalledResults IsNot Nothing AndAlso Lvw_InstalledResults.Items IsNot Nothing AndAlso Lvw_InstalledResults.Items.Count <> 0 Then
                Lvw_InstalledResults.ItemsSource = SortInstalledLevels(Lvw_InstalledResults.Items.OfType(Of InstalledLevel), _selectedSortingCriterion)
            End If
        End Sub


        ''' <summary>
        ''' Defines if SelectedItem is Folder or Level, and acts accordingly
        ''' </summary>
        ''' <param name="selectedItem"></param>
        Private Sub HandleSelectedItem(selectedItem As Object)

            If selectedItem.GetType() Is GetType(Helpers.DoomWorld.Models.Level) Then
                Dim item As Helpers.DoomWorld.Models.Level = DirectCast(selectedItem, Helpers.DoomWorld.Models.Level)
                GetLevel(item)

            ElseIf selectedItem.GetType() Is GetType(Helpers.DoomWorld.Models.InstalledLevel) Then
                Dim item As Helpers.DoomWorld.Models.InstalledLevel = DirectCast(selectedItem, Helpers.DoomWorld.Models.InstalledLevel)
                Dim level As New Level() With {.Id = item.Id}
                GetLevel(level)

            Else
                Dim item As Helpers.DoomWorld.Models.Folder = DirectCast(selectedItem, Helpers.DoomWorld.Models.Folder)
                _resourcePath = item.Name

                GetFolder(item)
            End If
        End Sub

        ''' <summary>
        ''' Get selected Level information and displays it in the right side ContentPresenter.
        ''' </summary>
        ''' <param name="level"></param>
        Private Async Sub GetLevel(level As Helpers.DoomWorld.Models.Level)
            SelectedLevel = Await _doomworldService.GetLevel(Convert.ToInt32(level.Id))
        End Sub

        ''' <summary>
        ''' Get selected Folder content and displays it in the left side List.
        ''' </summary>
        ''' <param name="folder"></param>
        Private Sub GetFolder(folder As Helpers.DoomWorld.Models.Folder)
            _resourcePath = folder.Name

            Me.GetBrowseResults(_resourcePath)
        End Sub

        ''' <summary>
        ''' Goes back to parent folder.
        ''' </summary>
        Private Async Sub BackToParentDirectory()
            Dim parentDir As Folder = Await _doomworldService.GetParentDirectory(_resourcePath)

            Me.GetFolder(parentDir)
        End Sub

        ''' <summary>
        ''' Enable/disable Parent folder button.
        ''' </summary>
        ''' <param name="resourcePath"></param>
        Private Sub ToggleParentFolderButtonEnabled(resourcePath As String)
            If String.IsNullOrWhiteSpace(resourcePath) OrElse resourcePath = "levels/" Then
                Btn_ParentFolder.IsEnabled = False
            Else
                Btn_ParentFolder.IsEnabled = True
            End If
        End Sub

        ''' <summary>
        ''' Get results from search input text.
        ''' </summary>
        ''' <param name="searchText"></param>
        Private Async Sub GetSearchResults(searchText As String)
            Try
                _lstSearchResults = Await _doomworldService.SearchLevels(searchText)
                If _lstSearchResults Is Nothing OrElse _lstSearchResults.Count = 0 Then
                    Return
                End If

                Lvw_SearchResults.ItemsSource = SortLevels(_lstSearchResults, _selectedSortingCriterion)
                Me.SetTxtResultText($"{_lstSearchResults.Count} results for ""{searchText}""", Txt_Lvw_SearchResults_Count)

            Catch ex As OverflowException
                _lstSearchResults = New List(Of Level)
                Me.SetTxtResultText($"Error: ""{searchText}"" returned too many results (100 max)", Txt_Lvw_SearchResults_Count, True)
                Lvw_SearchResults.ItemsSource = SortLevels(_lstSearchResults, _selectedSortingCriterion)

            Catch ex As Helpers.Exceptions.NoResultsException
                Me.SetTxtResultText($"No results for ""{searchText}""", Txt_Lvw_SearchResults_Count, False)
                Lvw_SearchResults.ItemsSource = Nothing

            Catch ex As Exception
                Me.SetTxtResultText("Error: Something wrong happened", Txt_Lvw_SearchResults_Count, True)
                Lvw_SearchResults.ItemsSource = Nothing

            End Try
        End Sub

        ''' <summary>
        ''' UI code 
        ''' </summary>
        Public Sub AfterLevelInstalled()
            Lvw_BrowseResults.Items.Refresh()
            Me.LoadInstalledLevelsItemsSource()
        End Sub

        ''' <summary>
        ''' Check if level in installed.
        ''' Should be in DoomWorldService. Not a proper way to do. But works for now so "child" SelectedLevel.xaml can call this function.
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns></returns>
        Public Function CheckIsInstalledLevel(id As Long) As Boolean
            If _lstInstalledLevelsResults IsNot Nothing Then
                Return _lstInstalledLevelsResults.Exists(Function(lvl) lvl.Id = id)
            End If

            Return False
        End Function

        ''' <summary>
        ''' Event raised when a sorting RadioButton is checked (Ascending / Descending)
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub Rbtn_Sort_Checked(sender As Object, e As RoutedEventArgs)
            Try
                Dim tabItem As TabItem = CType(Tbc_DWItems.SelectedItem, TabItem)

                Dim isAscending As Boolean = CBool(Rbtn_SortAsc.IsChecked)

                Select Case tabItem.Name
                    Case Tbi_DWBrowse.Name
                        Lvw_BrowseResults.ItemsSource = SortContents(_lstBrowseResults,
                                     DirectCast(Cbb_Sorting.SelectedItem, ComboBoxItem).Content.ToString(), isAscending)
                        SetTxtResultText($"{_lstBrowseResults.Count} results", Txt_Lvw_BrowseResults_Count)

                    Case Tbi_DWSearch.Name
                        Lvw_SearchResults.ItemsSource = SortLevels(Lvw_SearchResults.Items.OfType(Of Level),
                                   DirectCast(Cbb_Sorting.SelectedItem, ComboBoxItem).Content.ToString(), isAscending)
                        SetTxtResultText($"{_lstSearchResults.Count} results", Txt_Lvw_SearchResults_Count)

                    Case Tbi_DWInstalled.Name
                        Lvw_InstalledResults.ItemsSource = SortInstalledLevels(Lvw_InstalledResults.Items.OfType(Of InstalledLevel),
                                   DirectCast(Cbb_Sorting.SelectedItem, ComboBoxItem).Content.ToString(), isAscending)
                        SetTxtResultText($"{_lstInstalledLevelsResults.Count} results", Txt_Lvw_InstalledResults_Count)
                End Select

            Catch ex As Exception
                Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
                WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}{vbCrLf} Parameter(s) : {sender},{e}")
            End Try

        End Sub

        ''' <summary>
        ''' Waits for TabControl to be loaded before adding event on RadioButtons.
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub Tbc_DWItems_Loaded(sender As Object, e As RoutedEventArgs)
            AddHandler Rbtn_SortAsc.Checked, Sub(radioButtonSender, radioButtonEventArgs) Rbtn_Sort_Checked(radioButtonSender, radioButtonEventArgs)
            AddHandler Rbtn_SortDesc.Checked, Sub(radioButtonSender, radioButtonEventArgs) Rbtn_Sort_Checked(radioButtonSender, radioButtonEventArgs)
        End Sub

        ''' <summary>
        ''' Init search process when user presses Enter or clicks Search button.
        ''' </summary>
        Private Sub InitSearch()
            If Txt_DWSearchText.Text.Length < 3 Then
                Me.SetTxtResultText($"You must enter at least 3 characters", Txt_Lvw_SearchResults_Count, True)
            Else
                Me.GetSearchResults(Txt_DWSearchText.Text)
            End If
        End Sub

        ''' <summary>
        ''' When user enters Return key from "Search" Textbox.
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub Txt_DWSearchText_KeyDown(sender As Object, e As KeyEventArgs)
            If e.Key = Key.Return Then
                Me.InitSearch()
            End If
        End Sub

        ''' <summary>
        ''' When user clicks on "Search" button.
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub Btn_DWSearch_Click(sender As Object, e As RoutedEventArgs)
            Me.InitSearch()
        End Sub



        ''' <summary>
        ''' Set "X results" UI (content and color).
        ''' </summary>
        ''' <param name="displayText">Text to be displayed.</param>
        ''' <param name="targetTextBlockControl">Target Textblock</param>
        ''' <param name="isError">True : red text. False : black text.</param>
        Private Sub SetTxtResultText(displayText As String, targetTextBlockControl As TextBlock, Optional isError As Boolean = False)
            targetTextBlockControl.Text = displayText
            targetTextBlockControl.Foreground = If(isError, Windows.Media.Brushes.DarkRed, Windows.Media.Brushes.Black)
        End Sub

        ''' <summary>
        ''' Refresh UI after deleting a level
        ''' </summary>
        Public Sub AfterLevelDeleted()
            SelectedLevel = Nothing

            Me.LoadInstalledLevelsItemsSource()
        End Sub

        ''' <summary>
        ''' Displays (or not) the Level information on the right side when SelectedLevel is set.
        ''' </summary>
        Private Sub ToggleDisplayLevelContent()
            If ctpDisplayLevel IsNot Nothing Then
                If _selectedLevel IsNot Nothing Then
                    ctpDisplayLevel.Content = New Views.UserControls.DoomWorld.SelectedLevel With {.DataContext = SelectedLevel}
                Else
                    ctpDisplayLevel.Content = New Views.UserControls.DoomWorld.NoSelectedLevel
                End If
            End If
        End Sub

    End Class
End Namespace