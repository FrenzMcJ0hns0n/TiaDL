Namespace Views
    Public Class HelpWindow

        Private Sub PM_RequestNavigate(sender As Object, e As RequestNavigateEventArgs)
            Process.Start(New ProcessStartInfo(e.Uri.AbsoluteUri))
        End Sub

        Private Sub GZDoom_RequestNavigate(sender As Object, e As RequestNavigateEventArgs)
            Process.Start(New ProcessStartInfo(e.Uri.AbsoluteUri))
        End Sub

        Private Sub ModDB_RequestNavigate(sender As Object, e As RequestNavigateEventArgs)
            Process.Start(New ProcessStartInfo(e.Uri.AbsoluteUri))
        End Sub

    End Class
End Namespace
