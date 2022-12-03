Public Class PortParamsWindow

    Public UserValidation As Boolean
    Public OldValuesDict As Dictionary(Of String, String)
    Public NewValuesDict As Dictionary(Of String, String)



    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        LoadValues()
    End Sub

    Private Sub Btn_ApplyClose_Click(sender As Object, e As RoutedEventArgs)
        UserValidation = True
        SetDictionary()
        Close()
    End Sub

    Private Sub Tbk_CustomParams_GotFocus(sender As Object, e As RoutedEventArgs)
        Tbk_CustomParams.ClearValue(TextBlock.ForegroundProperty)
        Tbk_CustomParams.Text = ""
    End Sub

    Private Sub Tbk_CustomParams_LostFocus(sender As Object, e As RoutedEventArgs)
        If Tbk_CustomParams.Text = "" Then
            Tbk_CustomParams.Foreground = New SolidColorBrush(Colors.DarkGray)
            Tbk_CustomParams.Text = "Param1 Value1, Param2 Value2, etc."
        End If
    End Sub



    Private Sub LoadValues()
        If Rbtn_Predefined.IsChecked AndAlso OldValuesDict.Count > 0 Then
            Dim value As String = Nothing

            If OldValuesDict.TryGetValue("turbo", value) Then
                Cbx_TurboEnabled.IsChecked = True
                Tbx_TurboValue.Text = value
            End If

            If OldValuesDict.TryGetValue("nomonsters", value) Then
                Cbx_NoMonstersEnabled.IsChecked = True
            End If

        End If
    End Sub

    Private Sub SetDictionary()
        NewValuesDict = New Dictionary(Of String, String)

        If Cbx_TurboEnabled.IsChecked Or Cbx_NoMonstersEnabled.IsChecked Then
            If Cbx_TurboEnabled.IsChecked Then NewValuesDict.Add("turbo", Tbx_TurboValue.Text)
            If Cbx_NoMonstersEnabled.IsChecked Then NewValuesDict.Add("nomonsters", String.Empty)
        End If
    End Sub


End Class
