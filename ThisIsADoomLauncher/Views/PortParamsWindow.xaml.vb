Public Class PortParamsWindow

    Public ValuesDictionary As Dictionary(Of String, Object)



    Private Sub Btn_OkClose_Click(sender As Object, e As RoutedEventArgs)
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

    Private Sub Window_Closing(sender As Object, e As ComponentModel.CancelEventArgs)
        SetDictionary()
    End Sub



    Private Sub SetDictionary()
        'Dim checkboxes As New List(Of CheckBox) From
        '{
        '    Cbx_Turbo,
        '    Cbx_NoMonsters
        '}
        ValuesDictionary = New Dictionary(Of String, Object)

        If Cbx_TurboEnabled.IsChecked OrElse Cbx_NoMonstersEnabled.IsChecked Then
            If Cbx_TurboEnabled.IsChecked Then ValuesDictionary.Add("turbo", CType(Tbx_TurboValue.Text, Integer))
            If Cbx_NoMonstersEnabled.IsChecked Then ValuesDictionary.Add("nomonsters", Cbx_NoMonstersEnabled.IsChecked)
        End If
    End Sub



End Class
