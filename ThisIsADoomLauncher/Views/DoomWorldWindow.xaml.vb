Imports System.ComponentModel
Imports System.IO
Imports System.Windows.Forms.VisualStyles
Imports Microsoft.Win32
Imports ThisIsADoomLauncher.Helpers.DoomWorld
Imports ThisIsADoomLauncher.Helpers.DoomWorld.Models

Namespace Views
    Public Class DoomWorldWindow

        Private _doomworldService As New DoomWorldService()
        Private _resourcePath As String = "levels/"
        Private _selectedSortingMode As String
        Private _installedLevels As List(Of InstalledLevel)

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
                Dim dwContents As IEnumerable(Of Object) = Await _doomworldService.GetContent()
                LoadResultsItemsSource(dwContents)
                LoadInstalledLevelsItemsSource()

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

            Lvw_BrowseResults.ItemsSource = dwContents
        End Sub

        ''' <summary>
        ''' Sorts DoomWorld installed levels and sets as ItemsSource.
        ''' </summary>
        Private Sub LoadInstalledLevelsItemsSource()
            _installedLevels = _doomworldService.GetInstalledLevels(Path.Combine("DoomWorld", "doomworld_registry.json"))
            If _installedLevels Is Nothing OrElse _installedLevels.Count() = 0 Then
                Return
            End If

            Lvw_InstalledResults.ItemsSource = _installedLevels
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

        Private Function SortInstalledLevels(dwContents As IEnumerable(Of InstalledLevel), selectedSortingMode As String) As IEnumerable(Of Object)
            Select Case selectedSortingMode
                Case "Title"
                    Return dwContents.OrderBy(Function(levl)
                                                  Return levl.Title
                                              End Function)
                Case "Filename"
                    Return dwContents.OrderBy(Function(levl)
                                                  Return levl.FileName
                                              End Function)
                Case Else
                    Return dwContents
            End Select
        End Function

        Private Sub btnParentFolder_Click(sender As Object, e As RoutedEventArgs) Handles Btn_ParentFolder.Click
            Me.BackToParentDirectory()
        End Sub

        Private Sub Lvw_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles Lvw_BrowseResults.SelectionChanged, Lvw_SearchResults.SelectionChanged, Lvw_InstalledResults.SelectionChanged
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
            If Lvw_BrowseResults IsNot Nothing AndAlso Lvw_BrowseResults.Items IsNot Nothing AndAlso Lvw_BrowseResults.Items.Count <> 0 Then
                Me.LoadResultsItemsSource(Lvw_BrowseResults.Items.OfType(Of Object))
            End If

            If Lvw_SearchResults IsNot Nothing AndAlso Lvw_SearchResults.Items IsNot Nothing AndAlso Lvw_SearchResults.Items.Count <> 0 Then
                Lvw_SearchResults.ItemsSource = SortLevels(Lvw_SearchResults.Items.OfType(Of Level), _selectedSortingMode)
            End If

            If Lvw_InstalledResults IsNot Nothing AndAlso Lvw_InstalledResults.Items IsNot Nothing AndAlso Lvw_InstalledResults.Items.Count <> 0 Then
                Lvw_InstalledResults.ItemsSource = SortInstalledLevels(Lvw_InstalledResults.Items.OfType(Of InstalledLevel), _selectedSortingMode)
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
            Dim dwContent As List(Of Object) = Await _doomworldService.GetContent(_resourcePath)
            LoadResultsItemsSource(dwContent)

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

        Private Sub Txt_DWSearchText_TextChanged(sender As Object, e As TextChangedEventArgs) Handles Txt_DWSearchText.TextChanged
            Dim tbx As TextBox = CType(sender, TextBox)

            If tbx.Text.Length >= 3 Then
                Me.GetSearchResults(tbx.Text)
            End If
        End Sub

        Private Async Sub GetSearchResults(searchText As String)

            Dim searchLevels As List(Of Level) = Await _doomworldService.SearchLevels(searchText)
            If searchLevels Is Nothing OrElse searchLevels.Count = 0 Then
                ' TODO : Display no results view ?
                Return
            End If

            Lvw_SearchResults.ItemsSource = SortLevels(searchLevels, _selectedSortingMode)
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
            If _installedLevels IsNot Nothing Then
                Return _installedLevels.Exists(Function(lvl)
                                                   Return lvl.Id = id
                                               End Function)
            End If

            Return False
        End Function
    End Class
End Namespace