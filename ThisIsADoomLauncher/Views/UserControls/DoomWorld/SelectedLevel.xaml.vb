Imports System.IO
Imports ThisIsADoomLauncher.Helpers.DoomWorld
Imports ThisIsADoomLauncher.Helpers.DoomWorld.Models

Namespace Views.UserControls.DoomWorld
    Public Class SelectedLevel

        Private _doomWorldService As DoomWorldService
        Private _doomWorldWindow As Views.DoomWorldWindow
        Private _currentLevel As Level
        Public Sub New()

            ' This call is required by the designer.
            InitializeComponent()

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

            If _doomWorldWindow.CheckIsInstalledLevel(_currentLevel.Id) Then
                Me.UpdateUIInstalledLevel()
            End If
        End Sub

        Private Sub Img_DownloadLevel_MouseDown(sender As Object, e As MouseButtonEventArgs) Handles Img_DownloadLevel.MouseDown
            Me.DownloadLevel(_currentLevel)
        End Sub

        Private Async Sub DownloadLevel(levl As Level)

            Me.BeginUILevelDownload()

            Dim result As Boolean = Await _doomWorldService.DownloadLevelFull(levl)

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
            RemoveHandler Img_DownloadLevel.MouseDown, AddressOf Img_DownloadLevel_MouseDown
        End Sub

        Private Sub Img_OpenBrowser_MouseDown(sender As Object, e As MouseButtonEventArgs) Handles Img_OpenBrowser.MouseDown
            _doomWorldService.OpenInBrowser(_currentLevel)
        End Sub
    End Class
End Namespace