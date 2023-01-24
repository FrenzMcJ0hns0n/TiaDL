Public Class CreatingWindowOnlyForThat

    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        If String.IsNullOrEmpty(Tbx_PresetName.Text) Then
            MessageBox.Show("The value cannot be empty!")
        Else
            Close()
        End If
    End Sub

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        Tbx_PresetName.Focus()
    End Sub

End Class
