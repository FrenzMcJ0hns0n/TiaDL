Public Class Application

    Private Sub Application_Startup(sender As Object, e As StartupEventArgs)

        With My.Settings
            .DateTimeAtLaunch = Date.Now
            .Save()
        End With

    End Sub

End Class
