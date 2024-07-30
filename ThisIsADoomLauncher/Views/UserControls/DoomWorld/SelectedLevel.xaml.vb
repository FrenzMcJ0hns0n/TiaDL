Imports System.Diagnostics.Eventing.Reader
Imports System.IO
Imports ThisIsADoomLauncher.Helpers.DoomWorld
Imports ThisIsADoomLauncher.Helpers.DoomWorld.Models
Imports ThisIsADoomLauncher.Models

Namespace Views.UserControls.DoomWorld
    Public Class SelectedLevel

        Private _doomWorldService As DoomWorldService
        Private _doomWorldWindow As Views.DoomWorldWindow
        Private _currentLevel As Level
        Public Sub New()
            InitializeComponent()

            Txt_OpenFileExplorer.Visibility = Visibility.Collapsed

            _doomWorldWindow = Application.Current.Windows.OfType(Of Views.DoomWorldWindow).FirstOrDefault()

            _doomWorldService = _doomWorldWindow.GetDWServiceInstance()
        End Sub

        ''' <summary>
        ''' Used to get DataContext (Level) when the view has finished loading.
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub Usc_SelectedLevel_Loaded(sender As Object, e As RoutedEventArgs)
            _currentLevel = CType(Me.DataContext, Level)

            If _currentLevel Is Nothing Then
                Return
            End If

            If _doomWorldWindow.CheckIsInstalledLevel(_currentLevel.Id) Then
                Me.UpdateUIInstalledLevel()
            End If
        End Sub

        Private Sub Img_DownloadLevel_MouseDown(sender As Object, e As MouseButtonEventArgs) Handles Img_DownloadLevel.MouseDown
            Me.DownloadLevel(_currentLevel)
        End Sub

        Private Sub Txt_DownloadLevel_MouseDown(sender As Object, e As MouseButtonEventArgs) Handles Txt_DownloadLevel.MouseDown
            Me.DownloadLevel(_currentLevel)
        End Sub

        Private Async Sub DownloadLevel(levl As Level)

            Me.BeginUILevelDownload()

            Dim result As Boolean = Await _doomWorldService.DownloadExtractLevel(levl)

            Me.EndUILevelDownload(result)

            Me.AfterLevelInstalled()
        End Sub

        Private Sub AfterLevelInstalled()
            _doomWorldWindow.AfterLevelInstalled()
        End Sub

        ''' <summary>
        ''' Show downloading UI.
        ''' </summary>
        Private Sub BeginUILevelDownload()
            Stk_LevelDownload.Visibility = Visibility.Visible
            Pgb_LevelDownload.IsIndeterminate = True
            Txt_LevelDownload.Text = "Downloading..."
        End Sub

        ''' <summary>
        ''' Show downloading UI result.
        ''' </summary>
        Private Sub EndUILevelDownload(result As Boolean)
            Stk_LevelDownload.Visibility = Visibility.Visible

            Pgb_LevelDownload.IsIndeterminate = False
            Pgb_LevelDownload.Value = 100

            If result Then
                Txt_LevelDownload.Text = "Success!"
                Me.UpdateUIInstalledLevel()
            Else
                Pgb_LevelDownload.Foreground = Brushes.Red
                Txt_LevelDownload.Text = "Level download failed"
            End If
        End Sub

        Private Sub UpdateUIInstalledLevel()
            Img_DownloadLevel.Source = New BitmapImage(New Uri("/Resources/Images/doomworld_installed.png", UriKind.Relative))
            Txt_DownloadLevel.Text = "Downloaded"
            RemoveHandler Img_DownloadLevel.MouseDown, AddressOf Img_DownloadLevel_MouseDown
            RemoveHandler Txt_DownloadLevel.MouseDown, AddressOf Txt_DownloadLevel_MouseDown

            Txt_OpenFileExplorer.Visibility = Visibility.Visible
            Stk_DownloadLevel.ClearValue(StackPanel.CursorProperty) 'Reset cursor icon (Hand) to default
        End Sub

        Private Sub Url_OpenBrowser_Click(sender As Object, e As RoutedEventArgs)
            _doomWorldService.OpenInBrowser(_currentLevel)
        End Sub

        Private Sub Btn_DeleteLevel_Click(sender As Object, e As RoutedEventArgs)

        End Sub

        Private Sub Txt_OpenFileExplorer_Click(sender As Object, e As RoutedEventArgs)
            Try
                _doomWorldService.OpenInFileExplorer(_currentLevel)
            Catch ex As Exception
                Dim errDirectoryResult As MessageBoxResult = MessageBox.Show(
                    $"This folder cannot be found.{vbCrLf}Maybe it was moved or deleted.{vbCrLf}{vbCrLf}Remove it from the list?",
                    "Folder not found",
                    MessageBoxButton.YesNo, MessageBoxImage.Error, MessageBoxResult.No
                )

                If errDirectoryResult = MessageBoxResult.Yes Then
                    _doomWorldService.DeleteLevel(_currentLevel)

                    _doomWorldWindow.AfterLevelDeleted()
                End If
            End Try

        End Sub


    End Class
End Namespace