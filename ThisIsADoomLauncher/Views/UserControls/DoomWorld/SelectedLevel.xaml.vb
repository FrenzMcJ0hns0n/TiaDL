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

            DownloadLevel(levl)
        End Sub

        Private Async Sub DownloadLevel(levl As Level)

            ' TODO : download level
            ' display SUCCESS MESSAGE if success
            ' display ERROR MESSAGE if error

        End Sub
    End Class
End Namespace