Public Class PortParamsWindow

    Public UserValidation As Boolean
    Public OldValuesDict As Dictionary(Of String, String)
    Public NewValuesDict As Dictionary(Of String, String)



    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        LoadValues()
    End Sub

    Private Sub Btn_ApplyClose_Click(sender As Object, e As RoutedEventArgs)
        UserValidation = True
        SaveValues()
        Close()
    End Sub

    Private Sub Tbk_CustomParams_GotFocus(sender As Object, e As RoutedEventArgs)
        Tbk_CustomParams.ClearValue(ForegroundProperty)
        Tbk_CustomParams.Text = String.Empty
    End Sub

    Private Sub Tbk_CustomParams_LostFocus(sender As Object, e As RoutedEventArgs)
        If Tbk_CustomParams.Text = String.Empty Then
            Tbk_CustomParams.Foreground = New SolidColorBrush(Colors.DarkGray)
            Tbk_CustomParams.Text = "-param value, -param2 value2, etc."
        End If
    End Sub



    Private Sub LoadValues()
        If Rbtn_Predefined.IsChecked Then
            Dim value As String = Nothing

            If OldValuesDict.TryGetValue("turbo", value) Then
                Cbx_TurboEnabled.IsChecked = True
                Tbx_TurboValue.Text = value
            End If

            If OldValuesDict.TryGetValue("nomonsters", value) Then
                Cbx_NoMonsters.IsChecked = True
            End If

            If OldValuesDict.TryGetValue("nomusic", value) Then
                Cbx_NoMusic.IsChecked = True
            End If

            If OldValuesDict.TryGetValue("nosfx", value) Then
                Cbx_NoSFX.IsChecked = True
            End If

        ElseIf Rbtn_Custom.IsChecked Then
            'TODO
        End If
    End Sub

    Private Sub SaveValues()
        NewValuesDict = New Dictionary(Of String, String)

        If Rbtn_Predefined.IsChecked Then
            If Cbx_TurboEnabled.IsChecked Then NewValuesDict.Add("turbo", Tbx_TurboValue.Text)
            If Cbx_NoMonsters.IsChecked Then NewValuesDict.Add("nomonsters", String.Empty)
            If Cbx_NoMusic.IsChecked Then NewValuesDict.Add("nomusic", String.Empty)
            If Cbx_NoSFX.IsChecked Then NewValuesDict.Add("nosfx", String.Empty)
        ElseIf Rbtn_Custom.IsChecked Then
            'TODO
        End If
    End Sub


End Class
