﻿Imports System.Diagnostics.Eventing.Reader
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

            ' This call is required by the designer.
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
            Txt_DownloadLevel.Text = "Downloaded"
            RemoveHandler Img_DownloadLevel.MouseDown, AddressOf Img_DownloadLevel_MouseDown

            Txt_OpenFileExplorer.Visibility = Visibility.Visible

        End Sub

        Private Sub Url_OpenBrowser_Click(sender As Object, e As RoutedEventArgs)
            _doomWorldService.OpenInBrowser(_currentLevel)
        End Sub

        Private Sub Btn_CreatePresetLevel_Click(sender As Object, e As RoutedEventArgs)

            ' TO BE CONTINUED !

            Dim presetName As String = _currentLevel.Title
            Dim iwadInput As String = If(_currentLevel.Dir.Contains("/doom/"), "doom.wad", "doom2.wad")
            Dim mapsInput As String = "" 'TODO
            Dim miscInput As String = "" 'TODO

            Dim presetToCreate As LevelPreset = New LevelPreset With
            {
                .Name = String.Concat("Quick_", presetName),
                .Iwad = iwadInput,
                .Maps = mapsInput,
                .Misc = miscInput,
                .Type = If(iwadInput.ToLowerInvariant.Contains("doom2.wad"), "Doom 2", "Doom 1"),
                .Year = Now.Year,
                .Pict = "" 'TODO: Manage this input
            }
            General.CreateNewPreset(presetToCreate)

        End Sub

        Private Sub Btn_DeleteLevel_Click(sender As Object, e As RoutedEventArgs)

        End Sub

        Private Sub Txt_OpenFileExplorer_Click(sender As Object, e As RoutedEventArgs)
            Try
                _doomWorldService.OpenInFileExplorer(_currentLevel)
            Catch ex As Exception
                Dim errDirectoryResult As MessageBoxResult = MessageBox.Show("Le dossier est introuvable." +
                                            Environment.NewLine +
                                            "Peut-être a-t-il été déplacé ou supprimé." +
                                            Environment.NewLine +
                                            Environment.NewLine +
                                            "Voulez-vous le supprimer de cette liste ?",
                                            "Dossier introuvable",
                                            MessageBoxButton.YesNo, MessageBoxImage.Error, MessageBoxResult.No)

                If errDirectoryResult = MessageBoxResult.Yes Then
                    _doomWorldService.DeleteLevel(_currentLevel)

                    _doomWorldWindow.AfterDeletedLevel()
                End If
            End Try

        End Sub
    End Class
End Namespace