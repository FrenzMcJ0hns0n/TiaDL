Imports System.Collections.Specialized
Imports System.ComponentModel
Imports System.Drawing
Imports System.IO
Imports System.Reflection
Imports System.Windows.Forms.VisualStyles
Imports Microsoft.Win32
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

        Public Sub New()
            InitializeComponent()

            InitUI()

        End Sub

        Public Function GetDWServiceInstance() As DoomWorldService
            Return _doomworldService
        End Function

        ''' <summary>
        ''' Initializes some UI custom elements.
        ''' </summary>
        Private Sub InitUI()
            ctpDisplayLevel.Content = New Views.UserControls.DoomWorld.NoSelectedLevel

            InitLists()
            UpdateDirPathUI(_resourcePath)

        End Sub

        Private Sub UpdateDirPathUI(resourcePath As String)
            Txt_BrowseDirPath.Text = resourcePath
            Me.ToggleParentFolderButtonEnabled(_resourcePath)
        End Sub

        ''' <summary>
        ''' First initialize elements from DoomWorld API.
        ''' Then prepare results to be displayed as ItemsSource
        ''' </summary>
        Private Async Sub InitLists()
            Try
                _lstBrowseResults = Await _doomworldService.GetContent()
                LoadResultsItemsSource(_lstBrowseResults)
                LoadInstalledLevelsItemsSource()

            Catch ex As Exception
                Dim x As String = ex.Message
                'catch ex

            End Try
        End Sub

        ''' <summary>
        ''' Sorts DoomWorld results and sets as ItemsSource.
        ''' </summary>
        ''' <param name="dwContents"></param>
        Private Sub LoadResultsItemsSource(dwContents As IEnumerable(Of Object))
            If dwContents Is Nothing Or dwContents.Count() = 0 Then
                Me.SetTxtResultText("0 results", Txt_Lvw_BrowseResults_Count)

                Return
            End If

            'Sorting
            dwContents = SortContents(dwContents, _selectedSortingCriterion)

            Lvw_BrowseResults.ItemsSource = dwContents
            Me.SetTxtResultText($"{dwContents.Count().ToString()} results", Txt_Lvw_BrowseResults_Count)
        End Sub

        ''' <summary>
        ''' Sorts DoomWorld installed levels and sets as ItemsSource.
        ''' </summary>
        Private Sub LoadInstalledLevelsItemsSource()
            _lstInstalledLevelsResults = _doomworldService.GetInstalledLevels(Path.Combine("DoomWorld", "doomworld_registry.json"))
            If _lstInstalledLevelsResults Is Nothing OrElse _lstInstalledLevelsResults.Count() = 0 Then
                Me.SetTxtResultText($"0 results", Txt_Lvw_InstalledResults_Count)

                Return
            End If

            Lvw_InstalledResults.ItemsSource = _lstInstalledLevelsResults
            Me.SetTxtResultText($"{_lstInstalledLevelsResults.Count.ToString()} results", Txt_Lvw_InstalledResults_Count)
        End Sub

        ''' <summary>
        ''' Sorts DoomWorld elements by selected attribute 
        ''' <br>Folder : always by Name</br>
        ''' <br>Level : Title, Filename, ReleaseDate...</br>
        ''' </summary>
        ''' <param name="dwContents"></param>
        ''' <param name="selectedSortingCriterion"></param>
        ''' <returns></returns>
        Private Function SortContents(dwContents As IEnumerable(Of Object), selectedSortingCriterion As String, Optional ascending As Boolean = True) As IEnumerable(Of Object)
            If dwContents Is Nothing OrElse dwContents.Count() = 0 Then
                Return dwContents
            End If

            If dwContents.FirstOrDefault().GetType = GetType(Helpers.DoomWorld.Models.Folder) Then
                Return SortFolders(dwContents.OfType(Of Folder), ascending)
            Else
                Return SortLevels(dwContents.OfType(Of Level), selectedSortingCriterion, ascending)
            End If
        End Function

        ''' <summary>
        ''' Sort folders
        ''' </summary>
        ''' <param name="dwContents">List of Folders</param>
        ''' <param name="isAscending">Is ascending</param>
        ''' <returns></returns>
        Private Function SortFolders(dwContents As IEnumerable(Of Folder), Optional isAscending As Boolean = True) As IEnumerable(Of Object)
            Return If(isAscending, dwContents.OrderBy(Function(foldr)
                                                          Return foldr.Name
                                                      End Function),
                                 dwContents.OrderByDescending(Function(foldr)
                                                                  Return foldr.Name
                                                              End Function))
        End Function

        ''' <summary>
        ''' Sort levels regarding sorting criterion and direction.
        ''' </summary>
        ''' <param name="dwContents">List of Levels</param>
        ''' <param name="selectedSortingCriterion">Sorting criterion</param>
        ''' <param name="isAscending">Is ascending</param>
        ''' <returns></returns>
        Private Function SortLevels(dwContents As IEnumerable(Of Level), selectedSortingCriterion As String, Optional isAscending As Boolean = True) As IEnumerable(Of Object)
            Select Case selectedSortingCriterion
                Case "Title"
                    Return If(isAscending, dwContents.OrderBy(Function(levl)
                                                                  Return levl.Title
                                                              End Function),
                                           dwContents.OrderByDescending(Function(levl)
                                                                            Return levl.Title
                                                                        End Function))
                Case "Filename"
                    Return If(isAscending, dwContents.OrderBy(Function(levl)
                                                                  Return levl.Filename
                                                              End Function),
                                           dwContents.OrderByDescending(Function(levl)
                                                                            Return levl.Filename
                                                                        End Function))
                Case "ReleaseDate"
                    Return If(isAscending, dwContents.OrderBy(Function(levl)
                                                                  Return levl.ReleaseDate
                                                              End Function),
                                           dwContents.OrderByDescending(Function(levl)
                                                                            Return levl.ReleaseDate
                                                                        End Function))
                Case "Rating"
                    Return If(isAscending, dwContents.OrderBy(Function(levl)
                                                                  Return levl.Rating
                                                              End Function),
                                           dwContents.OrderByDescending(Function(levl)
                                                                            Return levl.Rating
                                                                        End Function))
                Case "Author"
                    Return If(isAscending, dwContents.OrderBy(Function(levl)
                                                                  Return levl.Author
                                                              End Function),
                                           dwContents.OrderByDescending(Function(levl)
                                                                            Return levl.Author
                                                                        End Function))
                Case "Size"
                    Return If(isAscending, dwContents.OrderBy(Function(levl)
                                                                  Return levl.Size
                                                              End Function),
                                           dwContents.OrderByDescending(Function(levl)
                                                                            Return levl.Size
                                                                        End Function))
                Case Else
                    Return dwContents
            End Select
        End Function

        ''' <summary>
        ''' Sort installed levels regarding sorting criterion and direction.
        ''' </summary>
        ''' <param name="dwContents">List of Levels</param>
        ''' <param name="selectedSortingCriterion">Sorting criterion</param>
        ''' <param name="isAscending">Is ascending</param>
        ''' <returns></returns>
        Private Function SortInstalledLevels(dwContents As IEnumerable(Of InstalledLevel), selectedSortingCriterion As String, Optional isAscending As Boolean = True) As IEnumerable(Of Object)
            Select Case selectedSortingCriterion
                Case "Title"
                    Return If(isAscending, dwContents.OrderBy(Function(levl)
                                                                  Return levl.Title
                                                              End Function),
                                           dwContents.OrderByDescending(Function(levl)
                                                                            Return levl.Title
                                                                        End Function))

                Case "Filename"
                    Return If(isAscending, dwContents.OrderBy(Function(levl)
                                                                  Return levl.FileName
                                                              End Function),
                                           dwContents.OrderByDescending(Function(levl)
                                                                            Return levl.FileName
                                                                        End Function))
                Case Else
                    Return dwContents
            End Select
        End Function

        Private Sub Btn_ParentFolder_Click(sender As Object, e As RoutedEventArgs) Handles Btn_ParentFolder.Click
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
                Dim item As Helpers.DoomWorld.Models.Level = CType(selectedItem, Helpers.DoomWorld.Models.Level)
                GetLevel(item)

            ElseIf selectedItem.GetType() Is GetType(Helpers.DoomWorld.Models.InstalledLevel) Then
                Dim item As Helpers.DoomWorld.Models.InstalledLevel = CType(selectedItem, Helpers.DoomWorld.Models.InstalledLevel)
                Dim level As New Level() With {.Id = item.Id}
                GetLevel(level)

            Else
                Dim item As Helpers.DoomWorld.Models.Folder = CType(selectedItem, Helpers.DoomWorld.Models.Folder)
                _resourcePath = item.Name
                ' + show path in UI
                GetFolder(item)
            End If
        End Sub

        ''' <summary>
        ''' Get selected Level information and displays it in the right side ContentPresenter.
        ''' </summary>
        ''' <param name="level"></param>
        Private Async Sub GetLevel(level As Helpers.DoomWorld.Models.Level)
            Dim selectedLevel As Helpers.DoomWorld.Models.Level = Await _doomworldService.GetLevel(Convert.ToInt32(level.Id))
            ctpDisplayLevel.Content = New Views.UserControls.DoomWorld.SelectedLevel With {.DataContext = selectedLevel}
        End Sub

        ''' <summary>
        ''' Get selected Folder content and displays it in the left side List.
        ''' </summary>
        ''' <param name="folder"></param>
        Private Async Sub GetFolder(folder As Helpers.DoomWorld.Models.Folder)
            _resourcePath = folder.Name
            _lstBrowseResults = Await _doomworldService.GetContent(_resourcePath)
            LoadResultsItemsSource(_lstBrowseResults)

            Me.UpdateDirPathUI(_resourcePath)
        End Sub

        Private Async Sub BackToParentDirectory()
            Dim parentDir As Folder = Await _doomworldService.GetParentDirectory(_resourcePath)

            Me.GetFolder(parentDir)
        End Sub

        ''' <summary>
        ''' Enable/disable Parent folder button.
        ''' </summary>
        ''' <param name="resourcePath"></param>
        Private Sub ToggleParentFolderButtonEnabled(resourcePath As String)
            If String.IsNullOrWhiteSpace(resourcePath) Or resourcePath = "levels/" Then
                Btn_ParentFolder.IsEnabled = False
            Else
                Btn_ParentFolder.IsEnabled = True
            End If
        End Sub

        Private Async Sub GetSearchResults(searchText As String)
            Try
                _lstSearchResults = Await _doomworldService.SearchLevels(searchText)
                If _lstSearchResults Is Nothing OrElse _lstSearchResults.Count = 0 Then
                    ' TODO : Display no results view ?
                    Return
                End If

                Lvw_SearchResults.ItemsSource = SortLevels(_lstSearchResults, _selectedSortingCriterion)
                Me.SetTxtResultText($"{_lstSearchResults.Count.ToString()} results", Txt_Lvw_SearchResults_Count)
            Catch ex As OverflowException
                _lstSearchResults = New List(Of Level)
                Me.SetTxtResultText("Error : Too many results (100 max)", Txt_Lvw_SearchResults_Count, True)
                Lvw_SearchResults.ItemsSource = SortLevels(_lstSearchResults, _selectedSortingCriterion)
            Catch ex As Exception

            End Try
        End Sub


        Public Sub AfterLevelInstalled()
            ' TODO : add level installed icon tick in Lvw_BrowseList ?
            ' -> CType(Lvw_BrowseResults.SelectedItem, Level).Installed = True

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
                Return _lstInstalledLevelsResults.Exists(Function(lvl)
                                                             Return lvl.Id = id
                                                         End Function)
            End If

            Return False
        End Function

        ''' <summary>
        ''' Event raised when a sorting RadioButton is checked (Ascending, Descending)
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
                        SetTxtResultText($"{_lstBrowseResults.Count().ToString()} results", Txt_Lvw_BrowseResults_Count)
                    Case Tbi_DWSearch.Name
                        Lvw_SearchResults.ItemsSource = SortLevels(Lvw_SearchResults.Items.OfType(Of Level),
                                   DirectCast(Cbb_Sorting.SelectedItem, ComboBoxItem).Content.ToString(), isAscending)
                        SetTxtResultText($"{_lstSearchResults.Count().ToString()} results", Txt_Lvw_SearchResults_Count)
                    Case Else 'Tbi_DWInstalled.Name
                        Lvw_InstalledResults.ItemsSource = SortInstalledLevels(Lvw_InstalledResults.Items.OfType(Of InstalledLevel),
                                   DirectCast(Cbb_Sorting.SelectedItem, ComboBoxItem).Content.ToString(), isAscending)
                        SetTxtResultText($"{_lstInstalledLevelsResults.Count().ToString()} results", Txt_Lvw_InstalledResults_Count)
                End Select

            Catch ex As Exception
                Dim currentMethodName As String = MethodBase.GetCurrentMethod().Name
                WriteToLog($"{Date.Now} - Error in '{currentMethodName}'{vbCrLf} Exception : {ex}{vbCrLf} Parameter(s) : {sender},{e}")
            End Try

        End Sub

        ''' <summary>
        ''' Waits for TabControl to be loaded before adding event on RadioButtons
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub Tbc_DWItems_Loaded(sender As Object, e As RoutedEventArgs)
            AddHandler Rbtn_SortAsc.Checked, Sub(radioButtonSender, radioButtonEventArgs) Rbtn_Sort_Checked(radioButtonSender, radioButtonEventArgs)
            AddHandler Rbtn_SortDesc.Checked, Sub(radioButtonSender, radioButtonEventArgs) Rbtn_Sort_Checked(radioButtonSender, radioButtonEventArgs)
        End Sub

        Private Sub Txt_DWSearchText_KeyDown(sender As Object, e As KeyEventArgs)
            If e.Key = Key.Return Then
                If Txt_DWSearchText.Text.Length >= 3 Then
                    Me.GetSearchResults(Txt_DWSearchText.Text)
                End If
            End If
        End Sub

        Private Sub Btn_DWSearch_Click(sender As Object, e As RoutedEventArgs)
            If Txt_DWSearchText.Text.Length >= 3 Then
                Me.GetSearchResults(Txt_DWSearchText.Text)
            End If
        End Sub

        Private Sub SetTxtResultText(text As String, txtControl As TextBlock, Optional isError As Boolean = False)
            txtControl.Text = text
            txtControl.Foreground = If(isError, Windows.Media.Brushes.Red, Windows.Media.Brushes.Black)
        End Sub
    End Class
End Namespace