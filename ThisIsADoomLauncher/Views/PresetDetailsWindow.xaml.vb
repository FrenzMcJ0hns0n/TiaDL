Public Class PresetDetailsWindow

    Public PresetPictureSrc As BitmapImage
    Public PresetProperties As SortedDictionary(Of String, Object)

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        Img_PresetImage.Source = PresetPictureSrc
        Dtg_Properties.ItemsSource = PresetProperties
    End Sub

End Class
