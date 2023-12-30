Imports System.ComponentModel
Imports Microsoft.Win32
Imports ThisIsADoomLauncher.Helpers.DoomWorld

Namespace Views
    Public Class DoomWorldWindow

        Private _doomWorldWindowViewModel As DoomWorldWindowViewModel

        Public Sub New()
            InitializeComponent()

            _doomWorldWindowViewModel = New DoomWorldWindowViewModel
            Me.DataContext = _doomWorldWindowViewModel
        End Sub

        Private Sub lstResults_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles lstResults.SelectionChanged
            Dim lst As ListView = CType(sender, ListView)
            If lst.SelectedIndex <> -1 Then
                _doomWorldWindowViewModel.SelectedItem = lst.SelectedItem
            Else
                _doomWorldWindowViewModel.SelectedItem = Nothing
            End If

        End Sub

        Private Sub btnParentFolder_Click(sender As Object, e As RoutedEventArgs) Handles btnParentFolder.Click
            _doomWorldWindowViewModel.ButtonParentFolderClicked()
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
        Private _contentsList As List(Of Object)
        Private _selectedItem As Object
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

        Public Property SelectedItem() As Object
            Get
                Return Me._selectedItem
            End Get

            Set(ByVal value As Object)
                If Not (value Is _selectedItem) And Not (value Is Nothing) Then
                    Me._selectedItem = value
                    Me.HandleSelectedItem()
                End If
            End Set
        End Property

        Public Sub New()
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
            ResourcePath = Await _doomworldService.GetParentDirectory(ResourcePath)
            ContentsList = Await _doomworldService.GetContent(ResourcePath)
        End Sub

        Private Sub HandleSelectedItem()

            If SelectedItem.GetType() Is GetType(Helpers.DoomWorld.Models.Level) Then
                Dim item As Helpers.DoomWorld.Models.Level = CType(SelectedItem, Helpers.DoomWorld.Models.Level)
                GetLevel(item)
            Else
                Dim item As Helpers.DoomWorld.Models.Folder = CType(SelectedItem, Helpers.DoomWorld.Models.Folder)
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