Imports System.Diagnostics.Eventing.Reader
Imports System.Globalization

Namespace Helpers.Converters.DoomWorld
    Public Class SelectedLevelConverter : Implements IValueConverter

        Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert

            If value Is Nothing Then
                Return New Views.UserControls.DoomWorld.NoSelectedLevel
            Else
                Return New Views.UserControls.DoomWorld.SelectedLevel With {.DataContext = value}
            End If
        End Function

        Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
            Throw New NotImplementedException()
        End Function
    End Class
End Namespace