Imports System.ComponentModel
Imports System.IO
Imports System.Windows.Forms.VisualStyles
Imports Microsoft.Win32
Imports ThisIsADoomLauncher.Helpers.DoomWorld
Imports ThisIsADoomLauncher.Helpers.DoomWorld.Models

Namespace Views
    Public Class DoomWorldWindow

        Private _doomworldService As New DoomWorldService()
        Private _resourcePath As String
        Private _selectedSortingMode As String

        Public Sub New()
            InitializeComponent()

            InitUI()
            InitDoomWorldList()

        End Sub

        ''' <summary>
        ''' Initializes some UI custom elements.
        ''' </summary>
        Private Sub InitUI()
            ctpDisplayLevel.Content = New Views.UserControls.DoomWorld.NoSelectedLevel
            Me.ToggleParentFolderButtonEnabled(_resourcePath)
        End Sub

        ''' <summary>
        ''' First initialize elements from DoomWorld API.
        ''' Then prepare results to be displayed as ItemsSource
        ''' </summary>
        Private Async Sub InitDoomWorldList()
            Try
                Dim dwContents As IEnumerable(Of Object) = Await _doomworldService.GetContent()
                LoadResultsItemsSource(dwContents)
            Catch ex As Exception
                'catch ex

            End Try
        End Sub

        ''' <summary>
        ''' Sorts DoomWorld results and sets as ItemsSource.
        ''' </summary>
        ''' <param name="dwContents"></param>
        Private Sub LoadResultsItemsSource(dwContents As IEnumerable(Of Object))
            If dwContents Is Nothing Or dwContents.Count() = 0 Then
                Return
            End If

            'Sorting
            dwContents = SortContents(dwContents, _selectedSortingMode)

            lstResults.ItemsSource = dwContents
        End Sub

        ''' <summary>
        ''' Sorts DoomWorld elements by selected attribute 
        ''' <br>Folder : always by Name</br>
        ''' <br>Level : Title, Filename, ReleaseDate...</br>
        ''' </summary>
        ''' <param name="dwContents"></param>
        ''' <param name="selectedSortingMode"></param>
        ''' <returns></returns>
        Private Function SortContents(dwContents As IEnumerable(Of Object), selectedSortingMode As String) As IEnumerable(Of Object)

            If dwContents.FirstOrDefault().GetType = GetType(Helpers.DoomWorld.Models.Folder) Then
                Return SortFolders(dwContents.OfType(Of Folder))
            Else
                Return SortLevels(dwContents.OfType(Of Level), selectedSortingMode)
            End If
        End Function

        Private Function SortFolders(dwContents As IEnumerable(Of Folder)) As IEnumerable(Of Object)
            Return dwContents.OrderBy(Function(foldr)
                                          Return foldr.Name
                                      End Function)
        End Function

        Private Function SortLevels(dwContents As IEnumerable(Of Level), selectedSortingMode As String) As IEnumerable(Of Object)
            Select Case selectedSortingMode
                Case "Title"
                    Return dwContents.OrderBy(Function(levl)
                                                  Return levl.Title
                                              End Function)
                Case "Filename"
                    Return dwContents.OrderBy(Function(levl)
                                                  Return levl.Filename
                                              End Function)
                Case "ReleaseDate"
                    Return dwContents.OrderBy(Function(levl)
                                                  Return levl.ReleaseDate
                                              End Function)
                Case "Rating"
                    Return dwContents.OrderBy(Function(levl)
                                                  Return levl.Rating
                                              End Function)
                Case "Author"
                    Return dwContents.OrderBy(Function(levl)
                                                  Return levl.Author
                                              End Function)
                Case "Size"
                    Return dwContents.OrderBy(Function(levl)
                                                  Return levl.Size
                                              End Function)
                Case Else
                    Return dwContents
            End Select
        End Function

        Private Sub lstResults_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles lstResults.SelectionChanged, Lvw_SearchResults.SelectionChanged
            Dim lst As ListView = CType(sender, ListView)
            If lst.SelectedIndex <> -1 Then
                Me.HandleSelectedItem(lst.SelectedItem)
            End If

        End Sub

        Private Sub Cbb_Sorting_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles Cbb_Sorting.SelectionChanged
            Dim cbb As ComboBox = CType(sender, ComboBox)
            If cbb.SelectedIndex <> -1 Then
                _selectedSortingMode = DirectCast(cbb.SelectedItem, ComboBoxItem).Content.ToString()

                SortAndRefreshAllLists()
            End If
        End Sub

        ''' <summary>
        ''' Sort and refresh all lists after Combobox sorting option has changed.
        ''' </summary>
        Private Sub SortAndRefreshAllLists()
            If lstResults IsNot Nothing And lstResults?.Items IsNot Nothing And lstResults?.Items?.Count <> 0 Then
                Me.LoadResultsItemsSource(lstResults.Items.OfType(Of Object))
            End If

            If Lvw_SearchResults IsNot Nothing And Lvw_SearchResults?.Items IsNot Nothing And Lvw_SearchResults?.Items?.Count <> 0 Then
                Lvw_SearchResults.ItemsSource = SortLevels(Lvw_SearchResults.Items.OfType(Of Level), _selectedSortingMode)
            End If

            If Lvw_InstalledResults IsNot Nothing And Lvw_InstalledResults?.Items IsNot Nothing And Lvw_InstalledResults?.Items?.Count <> 0 Then
                Lvw_InstalledResults.ItemsSource = SortLevels(Lvw_InstalledResults.Items.OfType(Of Level), _selectedSortingMode)
            End If
        End Sub

        Private Sub btnParentFolder_Click(sender As Object, e As RoutedEventArgs) Handles Btn_ParentFolder.Click
            Me.BackToParentDirectory()
        End Sub

        ''' <summary>
        ''' Defines if SelectedItem is Folder or Level, and acts accordingly
        ''' </summary>
        ''' <param name="selectedItem"></param>
        Private Sub HandleSelectedItem(selectedItem As Object)

            If selectedItem.GetType() Is GetType(Helpers.DoomWorld.Models.Level) Then
                Dim item As Helpers.DoomWorld.Models.Level = CType(selectedItem, Helpers.DoomWorld.Models.Level)
                GetLevel(item)
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
            Dim dwContent As List(Of Object) = Await _doomworldService.GetContent(_resourcePath)
            LoadResultsItemsSource(dwContent)

            Me.ToggleParentFolderButtonEnabled(_resourcePath)
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

        Private Sub Txt_DWSearchText_TextChanged(sender As Object, e As TextChangedEventArgs) Handles Txt_DWSearchText.TextChanged
            Dim tbx As TextBox = CType(sender, TextBox)

            If tbx.Text.Length >= 3 Then
                Me.GetSearchResults(tbx.Text)
            End If
        End Sub

        Private Async Sub GetSearchResults(searchText As String)

            Dim searchLevels As List(Of Level) = Await _doomworldService.SearchLevels(searchText)
            If searchLevels Is Nothing Or searchLevels.Count = 0 Then
                'Display no results
            End If

            '↓↓ WRONG : Use itemssource method to sort elements before set ItemsSource
            Lvw_SearchResults.ItemsSource = SortLevels(searchLevels, _selectedSortingMode)
            '↑↑ WRONG : Use itemssource method to sort elements before set ItemsSource
        End Sub
    End Class
End Namespace