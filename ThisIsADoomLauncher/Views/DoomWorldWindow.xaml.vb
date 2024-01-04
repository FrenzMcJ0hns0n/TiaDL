Imports System.ComponentModel
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

        Private Sub InitUI()
            ctpDisplayLevel.Content = New Views.UserControls.DoomWorld.NoSelectedLevel
            Me.ToggleBackButtonEnabled(_resourcePath)
        End Sub

        Private Async Sub InitDoomWorldList()
            Try
                lstResults.ItemsSource = Await _doomworldService.GetContent()
            Catch ex As Exception
                'catch ex

            End Try
        End Sub

        Private Sub lstResults_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles lstResults.SelectionChanged
            Dim lst As ListView = CType(sender, ListView)
            If lst.SelectedIndex <> -1 Then
                Me.HandleSelectedItem(lst.SelectedItem)
            End If

        End Sub

        Private Sub cbbSorting_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cbbSorting.SelectionChanged
            Dim cbb As ComboBox = CType(sender, ComboBox)
            If cbb.SelectedIndex <> -1 Then
                _selectedSortingMode = cbb.SelectedItem.ToString()
            End If
        End Sub

        Private Sub btnParentFolder_Click(sender As Object, e As RoutedEventArgs) Handles btnParentFolder.Click
            Me.BackToParentDirectory()
        End Sub

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

        Private Async Sub GetLevel(level As Helpers.DoomWorld.Models.Level)
            Dim selectedLevel As Helpers.DoomWorld.Models.Level = Await _doomworldService.GetLevel(Convert.ToInt32(level.Id))
            ctpDisplayLevel.Content = New Views.UserControls.DoomWorld.SelectedLevel With {.DataContext = selectedLevel}
        End Sub

        Private Async Sub GetFolder(folder As Helpers.DoomWorld.Models.Folder)
            _resourcePath = folder.Name
            lstResults.ItemsSource = Await _doomworldService.GetContent(_resourcePath)

            Me.ToggleBackButtonEnabled(_resourcePath)
        End Sub

        Private Async Sub BackToParentDirectory()
            Dim parentDir As Folder = Await _doomworldService.GetParentDirectory(_resourcePath)

            Me.GetFolder(parentDir)
        End Sub

        Private Sub ToggleBackButtonEnabled(resourcePath As String)
            If String.IsNullOrWhiteSpace(resourcePath) Or resourcePath = "levels/" Then
                btnParentFolder.IsEnabled = False
            Else
                btnParentFolder.IsEnabled = True
            End If
        End Sub
    End Class







    Public Class DoomWorldWindowViewModel
        Implements INotifyPropertyChanged

        Public Event PropertyChanged As PropertyChangedEventHandler _
        Implements INotifyPropertyChanged.PropertyChanged

        Private Sub NotifyPropertyChanged(ByVal info As String)
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(info))
        End Sub

        'Private Properties 
        Private _doomworldService As DoomWorldService

        Private _resourcePath As String
        Private _sortingModes As List(Of String)
        Private _contentsList As List(Of Object)
        Private _selectedSortingMode As String
        Private _listViewSelectedItem As Object
        Private _comboboxSortingSelectedItem As Object
        Private _selectedLevel As Helpers.DoomWorld.Models.Level

        'PropertyChange Properties 
        Public Property ResourcePath() As String
            Get
                Return Me._resourcePath
            End Get

            Set(ByVal value As String)
                If Not (value Is _resourcePath) Then
                    Me._resourcePath = value
                    NotifyPropertyChanged("ResourcePath")
                End If
            End Set
        End Property

        Public Property SortingModes() As List(Of String)
            Get
                Return Me._sortingModes
            End Get

            Set(ByVal value As List(Of String))
                If Not (value Is _sortingModes) Then
                    Me._sortingModes = value
                    NotifyPropertyChanged("SortingModes")
                End If
            End Set
        End Property

        Public Property ContentsList() As List(Of Object)
            Get
                Return Me._contentsList
            End Get

            Set(ByVal value As List(Of Object))
                If Not (value Is _contentsList) Then
                    Me._contentsList = value
                    NotifyPropertyChanged("ContentsList")
                End If
            End Set
        End Property

        Public Property SelectedLevel() As Helpers.DoomWorld.Models.Level
            Get
                Return Me._selectedLevel
            End Get

            Set(ByVal value As Helpers.DoomWorld.Models.Level)
                If Not (value Is _selectedLevel) Then
                    Me._selectedLevel = value
                    NotifyPropertyChanged("SelectedLevel")
                End If
            End Set
        End Property

        Public Property SelectedSortingMode() As String
            Get
                Return Me._selectedSortingMode
            End Get

            Set(ByVal value As String)
                If Not (value Is _selectedSortingMode) Then
                    Me._selectedSortingMode = value
                    NotifyPropertyChanged("SelectedSortingMode")
                End If
            End Set
        End Property

        Public Property ListViewSelectedItem() As Object
            Get
                Return Me._listViewSelectedItem
            End Get

            Set(ByVal value As Object)
                If Not (value Is _listViewSelectedItem) And Not (value Is Nothing) Then
                    Me._listViewSelectedItem = value
                    Me.HandleSelectedItem()
                End If
            End Set
        End Property

        Public Property ComboboxSelectedItem() As Object
            Get
                Return Me._comboboxSortingSelectedItem
            End Get

            Set(ByVal value As Object)
                If Not (value Is _comboboxSortingSelectedItem) And Not (value Is Nothing) Then
                    Me._comboboxSortingSelectedItem = value
                    SelectedSortingMode = CType(_comboboxSortingSelectedItem, String)
                End If
            End Set
        End Property

        Public Sub New()
            SortingModes = New List(Of String) From {"filename", "title", "description"}
            SelectedSortingMode = SortingModes.ElementAt(0)

            _doomworldService = New DoomWorldService()

            InitDoomWorldList()
        End Sub

        Private Async Sub InitDoomWorldList()
            Try
                ContentsList = Await _doomworldService.GetContent()
            Catch ex As Exception
                'catch ex
                ContentsList = Nothing
            End Try
        End Sub

        Public Sub ButtonParentFolderClicked()
            BackToParentDirectory()
        End Sub

        Private Async Sub BackToParentDirectory()
            'ResourcePath = Await _doomworldService.GetParentDirectory(ResourcePath)
            ContentsList = Await _doomworldService.GetContent(ResourcePath)
        End Sub

        Private Sub HandleSelectedItem()

            If ListViewSelectedItem.GetType() Is GetType(Helpers.DoomWorld.Models.Level) Then
                Dim item As Helpers.DoomWorld.Models.Level = CType(ListViewSelectedItem, Helpers.DoomWorld.Models.Level)
                GetLevel(item)
            Else
                Dim item As Helpers.DoomWorld.Models.Folder = CType(ListViewSelectedItem, Helpers.DoomWorld.Models.Folder)
                ResourcePath = item.Name
                ' + show path in UI
                GetFolder(item)
            End If
        End Sub

        Private Async Sub GetLevel(level As Helpers.DoomWorld.Models.Level)
            'get in cache
            ', if not : ↓↓
            SelectedLevel = Await _doomworldService.GetLevel(Convert.ToInt32(level.Id))
        End Sub

        Private Async Sub GetFolder(folder As Helpers.DoomWorld.Models.Folder)
            'get in cache
            ', if not : ↓↓
            ContentsList = Await _doomworldService.GetContent(folder.Name)
        End Sub

    End Class

End Namespace