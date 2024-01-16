Imports System.IO
Imports ThisIsADoomLauncher.Helpers.DoomWorld
Imports ThisIsADoomLauncher.Helpers.DoomWorld.Models

Namespace Views.UserControls.DoomWorld
    Public Class SelectedLevel

        Private _doomWorldService As DoomWorldService
        Public Sub New()

            ' This call is required by the designer.
            InitializeComponent()

            _doomWorldService = New DoomWorldService()
            ' Add any initialization after the InitializeComponent() call.

        End Sub

        Private Sub Img_DownloadLevel_MouseDown(sender As Object, e As MouseButtonEventArgs) Handles Img_DownloadLevel.MouseDown
            Dim imageControl As Image = CType(sender, Image)
            Dim levl As Level = CType(imageControl.DataContext, Level)

            Me.DownloadLevel(levl)
        End Sub

        Private Async Sub DownloadLevel(levl As Level)

            Me.BeginUILevelDownload()

            Dim result As Boolean = Await _doomWorldService.DownloadLevelFull(levl)

            Me.EndUILevelDownload(result)

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
                Img_DownloadLevel.Source = New BitmapImage(New Uri("pack://application:,,,/Resources/Images/doomworld_installed.png"))
            Else
                Pgb_LevelDownload.Foreground = Brushes.Red
                Txt_LevelDownload.Text = "Level download failed"
            End If
        End Sub
    End Class
End Namespace